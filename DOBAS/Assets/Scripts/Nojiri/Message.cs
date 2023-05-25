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
        // Text�I�u�W�F�N�g�̎擾
        MsgText = GameObject.Find("TextMessage").GetComponent<Text>();
        playerManager = gameObject.GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // PlayerManager����Ă΂��
    //public void ShowText(int text)
    //{
    //    //Enter���������Ƃ�
    //    if (Input.GetKey(KeyCode.Return))
    //    {
    //        // �擾�����e�L�X�g��S���ɕ\��
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
