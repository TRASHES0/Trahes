using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Class_Info : MonoBehaviour
{


    public string p_class;

    private void Awake()
    {
        p_class = "Default_Char";
        //p_class = "AimPossible_Char";
        DontDestroyOnLoad(this.gameObject);
    }


}
