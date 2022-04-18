using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Player : MonoBehaviour
{
    #region ½Ì±ÛÅæ
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
}
