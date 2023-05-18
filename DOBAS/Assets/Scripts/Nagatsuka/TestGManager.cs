using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class TestGManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] Text HaveTimeText;
    float HaveTime;//各プレイヤーの持ち時間.
    float DoubtTime;//ダウト宣言の持ち時間.
    bool DoubtFlg;
    bool timeflg;

    // Start is called before the first frame update
    void Start()
    {
        HaveTime = 60;
        DoubtFlg = false;
        timeflg = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //photonView.RPC(nameof(RpcSendMessage), RpcTarget.All, "こんにちは");
            photonView.RPC(nameof(StartTimer), RpcTarget.All);
        }

        if (DoubtFlg) ChangeDoubtTime();
        //else ChangeHaveTime();
        else if (timeflg) HaveTime -= Time.deltaTime;
        HaveTimeText.text = HaveTime.ToString("0");
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // timeを送信する
            stream.SendNext(HaveTime);
        }
        else
        {
            // timeを受信する
            HaveTime = (float)stream.ReceiveNext();
        }
    }
    [PunRPC]
    private void StartTimer()
    {
        timeflg = true;
    }

    [PunRPC]
    private void RpcSendMessage(string message)
    {
        timeflg = true;
        Debug.Log(message);
    }




    private void ChangeHaveTime()
    {
        if (HaveTime > 0)//残り時間が残っているなら.
        {
            HaveTime -= Time.deltaTime;
            if (HaveTime <= 10)//10秒以下になったら赤くする.
            {
                HaveTimeText.color = Color.red;
            }
            HaveTimeText.text = HaveTime.ToString("0");//小数点以下を表示しない.
        }
        else//0以下になったら.
        {
            HaveTime = 0;
            Debug.Log("ターン強制終了");
        }        
    }
    public void StartDoubtTime()
    {
        DoubtTime = 10;
        DoubtFlg = true;
    }
    private void ChangeDoubtTime()
    {
        if (DoubtTime > 0)//残り時間が残っているなら.
        {
            DoubtTime -= Time.deltaTime;
            HaveTimeText.color = Color.red;
            
            HaveTimeText.text = DoubtTime.ToString("0");//小数点以下を表示しない.
        }
        else//0以下になったら.
        {
            DoubtTime = 0;
            Debug.Log("ターン強制終了");
            DoubtFlg = false;
        }
    }
}
