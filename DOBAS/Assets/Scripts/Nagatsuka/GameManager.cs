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
    const int INPUT_NAME = 2;//名前入力の子オブジェクトを参照用.
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
    [Header("ゲーム状態")]
    public GameState NowGameState;//現在のゲーム状態.

    [Header("通信プレイ人数")]
    public int MaxPlayers;//プレイヤーの最大人数.

    [Header("プレイヤーの初期HP")]
    public int FirstHP;

    [Header("プレイヤーの持ち時間")]
    public float HaveTime;//各プレイヤーの持ち時間.

    [Header("public・SerializeField宣言")]
    [Header("ゲーム開始前（準備完了）関連")]
    [SerializeField] InputField InputNickName;  //名前を入力する用.
    [SerializeField] GameObject CanvasUI;       //ゲーム開始時にまとめてキャンバスを非表示にする.
    [SerializeField] GameObject StartButton;    //準備完了ボタン.
    [SerializeField] GameObject StandByCanvas;  //準備完了キャンバス.
    [SerializeField] Text HelloPlayerText;      //待機中にプレイヤーを表示するボタン.
    [SerializeField] Text StandByText;          //待機人数を表示するボタン.


    [SerializeField] Text WhoseTurnText;        //誰のターンかのテキスト.
    [SerializeField] Text HaveTimeText;         //持ち時間テキスト.
    [SerializeField] GameObject StandByGroup;   //準備完了のグループ.
    [SerializeField] GameObject ShakeDiceButton;//さいころを振るボタン.

    public List<string> PlayersName = new List<string>();
    public int[] PlayersHP;
    public static int MaxPlayersNum;//他スクリプトからアクセスする用.
    public static int WhoseTurn;//誰のターンか（プレイヤーIDを参照してこの変数と比べてターン制御をする）.

    [SerializeField] GameObject PlayersNameGroupPrefab;//右上に表示する名前を生成する用.
    [SerializeField] Sprite[] Hearts = new Sprite[5];
    #endregion

    private bool[] UseID = new bool[4];//プレイヤーに割り当てるIDの使用状況.
    private int ReadyPeople;//準備完了人数.
    string TurnName;//誰のターンかの名前用.

    float DoubtTime;//ダウト宣言の持ち時間.
    bool DoubtFlg;
    bool timeflg;
    public List<GameObject> PlayersNameGroup = new List<GameObject>();

    #region Unityイベント(Start・Update)
    // Start is called before the first frame update
    void Start()
    {
        WhoseTurn = FIRST_TURN;
        DoubtFlg = false;
        timeflg = false;
        PlayersHP = new int[MaxPlayers];//Playerの人数分HP配列を用意.
        for (int i = 0; i < MaxPlayers; i++)
        {
            PlayersHP[i] = FirstHP;//HPの初期値を代入.
        }
        MaxPlayersNum = MaxPlayers;
    }

    // Update is called once per frame
    void Update()
    {
        switch (NowGameState)//ゲームモードによって処理を分岐する.
        {
            case GameState.InitGame:
                break;
            case GameState.SetGame:
                StartButton.SetActive(true);
                StandByText.text = "あと" + (MaxPlayers - ReadyPeople)
                                    + "人待っています・・・";//最大人数と現在の人数を引いて待っている人数を表示.
                break;
            case GameState.InGame:
                InGameRoop();
                break;
            case GameState.EndGame:
                    EndGame();
                break;
            default:
                Debug.Log("エラー:予期せぬゲームモード");
                break;
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
    }
    #endregion

    #region ゲーム状況
    /// <summary>
    /// InGameの際にUpdateで呼び出し続ける関数
    /// 自分のターンの処理・時間の制御をここで行う.
    /// </summary>
    private void InGameRoop()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == WhoseTurn)
        {
            ShakeDiceButton.SetActive(true);
            WhoseTurnText.color = Color.red;
            WhoseTurnText.text = "あなたのターン";
        }
        else
        {
            ShakeDiceButton.SetActive(false);
            WhoseTurnText.color = Color.white;
            WhoseTurnText.text = PlayersName[WhoseTurn-1]+ "のターン";
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            photonView.RPC(nameof(StartTimer), RpcTarget.All);
        }

        if (DoubtFlg) ChangeDoubtTime();
        //else ChangeHaveTime();
        else if (timeflg) HaveTime -= Time.deltaTime;
        HaveTimeText.text = HaveTime.ToString("0");
    }

    void EndGame()
    {

    }
    #endregion


    public void ChangePlayersHP(int addHP,int subject)//subjectは対象という意味.
    {
        if(subject== PhotonNetwork.LocalPlayer.ActorNumber)//自分自身が対象の場合のみHPを変化させる関数を呼ぶ.
        {
            photonView.RPC(nameof(ChangeHP), RpcTarget.All, addHP, subject);
        }
    }
    /// <summary>
    /// 対象と引数を指定し、全員にHP状態を同期する.
    /// </summary>
    [PunRPC]
    void ChangeHP(int addHP,int subject)
    {
        PlayersHP[subject - 1] += addHP;
        PlayersNameGroup[subject - 1].transform.GetChild(2).GetComponent<Image>().sprite = Hearts[PlayersHP[subject - 1]];
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
            stream.SendNext(PlayersHP);
        }
        else
        {
            HaveTime = (float)stream.ReceiveNext(); // timeを受信する
            WhoseTurn = (int)stream.ReceiveNext();
            PlayersHP = (int[])stream.ReceiveNext();
        }
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
        NowGameState = GameState.InGame;//ゲーム状態をInGameにする.
        CanvasUI.SetActive(true);       //ゲームに必要なキャンバスを表示する.
        var position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);//プレイヤーを生成する.
        int i=0;                                                             // ループを使用して、全てのプレイヤーに対して名前を表示する例
        foreach (var player in PhotonNetwork.PlayerList)//プレイヤーの名前を取得.
        {
            PlayersName.Add(player.NickName);
            var obj = Instantiate(PlayersNameGroupPrefab, new Vector3(0f,0f,0f), Quaternion.identity,CanvasUI.transform);
            obj.GetComponent<RectTransform>().localPosition = new Vector3(760f, 465f -150f * i, 0f);
            obj.transform.GetChild(1).GetComponent<Text>().text = player.NickName;
            obj.transform.GetChild(2).GetComponent<Image>().sprite = Hearts[PlayersHP[i] - 1];
            PlayersNameGroup.Add(obj);
            i++;
        }
    }

    /// <summary>
    /// 準備完了ボタンを押した際に呼び出す関数.
    /// </summary>
    public void PushGameStart()
    {
        photonView.RPC(nameof(AddReadyPeaple), RpcTarget.All);//準備完了人数を増やす.
        StartButton.SetActive(false);                         //ボタンを押したら非表示にする.

        PhotonNetwork.NickName = InputNickName.transform.GetChild(INPUT_NAME).GetComponent<Text>().text;// プレイヤー自身の名前を入力された名前に設定する
        //Debug.Log(InputNickName.transform.GetChild(2).GetComponent<Text>().text);

        if (ReadyPeople != MaxPlayers)                        //入室した人数が指定した数に満たない場合
        {
            StandByGroup.SetActive(true);                     //待機人数を表示するキャンバスを表示する.
            HelloPlayerText.text = PhotonNetwork.NickName + "さんようこそ！";
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
    /// プレイヤーにIDを与える関数
    /// </summary>
    public int Give_ID_Player()
    {
        int ID = 1;
        for(int i = 0; i < UseID.Length; i++)
        {
            if (UseID[i] == false)
            {
                UseID[i] = true;
                break;
            }
            else
            {
                ID++;
            }
        }

        return ID;
    }

    public int GiveID()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber;
    }
    #region 持ち時間減少関連

    [PunRPC]
    private void StartTimer()
    {
        timeflg = true;
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
    #endregion

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
