using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Skill", menuName = "New Skill/Skill")]
public class Skill_base : ScriptableObject
{
    public float cool;

    public string characterName;
    public string skill_name;
    public Sprite icon;
    public GameObject Prefab;
}
