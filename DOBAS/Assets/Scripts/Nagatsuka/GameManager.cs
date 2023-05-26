using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region 定数宣言(Const)

    private const int FIRST_TURN = 1;//最初の人のターン.
    #endregion


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
    [SerializeField] GameObject StandByCanvas;  //準備完了キャンバス.
    [SerializeField] Text StandByText;          //待機人数を表示するボタン.
    [SerializeField] Text WhoseTurnText;        //誰のターンかのテキスト.
    [SerializeField] Text HaveTimeText;         //持ち時間テキスト.
    [SerializeField] GameObject StandByGroup;   //準備完了のグループ.
    [SerializeField] GameObject ShakeDiceButton;//さいころを振るボタン.

    [Header("public宣言")]
    public static int WhoseTurn;//誰のターンか（プレイヤーIDを参照してこの変数と比べてターン制御をする）.
    public int MaxPlayers;//プレイヤーの最大人数.

    #endregion

    int ReadyPeople;//準備完了人数.
    string TurnName;//誰のターンかの名前用.

    float HaveTime;//各プレイヤーの持ち時間.
    float DoubtTime;//ダウト宣言の持ち時間.
    bool DoubtFlg;
    bool timeflg;

    #region Unityイベント(Start・Update)
    // Start is called before the first frame update
    void Start()
    {
        WhoseTurn = FIRST_TURN;
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
            if (PhotonNetwork.LocalPlayer.ActorNumber == WhoseTurn)
            {
                ShakeDiceButton.SetActive(true);
                WhoseTurnText.text = "あなたのターン";
            }
            else
            {
                ShakeDiceButton.SetActive(false);
                WhoseTurnText.text = "";
            }

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
            StartButton.SetActive(true);
            StandByText.text = "あと" +
                                (MaxPlayers - ReadyPeople)
                                + "人待っています・・・";//最大人数と現在の人数を引いて待っている人数を表示.
            //int playercnt = PhotonNetwork.CurrentRoom.PlayerCount;
            //if (playercnt >= MaxPlayers)
            //{
            //    StartButton.SetActive(true);
            //}
        }
    }
    #endregion


    #region ターン変更関連関数
    /// <summary>
    /// さいころを振った後にRPCを呼び出し、ターンを変更する関数.
    /// </summary>
    public void FinishDice()
    {
        photonView.RPC(nameof(ChangeTurn), RpcTarget.All);//WhoseTurnを増やしてターンを変える.
    }

    /// <summary>
    /// ターンを管理する
    /// WhoseTurnを変更してターンを変更する.
    /// </summary>
    [PunRPC]
    public void ChangeTurn()
    {
        if (WhoseTurn == MaxPlayers) WhoseTurn = FIRST_TURN;//WhoseTurnがプレイヤーの最大数を超えたら最初の人に戻す.
        else WhoseTurn++;                                   //次の人のターンにする.

        //if(PhotonNetwork.LocalPlayer.ActorNumber == WhoseTurn)//自分のターンになったら名前を送る.
        //{
        //    TurnName = PhotonNetwork.NickName;
        //    WhoseTurnText.text = PhotonNetwork.NickName + "のターン";//誰のターンかテキストで表示する.
        //}
    }
    #endregion

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
    /// PUNを使って変数を同期する.
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

    #region 準備完了操作関連

    /// <summary>
    /// 待機人数を増やす関数.
    /// </summary>
    [PunRPC]
    private void AddReadyPeaple()
    {
        ReadyPeople++;
    }
    /// <summary>
    /// 基底人数に達したらプレイヤーを生成する関数.
    /// </summary>
    [PunRPC]
    private void StartGame()
    {
        StandByCanvas.SetActive(false); //準備完了関連のキャンバスを非表示にする.
        NowGameState = GameState.InGame;//ゲームモードをInGameにする.
        CanvasUI.SetActive(true);       //ゲームに必要なキャンバスを表示する.
        var position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);//プレイヤーを生成する.
    }

    /// <summary>
    /// 準備完了ボタンを押した際に呼び出す関数.
    /// </summary>
    public void PushGameStart()
    {
        photonView.RPC(nameof(AddReadyPeaple), RpcTarget.All);//準備完了人数を増やす.
        StartButton.SetActive(false);                         //ボタンを押したら非表示にする.
        if (ReadyPeople != MaxPlayers)                        //入室した人数が指定した数に満たない場合
        {
            StandByGroup.SetActive(true);                     //待機人数を表示するキャンバスを表示する.
            StandByText.text = "あと" + (MaxPlayers - ReadyPeople )
                                + "人待っています・・・";//最大人数と現在の人数を引いて待っている人数を表示.
        }
        else
        {
            photonView.RPC(nameof(StartGame), RpcTarget.All);//準備完了人数を増やす.
        }
    }
    #endregion

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
