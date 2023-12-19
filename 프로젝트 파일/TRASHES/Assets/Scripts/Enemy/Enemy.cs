using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;


public class Enemy : MonoBehaviourPun
{
    // Start is called before the first frame update
    public NavMeshAgent navMeshAgent;
    public Transform target; // target == player

    public Transform[] waypoints;
    public int Player_count = 0;
    public float speed = 1f;
    public bool m_IsPlayerInRange;
	public bool m_IsInRange; // 이동가능 범위에 있는지 확인 
    int m_CurrentWaypointIndex = 0;
    
    void Start()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Transform temp = enemy.GetComponent<Transform>();
            if (temp.position == gameObject.transform.position)
                waypoints[0] = temp;
        }
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(waypoints[0].position);
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis  = false;
    }
    

    void Move()
    {
        if ( m_IsPlayerInRange  == true && m_IsInRange == true)
        {
            navMeshAgent.speed = 3f;
            navMeshAgent.SetDestination(target.transform.position);
        }// 플레이어를 발견 
        else if ( m_IsPlayerInRange == true && m_IsInRange == false )
        {
            if (!navMeshAgent.pathPending&&navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance )// 목적지 에 도착했을때
            {

                /*m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length; 
                navMeshAgent.speed = 5f;*/
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            }

        }
		else if(m_IsPlayerInRange == false && m_IsInRange == false)
		{
			navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
		}else if(m_IsPlayerInRange == false && m_IsInRange == true)
		{
			navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
		}
        // 아무것도 없으면 패트롤

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHitScan") && Player_count == 0)
        {
            Player_count ++;
            target = other.transform;
            m_IsPlayerInRange = true;
        }
        else if(other.CompareTag("PlayerHitScan") && Player_count > 0)
        {
            Player_count ++;
        }
		if(other.CompareTag("EnemyRange")){
			m_IsInRange = true;
		}
    }
	void OnTriggerStay2D(Collider2D other){
		if(other.CompareTag("EnemyRange")){
			m_IsInRange = true;
		}
	}
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHitScan") && Player_count == 1)
        {
            Player_count = 0;
            target = waypoints[0];
            m_IsPlayerInRange = false;
            
            
        }else if(other.CompareTag("PlayerHitScan") && Player_count > 1){
            --Player_count;
        }
		if(other.CompareTag("EnemyRange")){
			m_IsInRange = false;
		}
    }
    public void Destroy()
    {
        photonView.RPC(nameof(DestroyRPC), RpcTarget.All);
    }
    // Update is called once per frame
    void Update()
    {
        if(waypoints[0] != null)
            Move();
    }

    [PunRPC]
    void DestroyRPC()
    { 
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(this.gameObject);
    }


}


