using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hat", menuName = "Hat/Type")]
public class Hat : ScriptableObject
{
    public string characterName;
    public string characterDesc;
    public Sprite sprt;

    public int dashLength;
    public int dashRegen;
    public int dashDmg;
    public float spd;
}
