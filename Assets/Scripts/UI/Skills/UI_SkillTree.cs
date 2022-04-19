using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_SkillTree : UI_Base
{
    // UI_SkillSlot이 등록된 오브젝트의 자식 오브젝트 중 어떠한 오브젝트를 바인딩 할 것인지 리스트.
    #region UI_Objects
    enum Texts
    {
        SkillPointAmountText,
    }

    enum Buttons
    {
        ResetButton,
    }
    #endregion

    private static UI_SkillTree s_instance = null;
    public static UI_SkillTree GetInstance => s_instance;

    public int SkillPoint { get; private set; } = 0;
    public UI_SkillToolTip SkillToolTip { get; private set; }

    [SerializeField]
    private List<UI_SkillSlot> _rootSkills;

    // 루트 스킬들의 체크 메서드를 호출하기위한 대리자
    private Action _rootSkillAction = null;

    private TextMeshProUGUI _skillPointAmountText = null;
    private GameObject _resetButton = null;

    // 여태까지 얻는 전체 스킬 포인트.
    private int _totalSkillPoint = 0;

    // UI_SkillTreeEditor와 UI_SkillSlot의 간선 데이터를 이어주기 위한 임시 변수
    [SerializeField]
    private SkillSlotInt _skillEdges = null;

    // 하위 오브젝트들을 바인드하고 가져와 참조한다.
    // 툴팁박스 프리팹을 로드하여 생성하고 하위 오브젝트로 둔다.
    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }

        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        _skillPointAmountText = Get<TextMeshProUGUI>((int)Texts.SkillPointAmountText);
        _resetButton = Get<Button>((int)Buttons.ResetButton).gameObject;

        AddUIEvent(_resetButton, OnButtonReset, Define.UIEvent.Click);

        SkillToolTip = Resources.Load<UI_SkillToolTip>("Prefabs/UI/UI_SkillToolTip");
        if (SkillToolTip == null)
        {
            Util.DebugLog("[UI_SkillTree] Faild Load to SkillToolTip");
        }
        else
        {
            SkillToolTip = Instantiate(SkillToolTip);
            SkillToolTip.transform.SetParent(transform);
        }
    }

    // 루트 스킬들의 체크 메서드들을 등록한다.
    // 초기에 루트 스킬들을 체크하여 해제가 가능한지 알아본다.
    private void Start()
    {
        foreach (var skill in _rootSkills)
        {
            _rootSkillAction += skill.CheckSkill;
        }

        UpdateSkillPointAmountText();
        CheckRootSkills();
    }

    // 스킬 포인트를 쓰거나 소비할 때 해당 값만큼 변경하고 루트 스킬들을 체크하여
    // 연속적으로 하위 스킬들을 체크하도록 한다.
    // 이유는 스킬 포인트에 따라 스킬을 해제하거나 레벨업을 할 수 있기 때문이다.

    public void AddSkillPoint(int point)
    {
        SkillPoint += point;
        _totalSkillPoint += point;
        UpdateSkillPointAmountText();
        CheckRootSkills();
    }

    public void UseSkillPoint(int point)
    {
        SkillPoint = SkillPoint >= point ? SkillPoint - point : SkillPoint;
        UpdateSkillPointAmountText();
        CheckRootSkills();
    }

    // UI_SkillTreeEditor를 위한 셋 메서드
    public void SetSelectSkillSlotEdges(SkillSlotInt edges) => _skillEdges = edges;

    private void CheckRootSkills()
    {
        if (_rootSkillAction != null)
        {
            _rootSkillAction.Invoke();
        }
    }

    private void UpdateSkillPointAmountText() => _skillPointAmountText.text = $"{SkillPoint}";

    // 스킬 초기화 버튼을 누를시 호출되는 메서드.
    // 하위 스킬부터 루트스킬까지 쭉 초기화 시킨다.
    private void OnButtonReset(PointerEventData data)
    {
        foreach (var skill in _rootSkills)
        {
            skill.Reset();
        }

        SkillPoint = _totalSkillPoint;
        UpdateSkillPointAmountText();
        CheckRootSkills();
    }
}
