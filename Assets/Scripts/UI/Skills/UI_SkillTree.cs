using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UI_SkillTree : UI_Base
{
    enum Texts
    {
        SkillPointAmountText,
    }

    private static UI_SkillTree s_instance = null;
    public static UI_SkillTree GetInstance => s_instance;

    public int SkillPoint { get; private set; } = 2;
    public UI_SkillToolTip SkillToolTip { get; private set; }

    [SerializeField]
    private List<UI_SkillSlot> _rootSkills;

    private Action _rootSkillAction = null;

    private TextMeshProUGUI _skillPointAmountText = null;

    // UI_SkillTreeEditor와 UI_SkillSlot을 이어주기 위한 임시 변수
    [SerializeField]
    private SkillSlotInt _skillEdges = null;

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }

        Bind<TextMeshProUGUI>(typeof(Texts));
        _skillPointAmountText = Get<TextMeshProUGUI>((int)Texts.SkillPointAmountText);

        SkillToolTip = Resources.Load<UI_SkillToolTip>("Prefabs/UI/UI_SkillToolTip");
        if (SkillToolTip == null)
        {
            Util.DebugLog("Faild Load to SkillToolTip");
        }
        else
        {
            SkillToolTip = Instantiate(SkillToolTip);
            SkillToolTip.transform.SetParent(transform);
        }
    }

    private void Start()
    {
        foreach (var skill in _rootSkills)
        {
            _rootSkillAction += skill.CheckSkill;
        }

        UpdateSkillPointAmountText();
        CheckRootSkills();
    }

    public void AddSkillPoint(int point)
    {
        SkillPoint += point;
        UpdateSkillPointAmountText();
        CheckRootSkills();
    }

    public void UseSkillPoint(int point)
    {
        SkillPoint = SkillPoint >= point ? SkillPoint - point : SkillPoint;
        UpdateSkillPointAmountText();
        CheckRootSkills();
    }

    private void CheckRootSkills()
    {
        if (_rootSkillAction != null)
        {
            _rootSkillAction.Invoke();
        }
    }

    public void SetSelectSkillSlotEdges(SkillSlotInt edges) => _skillEdges = edges;

    private void UpdateSkillPointAmountText() => _skillPointAmountText.text = $"{SkillPoint}";
}
