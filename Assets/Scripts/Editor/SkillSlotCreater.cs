using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class SkillSlotCreater : EditorWindow
{
    private SkillInfoData _skillInfoData = null;
    private string _skillInfoDataName;
    private string _skillSlotName;

    private bool _isAllPass = false;

    private const string SKILL_INFO_DATA_PATH = "Assets/Data/SkillInfo/";
    private const string SKILL_SLOT_PATH = "Assets/Resources/Prefabs/UI/SkillSlots/";

    [MenuItem("Window/Skill Slot Creater")]
    private static void Init()
    {
        SkillSlotCreater window = GetWindow<SkillSlotCreater>();
        window.minSize = new Vector2(100, 100);
        window.maxSize = new Vector2(1000, 1000);
        window.Show();
    }

    private void OnGUI()
    {
        if (_skillInfoData == null)
        {
            _skillInfoData = ScriptableObject.CreateInstance<SkillInfoData>();
        }

        _isAllPass = true;

        _skillInfoData.SkillIcon = (Sprite)EditorGUILayout.ObjectField("스킬 아이콘", _skillInfoData.SkillIcon, typeof(Sprite), false);
        if (_skillInfoData.SkillIcon == null)
        {
            EditorGUILayout.HelpBox("스킬 아이콘을 선택하세요.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillInfoData.SkillFrame = (Sprite)EditorGUILayout.ObjectField("스킬 프레임", _skillInfoData.SkillFrame, typeof(Sprite), false);
        if (_skillInfoData.SkillFrame == null)
        {
            EditorGUILayout.HelpBox("스킬 프레임을 선택하세요.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillInfoData.SkillType = (SkillInfoData.SkillTypes)EditorGUILayout.EnumPopup("스킬 타입", _skillInfoData.SkillType);

        EditorGUILayout.Space();

        _skillInfoData.SkillName = EditorGUILayout.TextField("스킬 이름", _skillInfoData.SkillName);
        if (string.IsNullOrEmpty(_skillInfoData.SkillName))
        {
            EditorGUILayout.HelpBox("스킬 이름을 입력하세요.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("스킬 설명");
        _skillInfoData.SkillDesc = EditorGUILayout.TextArea(_skillInfoData.SkillDesc, GUILayout.MinHeight(50));
        if (string.IsNullOrEmpty(_skillInfoData.SkillDesc))
        {
            EditorGUILayout.HelpBox("스킬 설명을 입력하세요.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillInfoData.MaxLevel = EditorGUILayout.IntField("최대 레벨", _skillInfoData.MaxLevel);
        if (_skillInfoData.MaxLevel < 1)
        {
            EditorGUILayout.HelpBox("스킬 최대레벨은 1레벨 이상을이어야 합니다.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillInfoData.PlayerLevel = EditorGUILayout.IntField("플레이어 제한레벨", _skillInfoData.PlayerLevel);
        if (_skillInfoData.PlayerLevel < 1)
        {
            EditorGUILayout.HelpBox("플레이어 제한 레벨은 1레벨 이상이어야 합니다.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillInfoData.NeedSkillPoint = EditorGUILayout.IntField("필요 스킬 포인트", _skillInfoData.NeedSkillPoint);
        if (_skillInfoData.NeedSkillPoint < 1)
        {
            EditorGUILayout.HelpBox("필요 스킬 포인트는 1포인트 이상이어야 합니다.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillInfoDataName = EditorGUILayout.TextField("SkillInfoData 파일이름", _skillInfoDataName);
        if (string.IsNullOrEmpty(_skillInfoDataName))
        {
            EditorGUILayout.HelpBox("파일이름을 입력하세요.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillSlotName = EditorGUILayout.TextField("스킬슬롯 이름", _skillSlotName);
        if (string.IsNullOrEmpty(_skillSlotName))
        {
            EditorGUILayout.HelpBox("스킬 슬롯 이름을 입력하세요.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Skill"))
        {
            if (_isAllPass)
            {
                CreateSkillSlot();
            }
        }

        if (!_isAllPass)
        {
            EditorGUILayout.HelpBox("필수 입력 사항을 만족하세요.", MessageType.Error, true);
        }

        EditorUtility.SetDirty(_skillInfoData);
    }

    private void CreateSkillSlot()
    {
        _skillInfoDataName.Replace(" ", "");
        _skillSlotName.Replace(" ", "");

        AssetDatabase.DeleteAsset($"{SKILL_INFO_DATA_PATH}{_skillInfoDataName}.Asset");
        AssetDatabase.CreateAsset(_skillInfoData, $"{SKILL_INFO_DATA_PATH}{_skillInfoDataName}.Asset");
        
        GameObject skillSlot = Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI_SkillSlot"));
        skillSlot.GetComponent<UI_SkillSlot>().SkillInfoData = _skillInfoData;
        skillSlot.transform.Find("SkillIcon").GetComponent<Image>().sprite = _skillInfoData.SkillIcon;
        skillSlot.transform.Find("SkillFrame").GetComponent<Image>().sprite = _skillInfoData.SkillFrame;
        PrefabUtility.SaveAsPrefabAsset(skillSlot, $"{SKILL_SLOT_PATH}{_skillSlotName}.prefab");
        DestroyImmediate(skillSlot);

        AssetDatabase.SaveAssets();
    }
}
