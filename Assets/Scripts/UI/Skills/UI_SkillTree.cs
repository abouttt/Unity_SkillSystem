using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_SkillTree : UI_Base
{
    // UI_SkillSlot�� ��ϵ� ������Ʈ�� �ڽ� ������Ʈ �� ��� ������Ʈ�� ���ε� �� ������ ����Ʈ.
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

    // ��Ʈ ��ų���� üũ �޼��带 ȣ���ϱ����� �븮��
    private Action _rootSkillAction = null;

    private TextMeshProUGUI _skillPointAmountText = null;
    private GameObject _resetButton = null;

    // ���±��� ��� ��ü ��ų ����Ʈ.
    private int _totalSkillPoint = 0;

    // UI_SkillTreeEditor�� UI_SkillSlot�� ���� �����͸� �̾��ֱ� ���� �ӽ� ����
    [SerializeField]
    private SkillSlotInt _skillEdges = null;

    // ���� ������Ʈ���� ���ε��ϰ� ������ �����Ѵ�.
    // �����ڽ� �������� �ε��Ͽ� �����ϰ� ���� ������Ʈ�� �д�.
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

    // ��Ʈ ��ų���� üũ �޼������ ����Ѵ�.
    // �ʱ⿡ ��Ʈ ��ų���� üũ�Ͽ� ������ �������� �˾ƺ���.
    private void Start()
    {
        foreach (var skill in _rootSkills)
        {
            _rootSkillAction += skill.CheckSkill;
        }

        UpdateSkillPointAmountText();
        CheckRootSkills();
    }

    // ��ų ����Ʈ�� ���ų� �Һ��� �� �ش� ����ŭ �����ϰ� ��Ʈ ��ų���� üũ�Ͽ�
    // ���������� ���� ��ų���� üũ�ϵ��� �Ѵ�.
    // ������ ��ų ����Ʈ�� ���� ��ų�� �����ϰų� �������� �� �� �ֱ� �����̴�.

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

    // UI_SkillTreeEditor�� ���� �� �޼���
    public void SetSelectSkillSlotEdges(SkillSlotInt edges) => _skillEdges = edges;

    private void CheckRootSkills()
    {
        if (_rootSkillAction != null)
        {
            _rootSkillAction.Invoke();
        }
    }

    private void UpdateSkillPointAmountText() => _skillPointAmountText.text = $"{SkillPoint}";

    // ��ų �ʱ�ȭ ��ư�� ������ ȣ��Ǵ� �޼���.
    // ���� ��ų���� ��Ʈ��ų���� �� �ʱ�ȭ ��Ų��.
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
