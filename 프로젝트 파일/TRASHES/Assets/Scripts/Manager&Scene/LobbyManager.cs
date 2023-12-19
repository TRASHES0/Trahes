using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviour
{
    private PhotonView pv;

    private Ray ray;
    private RaycastHit hit;
    private new Camera camera;

    public Renderer player;

    void Start()
    {
        Destroy(GameObject.Find("P_Class_info_Manager"));
        
        pv = GetComponent<PhotonView>();
        camera = Camera.main;
        
        
        
    }
    

    /*
    private void Update()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);

        if(Input.GetMouseButtonDown(0))
        {
            if(Physics.Raycast(ray, out hit, 100.0f, 1 << 6))
            {
                string item = hit.collider.gameObject.name;
                string[] words = item.Split('_');
            }
        }
    } 
    */
}
