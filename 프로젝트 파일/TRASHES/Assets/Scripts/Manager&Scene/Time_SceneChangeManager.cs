using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class Time_SceneChangeManager : MonoBehaviourPun
{
    public Text timeText;
    private float time;
    private bool isZero = false;

    private void Awake()
    {
        // time�� 0�� �Ǹ� �� ü����
        time = 30f;
    }

    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
    }
    private void Update()
    {
        if (time > 0)
            time -= Time.deltaTime;
        else if(!isZero)
        {
            isZero = true;
            StartCoroutine(Destroy());
        }
           

        timeText.text = Mathf.Ceil(time).ToString();
    }

    IEnumerator Destroy()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            //Debug.Log("hi");
            if (player.GetComponent<PhotonView>().OwnerActorNr == PhotonNetwork.LocalPlayer.GetPlayerNumber())
            {
                player.GetComponent<Simple_Char_Move>().SetIdle();
                PhotonNetwork.Destroy(player);
            }
        }
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.IsMessageQueueRunning = false;
        LoadingSceneController.Instance.LoadStart();
        yield return new WaitForSeconds(1f);
        int selectLoadLevel = UnityEngine.Random.Range(0, 3);
        
        switch (selectLoadLevel)
        {
            case 0:
                PhotonNetwork.LoadLevel("NewForest");
                break;
            case 1:
                PhotonNetwork.LoadLevel("NewDesert");
                break;
            case 2:
                PhotonNetwork.LoadLevel("NewFactory");
                break;
            default:
                PhotonNetwork.LoadLevel("NewForest");
                break;
        }
    }

}
