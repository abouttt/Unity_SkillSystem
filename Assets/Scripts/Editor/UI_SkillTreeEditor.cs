using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

// UI_SkillTree 커스텀 에디터
[CustomEditor(typeof(UI_SkillTree))]
[CanEditMultipleObjects]
public class UI_SkillTreeEditor : Editor
{
    private UI_SkillTree _skillTree;

    // List타입과 Dictionary는 Inspector에 보이게 하기 위해서는 원본을 가져와야 한다.
    private SerializedProperty _rootSkills;
    private SerializedProperty _skillSlotEdges;

    // 스킬 슬롯 오브젝트들을 가지는 부모 오브젝트 트랜스폼.
    private Transform _skillSlotsTransform;
    private string[] _skillNames;    // 등록된 스킬들의 이름.
    private int _skillIndex;         // 선택한 스킬 인덱스.
    private GameObject _skillSlot;   // 프리팹 형태의 스킬 슬롯을 선택하기 위한 변수.

    // 텍스트 스타일.
    private GUIStyleState _headerStyleState;
    private GUIStyle _headerStyle;

    // 접고 펴기를 위한 변수
    private bool _isSkillSettingFoldOut = true;

    // 변수 초기화를 위해 원본 변수들을 가져온다.
    // 등록된 스킬 슬롯 만큼 문자열 배열을 생성하여 등록한다.
    // 커스텀 에디터를 통해 만들지 않고 다른 방법으로 만든 스킬 슬롯을 위해
    // UI_SkillTree를 클릭시 스킬 아이콘과 프레임을 SkillInfoData에서 가져와 설정한다.
    private void OnEnable()
    {
        _skillTree = target as UI_SkillTree;

        _rootSkills = serializedObject.FindProperty("_rootSkills");
        _skillSlotEdges = serializedObject.FindProperty("_skillEdges");
        _skillSlotsTransform = _skillTree.transform.Find("SkillSlots");

        _skillNames = new string[_skillSlotsTransform.childCount];
        for (int i = 0; i < _skillSlotsTransform.childCount; i++)
        {
            _skillNames[i] = _skillSlotsTransform.GetChild(i).name;
            GetChildComponent<Image>(i, "SkillIcon").sprite =
                _skillSlotsTransform.GetChild(i).GetComponent<UI_SkillSlot>().SkillInfoData.SkillIcon;
            GetChildComponent<Image>(i, "SkillFrame").sprite =
                _skillSlotsTransform.GetChild(i).GetComponent<UI_SkillSlot>().SkillInfoData.SkillFrame;
        }

        InitHeaderStyle();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_rootSkills);

        EditorGUILayout.LabelField("[Add Skill Slot]", _headerStyle);
        _skillSlot = (GameObject)EditorGUILayout.ObjectField("Skill Slot Prefab", _skillSlot, typeof(GameObject), false);

        // 스킬 슬롯을 추가할 때 Instantiate메서드로 생성하는 것이기 때문에 뒤에 (Clone)이 붙는다.
        // 이것을 없애는 작업을 해주고 초기 위치를 anchors에 다른 좌표 (0, 0)에 배치한다.
        if (GUILayout.Button("Add Skill Slot"))
        {
            _skillSlot = Instantiate(_skillSlot);
            int index = _skillSlot.name.IndexOf("(Clone)");
            if (index > 0)
            {
                _skillSlot.name = _skillSlot.name.Substring(0, index);
            }
            _skillSlot.transform.SetParent(_skillSlotsTransform);
            _skillSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
            _skillSlot = null;
        }

        EditorGUILayout.Space();

        // 등록된 스킬 슬롯 오브젝트가 없으면 메서드를 바로 벗어난다.
        // 굳이 메서드를 하위 코드들을 실행할 필요가 없다.
        if (_skillSlotsTransform.childCount == 0)
        {
            return;
        }

        // SkillInfoData에 있는 변수들을 보여주고 입력할 수 있도록 한다.

        // 접고 펴기 기능.
        #region SkillSetting
        if (_isSkillSettingFoldOut = EditorGUILayout.Foldout(_isSkillSettingFoldOut, "Skill Setting"))
        {
            EditorGUILayout.Space();

            // Skill Option
            EditorGUILayout.LabelField("[Skill Option]", _headerStyle);
            _skillIndex = EditorGUILayout.Popup("Skill List", _skillIndex, _skillNames);

            GetSkillData().SkillIcon = (Sprite)EditorGUILayout.ObjectField("Skill Icon", GetSkillData().SkillIcon, typeof(Sprite), false);
            GetChildComponent<Image>(_skillIndex, "SkillIcon").sprite = GetSkillData().SkillIcon;

            GetSkillData().SkillFrame = (Sprite)EditorGUILayout.ObjectField("Skill Frame", GetSkillData().SkillFrame, typeof(Sprite), false);
            GetChildComponent<Image>(_skillIndex, "SkillFrame").sprite = GetSkillData().SkillFrame;

            GetSkillData().SkillType = (SkillInfoData.SkillTypes)EditorGUILayout.EnumPopup("Skill Type", GetSkillData().SkillType);
            GetSkillData().SkillName = EditorGUILayout.TextField("Skill Name", GetSkillData().SkillName);
            GetSkillData().SkillDesc = EditorGUILayout.TextField("Skill Description", GetSkillData().SkillDesc);
            GetSkillData().MaxLevel = EditorGUILayout.IntField("Max Level", GetSkillData().MaxLevel);
            GetSkillSlot().CurrentLevel = EditorGUILayout.IntField("Current Level", GetSkillSlot().CurrentLevel);

            EditorGUILayout.Space();

            _skillTree.SetSelectSkillSlotEdges(GetSkillSlot().SkillEdges);
            EditorGUILayout.PropertyField(_skillSlotEdges);
            // End

            EditorGUILayout.Space();

            // Skill Condition
            EditorGUILayout.LabelField("[Skill Level Up Condition]", _headerStyle);
            GetSkillData().NeedPlayerLevel = EditorGUILayout.IntField("Need Player Level", GetSkillData().NeedPlayerLevel);
            GetSkillData().NeedSkillPoint = EditorGUILayout.IntField("Need Skill Point", GetSkillData().NeedSkillPoint);
            // End
        }
        #endregion

        // 변경한 값들을 에디터 모드를 벗어날 시 저장한다.
        // 이렇게 하지 않으면 에디터 모드에서 변경한 데이터는 날라간다.
        EditorUtility.SetDirty(_skillTree);
        EditorUtility.SetDirty(GetSkillSlot());
        EditorUtility.SetDirty(GetSkillData());
        EditorUtility.SetDirty(GetChildComponent<Image>(_skillIndex, "SkillIcon"));
        EditorUtility.SetDirty(GetChildComponent<Image>(_skillIndex, "SkillFrame"));
        serializedObject.ApplyModifiedProperties();
    }

    // 선택한 스킬 슬롯을 반환한다.
    private UI_SkillSlot GetSkillSlot() => _skillSlotsTransform.GetChild(_skillIndex).GetComponent<UI_SkillSlot>();

    // 선택한 스킬 슬롯에 있는 SkillData를 반환한다.
    private SkillInfoData GetSkillData() => GetSkillSlot().SkillInfoData;

    // 해당 인덱스에 해당하는 자식오브젝트에서 name과 같은 자식 오브젝트를 찾아 등록된 컴포넌트를 반환한다.
    private T GetChildComponent<T>(int skillIndex, string name) => _skillSlotsTransform.GetChild(skillIndex).Find(name).GetComponent<T>();

    // 글씨체 초기화.
    private void InitHeaderStyle()
    {
        _headerStyleState = new GUIStyleState()
        {
            textColor = Color.white,
        };

        _headerStyle = new GUIStyle()
        {
            fontStyle = FontStyle.Bold,
            normal = _headerStyleState,
        };
    }
}
