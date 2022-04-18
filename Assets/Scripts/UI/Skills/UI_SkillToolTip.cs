using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SkillToolTip : UI_Base
{
    #region UI_Objects
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
    #endregion

    private RectTransform _backgroundRt;
    private StringBuilder _sb;

    private GameObject _background = null;
    private TextMeshProUGUI _skillName = null;
    private TextMeshProUGUI _skillType = null;
    private TextMeshProUGUI _skillDesc = null;

    private const float MOUSE_GAP = 15.0f;

    private void Awake()
    {
        _sb = new StringBuilder(50);

        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));

        _background = Get<Image>((int)Images.Background).gameObject;
        _skillName = Get<TextMeshProUGUI>((int)Texts.SkillNameText);
        _skillType = Get<TextMeshProUGUI>((int)Texts.SkillTypeText);
        _skillDesc = Get<TextMeshProUGUI>((int)Texts.SkillDescText);

        _backgroundRt = _background.GetComponent<RectTransform>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(UI_SkillSlot skillSolot)
    {
        gameObject.SetActive(true);

        if (_skillName.text.Equals(skillSolot.SkillInfoData.SkillName))
        {
            return;
        }

        _skillName.text = skillSolot.SkillInfoData.SkillName;
        _skillType.text = $"[{Enum.GetName(typeof(SkillInfoData.SkillTypes), skillSolot.SkillInfoData.SkillType)}]";

        _sb.Clear();
        _sb.AppendFormat("{0}\n\n", skillSolot.SkillInfoData.SkillDesc);
        _sb.AppendFormat("※습득조건※\n");

        if (skillSolot._parentSkills != null)
        {
            foreach (var skill in skillSolot._parentSkills)
            {
                _sb.AppendFormat("- {0} Lv.{1}\n", skill.Key.SkillInfoData.SkillName, skill.Value);
            }
        }

        _sb.AppendFormat("- 필요 스킬 포인트 : {0}\n", skillSolot.SkillInfoData.NeedSkillPoint);
        _sb.AppendFormat("- 제한레벨 : {0}\n", skillSolot.SkillInfoData.PlayerLevel);
        _skillDesc.text = _sb.ToString();

        _skillDesc.rectTransform.sizeDelta = new Vector2(_skillDesc.rectTransform.sizeDelta.x, _skillDesc.preferredHeight);
        _backgroundRt.sizeDelta = new Vector2(_backgroundRt.sizeDelta.x, _skillDesc.rectTransform.rect.height + 100.0f);
    }

    public void SetPos(Vector3 pos)
    {
        Vector3 nextPos = new Vector3();
        nextPos.x = pos.x + (_backgroundRt.rect.width * 0.5f) + MOUSE_GAP;
        nextPos.y = pos.y - (_backgroundRt.rect.height * 0.5f) - MOUSE_GAP;
        transform.position = nextPos;
    }

    public void Close() => gameObject.SetActive(false);
}
