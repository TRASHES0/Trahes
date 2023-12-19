using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    public bool state;

    public GameObject game;
    public GameManager GameManager;
    
    public GameObject UI;

    public Text Key;
    public Text Lock;

    public Text timeText;  // 타이머 관련
    private float time;
    private bool isZero = false;
    public static bool isDay = false;
    public Text Day;
    public static int wave = 1;

    public void start()
    {
        state = true;
        Day.text = wave + "일차 낮";
    }

    public void Awake()
    {
        time = 60f;
    }

    public void Update()
    {
        if (Simple_Char_Move.hasKey == true)
        {
            Key.text = "1";
        }
        else if (Simple_Char_Move.hasKey == false)
        {
            Key.text = "0";
        }

        if (Simple_Char_Move.hasLock == true)
        {
            Lock.text = "1";
        }
        else if (Simple_Char_Move.hasLock == false)
        {
            Lock.text = "0";
        }

        if (time > 0)
            time -= Time.deltaTime;
        else if (!isZero)
        {
            if (isDay == false)
            {
                isDay = true;
                Day.text = wave + "일차 밤";
                GameManager.NoticeMention(Day.text);
                time = 60f; wave++;
            }
            else
            {
                isDay = false;
                Day.text = wave + "일차 낮";
                GameManager.NoticeMention(Day.text);
                time = 60f;
            }
        }
        timeText.text = Mathf.Ceil(time).ToString();
    }

    public void ActiveGame()
    {
        game.SetActive(true);
        GameObject.Find("QTE_Minigame").GetComponent<SpawnManager>().Spawn();
    }
    public void ExitGame()
    {
        Destroy(GameObject.FindWithTag("CheckZone"));
        Destroy(GameObject.FindWithTag("SkillSpin"));
        Destroy(GameObject.FindWithTag("CheckText"));
        Destroy(GameObject.FindWithTag("Zkey"));
        Destroy(GameObject.FindWithTag("ZkeyText"));
        SoundManager.instance.ContinuousSoundPlay((int)SoundManager.EffectType.SearchBox);
        Simple_Char_Move.isDoingQTE = false;
        game.SetActive(false);
    }
    public void ActiveUI()
    {
        UI.SetActive(true);
    }
    public void ExitUI()
    {
        UI.SetActive(false);
    }

    public void UseKey()
    {
        if(Simple_Char_Move.hasLock == true)
        {
            Simple_Char_Move.hasLock = false;
        }
        if(Simple_Char_Move.hasKey == true)
        {
            Simple_Char_Move.hasKey = false;
        }
    }
}
