using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Object/Skill Data", order = int.MaxValue)]
public class SkillData : ScriptableObject
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
    public string SkillDescription { get; set; }

    [field: SerializeField]
    public int MaxLevel { get; set; }

    [field: SerializeField]
    public int PlayerLevel { get; set; }

    [field: SerializeField]
    public int NeedSkillPoint { get; set; } = 1;

    [field: SerializeField]
    public int NextSkillOpenLevel { get; set; } = 1;

    [field: SerializeField]
    public bool IsParent { get; set; }
}
