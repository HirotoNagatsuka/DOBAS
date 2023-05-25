using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviourPunCallbacks
{
    Text MsgText;
    PlayerManager playerManager;

    const int TextChild = 0;
    public string MessageLog;

    // Start is called before the first frame update
    void Start()
    {
        // Textオブジェクトの取得
        MsgText = GameObject.Find("TextMessage").GetComponent<Text>();
        playerManager = gameObject.GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // PlayerManagerから呼ばれる
    //public void ShowText(int text)
    //{
    //    //Enterを押したとき
    //    if (Input.GetKey(KeyCode.Return))
    //    {
    //        // 取得したテキストを全員に表示
    //        RpcSendText(MsgText.text);
    //        photonView.RPC(nameof(RpcSendText), RpcTarget.All, MsgText.text);
    //    }
    //}

    [PunRPC]
    private void RpcSendText(string message)
    {
        Debug.Log(message);
    }
}
