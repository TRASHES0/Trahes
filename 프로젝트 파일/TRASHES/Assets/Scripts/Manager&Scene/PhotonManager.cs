using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "v1.0";
    private string userId = "0jui";

    public TMP_InputField userIdText;
    public TMP_InputField roomNameText;

    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();

    public GameObject roomPrefab;

    public Transform scrollContent;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.GameVersion = gameVersion;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            // Remember you do not need to disconnect and connect again, you can join lobbies and create rooms
            PhotonNetwork.Disconnect();
        }
    }
    
    void Start()
    {
        LoadingSceneController.Instance.LoadStart();
        Screen.SetResolution(1920, 1080, false);
        Debug.Log("Game Started...");

        PhotonNetwork.IsMessageQueueRunning = true;
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 100):00}");
        userIdText.text = userId;
        PhotonNetwork.NickName = userId;
        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected...");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby Joined...");
        LoadingSceneController.Instance.LoadEnd();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed..." + returnCode);

        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 3;

        roomNameText.text = $"Room_{Random.Range(1, 100):000}";

        PhotonNetwork.CreateRoom("room_1", ro);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Creating Room...");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined...");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Room");
        }
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;
        foreach (var room in roomList)
        {
            if (room.RemovedFromList == true)
            {
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            else
            {
                if (roomDict.ContainsKey(room.Name) == false)
                {
                    GameObject _room = Instantiate(roomPrefab, scrollContent);
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    roomDict.Add(room.Name, _room);
                }
                else
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    if (room.IsOpen == false)
                    {
                        tempRoom.GetComponent<RoomData>().GetComponent<Button>().interactable = false;
                        tempRoom.GetComponent<RoomData>().GetComponent<Image>().color = Color.grey;
                    }
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }
            }
        }
    }

    #region UI_BUTTON_CALLBACK

    public void OnRandomBtn()
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 3;

        if (string.IsNullOrEmpty(userIdText.text))
        {
            userId = $"USER_{Random.Range(0, 100):00}";
            userIdText.text = userId;
        }
        if (string.IsNullOrEmpty(roomNameText.text))
        {
            roomNameText.text = $"ROOM_{Random.Range(0, 100):000}";
        }

        PlayerPrefs.SetString("USER_ID", userIdText.text);
        PhotonNetwork.NickName = userIdText.text;
        SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.ButtonClick);
        PhotonNetwork.JoinRandomOrCreateRoom(null, 3, MatchmakingMode.FillRoom, null, null, roomNameText.text, ro);
    }

    public void OnMakeRoomClick()
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 3;


        if (string.IsNullOrEmpty(roomNameText.text))
        {
            roomNameText.text = $"ROOM_{Random.Range(0, 100):000}";
        }

        PlayerPrefs.SetString("USER_ID", userIdText.text);
        PhotonNetwork.NickName = userIdText.text;
        SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.ButtonClick);
        PhotonNetwork.CreateRoom(roomNameText.text, ro);
    }
    #endregion
}
