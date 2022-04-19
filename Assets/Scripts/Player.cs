using UnityEngine;
using TMPro;

// �׽�Ʈ�ϱ� ���� ���� �÷��̾� Ŭ����
// �������� �ϰ� ��ų ����Ʈ �߰��ϴ� ����� �Ѵ�.
public class Player : MonoBehaviour
{
    #region �̱���
    private static Player s_instance = null;
    public static Player GetInstance => s_instance;
    private void Awake() => s_instance = this;
    #endregion

    public int Level { get; private set; } = 1;

    [SerializeField]
    private TextMeshProUGUI _levelText = null;

    private void Start()
    {
        _levelText.text = Level.ToString();
    }

    public void OnButtonLevelUp()
    {
        Level++;
        _levelText.text = Level.ToString();
        UI_SkillTree.GetInstance.AddSkillPoint(1);
    }

    public void OnButtonExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
