using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region public・SerializeField宣言

    /// <summary>
    /// ゲームの状況.
    /// </summary>
    public enum GameState
    {
        InitGame,   //初期状態(Photonと接続する前にカウント関数が呼ばれないように初期と準備を分ける).
        SetGame,    //ゲーム準備状態.
        InGame,     //ゲームプレイ状態.
        EndGame,    //ゲーム終了状態.
    }
    [Header("ゲームモード")]
    public GameState NowGameState;//現在のゲームモード.

    [Header("SerializeField宣言")]
    [SerializeField] GameObject CanvasUI;       //ゲーム開始時にまとめてキャンバスを非表示にする.
    [SerializeField] GameObject StartButton;    //準備完了ボタン.
    [SerializeField] Text HaveTimeText;         //持ち時間テキスト.
    [SerializeField] GameObject ShakeDiceButton;//さいころを振るボタン.

    [Header("public宣言")]
    public int WhoseTurn;//誰のターンか（プレイヤーIDを参照してこの変数と比べてターン制御をする）.
    public int MaxPlayers;//プレイヤーの最大人数.

    #endregion


    float HaveTime;//各プレイヤーの持ち時間.
    float DoubtTime;//ダウト宣言の持ち時間.
    bool DoubtFlg;
    bool timeflg;

    #region Unityイベント(Start・Update)
    // Start is called before the first frame update
    void Start()
    {
        HaveTime = 60;
        DoubtFlg = false;
        timeflg = false;
       // SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        if (NowGameState == GameState.InGame)//ゲームモードがゲーム中なら.
        {
            Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
            if (PhotonNetwork.LocalPlayer.ActorNumber == WhoseTurn) ShakeDiceButton.SetActive(true);
            else ShakeDiceButton.SetActive(false);

            //ChangeTurn();
            if (Input.GetKeyDown(KeyCode.P))
            {
                photonView.RPC(nameof(StartTimer), RpcTarget.All);
            }

            if (DoubtFlg) ChangeDoubtTime();
            //else ChangeHaveTime();
            else if (timeflg) HaveTime -= Time.deltaTime;
            HaveTimeText.text = HaveTime.ToString("0");

            // ゲームの状況管理
            if (NowGameState == GameState.InGame)
            {
               // MainGame();
            }
            if (NowGameState == GameState.EndGame)
            {
                EndGame();
            }
        }
        else if (NowGameState == GameState.SetGame)
        {
            int playercnt = PhotonNetwork.CurrentRoom.PlayerCount;
            if (playercnt >= MaxPlayers)
            {
                StartButton.SetActive(true);
            }
        }
    }
    #endregion

    /// <summary>
    /// さいころを振った後にRPCを呼び出す
    /// </summary>
    public void FinishDice()
    {
        photonView.RPC(nameof(ChangeTurn), RpcTarget.All);
    }

    /// <summary>
    /// ターンを管理する
    /// WhoseTurnを変更してターンを変更する.
    /// </summary>
    [PunRPC]
    public void ChangeTurn()
    {
        if (WhoseTurn == MaxPlayers) WhoseTurn = 1;//WhoseTurnがプレイヤーの最大数を超えたら1に戻す.
        else WhoseTurn++;                          //次の人のターンにする.
    }


    #region ゲーム状況
    public void StateGame(int Param)
    {
        if (Param == 1)
        {
            NowGameState = GameState.InGame;
        }
        if (Param == 2)
        {
            NowGameState = GameState.EndGame;
        }
    }
    #endregion

    void EndGame()
    {

    }

    /// <summary>
    /// 変数をPUNを使って同期する.
    /// </summary>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(HaveTime);// timeを送信する
            stream.SendNext(WhoseTurn);
        }
        else
        {
            HaveTime = (float)stream.ReceiveNext(); // timeを受信する
            WhoseTurn = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    private void StartTimer()
    {
        timeflg = true;
    }

    /// <summary>
    /// 準備完了ボタンを押した際に呼び出す関数.
    /// </summary>
    public void PushGameStart()
    {
        StartButton.SetActive(false);
        NowGameState = GameState.InGame;
        CanvasUI.SetActive(true);
        var position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
    }

    /// <summary>
    /// 持ち時間を減らす関数
    /// </summary>
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
        DoubtTime = 10;//ダウト指摘時間を設定
        DoubtFlg = true;
    }

    /// <summary>
    /// ダウト指摘時間の減少関数.
    /// </summary>
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
    #region 早坂ダウト判定
    //public void DoutDis(int Number)
    //{
    //    for (int i = 0; i < Players.Length; i++)
    //    {
    //        Players[i].DoutDec = true;
    //    }
    //    if (Players[WhoseTurn].Tarn)
    //    {
    //        if (Number == 5 || Number == 6)
    //        {
    //            Debug.Log("嘘目");
    //            Players[WhoseTurn].DoutFlg = true;
    //        }
    //        else
    //        {
    //            Debug.Log("真目");
    //            Players[WhoseTurn].DoutFlg = false;
    //        }
    //    }
    //}
    //public void DoutJudge()
    //{
    //    Debug.Log(Votes);
    //    for (int i = 0; i < Players.Length; i++)
    //    {
    //        Players[i].DoutDec = false;
    //        Players[i].Count = 5.0f;
    //    }
    //    for (int i = 0; i < Players.Length; i++)
    //    {
    //        if (Players[WhoseTurn].DoutFlg)
    //        {
    //            if (Players[i].MyDec)
    //            {
    //                Debug.Log(Players[WhoseTurn] + "は嘘ついてたわ");
    //                break;
    //            }
    //        }
    //        if (!Players[WhoseTurn].DoutFlg)
    //        {
    //            if (Players[i].MyDec)
    //            {
    //                Debug.Log(Players[WhoseTurn] + "は嘘ついてない");
    //                break;
    //            }
    //        }
    //        if (!Players[i].MyDec)
    //        {
    //            Votes++;
    //        }
    //        if (Votes == 4)
    //        {
    //            Debug.Log("スルー");
    //        }
    //    }
    //    Players[WhoseTurn].TarnEnd = true; // 時間差で他のTARNENDがオンになっている可能性
    //    Votes = 0;

    //}

    #endregion
}
