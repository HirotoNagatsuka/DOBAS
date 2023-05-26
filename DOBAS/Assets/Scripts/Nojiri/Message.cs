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
        // Textオブジェクトの取得
        MsgText = GameObject.Find("TextMessage").GetComponent<Text>();
        TextLog = GameObject.Find("TextMessageLog").GetComponent<Text>();
        playerManager = gameObject.GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region ターン表示 （形だけ作成）
    //public int TurnNum(int turn)
    //{
    //    switch (turn)
    //    {
    //        case 1:
    //            MsgText.text = "プレイヤー１のターン";
    //            break;
    //        case 2:
    //            MsgText.text = "プレイヤー２のターン";
    //            break;
    //        case 3:
    //            MsgText.text = "プレイヤー３のターン";
    //            break;
    //        case 4:
    //            MsgText.text = "プレイヤー４のターン";
    //            break;
    //        default:
    //            break;
    //    }
    //}
    #endregion

    #region マス効果の表示
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
                MsgText.text = "周回ボーナスゲット！　攻撃力＋１";
                break;
            case "Card":
                MsgText.text = "カードを１枚ゲット！";
                break;
            case "Move":
                MsgText.text = "3マス進む！";
                break;
            case "Hp":
                MsgText.text = "HPが１回復！";
                break;
            case "Attack":
                MsgText.text = "他のプレイヤーを攻撃！";
                break;
            default:
                MsgText.text = "効果なし";
                break;
        }

        // 取得したテキストを全員に表示
        photonView.RPC(nameof(RpcSendText), RpcTarget.All, text);
    }
    #endregion

    [PunRPC]
    private void RpcSendText(string message)
    {
        Debug.Log(message);
    }
}
