using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class TrapDoor : MonoBehaviourPun
{
    // Start is called before the first frame update

    public void Exit(int enteredPlayerNum)
    {
        Debug.Log("enter player num = "+enteredPlayerNum);
        photonView.RPC(nameof(ExitRPC), RpcTarget.All, enteredPlayerNum);
    }

    [PunRPC]
    private void ExitRPC(int exitedPlayerNum)//, PhotonMessageInfo info)
    {
        //float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        //Debug.Log(lag);
        
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Debug.Log("enter player num = "+exitedPlayerNum+" find player actor num = "+ player.GetComponent<PhotonView>().OwnerActorNr);
            player.GetComponent<Simple_Char_Move>().Set_Exit(true);
            if (player.GetComponent<PhotonView>().OwnerActorNr != exitedPlayerNum)
            {
                player.GetComponent<Simple_Char_Move>().Set_Timer(0);
            }
        }
    }
    
}
