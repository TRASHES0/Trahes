using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ChatManager : MonoBehaviourPun
{
    public bool isChatting = false;
    string chatInput = "";
    public Text ChatLog;
    public GameObject ChatUI;
    public InputField ChatIn;

    [System.Serializable]
    public class ChatMessage
    {
        public string sender = "";
        public string message = "";
    }

    List<ChatMessage> chatMessages = new List<ChatMessage>();

    // Start is called before the first frame update
    void Start()
    {
        //Initialize Photon View
        if(gameObject.GetComponent<PhotonView>() == null)
        {
            PhotonView photonView = gameObject.AddComponent<PhotonView>();
            photonView.ViewID = 2;
        }
        else
        {
            photonView.ViewID = 2; // 플레이어 두개 생기는 버그 문제인듯
        }
    }

    public void ChatOn(){
        if(!isChatting){
            ChatUI.SetActive(true);
            isChatting = true;
            chatInput = "";
            ChatIn.Select();
        }
        else{
            ChatUI.SetActive(false);
            isChatting = false;
            chatInput = "";
        }
    }

    public void SendM(){
        if(ChatIn.text != ""){
            chatInput = ChatIn.text;
            photonView.RPC("SendChat", RpcTarget.All, PhotonNetwork.LocalPlayer, chatInput);
            ChatIn.text = "";
        }
    }

    void OnJoinedRoom(){
        photonView.RPC("JoinMsg", RpcTarget.All, PhotonNetwork.LocalPlayer);
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

    void JoinMsg(Player player){
        ChatLog.text += player.NickName + " joined the game.";
    }
}