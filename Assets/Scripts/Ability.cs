using System;
using UnityEngine;

#pragma warning disable 0649
[System.Serializable]
public abstract class Ability : MonoBehaviour
{
    [SerializeField] private Sprite skillImg;
    [SerializeField] private string skillDesc;
    [SerializeField] private string skillName;

    public Sprite SkillImg => skillImg;
    public string SkillDesc => skillDesc;
    public string SkillName => skillName;

    public abstract void SkillCall();
}
#pragma warning restore 0649