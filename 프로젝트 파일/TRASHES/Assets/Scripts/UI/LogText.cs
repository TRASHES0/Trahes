
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogText : MonoBehaviour
{
    [SerializeField]
    private Text _text;
    
    public void SetLogInfo(String log){
        _text.text = log;
    }
}
