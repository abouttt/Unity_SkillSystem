using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(UI_SkillTree))]
[CanEditMultipleObjects]
public class UI_SkillTreeEditor : Editor
{
    private UI_SkillTree _skillTree;

    private SerializedProperty _rootSkills;
    private SerializedProperty _skillSlotEdges;

    private Transform _skills;
    private string[] _skillNames;
    private int _skillIndex;

    private GUIStyleState _headerStyleState;
    private GUIStyle _headerStyle;

    private bool _isSkillSettingFoldOut = true;

    private void OnEnable()
    {
        _skillTree = target as UI_SkillTree;

        _rootSkills = serializedObject.FindProperty("_rootSkills");
        _skillSlotEdges = serializedObject.FindProperty("_skillEdges");
        _skills = _skillTree.transform.Find("Skills");

        _skillNames = new string[_skills.childCount];
        for (int i = 0; i < _skills.childCount; i++)
        {
            _skillNames[i] = _skills.GetChild(i).name;
        }

        InitHeaderStyle();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_rootSkills);

        #region SkillSetting
        if (_isSkillSettingFoldOut = EditorGUILayout.Foldout(_isSkillSettingFoldOut, "Skill Setting"))
        {
            EditorGUILayout.Space();

            // Skill Option
            EditorGUILayout.LabelField("[Skill Option]", _headerStyle);
            _skillIndex = EditorGUILayout.Popup("Skill List", _skillIndex, _skillNames);

            GetSkillData().SkillIcon = (Sprite)EditorGUILayout.ObjectField("Skill Icon", GetSkillData().SkillIcon, typeof(Sprite), false);
            GetChildComponent<Image>("SkillIcon").sprite = GetSkillData().SkillIcon;

            GetSkillData().SkillFrame = (Sprite)EditorGUILayout.ObjectField("Skill Frame", GetSkillData().SkillFrame, typeof(Sprite), false);
            GetChildComponent<Image>("SkillFrame").sprite = GetSkillData().SkillFrame;

            GetSkillData().SkillType = (SkillData.SkillTypes)EditorGUILayout.EnumPopup("Skill Type", GetSkillData().SkillType);
            GetSkillData().SkillName = EditorGUILayout.TextField("Skill Name", GetSkillData().SkillName);
            GetSkillData().SkillDescription = EditorGUILayout.TextField("Skill Description", GetSkillData().SkillDescription);
            GetSkillData().MaxLevel = EditorGUILayout.IntField("Max Level", GetSkillData().MaxLevel);
            GetSkillSlot().CurrentLevel = EditorGUILayout.IntField("Current Level", GetSkillSlot().CurrentLevel);
            GetSkillData().NextSkillOpenLevel = EditorGUILayout.IntField("Next Skill Open Level", GetSkillData().NextSkillOpenLevel);
            GetSkillData().IsParent = EditorGUILayout.Toggle("Is Parent Skill", GetSkillData().IsParent);

            EditorGUILayout.Space();

            _skillTree.SetSelectSkillSlotEdges(GetSkillSlot().SkillEdges);
            EditorGUILayout.PropertyField(_skillSlotEdges);

            // End

            EditorGUILayout.Space();

            // Skill Condition
            EditorGUILayout.LabelField("[Skill Level Up Condition]", _headerStyle);
            GetSkillData().PlayerLevel = EditorGUILayout.IntField("Player Level", GetSkillData().PlayerLevel);
            GetSkillData().NeedSkillPoint = EditorGUILayout.IntField("Need Skill Point", GetSkillData().NeedSkillPoint);
            // End
        }
        #endregion

        EditorUtility.SetDirty(_skillTree);
        EditorUtility.SetDirty(GetSkillSlot());
        EditorUtility.SetDirty(GetSkillData());
        EditorUtility.SetDirty(GetChildComponent<Image>("SkillIcon"));
        EditorUtility.SetDirty(GetChildComponent<Image>("SkillFrame"));
        serializedObject.ApplyModifiedProperties();
    }

    private UI_SkillSlot GetSkillSlot() => _skills.GetChild(_skillIndex).GetComponent<UI_SkillSlot>();

    private SkillData GetSkillData() => GetSkillSlot().SkillData;

    private T GetChildComponent<T>(string name) => _skills.GetChild(_skillIndex).Find(name).GetComponent<T>();

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
