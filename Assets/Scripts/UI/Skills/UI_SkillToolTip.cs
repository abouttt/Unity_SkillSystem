using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SkillToolTip : UI_Base
{
    enum Texts
    {
        SkillNameText,
        SkillTypeText,
        SkillDescText,
    }

    enum Images
    {
        Background,
    }

    #region 싱글톤
    private static UI_SkillToolTip s_instance = null;
    public static UI_SkillToolTip GetInstance => s_instance;
    private void Awake() => s_instance = this;
    #endregion

    private RectTransform _backgroundRt;
    private StringBuilder _sb;

    private GameObject _background = null;
    private TextMeshProUGUI _skillName = null;
    private TextMeshProUGUI _skillType = null;
    private TextMeshProUGUI _skillDesc = null;

    private void Start()
    {
        _sb = new StringBuilder(30);

        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));

        _background = Get<Image>((int)Images.Background).gameObject;
        _skillName = Get<TextMeshProUGUI>((int)Texts.SkillNameText);
        _skillType = Get<TextMeshProUGUI>((int)Texts.SkillTypeText);
        _skillDesc = Get<TextMeshProUGUI>((int)Texts.SkillDescText);

        _backgroundRt = _background.GetComponent<RectTransform>();

        gameObject.SetActive(false);
    }

    public void Show(UI_SkillSlot skill, Vector3 pos)
    {
        gameObject.SetActive(true);

        _skillName.text = skill.SkillData.SkillName;
        _skillType.text = $"[{Enum.GetName(typeof(SkillData.SkillTypes), skill.SkillData.SkillType)}]";

        _sb.Clear();
        _sb.AppendFormat("{0}\n\n", skill.SkillData.SkillDescription);
        _sb.AppendFormat("※습득조건※\n");

        SkillData skillData;
        foreach (var parentSkill in skill.ParentSkills)
        {
            skillData = parentSkill.SkillData;
            _sb.AppendFormat("- {0} Lv.{1}\n", skillData.SkillName, skillData.NextSkillOpenLevel);
        }
        _sb.AppendFormat("- 필요 스킬 포인트 : {0}\n", skill.SkillData.NeedSkillPoint);
        _sb.AppendFormat("- 제한레벨 : {0}\n", skill.SkillData.PlayerLevel);
        _skillDesc.text = _sb.ToString();

        _skillDesc.rectTransform.sizeDelta = new Vector2(_skillDesc.rectTransform.sizeDelta.x, _skillDesc.preferredHeight);
        _backgroundRt.sizeDelta = new Vector2(_backgroundRt.sizeDelta.x, _skillDesc.rectTransform.rect.height + 100.0f);
    }

    public void SetPos(Vector3 pos)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        Vector3 nextPos = new Vector3();
        nextPos.x = pos.x + (_backgroundRt.rect.width * 0.5f) + 15.0f;
        nextPos.y = pos.y - (_backgroundRt.rect.height * 0.5f) - 15.0f;
        transform.position = nextPos;

        transform.position = nextPos;
    }

    public void Close() => gameObject.SetActive(false);
}
