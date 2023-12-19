using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;

public class InGameRoomChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    string chatInput = "";
    public Text ChatLog;
    public InputField ChatIn;
    private Simple_Char_Move MyChar;

    [System.Serializable]
    public class ChatMessage
    {
        public string sender = "";
        public string message = "";
    }

    List<ChatMessage> chatMessages = new List<ChatMessage>();

    void Start()
    {
        //Initialize Photon View
        if(gameObject.GetComponent<PhotonView>() == null)
        { 
            Debug.Log("Null");  
            //PhotonView photonView = gameObject.AddComponent<PhotonView>();
            //photonView.ViewID = 2;
        }
        /*
        else
        {
            photonView.ViewID = 2; // 플레이어 두개 생기는 버그 문제인듯
        }*/

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            //Debug.Log("hi");
            if (player.GetComponent<PhotonView>().OwnerActorNr == PhotonNetwork.LocalPlayer.GetPlayerNumber())
            {
                MyChar = player.GetComponent<Simple_Char_Move>();
            }
        }
        
        photonView.RPC("JoinMsg", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    private void Update()
    {
        if (ChatIn.isFocused && photonView.IsMine)
        {
            MyChar.isChatting = true;
        }
        else
        {
            MyChar.isChatting = false;
        }
    }

    public void SendM(){
        if(ChatIn.text != ""){
            chatInput = ChatIn.text;
            photonView.RPC("SendChat", RpcTarget.All, PhotonNetwork.LocalPlayer, chatInput);
            ChatIn.text = "";
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ChatLog.text += "\n" + otherPlayer.NickName + " left the game.\n";
    }

    [PunRPC]
    void SendChat(Player sender, string message)
    {
        ChatMessage m = new ChatMessage();
        m.sender = sender.NickName;
        m.message = message;

        chatMessages.Add(m);
        if(ChatLog.text == "")
            ChatLog.text += sender.NickName + ": " +  message;
        else
            ChatLog.text += "\n" + sender.NickName + ": " +  message;
    }

    [PunRPC]
    void JoinMsg(Player player){
        ChatLog.text += "\n" + player.NickName + " joined the game.\n";
    }
}