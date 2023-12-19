using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class SetMainCamera : MonoBehaviour
{

    public Canvas test;

    // Start is called before the first frame update

    private void Awake()
    {
        test.worldCamera = Camera.main;
    }
}

