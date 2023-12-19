using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AimManager : MonoBehaviourPun
{
    public Material laserMaterial;
    public LayerMask hitLayer;

    public LineRenderer laserLineRenderer;
    private bool isLaserActive = false;

    void Start()
    {
        CreateLaser();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) & photonView.IsMine)
        {
            ToggleLaser();
        }

        if (isLaserActive & photonView.IsMine)
        {
            UpdateLaser();
        }
    }

    void CreateLaser()
    {
        laserLineRenderer.material = laserMaterial;
        laserLineRenderer.startWidth = 0.04f;
        laserLineRenderer.endWidth = 0.04f;
        laserLineRenderer.enabled = false;
    }

    void ToggleLaser()
    {
        isLaserActive = !isLaserActive;

        if (isLaserActive)
        {
            laserLineRenderer.enabled = true;
            ShootLaser();
        }
        else
        {
            laserLineRenderer.enabled = false;
        }
    }

    void UpdateLaser()
    {
        if (isLaserActive)
        {
            ShootLaser();
        }
    }

    void ShootLaser()
    {
        Vector2 playerPosition = transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(playerPosition, mousePosition - playerPosition, Mathf.Infinity, hitLayer);

        laserLineRenderer.SetPosition(0, playerPosition);

        if (hit.collider != null)
        {
            laserLineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            /*Vector3 vector3 = playerPosition + (mousePosition - playerPosition).normalized * 10.0f;

            laserLineRenderer.SetPosition(1, vector3 + new Vector3(0, 0, -20));*/
            Vector3 laserDirection = new Vector3(mousePosition.x - playerPosition.x, mousePosition.y - playerPosition.y, 0).normalized;
            Vector3 laserEndPosition = new Vector3(playerPosition.x, playerPosition.y, -0.1f) + laserDirection * 10.0f;

            laserLineRenderer.SetPosition(1, laserEndPosition);
        }
    }
}
