using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ButtonManager : MonoBehaviourPun
{
    [SerializeField]
    private Text _text;
    
    [SerializeField]
    private Transform _button;
    
    void Update()
    {
        if(photonView.IsMine){
            _text.text = "START";
        }
    }

    public void BtnClick(){
        if(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 0){ //3
            PhotonNetwork.CurrentRoom.IsOpen = false;
            SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.StartButton);
            LoadingSceneController.Instance.LoadStart();
            StartCoroutine(DelayedLoadLevel());
        }
    }

    private IEnumerator DelayedLoadLevel() {
        yield return new WaitForSeconds(0.5f); // Add a 0.5-second delay
        PhotonNetwork.IsMessageQueueRunning = false;
        PhotonNetwork.LoadLevel("60SecondMap");
    }
}
