using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPun
{

    public Text NoticeText;
    public static GameManager instance = null;
    public bool isConnect = false;
    private Transform[] spawnPoints;
    public int maxBoxes = 15;
    public List<Vector3> boxLocations = new List<Vector3>();
    public PhotonView pv;
    public Transform exitKeySpawnPoint;
    public Transform[] EnemySpawnPoints;
    private bool createPlayer;
    
    float time = 0;

    IEnumerator Start()
    {
        LoadingSceneController.Instance.LoadStart();
        PhotonNetwork.IsMessageQueueRunning = true;
        pv = GetComponent<PhotonView>();
        
        if(SceneManager.GetActiveScene().name == "NewForest" || SceneManager.GetActiveScene().name == "NewDesert" || SceneManager.GetActiveScene().name == "NewFactory")
        {
            yield return new WaitForSeconds(0.5f);
            CreatePlayer_class(GameObject.Find("P_Class_info_Manager").GetComponent<Player_Class_Info>().p_class);
            if (PhotonNetwork.IsMasterClient)
            {
                SpawnBox();
                Invoke(nameof(CreateEnemy), 60f);
                Invoke(nameof(CreateKey), 70f);
                Invoke(nameof(CreateTrapdoor), 130f);
                Invoke(nameof(CreateEnemy), 120f);
            }
        }
        else
        {
            CreatePlayer();
        }
        
        //pv.RPC(nameof(CreateBox), RpcTarget.MasterClient);
        LoadingSceneController.Instance.LoadEnd();
        NoticeMention("게임이 시작되었습니다.");//게임 시작시 문구 출력
    }

    private Vector3 GetRandomAvailableSpawnPosition()
    {
        // 사용 가능한 위치 중 무작위로 선택
        int randomIndex = Random.Range(0, boxLocations.Count);
        Vector3 spawnPosition = boxLocations[randomIndex];

        // 사용된 위치를 리스트에서 제거
        boxLocations.RemoveAt(randomIndex);
        return spawnPosition;
    }

    IEnumerator Notice_Fade(string str){

        NoticeText.gameObject.SetActive(true);
        NoticeText.text = str;
        NoticeText.color = new Color(1, 1, 1, 0);

        while(true){

            yield return null;

            if(time < 2f){
                NoticeText.color = new Color(1, 1, 1, time/2);
            }
            else if(time > 4f && time < 6f){
                NoticeText.color = new Color(1, 1, 1, 1f - 1f / (6f - time));
            }
            else if(time > 6f){
                NoticeText.text = "";
                NoticeText.gameObject.SetActive(false);
                time = 0;
                yield break;
            }
            time += Time.deltaTime;
        }
    }

    public void NoticeMention(string str)
    {
        StartCoroutine(Notice_Fade(str));
    }
    private void SpawnBox()
    {
        for(int i = 0; i < maxBoxes; i++)
        {
            Vector3 spawnPosition = GetRandomAvailableSpawnPosition();
            PhotonNetwork.InstantiateRoomObject("Box", spawnPosition, Quaternion.identity);
        }
    }
    void CreateKey()
    {
        Vector3 key_pos = exitKeySpawnPoint.position;
        photonView.RPC(nameof(LogUpdate), RpcTarget.All, "열쇠");
        PhotonNetwork.InstantiateRoomObject("ExitKey", key_pos, Quaternion.identity);
    }
    void CreateTrapdoor()
    {
        Vector3 spawnPosition = GetRandomAvailableSpawnPosition();
        photonView.RPC(nameof(LogUpdate), RpcTarget.All, "비상탈출구");
        PhotonNetwork.InstantiateRoomObject("Trapdoor", spawnPosition, Quaternion.identity);
    }
    void CreateEnemy()
    {
        //int randomIndex = Random.Range(0, 3);
        photonView.RPC(nameof(LogUpdate), RpcTarget.All, "박쥐");
        for (int i = 0; i < 3; i++)
        {
            GameObject enemy = PhotonNetwork.InstantiateRoomObject("Enemy", EnemySpawnPoints[i].position, Quaternion.identity);
        }
    }
    
    void CreatePlayer()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.GetPlayerNumber());
        spawnPoints = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

        Vector3 pos = spawnPoints[PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1].position;
        Quaternion rot = spawnPoints[PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1].rotation;
        
        PhotonNetwork.Instantiate("Default_Char", pos, rot, 0);

    }

    void CreatePlayer_class(string pClass)
    {
        Debug.Log(PhotonNetwork.LocalPlayer.GetPlayerNumber());
        spawnPoints = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        Vector3 pos = spawnPoints[PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1].position;
        Quaternion rot = spawnPoints[PhotonNetwork.LocalPlayer.GetPlayerNumber() - 1].rotation;
        PhotonNetwork.Instantiate(pClass, pos, rot, 0);
    }

    [PunRPC]
    void LogUpdate(string temp)
    {
        GameObject.FindWithTag("MainUI").GetComponent<PlayerUI>().LogUpdate_Object(temp);
    }

}
