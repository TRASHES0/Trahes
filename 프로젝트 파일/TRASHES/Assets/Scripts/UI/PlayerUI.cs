using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviourPun
{
    [SerializeField]
    private Transform _content;

    [SerializeField]
    private Text _logText;
    
    private List<Text> _list = new List<Text>();
    
    public enum Result
    {
        Win,
        Dead,
        Exit,
        OtherExit
    };
    private int start_HP;
    private int minutes;
    private int seconds;
    public bool nightCheck = true;
    private bool isGameDone = false;
    private bool _initFinish;

    public Image[] HP;

    public CanvasGroup bloodScreenGroup;
    public CanvasGroup dayGroup;
    public CanvasGroup nightGroup;
    public CanvasGroup resultGroup;
    public CanvasGroup SuccessGroup;
    public CanvasGroup FailGroup;
    public CanvasGroup noticeGroup;

    //public Text Actor;
    public Text Bullet;
    public Text timerText;
    public Text skill1_text;
    public Text skill2_text;
    public Image Skill2_Image;
    public Button resultButton;
    public Text resultText;
    private GameObject _player;

    private int _currentPlayerCount;

    // Start is called before the first frame update
    void Awake()
    {
        //Actor.text = transform.GetComponentInParent<PlayerAttack>().actorNum.ToString();
        InvokeRepeating(nameof(NoticeChangeDay), 60f, 60f);
    }

    private void Start()
    {
        _currentPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }


    // Update is called once per frame
    void Update()
    {
        if(!isGameDone && _initFinish)
        {
            // 시간을 분과 초로 나누기
            minutes = Mathf.FloorToInt(_player.GetComponent<Simple_Char_Move>().Get_Timer() / 60);
            seconds = Mathf.FloorToInt(_player.GetComponent<Simple_Char_Move>().Get_Timer() % 60);

            // UI Text에 시간 표시 (분:초 형식)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            //ShowBullet();
            //ShowHP();
            //Actor.text = transform.GetComponentInParent<PlayerAttack>().actorNum.ToString();
        }

    }
    public void init_UI()
    {
        FindPlayer();
        setStartHP();
        ShowBullet();
        Skill2_Image.sprite = _player.GetComponent<Simple_Char_Move>().Skill2.icon;
        _initFinish = true;
    }
    
    public void FindPlayer()
    {
        while (_player == null)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponent<PhotonView>().OwnerActorNr == PhotonNetwork.LocalPlayer.GetPlayerNumber())
                {
                    _player = player;
                }
            }
        }
    }
    public void ShowBullet()
    {
        Bullet.text = _player.GetComponent<Simple_Char_Move>().Get_BulletText().ToString();
    }

    public void ShowHP(int hp, int damage)
    {
        Debug.Log(hp + " " + damage);
        if (damage > 0)
        {
            gameObject.GetComponent<ImageFade>().FadeInAndOut(1.0f, bloodScreenGroup); ;
            for (int i = hp + damage; i > hp; i--)
            {
                Debug.Log(i);
                HP[i - 1].enabled = false;
            }
        }
        else
        {
            for (int i = hp + damage; i < hp; i++)
            {
                Debug.Log(i);
                HP[i].enabled = true;
            }
        }

    }

    public IEnumerator Skill_CoolTime (float cool, int WhatSkill)
    {
        print("쿨타임 코루틴 실행");
        if(WhatSkill == 1)
        {
            while (cool > 0f)
            {
                skill1_text.text = Mathf.FloorToInt(cool).ToString();
                cool -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            skill1_text.text = " ";
        }
        else if (WhatSkill == 2)
        {
            while (cool > 0f)
            {
                skill2_text.text = Mathf.FloorToInt(cool).ToString();
                cool -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            skill2_text.text = " ";
        }
        print("쿨타임 코루틴 완료");
    }
    
    public void setStartHP()
    {
        start_HP = _player.GetComponent<Simple_Char_Move>().stat.MAX_HP_set;

        for (int i = start_HP; i < 5; i++)
        {
            HP[i].enabled = false;
        }
    }

    public void NoticeChangeDay()
    {
        Debug.Log("낮밤 : " + nightCheck);
        if (nightCheck)
        {
            Debug.Log("밤입니다");
            gameObject.GetComponent<ImageFade>().FadeInAndOut(2.0f, nightGroup);
            //GetComponentInParent<ImageFade>().FadeInAndOut(2.0f, nightGroup);
        }
        else
        {
            Debug.Log("낮입니다");
            gameObject.GetComponent<ImageFade>().FadeInAndOut(2.0f, dayGroup);
            //GetComponentInParent<ImageFade>().FadeInAndOut(2.0f, dayGroup);
        }

        nightCheck = !nightCheck;
    }
    
    public void SkillFeedBack(bool success)
    {
        if (success)
        {
            gameObject.GetComponent<ImageFade>().FadeInAndOut(2.2f, SuccessGroup);
        }
        else
        {
            gameObject.GetComponent<ImageFade>().FadeInAndOut(2.2f, FailGroup);
        }
    }

    public void NoticeResult(Result result)
    {
        resultText.text = "";
        switch (result)
        {
            case Result.Win:
                resultText.text = "승리하셨습니다!";
                break;
            case Result.Dead:
                resultText.text = "죽었습니다...";
                break;
            case Result.Exit:
                resultText.text = "탈출하셨습니다!";
                break;
            case Result.OtherExit:
                resultText.text = "누군가 탈출했습니다...";
                break;
            default:
                resultText.text = "Error";
                break;
        }
        resultGroup.alpha = 1f;
        resultButton.gameObject.GetComponent<Button>().interactable = true;
        isGameDone = true;

    }

    public void LogUpdate(String diedPlayerName, String causeOfDeath)
    {
        String log = " " + diedPlayerName + "가 죽었습니다. 원인 : " + causeOfDeath;

        Text logText = Instantiate(_logText, _content);
        logText.text = log;
        if(_list != null){
            _list.Add(logText);
        }
        SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.Alarm);
    }
    
    public void LogUpdate_Object(String spawnItem)
    {
        String log = " " + spawnItem + "가 생성되었습니다!";

        Text logText = Instantiate(_logText, _content);
        logText.text = log;
        if(_list != null){
            _list.Add(logText);
        }
        if (spawnItem == "열쇠")
        {
            SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.SpawnKey);
        }
        else if (spawnItem == "비상탈출구")
        {
            SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.SpawnTrapdoor);
        }
        else if (spawnItem == "박쥐")
        {
            SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.Alarm);
        }
    }


}
