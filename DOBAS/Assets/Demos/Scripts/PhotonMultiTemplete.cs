using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // ���ǉ�
using Photon.Realtime; // ���ǉ�

public class PhotonMultiTemplete : Photon.Pun.MonoBehaviourPun // ���p���N���X�̕ύX
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)//���g�̂ݓ��͂��ł���.
        {
            return;
        }
            var input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
            transform.Translate(6f * Time.deltaTime * input.normalized);
    }
}
