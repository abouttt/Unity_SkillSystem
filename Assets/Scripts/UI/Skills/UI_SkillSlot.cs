using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class SkillSlotInt : SerializableDictionary<UI_SkillSlot, int> { }

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
    public SkillInfoData SkillInfoData { get; set; }

    [field: SerializeField]
    public SkillSlotInt SkillEdges { get; set; }

    [field: SerializeField]
    public int CurrentLevel { get; set; }

    public Dictionary<UI_SkillSlot, int> _parentSkills { get; private set; }

    private Action _skillEdgesAction = null;

    private GameObject _levelUpButton = null;
    private GameObject _levelUpButtonDisabledImage = null;
    private GameObject _skillDisabledImage = null;
    private GameObject _skillToolTipEventImage = null;
    private TextMeshProUGUI _levelText = null;
    private UI_SkillToolTip _skillToolTip = null;

    private bool _hasSkill = false;
    private bool _isOnMouse = false;
    private bool _canGet = false;

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

        AddUIEvent(_levelUpButton, OnButtonAddLevel, Define.UIEvent.Click);
        AddUIEvent(_skillToolTipEventImage, OnPointerEnter, Define.UIEvent.PointerEnter);
        AddUIEvent(_skillToolTipEventImage, OnPointerExit, Define.UIEvent.PointerExit);
    }

    private void Start()
    {
        _skillToolTip = UI_SkillTree.GetInstance.SkillToolTip;

        foreach (var skill in SkillEdges)
        {
            _skillEdgesAction += skill.Key.CheckSkill;
            skill.Key.AddParentSkill(this, skill.Value);
        }

        _levelUpButtonDisabledImage.SetActive(false);
        _levelUpButton.SetActive(false);

        UpdateLevelText();
    }

    private void Update()
    {
        if (_isOnMouse)
        {
            _skillToolTip.SetPos(Input.mousePosition);
        }
    }

    public void CheckSkill()
    {
        if (_parentSkills != null)
        {
            if (!CheckParentSkillLevel())
            {
                return;
            }
        }

        if (!_canGet)
        {
            _levelUpButtonDisabledImage.SetActive(true);
            _levelUpButton.SetActive(true);
            _canGet = true;
        }

        if (!_hasSkill && CurrentLevel > 0)
        {
            _skillDisabledImage.SetActive(false);
            _hasSkill = true;
        }

        if (_hasSkill && _skillEdgesAction != null)
        {
            if (_skillEdgesAction != null)
            {
                _skillEdgesAction.Invoke();
            }
        }

        if (CurrentLevel >= SkillInfoData.MaxLevel)
        {
            _levelUpButtonDisabledImage.SetActive(false);
            _levelUpButton.SetActive(false);
            return;
        }

        if (CheckPlayerLevel() && CheckSkillPoint())
        {
            _levelUpButtonDisabledImage.SetActive(false);
        }
        else
        {
            _levelUpButtonDisabledImage.SetActive(true);
        }
    }

    private void AddParentSkill(UI_SkillSlot skill, int level)
    {
        if (_parentSkills == null)
        {
            _parentSkills = new Dictionary<UI_SkillSlot, int>();
        }

        _parentSkills.Add(skill, level);
    }

    private void OnButtonAddLevel(PointerEventData data)
    {
        CurrentLevel++;
        UpdateLevelText();
        UI_SkillTree.GetInstance.UseSkillPoint(SkillInfoData.NeedSkillPoint);
    }

    private void OnPointerEnter(PointerEventData data)
    {
        _skillToolTip.Show(this);
        _isOnMouse = true;
    }

    private void OnPointerExit(PointerEventData data)
    {
        _skillToolTip.Close();
        _isOnMouse = false;
    }

    private bool CheckParentSkillLevel()
    {
        int level;

        foreach (var skill in _parentSkills)
        {
            if (skill.Key.SkillEdges.TryGetValue(this, out level))
            {
                if (skill.Key.CurrentLevel < level)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void UpdateLevelText() => _levelText.text = $"{CurrentLevel} / {SkillInfoData.MaxLevel}";

    private bool CheckPlayerLevel() => Player.GetInstance.Level >= SkillInfoData.PlayerLevel;

    private bool CheckSkillPoint() => UI_SkillTree.GetInstance.SkillPoint >= SkillInfoData.NeedSkillPoint;
}
