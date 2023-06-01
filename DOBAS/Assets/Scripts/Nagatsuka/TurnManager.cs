using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;

public class TurnManager : MonoBehaviour, IPunObservable
{
    public static int PlayerNum;
    public static int WhoseTurn;

    // Start is called before the first frame update
    void Start()
    {
        WhoseTurn = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log(PhotonNetwork.CountOfPlayers);
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 自身のアバターのスタミナを送信する
            stream.SendNext(PlayerNum);
        }
        else
        {
            // 他プレイヤーのアバターのスタミナを受信する
            PlayerNum = (int)stream.ReceiveNext();
        }
    }
}
