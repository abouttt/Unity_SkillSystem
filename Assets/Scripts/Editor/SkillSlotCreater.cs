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

        _skillInfoData.SkillIcon = (Sprite)EditorGUILayout.ObjectField("��ų ������", _skillInfoData.SkillIcon, typeof(Sprite), false);
        if (_skillInfoData.SkillIcon == null)
        {
            EditorGUILayout.HelpBox("��ų �������� �����ϼ���.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillInfoData.SkillFrame = (Sprite)EditorGUILayout.ObjectField("��ų ������", _skillInfoData.SkillFrame, typeof(Sprite), false);
        if (_skillInfoData.SkillFrame == null)
        {
            EditorGUILayout.HelpBox("��ų �������� �����ϼ���.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillInfoData.SkillType = (SkillInfoData.SkillTypes)EditorGUILayout.EnumPopup("��ų Ÿ��", _skillInfoData.SkillType);

        EditorGUILayout.Space();

        _skillInfoData.SkillName = EditorGUILayout.TextField("��ų �̸�", _skillInfoData.SkillName);
        if (string.IsNullOrEmpty(_skillInfoData.SkillName))
        {
            EditorGUILayout.HelpBox("��ų �̸��� �Է��ϼ���.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("��ų ����");
        _skillInfoData.SkillDesc = EditorGUILayout.TextArea(_skillInfoData.SkillDesc, GUILayout.MinHeight(50));
        if (string.IsNullOrEmpty(_skillInfoData.SkillDesc))
        {
            EditorGUILayout.HelpBox("��ų ������ �Է��ϼ���.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillInfoData.MaxLevel = EditorGUILayout.IntField("�ִ� ����", _skillInfoData.MaxLevel);
        if (_skillInfoData.MaxLevel < 1)
        {
            EditorGUILayout.HelpBox("��ų �ִ뷹���� 1���� �̻����̾�� �մϴ�.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillInfoData.PlayerLevel = EditorGUILayout.IntField("�÷��̾� ���ѷ���", _skillInfoData.PlayerLevel);
        if (_skillInfoData.PlayerLevel < 1)
        {
            EditorGUILayout.HelpBox("�÷��̾� ���� ������ 1���� �̻��̾�� �մϴ�.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillInfoData.NeedSkillPoint = EditorGUILayout.IntField("�ʿ� ��ų ����Ʈ", _skillInfoData.NeedSkillPoint);
        if (_skillInfoData.NeedSkillPoint < 1)
        {
            EditorGUILayout.HelpBox("�ʿ� ��ų ����Ʈ�� 1����Ʈ �̻��̾�� �մϴ�.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillInfoDataName = EditorGUILayout.TextField("SkillInfoData �����̸�", _skillInfoDataName);
        if (string.IsNullOrEmpty(_skillInfoDataName))
        {
            EditorGUILayout.HelpBox("�����̸��� �Է��ϼ���.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillSlotName = EditorGUILayout.TextField("��ų���� �̸�", _skillSlotName);
        if (string.IsNullOrEmpty(_skillSlotName))
        {
            EditorGUILayout.HelpBox("��ų ���� �̸��� �Է��ϼ���.", MessageType.Error, true);
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
            EditorGUILayout.HelpBox("�ʼ� �Է� ������ �����ϼ���.", MessageType.Error, true);
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
