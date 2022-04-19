using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 스킬 정보를 나타내는 툴팁박스
public class UI_SkillToolTip : UI_Base
{
    // UI_SkillSlot이 등록된 오브젝트의 자식 오브젝트 중 어떠한 오브젝트를 바인딩 할 것인지 리스트.
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

    // 마우스와의 위치 차이를 두기 위한 값.
    private const float MOUSE_GAP = 15.0f;

    // 하위 오브젝트들을 바인드하고 가져와 참조한다.
    // StringBuilder는 툴팁 박스안 텍스트들이 이어져 써지기 때문에
    // String 가비지 값이 나오는것을 방지한다.
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

    // 인자값으로 온 스킬 슬롯을 참조하여 SkillInfoData를 가져와 텍스트 형태로 적는다.
    // 툴팁 박스 높이는 텍스트의 크기에 따라 달라지게 한다.
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

        int level;
        if (skillSolot.ParentSkills != null)
        {
            foreach (var skill in skillSolot.ParentSkills)
            {
                if (skill.SkillEdges.TryGetValue(skillSolot, out level))
                {
                    _sb.AppendFormat("- {0} Lv.{1}\n", skill.SkillInfoData.SkillName, level);
                }
            }
        }

        _sb.AppendFormat("- 필요 스킬 포인트 : {0}\n", skillSolot.SkillInfoData.NeedSkillPoint);
        _sb.AppendFormat("- 제한레벨 : {0}\n", skillSolot.SkillInfoData.NeedPlayerLevel);
        _skillDesc.text = _sb.ToString();

        _skillDesc.rectTransform.sizeDelta = new Vector2(_skillDesc.rectTransform.sizeDelta.x, _skillDesc.preferredHeight);
        _backgroundRt.sizeDelta = new Vector2(_backgroundRt.sizeDelta.x, _skillDesc.rectTransform.rect.height + 100.0f);
    }

    // 툴팁 박스 위치는 박스 맨 왼쪽위가 마우스에 오게하고, MOUSE_GAP만큼 벌어지게한다.
    // 이유는 마우스와 같은 위치에 만들면 스킬 슬롯위에 있는지 없는지가 계속 반복되어 깜빡거리게 된다.
    public void SetPos(Vector3 pos)
    {
        Vector3 nextPos = new Vector3();
        nextPos.x = pos.x + (_backgroundRt.rect.width * 0.5f) + MOUSE_GAP;
        nextPos.y = pos.y - (_backgroundRt.rect.height * 0.5f) - MOUSE_GAP;
        transform.position = nextPos;
    }

    public void Close() => gameObject.SetActive(false);
}
