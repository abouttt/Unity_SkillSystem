using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UI_SkillSlot : UI_Base
{
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
    public SkillData SkillData { get; private set; }

    [field: SerializeField]
    public int CurrentLevel { get; set; }

    [field: SerializeField]
    public List<UI_SkillSlot> SkillEdges { get; set; }

    [field: SerializeField]
    public List<UI_SkillSlot> ParentSkills { get; set; } = new List<UI_SkillSlot>();

    private Action _checkNextSkillAction { get; set; } = null;

    private GameObject _levelUpButton = null;
    private GameObject _levelUpButtonDisabledImage = null;
    private GameObject _skillDisabledImage = null;
    private GameObject _skillToolTipEventImage = null;
    private TextMeshProUGUI _levelText = null;

    private bool _hasSkill = false;
    private bool _isNextSkillOpen = false;
    private bool _isMouseOn = false;

    private void Awake()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));

        Get<Image>((int)Images.SkillIcon).sprite = SkillData.SkillIcon;
        Get<Image>((int)Images.SkillFrame).sprite = SkillData.SkillFrame;

        _levelUpButton = Get<Button>((int)Buttons.LevelUpButton).gameObject;
        _levelUpButtonDisabledImage = Get<Image>((int)Images.LevelUpDisabledImage).gameObject;
        _skillDisabledImage = Get<Image>((int)Images.SkillDisabledImage).gameObject;
        _skillToolTipEventImage = Get<Image>((int)Images.SkillInfoEventImage).gameObject;
        _levelText = Get<TextMeshProUGUI>((int)Texts.LevelText);

        AddUIEvent(_levelUpButton, OnButtonAddLevel, Define.UIEvent.Click);
        AddUIEvent(_skillToolTipEventImage, OnPointerEnter, Define.UIEvent.PointerEnter);
        AddUIEvent(_skillToolTipEventImage, OnPointerExit, Define.UIEvent.PointerExit);
    }

    private void Start()
    {
        foreach (var skill in SkillEdges)
        {
            _checkNextSkillAction += skill.CheckSkill;
        }

        if (SkillData.IsParent)
        {
            foreach (var skill in SkillEdges)
            {
                skill.SetParentSkill(this);
            }
        }

        UpdateLevelText();
        CheckSkill();
    }

    private void Update()
    {
        if (_isMouseOn)
        {
            UI_SkillToolTip.GetInstance.SetPos(Input.mousePosition);
        }
    }

    public void CheckSkill()
    {
        foreach (var skill in ParentSkills)
        {
            if (!skill._isNextSkillOpen)
            {
                return;
            }
        }

        if (!_hasSkill && CurrentLevel > 0)
        {
            _skillDisabledImage.SetActive(false);
            _hasSkill = true;
        }

        if (_hasSkill && _checkNextSkillAction != null)
        {
            if (CurrentLevel >= SkillData.NextSkillOpenLevel)
            {
                _isNextSkillOpen = true;
                _checkNextSkillAction.Invoke();
            }
        }

        if (CurrentLevel >= SkillData.MaxLevel)
        {
            _levelUpButtonDisabledImage.SetActive(false);
            _levelUpButton.SetActive(false);
            return;
        }

        if (CheckPlayerLevel() && CheckSkillPoint())
        {
            if (!CheckParentSkills())
            {
                return;
            }

            _levelUpButtonDisabledImage.SetActive(false);
        }
        else
        {
            _levelUpButtonDisabledImage.SetActive(true);
        }
    }

    public void SetParentSkill(UI_SkillSlot parent) => ParentSkills.Add(parent);

    private bool CheckParentSkills()
    {
        foreach (var skill in ParentSkills)
        {
            if (!skill._hasSkill)
            {
                return false;
            }
        }

        return true;
    }

    private void OnButtonAddLevel(PointerEventData data)
    {
        CurrentLevel++;
        UpdateLevelText();
        UI_SkillTree.GetInstance.UseSkillPoint(SkillData.NeedSkillPoint);
    }

    private void OnPointerEnter(PointerEventData data)
    {
        UI_SkillToolTip.GetInstance.Show(this, Input.mousePosition);
        _isMouseOn = true;
    }

    private void OnPointerExit(PointerEventData data)
    {
        UI_SkillToolTip.GetInstance.Close();
        _isMouseOn = false;
    }

    private void UpdateLevelText() => _levelText.text = $"{CurrentLevel} / {SkillData.MaxLevel}";

    private bool CheckPlayerLevel() => Player.GetInstance.Level >= SkillData.PlayerLevel;

    private bool CheckSkillPoint() => UI_SkillTree.GetInstance.SkillPoint >= SkillData.NeedSkillPoint;
}
