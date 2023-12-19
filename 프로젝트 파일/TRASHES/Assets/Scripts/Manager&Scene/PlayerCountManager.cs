using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.UI;


public class PlayerCountManager : MonoBehaviourPun
{
    private int _playerCount;
    private bool _doOnce = true;
    private bool[] _deadPlayer = new bool[] { false, false, false, false };
    // Start is called before the first frame update
    
    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        _playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerCount == 1 && _doOnce)
        {
            _doOnce = false;
            photonView.RPC(nameof(WhoWinRPC), RpcTarget.All);
        }
    }

    public void WhoDie(int diedPlayerNum, String diedPlayerNick, String causeofDeath)
    {
        photonView.RPC(nameof(WhoDieRPC), RpcTarget.All, diedPlayerNum, diedPlayerNick, causeofDeath);
    }

    [PunRPC]
    void WhoDieRPC(int diedPlayerNum, String diedPlayerNick, String causeOfDeath)
    {
        Debug.Log(diedPlayerNick +" 죽음, 원인 : " + causeOfDeath);
        //Debug.Log("Before "+_playerCount);
        
        _playerCount--;
        _deadPlayer[diedPlayerNum] = true;
        //Debug.Log(_deadPlayer[diedPlayerNum].ToString());
        //Debug.Log("After "+_playerCount);
        
        GameObject.FindWithTag("MainUI").GetComponent<PlayerUI>().LogUpdate(diedPlayerNick, causeOfDeath);
    }

    [PunRPC]
    void WhoWinRPC()
    {
        for (int i = 1; i < 4; i++)
        {
            if (!_deadPlayer[i] && i == PhotonNetwork.LocalPlayer.GetPlayerNumber())
            {
                Debug.Log(i+" "+PhotonNetwork.LocalPlayer.GetPlayerNumber());
                //GameObject.FindGameObjectWithTag("MainUI").GetComponent<PlayerUI>().NoticeResult(true);
                GameObject.FindWithTag("Player").GetComponent<Simple_Char_Move>().isGameDone = true;
            }
        }
    }
}
