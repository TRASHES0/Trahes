using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerListing : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    public Player Player { get; private set; }

    public void SetPlayerInfo(Player player){
        Player = player;
        _text.text = player.NickName;
        _text.font = Resources.Load<Font>("Fonts/Maplestory Bold");
    }

}
