using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _button;
    string chatInput = "";
    public Text ChatLog;
    public InputField ChatIn;

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
        }
        /*
        else
        {
            photonView.ViewID = 2;
        }
        */
        
        photonView.RPC("JoinMsg", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    void Update(){
        if(!PhotonNetwork.IsMasterClient){
            _button.gameObject.SetActive(false);
        }
        else{
            _button.gameObject.SetActive(true);
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