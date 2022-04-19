using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillInfoData", menuName = "Scriptable Object/Skill Info Data", order = int.MaxValue)]
public class SkillInfoData : ScriptableObject
{
    public enum SkillTypes
    {
        Active,
        Passive,
    }

    [field: SerializeField]
    public Sprite SkillIcon { get; set; }

    [field: SerializeField]
    public Sprite SkillFrame { get; set; }

    [field: SerializeField]
    public SkillTypes SkillType { get; set; }

    [field: SerializeField]
    public string SkillName { get; set; }

    [field: SerializeField]
    public string SkillDesc { get; set; }

    [field: SerializeField]
    public int MaxLevel { get; set; }

    [field: SerializeField]
    public int NeedPlayerLevel { get; set; } = 1;

    [field: SerializeField]
    public int NeedSkillPoint { get; set; } = 1;
}
