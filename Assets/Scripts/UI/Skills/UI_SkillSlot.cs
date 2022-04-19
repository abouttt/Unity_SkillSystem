using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// Inspector에 Dictionary 타입을 표시하기 위한 직렬화 작업
[System.Serializable]
public class SkillSlotInt : SerializableDictionary<UI_SkillSlot, int> { }

public class UI_SkillSlot : UI_Base
{
    // UI_SkillSlot이 등록된 오브젝트의 자식 오브젝트 중 어떠한 오브젝트를 바인딩 할 것인지 리스트.
    #region UI_Objects
    enum Buttons
    {
        LevelUpButton,
    }

    enum Texts
    {
        LevelText,
    }

    enum Images
    {
        SkillIcon,
        SkillFrame,
        SkillDisabledImage,
        LevelUpDisabledImage,
        SkillInfoEventImage,
    }
    #endregion

    [field: SerializeField]
    public SkillInfoData SkillInfoData { get; set; }

    [field: SerializeField]
    public SkillSlotInt SkillEdges { get; set; }

    [field: SerializeField]
    public int CurrentLevel { get; set; }

    public List<UI_SkillSlot> ParentSkills { get; private set; }

    private Action _skillEdgesAction = null;

    private GameObject _levelUpButton = null;              // 레벨 업 버튼
    private GameObject _levelUpButtonDisabledImage = null; // 레벨 업 버튼 비활성화
    private GameObject _skillDisabledImage = null;         // 스킬 아이콘 비활성화
    private GameObject _skillToolTipEventImage = null;     // 스킬 툴팁 이벤트 이미지
    private TextMeshProUGUI _levelText = null;             // 스킬 레벨
    private UI_SkillToolTip _skillToolTip = null;          // 스킬 툴팁 박스

    private bool _isOpen = false;       // 해제 가능한 상태인지
    private bool _hasSkill = false;     // 해제 했는지
    private bool _isOnMouse = false;    // 마우스가 위에 올려져 있는지

    // Bind 메서드는 매개변수 typeof(enum type)에 있는 enum 리스트들의 이름들을 통해
    // 해당 이름과 같은 오브젝트를 찾아 제네릭 인자값 컴포넌트를 
    // Dictionary<Type, UnityEngine.Object[]> 형태로 저장한다.
    // Get 메서드는 Bind하여 저장하고 있는 데이터에서 매개변수값을 찾아 반환한다.
    // _levelUpButtonDisabledImage, _levelUpButton는 처음에 비활성화하여 해제 가능하지 않도록 한다.
    // 이후에 해제 가능한지 체크하여 활성화 한다.
    // AddUIEvent메서드는 해당 오브젝트에 지정한 이벤트타입으로 메서드는 추가한다.
    private void Awake()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));

        Get<Image>((int)Images.SkillIcon).sprite = SkillInfoData.SkillIcon;
        Get<Image>((int)Images.SkillFrame).sprite = SkillInfoData.SkillFrame;
        _levelUpButton = Get<Button>((int)Buttons.LevelUpButton).gameObject;
        _levelUpButtonDisabledImage = Get<Image>((int)Images.LevelUpDisabledImage).gameObject;
        _skillDisabledImage = Get<Image>((int)Images.SkillDisabledImage).gameObject;
        _skillToolTipEventImage = Get<Image>((int)Images.SkillInfoEventImage).gameObject;
        _levelText = Get<TextMeshProUGUI>((int)Texts.LevelText);

        _levelUpButtonDisabledImage.SetActive(false);
        _levelUpButton.SetActive(false);

        AddUIEvent(_levelUpButton, OnButtonAddLevel, Define.UIEvent.Click);
        AddUIEvent(_skillToolTipEventImage, OnPointerEnter, Define.UIEvent.PointerEnter);
        AddUIEvent(_skillToolTipEventImage, OnPointerExit, Define.UIEvent.PointerExit);
    }

    // 툴팁 박스를 가져온다.
    // 간선으로 이은 스킬들의 체크 메서드를 가져와 Action 대리자에 등록하고
    // 간선 스킬들의 부모 스킬로 등록한다.
    // skill.Value값은 이 스킬이 Value값이 되면 간선 스킬을 해제 가능한 상태로 변경한다는 값이다.
    private void Start()
    {
        _skillToolTip = UI_SkillTree.GetInstance.SkillToolTip;

        foreach (var skill in SkillEdges)
        {
            _skillEdgesAction += skill.Key.CheckSkill;
            skill.Key.AddParentSkill(this, skill.Value);
        }

        UpdateLevelText();
    }

    // 마우스가 위에 있을시 툴팁박스 위치를 마우스 좌표로 보낸다.
    private void Update()
    {
        if (_isOnMouse)
        {
            _skillToolTip.SetPos(Input.mousePosition);
        }
    }

    // 스킬이 해제 가능한 상태인지, 해제할 수 있는지 체크한다.
    public void CheckSkill()
    {
        // 부모 스킬들을 확인한다.
        if (!_hasSkill && ParentSkills != null)
        {
            if (!CheckParentSkillLevel())
            {
                return;
            }
        }

        // 스킬 아이콘 비활성화 아이콘을 비활성화 시키고 한번만 비활성화 하기 위해
        // _hasSkill을 true로 만든다.
        if (!_hasSkill && CurrentLevel > 0)
        {
            _skillDisabledImage.SetActive(false);
            _hasSkill = true;
        }

        // 간선으로 이은 스킬들의 체크 함수들을 호출한다.
        if (_hasSkill && _skillEdgesAction != null)
        {
            if (_skillEdgesAction != null)
            {
                _skillEdgesAction.Invoke();
            }
        }

        // 현재 레벨이 최대레벨이 되었으면 더이상 레벨업 버튼을 못누르게 한다.
        if (CurrentLevel >= SkillInfoData.MaxLevel)
        {
            _levelUpButtonDisabledImage.SetActive(false);
            _levelUpButton.SetActive(false);
            return;
        }

        // 플레이어 레벨을 확인하고 해제 가능한 상태이면 레벨업 버튼과 비활성화 이미지를
        // 활성화 시키고 스킬 포인트를 체크하여 레벨업 버튼 비활성화 이미지를 키고 끈다.
        if (CheckPlayerLevel())
        {
            if (!_isOpen)
            {
                _levelUpButtonDisabledImage.SetActive(true);
                _levelUpButton.SetActive(true);
                _isOpen = true;
            }

            if (CheckSkillPoint())
            {
                _levelUpButtonDisabledImage.SetActive(false);
            }
            else
            {
                _levelUpButtonDisabledImage.SetActive(true);
            }
        }
    }

    // 부모 스킬을 추가한다.
    private void AddParentSkill(UI_SkillSlot skill, int level)
    {
        if (ParentSkills == null)
        {
            ParentSkills = new List<UI_SkillSlot>();
        }

        ParentSkills.Add(skill);
    }

    // 초기화 메서드.
    // 간선으로 이은 스킬들도 초기화 메서드를 호출하여 맨 하위 스킬까지 초기화 하도록 한다.
    public void Reset()
    {
        foreach (var skill in SkillEdges)
        {
            skill.Key.Reset();
        }

        _skillDisabledImage.SetActive(true);
        _levelUpButtonDisabledImage.SetActive(false);
        _levelUpButton.SetActive(false);
        _isOpen = false;
        _hasSkill = false;
        CurrentLevel = 0;
        UpdateLevelText();
    }

    // 레벨업 버튼을 누를시 호출되는 메서드.
    // 현재 레벨을 올리고 레벨 텍스트를 업데이트 시킨 뒤 스킬 포인트를 차감한다.
    private void OnButtonAddLevel(PointerEventData data)
    {
        CurrentLevel++;
        UpdateLevelText();
        UI_SkillTree.GetInstance.UseSkillPoint(SkillInfoData.NeedSkillPoint);
    }

    // 마우스 포인터가 처음 들어왔을 때 호출되는 메서드.
    // 툴팁 박스를 보이게한다. 이 때 스킬 슬롯을 보내 스킬 정보를 보이도록 한다.
    private void OnPointerEnter(PointerEventData data)
    {
        _skillToolTip.Show(this);
        _isOnMouse = true;
    }

    // 마우스 포인터가 위에서 벗어날 때 툴팁 박스를 닫는다.
    private void OnPointerExit(PointerEventData data)
    {
        _skillToolTip.Close();
        _isOnMouse = false;
    }

    // 부모 스킬들의 레벨을 확인하는 메서드.
    // 처음에 등록된 부모스킬들을 모두 체크하여 해당 부모스킬이 만족되는 레벨이 되었는지 확인한다.
    private bool CheckParentSkillLevel()
    {
        int level;

        foreach (var skill in ParentSkills)
        {
            if (skill.SkillEdges.TryGetValue(this, out level))
            {
                if (skill.CurrentLevel < level)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool CheckPlayerLevel() => Player.GetInstance.Level >= SkillInfoData.NeedPlayerLevel;

    private void UpdateLevelText() => _levelText.text = $"{CurrentLevel} / {SkillInfoData.MaxLevel}";
    private bool CheckSkillPoint() => UI_SkillTree.GetInstance.SkillPoint >= SkillInfoData.NeedSkillPoint;
}
