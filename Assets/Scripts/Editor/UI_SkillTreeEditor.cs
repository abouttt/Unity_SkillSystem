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

    private Transform _skillSlotsTransform;
    private string[] _skillNames;
    private int _skillIndex;
    private GameObject _skillSlot;

    private GUIStyleState _headerStyleState;
    private GUIStyle _headerStyle;

    private bool _isSkillSettingFoldOut = true;

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
        }

        InitHeaderStyle();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_rootSkills);

        EditorGUILayout.LabelField("[Add SkillSlot]", _headerStyle);
        _skillSlot = (GameObject)EditorGUILayout.ObjectField("Skill Slot Prefab", _skillSlot, typeof(GameObject), false);
        
        if (GUILayout.Button("Add Skill Slot"))
        {
            _skillSlot = Instantiate(_skillSlot);
            int index = _skillSlot.name.IndexOf("(Clone)");
            if (index > 0)
            {
                _skillSlot.name = _skillSlot.name.Substring(0, index);
            }
            _skillSlot.transform.SetParent(_skillSlotsTransform);
            _skillSlot.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            _skillSlot = null;
        }

        EditorGUILayout.Space();

        if (_skillSlotsTransform.childCount == 0)
        {
            return;
        }

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

    private UI_SkillSlot GetSkillSlot() => _skillSlotsTransform.GetChild(_skillIndex).GetComponent<UI_SkillSlot>();

    private SkillInfoData GetSkillData() => GetSkillSlot().SkillInfoData;

    private T GetChildComponent<T>(string name) => _skillSlotsTransform.GetChild(_skillIndex).Find(name).GetComponent<T>();

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
