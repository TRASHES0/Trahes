using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class Attacked : MonoBehaviourPun
{
    public GameObject player;
    public int Player_Assigment;

    private void Start()
    {
        Player_Assigment = player.GetComponent<PhotonView>().CreatorActorNr;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (other.GetComponent<Bullet_Test>().BulletOnwer != Player_Assigment)
            {
                Debug.Log(other.name);
                if (other.name == "IceBullet(Clone)")
                {
                    Debug.Log("Ice");
                    player.GetComponent<Simple_Char_Move>().Attacked(other.gameObject.GetComponent<Bullet_Test>().BulletOnwerNickName, "iceBullet");
                }
                else if (other.name == "SniperBullet(Clone)")
                {
                    player.GetComponent<Simple_Char_Move>().Attacked(other.gameObject.GetComponent<Bullet_Test>().BulletOnwerNickName, "SniperBullet");
                }
                else if (other.name == "SniperSkillBullet(Clone)")
                {
                    player.GetComponent<Simple_Char_Move>().Attacked(other.gameObject.GetComponent<Bullet_Test>().BulletOnwerNickName, "SniperSkillBullet");
                }
                else
                {
                    player.GetComponent<Simple_Char_Move>().Attacked(other.gameObject.GetComponent<Bullet_Test>().BulletOnwerNickName, "normalBullet");
                }
            }
        }

        if (other.CompareTag("EnemyHitScan"))
        {
            player.GetComponent<Simple_Char_Move>().Attacked("박쥐", "None");
        }
    }
}
