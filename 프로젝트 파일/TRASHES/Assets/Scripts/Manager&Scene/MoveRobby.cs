using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;


public class MoveRobby : MonoBehaviourPunCallbacks
{
    public GameObject Player;

    public void LeaveRoom()
    {
        if (SceneManager.GetActiveScene().name == "NewForest" || SceneManager.GetActiveScene().name == "NewDesert" || SceneManager.GetActiveScene().name == "NewFactory")
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                //Debug.Log("hi");
                if (player.GetComponent<PhotonView>().OwnerActorNr == PhotonNetwork.LocalPlayer.GetPlayerNumber())
                {
                    Player = player;
                    //Debug.Log(player.GetComponent<Simple_Char_Move>().Get_PA().ToString());
                    //Debug.Log(player.GetComponent<PhotonView>().OwnerActorNr.ToString());
                    //Debug.Log(PhotonNetwork.LocalPlayer.GetPlayerNumber().ToString());
                }
            }
            Player.GetComponent<Simple_Char_Move>().set_initStat();
        }
        SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.ButtonClick);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene().name == "Room" || SceneManager.GetActiveScene().name == "NewForest" || SceneManager.GetActiveScene().name == "NewDesert" || SceneManager.GetActiveScene().name == "NewFactory")
        {
            PhotonNetwork.IsMessageQueueRunning = false;
            SceneManager.LoadScene("Lobby");
            return;
        }
    }
}
