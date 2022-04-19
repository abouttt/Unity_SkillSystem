using UnityEngine;
using TMPro;

// 테스트하기 위함 샘플 플레이어 클래스
// 레벨업을 하고 스킬 포인트 추가하는 기능을 한다.
public class Player : MonoBehaviour
{
    #region 싱글톤
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
