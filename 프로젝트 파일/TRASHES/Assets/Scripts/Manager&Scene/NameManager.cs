using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NameManager : MonoBehaviourPun
{

    [SerializeField] TMP_Text nameText;

    // Start is called before the first frame update
    void Start()
    {
        nameText.transform.position = new Vector3(nameText.transform.position.x, nameText.transform.position.y, nameText.transform.position.z + 10);
        if(photonView.IsMine){  }

        SetName();
    }

    private void SetName() => nameText.text = photonView.Owner.NickName;
}
