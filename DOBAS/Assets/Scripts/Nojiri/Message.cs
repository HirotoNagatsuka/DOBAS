using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviourPunCallbacks
{
    Text MsgText;
    Text TextLog;
    PlayerManager playerManager;

    const int TextChild = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Text�I�u�W�F�N�g�̎擾
        MsgText = GameObject.Find("TextMessage").GetComponent<Text>();
        TextLog = GameObject.Find("TextMessageLog").GetComponent<Text>();
        playerManager = gameObject.GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region �^�[���\�� �i�`�����쐬�j
    //public int TurnNum(int turn)
    //{
    //    switch (turn)
    //    {
    //        case 1:
    //            MsgText.text = "�v���C���[�P�̃^�[��";
    //            break;
    //        case 2:
    //            MsgText.text = "�v���C���[�Q�̃^�[��";
    //            break;
    //        case 3:
    //            MsgText.text = "�v���C���[�R�̃^�[��";
    //            break;
    //        case 4:
    //            MsgText.text = "�v���C���[�S�̃^�[��";
    //            break;
    //        default:
    //            break;
    //    }
    //}
    #endregion

    #region �}�X���ʂ̕\��
    public void ShowText(string tag)
    {
        string text = MsgText.text;

        if(MsgText.text != null)
        {
            TextLog.text = text;
        }

        switch (tag)
        {
            case "Start":
                MsgText.text = "����{�[�i�X�Q�b�g�I�@�U���́{�P";
                break;
            case "Card":
                MsgText.text = "�J�[�h���P���Q�b�g�I";
                break;
            case "Move":
                MsgText.text = "3�}�X�i�ށI";
                break;
            case "Hp":
                MsgText.text = "HP���P�񕜁I";
                break;
            case "Attack":
                MsgText.text = "���̃v���C���[���U���I";
                break;
            default:
                MsgText.text = "���ʂȂ�";
                break;
        }

        // �擾�����e�L�X�g��S���ɕ\��
        photonView.RPC(nameof(RpcSendText), RpcTarget.All, text);
    }
    #endregion

    [PunRPC]
    private void RpcSendText(string message)
    {
        Debug.Log(message);
    }
}
