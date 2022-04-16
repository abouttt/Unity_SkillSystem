using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UI_SkillTree : UI_Base
{
    enum Texts
    {
        SkillPointAmountText,
    }

    #region 싱글톤
    private static UI_SkillTree s_instance = null;
    public static UI_SkillTree GetInstance => s_instance;
    private void Awake() => s_instance = this;
    #endregion

    public int SkillPoint { get; private set; } = 0;

    [SerializeField]
    private List<UI_SkillSlot> _rootSkills;

    private Action _checkNextSkillAction = null;

    private TextMeshProUGUI _skillPointAmountText = null;

    // UI_SkillTreeEditor와 UI_SkillSlot을 이어주기 위한 임시 변수
    [SerializeField]
    private List<UI_SkillSlot> _skillEdges = null;

    private void Start()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        _skillPointAmountText = Get<TextMeshProUGUI>((int)Texts.SkillPointAmountText);

        UpdateSkillPointAmountText();

        foreach (var skill in _rootSkills)
        {
            _checkNextSkillAction += skill.CheckSkill;
        }
    }

    private void OnEnable()
    {
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
        if (_checkNextSkillAction != null)
        {
            _checkNextSkillAction.Invoke();
        }
    }

    private void test()
    {
        Queue<UI_SkillTree> q = new Queue<UI_SkillTree>();
        
    }

    public void SetSelectSkillSlotEdges(List<UI_SkillSlot> edges)
    {
        _skillEdges = edges;
    }

    private void UpdateSkillPointAmountText() => _skillPointAmountText.text = $"{SkillPoint}";
}
