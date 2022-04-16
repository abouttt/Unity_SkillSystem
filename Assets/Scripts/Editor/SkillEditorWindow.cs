using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class SkillEditorWindow : EditorWindow
{
    private Sprite _skillIcon = null;
    private string _scriptName;
    private string _skillName;
    private string _skillDescription;
    private int _playerLevel;
    private int _skillPoint;

    private bool _isAllPass = false;

    [MenuItem("Window/SkillEditor")]
    private static void Init()
    {
        SkillEditorWindow window = GetWindow<SkillEditorWindow>();
        window.minSize = new Vector2(600, 600);
        window.maxSize = new Vector2(600, 600);
        window.Show();
    }

    private void CreateSkillSlot()
    {
        
    }

    private void OnGUI()
    {
        _isAllPass = true;

        _skillIcon = (Sprite)EditorGUILayout.ObjectField("Skill Icon", _skillIcon, typeof(Sprite), false);
        if (_skillIcon == null)
        {
            EditorGUILayout.HelpBox("��ų �������� �����ϼ���.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _scriptName = EditorGUILayout.TextField("Script Name", _scriptName);
        if (string.IsNullOrEmpty(_scriptName))
        {
            EditorGUILayout.HelpBox("��ũ��Ʈ �̸��� �Է��ϼ���.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillName = EditorGUILayout.TextField("Skill Name", _skillName);
        if (string.IsNullOrEmpty(_skillName))
        {
            EditorGUILayout.HelpBox("��ų �̸��� �Է��ϼ���.", MessageType.Error, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillDescription = EditorGUILayout.TextField("Skill Description", _skillDescription);
        if (string.IsNullOrEmpty(_skillDescription))
        {
            EditorGUILayout.HelpBox("��ų ������ �Է��ϼ���.", MessageType.Error, true);
            _isAllPass = false;
        }

        _playerLevel = EditorGUILayout.IntField("Player Level", _playerLevel);
        if (_playerLevel < 1)
        {
            EditorGUILayout.HelpBox("�÷��̾� ���� ������ 1���� �̻��� ����.", MessageType.Warning, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        _skillPoint = EditorGUILayout.IntField("Skill Point", _skillPoint);
        if (_skillPoint < 1)
        {
            EditorGUILayout.HelpBox("�ʿ� ��ų ����Ʈ�� 1����Ʈ �̻��� ����.", MessageType.Warning, true);
            _isAllPass = false;
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Skill"))
        {
            if (_isAllPass)
            {
                CreateSkillSlot();
                Close();
            }
        }

        if (!_isAllPass)
        {
            EditorGUILayout.HelpBox("�ʼ� �Է� ������ �����ϼ���.", MessageType.Error, true);
        }
    }
}
