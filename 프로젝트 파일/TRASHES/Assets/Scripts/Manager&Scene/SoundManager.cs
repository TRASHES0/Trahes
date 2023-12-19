using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource bgSound;
    public AudioSource effectSound;
    public AudioSource objectSound;
    public AudioSource walkSound;
    public AudioSource healthSound;
    public AudioClip[] bgList;
    public AudioClip[] effectList;

    public enum EffectType
    {
        ButtonClick,
        StartButton,
        SearchBox,
        QteSuccess,
        QteFail,
        EnemyHit,
        Shot,
        Hit,
        SpawnKey,
        GetKey,
        SpawnTrapdoor,
        Win,
        Die,
        Boom,
        Alarm,
        IceBullet
    };
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for (int i = 0; i < bgList.Length; i++)
        {
            Debug.Log("비교 맵 : "+ bgList[i].name + " 현재 맵 " + arg0.name);
            if (bgList[i].name == arg0.name)
            {
                BgSoundPlay(bgList[i]);
                break;
            }
        }
        
        if (walkSound.isPlaying)
        {
            walkSound.Stop();
        }
    }

    public void BgSoundPlay(AudioClip clip)
    {
        bgSound.clip = clip;
        bgSound.loop = true;
        bgSound.volume = 0.35f;
        bgSound.Play();
    }

    public void EffectSoundPlay(int type)
    {
        effectSound.volume = 0.7f;
        effectSound.PlayOneShot(effectList[type]);
    }
    
    public void SkillSoundPlay(AudioClip skillSound)
    {
        effectSound.volume = 0.7f;
        effectSound.PlayOneShot(skillSound);
    }
    
    public void ContinuousSoundPlay(int type)
    {
        if (!objectSound.isPlaying)
        {
            objectSound.clip = effectList[type];
            objectSound.volume = 0.9f;
            objectSound.Play();
        }
        else
        {
            objectSound.Stop();
        } 
    }
    
}
