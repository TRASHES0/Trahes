using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TextManager : MonoBehaviour
{
    public Text check_text;

    private void Awake()
    {
        
    }
    // Update is called once per frame

    public void Text_Spawn(string text)
    {
        Text newPrefab;
        newPrefab = Instantiate(check_text, new Vector2(500, 0), transform.rotation);
        newPrefab.text = text;
        newPrefab.transform.SetParent(GameObject.Find("QTE").transform, false);
    }
}
