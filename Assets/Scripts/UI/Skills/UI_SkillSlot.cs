using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// Inspector�� Dictionary Ÿ���� ǥ���ϱ� ���� ����ȭ �۾�
[System.Serializable]
public class SkillSlotInt : SerializableDictionary<UI_SkillSlot, int> { }

public class UI_SkillSlot : UI_Base
{
    // UI_SkillSlot�� ��ϵ� ������Ʈ�� �ڽ� ������Ʈ �� ��� ������Ʈ�� ���ε� �� ������ ����Ʈ.
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

    private GameObject _levelUpButton = null;              // ���� �� ��ư
    private GameObject _levelUpButtonDisabledImage = null; // ���� �� ��ư ��Ȱ��ȭ
    private GameObject _skillDisabledImage = null;         // ��ų ������ ��Ȱ��ȭ
    private GameObject _skillToolTipEventImage = null;     // ��ų ���� �̺�Ʈ �̹���
    private TextMeshProUGUI _levelText = null;             // ��ų ����
    private UI_SkillToolTip _skillToolTip = null;          // ��ų ���� �ڽ�

    private bool _isOpen = false;       // ���� ������ ��������
    private bool _hasSkill = false;     // ���� �ߴ���
    private bool _isOnMouse = false;    // ���콺�� ���� �÷��� �ִ���

    // Bind �޼���� �Ű����� typeof(enum type)�� �ִ� enum ����Ʈ���� �̸����� ����
    // �ش� �̸��� ���� ������Ʈ�� ã�� ���׸� ���ڰ� ������Ʈ�� 
    // Dictionary<Type, UnityEngine.Object[]> ���·� �����Ѵ�.
    // Get �޼���� Bind�Ͽ� �����ϰ� �ִ� �����Ϳ��� �Ű��������� ã�� ��ȯ�Ѵ�.
    // _levelUpButtonDisabledImage, _levelUpButton�� ó���� ��Ȱ��ȭ�Ͽ� ���� �������� �ʵ��� �Ѵ�.
    // ���Ŀ� ���� �������� üũ�Ͽ� Ȱ��ȭ �Ѵ�.
    // AddUIEvent�޼���� �ش� ������Ʈ�� ������ �̺�ƮŸ������ �޼���� �߰��Ѵ�.
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

    // ���� �ڽ��� �����´�.
    // �������� ���� ��ų���� üũ �޼��带 ������ Action �븮�ڿ� ����ϰ�
    // ���� ��ų���� �θ� ��ų�� ����Ѵ�.
    // skill.Value���� �� ��ų�� Value���� �Ǹ� ���� ��ų�� ���� ������ ���·� �����Ѵٴ� ���̴�.
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

    // ���콺�� ���� ������ �����ڽ� ��ġ�� ���콺 ��ǥ�� ������.
    private void Update()
    {
        if (_isOnMouse)
        {
            _skillToolTip.SetPos(Input.mousePosition);
        }
    }

    // ��ų�� ���� ������ ��������, ������ �� �ִ��� üũ�Ѵ�.
    public void CheckSkill()
    {
        // �θ� ��ų���� Ȯ���Ѵ�.
        if (!_hasSkill && ParentSkills != null)
        {
            if (!CheckParentSkillLevel())
            {
                return;
            }
        }

        // ��ų ������ ��Ȱ��ȭ �������� ��Ȱ��ȭ ��Ű�� �ѹ��� ��Ȱ��ȭ �ϱ� ����
        // _hasSkill�� true�� �����.
        if (!_hasSkill && CurrentLevel > 0)
        {
            _skillDisabledImage.SetActive(false);
            _hasSkill = true;
        }

        // �������� ���� ��ų���� üũ �Լ����� ȣ���Ѵ�.
        if (_hasSkill && _skillEdgesAction != null)
        {
            if (_skillEdgesAction != null)
            {
                _skillEdgesAction.Invoke();
            }
        }

        // ���� ������ �ִ뷹���� �Ǿ����� ���̻� ������ ��ư�� �������� �Ѵ�.
        if (CurrentLevel >= SkillInfoData.MaxLevel)
        {
            _levelUpButtonDisabledImage.SetActive(false);
            _levelUpButton.SetActive(false);
            return;
        }

        // �÷��̾� ������ Ȯ���ϰ� ���� ������ �����̸� ������ ��ư�� ��Ȱ��ȭ �̹�����
        // Ȱ��ȭ ��Ű�� ��ų ����Ʈ�� üũ�Ͽ� ������ ��ư ��Ȱ��ȭ �̹����� Ű�� ����.
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

    // �θ� ��ų�� �߰��Ѵ�.
    private void AddParentSkill(UI_SkillSlot skill, int level)
    {
        if (ParentSkills == null)
        {
            ParentSkills = new List<UI_SkillSlot>();
        }

        ParentSkills.Add(skill);
    }

    // �ʱ�ȭ �޼���.
    // �������� ���� ��ų�鵵 �ʱ�ȭ �޼��带 ȣ���Ͽ� �� ���� ��ų���� �ʱ�ȭ �ϵ��� �Ѵ�.
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

    // ������ ��ư�� ������ ȣ��Ǵ� �޼���.
    // ���� ������ �ø��� ���� �ؽ�Ʈ�� ������Ʈ ��Ų �� ��ų ����Ʈ�� �����Ѵ�.
    private void OnButtonAddLevel(PointerEventData data)
    {
        CurrentLevel++;
        UpdateLevelText();
        UI_SkillTree.GetInstance.UseSkillPoint(SkillInfoData.NeedSkillPoint);
    }

    // ���콺 �����Ͱ� ó�� ������ �� ȣ��Ǵ� �޼���.
    // ���� �ڽ��� ���̰��Ѵ�. �� �� ��ų ������ ���� ��ų ������ ���̵��� �Ѵ�.
    private void OnPointerEnter(PointerEventData data)
    {
        _skillToolTip.Show(this);
        _isOnMouse = true;
    }

    // ���콺 �����Ͱ� ������ ��� �� ���� �ڽ��� �ݴ´�.
    private void OnPointerExit(PointerEventData data)
    {
        _skillToolTip.Close();
        _isOnMouse = false;
    }

    // �θ� ��ų���� ������ Ȯ���ϴ� �޼���.
    // ó���� ��ϵ� �θ�ų���� ��� üũ�Ͽ� �ش� �θ�ų�� �����Ǵ� ������ �Ǿ����� Ȯ���Ѵ�.
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
