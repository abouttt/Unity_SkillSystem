using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

// UI_SkillTree Ŀ���� ������
[CustomEditor(typeof(UI_SkillTree))]
[CanEditMultipleObjects]
public class UI_SkillTreeEditor : Editor
{
    private UI_SkillTree _skillTree;

    // ListŸ�԰� Dictionary�� Inspector�� ���̰� �ϱ� ���ؼ��� ������ �����;� �Ѵ�.
    private SerializedProperty _rootSkills;
    private SerializedProperty _skillSlotEdges;

    // ��ų ���� ������Ʈ���� ������ �θ� ������Ʈ Ʈ������.
    private Transform _skillSlotsTransform;
    private string[] _skillNames;    // ��ϵ� ��ų���� �̸�.
    private int _skillIndex;         // ������ ��ų �ε���.
    private GameObject _skillSlot;   // ������ ������ ��ų ������ �����ϱ� ���� ����.

    // �ؽ�Ʈ ��Ÿ��.
    private GUIStyleState _headerStyleState;
    private GUIStyle _headerStyle;

    // ���� ��⸦ ���� ����
    private bool _isSkillSettingFoldOut = true;

    // ���� �ʱ�ȭ�� ���� ���� �������� �����´�.
    // ��ϵ� ��ų ���� ��ŭ ���ڿ� �迭�� �����Ͽ� ����Ѵ�.
    // Ŀ���� �����͸� ���� ������ �ʰ� �ٸ� ������� ���� ��ų ������ ����
    // UI_SkillTree�� Ŭ���� ��ų �����ܰ� �������� SkillInfoData���� ������ �����Ѵ�.
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

        // ��ų ������ �߰��� �� Instantiate�޼���� �����ϴ� ���̱� ������ �ڿ� (Clone)�� �ٴ´�.
        // �̰��� ���ִ� �۾��� ���ְ� �ʱ� ��ġ�� anchors�� �ٸ� ��ǥ (0, 0)�� ��ġ�Ѵ�.
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

        // ��ϵ� ��ų ���� ������Ʈ�� ������ �޼��带 �ٷ� �����.
        // ���� �޼��带 ���� �ڵ���� ������ �ʿ䰡 ����.
        if (_skillSlotsTransform.childCount == 0)
        {
            return;
        }

        // SkillInfoData�� �ִ� �������� �����ְ� �Է��� �� �ֵ��� �Ѵ�.

        // ���� ��� ���.
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

        // ������ ������ ������ ��带 ��� �� �����Ѵ�.
        // �̷��� ���� ������ ������ ��忡�� ������ �����ʹ� ���󰣴�.
        EditorUtility.SetDirty(_skillTree);
        EditorUtility.SetDirty(GetSkillSlot());
        EditorUtility.SetDirty(GetSkillData());
        EditorUtility.SetDirty(GetChildComponent<Image>(_skillIndex, "SkillIcon"));
        EditorUtility.SetDirty(GetChildComponent<Image>(_skillIndex, "SkillFrame"));
        serializedObject.ApplyModifiedProperties();
    }

    // ������ ��ų ������ ��ȯ�Ѵ�.
    private UI_SkillSlot GetSkillSlot() => _skillSlotsTransform.GetChild(_skillIndex).GetComponent<UI_SkillSlot>();

    // ������ ��ų ���Կ� �ִ� SkillData�� ��ȯ�Ѵ�.
    private SkillInfoData GetSkillData() => GetSkillSlot().SkillInfoData;

    // �ش� �ε����� �ش��ϴ� �ڽĿ�����Ʈ���� name�� ���� �ڽ� ������Ʈ�� ã�� ��ϵ� ������Ʈ�� ��ȯ�Ѵ�.
    private T GetChildComponent<T>(int skillIndex, string name) => _skillSlotsTransform.GetChild(skillIndex).Find(name).GetComponent<T>();

    // �۾�ü �ʱ�ȭ.
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
