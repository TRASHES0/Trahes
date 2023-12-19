using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class PlayerListingsMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private PlayerListing _playerListing;

    private List<PlayerListing> _listings = new List<PlayerListing>();

    private void Awake(){
        GetCurrentRoomPlayers();
    }

    private void GetCurrentRoomPlayers(){
        foreach(KeyValuePair<int, Player> playerinfo in PhotonNetwork.CurrentRoom.Players){
            AddPlayerListing(playerinfo.Value);
            if (playerinfo.Key == 1)
            {
                playerinfo.Value.SetPlayerNumber(1);
            }
        }
    }

    private void AddPlayerListing(Player player){
        PlayerListing listing = Instantiate(_playerListing, _content);
        if(listing != null){
            listing.SetPlayerInfo(player);
            _listings.Add(listing);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
        int tmp = 0;
        if(PhotonNetwork.IsMasterClient)
            foreach (var i in PhotonNetwork.CurrentRoom.Players)
            {
                i.Value.SetPlayerNumber(++tmp);
                Debug.Log(i.Value.GetPlayerNumber());
            }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if(index != -1){
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
        int tmp = 0;
        if(PhotonNetwork.IsMasterClient)
            foreach (var i in PhotonNetwork.CurrentRoom.Players)
            {
                i.Value.SetPlayerNumber(++tmp);
                Debug.Log(i.Value.GetPlayerNumber());
            }
    }
}
