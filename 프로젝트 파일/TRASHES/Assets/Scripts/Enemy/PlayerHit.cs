using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerHit : MonoBehaviourPun
{
   
    public GameObject enemy;

    public int enemyHP = 1;
    // Start is called before the first frame update
    
    
    void OnTriggerEnter2D(Collider2D other){
        
        if(other.CompareTag("Bullet")){
            if(enemyHP > 0)
            {
                enemyHP--;
            }
            else
            {
                enemy.GetComponent<Enemy>().Destroy();
            }
        }
        if(other.CompareTag("PlayerHitScan")){
            enemy.GetComponent<Enemy>().Destroy();
        }
    }
    // Update is called once per frame
    
}
