using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // ★追加
using Photon.Realtime; // ★追加

public class PhotonMultiTemplete : Photon.Pun.MonoBehaviourPun // ★継承クラスの変更
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)//自身のみ入力ができる.
        {
            return;
        }
            var input = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
            transform.Translate(6f * Time.deltaTime * input.normalized);
    }
}
