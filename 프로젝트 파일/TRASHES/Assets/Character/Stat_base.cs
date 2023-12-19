using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Stat", menuName = "New Stat/Stat")]

public class Stat_base : ScriptableObject
{
    public float speed_set;
    public int MAX_HP_set;
    public float firelate_set;
    public float firedelay_set;
    public int Max_Bullet_set;
    public float regenerativeTime;
    public AudioClip ghost;
    public AudioClip[] skill2Sound;
    
}
