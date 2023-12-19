using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class IntroStart : MonoBehaviourPunCallbacks
{

    public void start_btn()
    {
        SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.StartButton);
        StartCoroutine(IntroCoroutine(true));
    }

    IEnumerator IntroCoroutine(bool isStart)
    {
        if (isStart)
        {
            yield return new WaitForSeconds(1f);
            PhotonNetwork.LoadLevel("Lobby");
        }
        else
        {
            yield return new WaitForSeconds(1f);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
    public void quit_btn()
    {
        StartCoroutine(IntroCoroutine(false));
    }
}
