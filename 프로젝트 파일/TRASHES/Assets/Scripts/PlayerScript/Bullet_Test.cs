using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class Bullet_Test : MonoBehaviourPun
{
    public float speed;
    public float gradient;
    public float distance;
    private bool _isHit;
    public AudioSource sound;
    public int BulletOnwer;
    public string BulletOnwerNickName;
    
    // Start is called before the first frame update
    
    // Update is called once per frame

    public void Start()
    {
        sound.PlayOneShot(sound.clip);
    }

    public void InitBullet(int num, string nickname, float lag)
    {
        BulletOnwer = num;
        BulletOnwerNickName = nickname;
        //Debug.Log(lag);
        
    }
    void Update()
    {
        transform.Translate(transform.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHitScan") && other.GetComponent<Attacked>().Player_Assigment != BulletOnwer)
        {
            Debug.Log("BULLET HIT");
            Destroy(this.gameObject);
        }

        if (other.CompareTag("Wall"))
        {
            Debug.Log("BULLET HIT Wall");
            Destroy(this.gameObject);
        }

        if (other.CompareTag("EnemyHitScan"))
        {
            Debug.Log("BULLET HIT Enemy");
            Destroy(this.gameObject);
        }
    }

}
