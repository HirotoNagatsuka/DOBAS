using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region 定数宣言(Const)
    private const int FIRST_TURN = 1;//最初の人のターン.
    const int INPUT_NAME = 2;//名前入力の子オブジェクトを参照用.

    const int ATTACK = 4;//攻撃出目の数字.
    const int DOUBT = 5; //ダウト目の数字(5か6の場合).
    //プレイヤーの詳細パネルの参照用.
    const int PLAYER_NAME = 1;
    const int PLAYER_HP = 2;
    const int PLAYER_ATTACK = 4;

    const int SAIKORO_RESULT = 1;//サイコロの出目を他の人に通知するパネル用.
    #endregion



    #region private変数
    private GameObject Dice;//サイコロ用の表示・非表示を繰り返す用.
    Vector3 diceCameradefPos;//サイコロカメラの初期値.
    Vector3 diceCameraPos;//サイコロを振る初期位置.
    private bool DiceFlg;//さいころを振ったかのフラグ.
    private int Number;       //サイコロの出目をリザルトパネルに表示する用の変数.
    private bool diceBtnFlg;//さいころボタンを押したかの判定用.
    #region ランダムにさいころを回転させる用の変数宣言.
    private int rotateX;
    private int rotateY;
    private int rotateZ;
    #endregion
    private int ThroughNum;//信じた(スルーした)人数.
    private bool[] UseID = new bool[4];//プレイヤーに割り当てるIDの使用状況.
    //private int ReadyPeople;           //準備完了人数.
    private bool timeflg;//持ち時間を減らすためのフラグ.
    private bool doubtFlg;//さいころが5か6の場合、フラグを切り換える.

    private List<GameObject> PlayersNameGroup = new List<GameObject>();//Playerの詳細情報を表示するオブジェクト.
    private GameObject PlayerInfo;


    #region おまかせName配列
    private static readonly string[] OMAKASE_NAMES = new string[] { "すねえく", "くらあけん", "さかな","いか","ねずみ","ごりら",
        "さかなくん","いっぬ","おすすめです","オススメです","おっと要チェック！","海賊王"};
    #endregion

    #endregion

    int readyccnt = 0;
    int NowTutorial = 0;

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
    [SerializeField] GameObject TutorialCanvas;
    [SerializeField] InputField InputNickName;  //名前を入力する用.
    [SerializeField] GameObject CanvasUI;       //ゲーム開始時にまとめてキャンバスを非表示にする.
    [SerializeField] GameObject StartButton;    //準備完了ボタン.
    [SerializeField] GameObject StandByCanvas;  //準備完了キャンバス.
    [SerializeField] Text HelloPlayerText;      //待機中にプレイヤーを表示するボタン.
    [SerializeField] Text StandByText;          //待機人数を表示するテキスト.
    [SerializeField] GameObject MainCamera;
    [SerializeField] AnimalsManager SelectAnimals;
    [SerializeField] GameObject ChangeButtons;  //キャラクター変更用ボタン.

    [SerializeField] Text WhoseTurnText;        //誰のターンかのテキスト.
    [SerializeField] Text HaveTimeText;         //持ち時間テキスト.
    [SerializeField] GameObject StandByGroup;   //準備完了のグループ.
    [SerializeField] GameObject CardButton;

    [Header("さいころ関連")]
    [SerializeField] GameObject DicePrefab;//サイコロのプレファブを入れる.
    [SerializeField] GameObject ShakeDiceButton;//さいころを振るボタン.
    [SerializeField] GameObject DiceCamera;
    [SerializeField] Text DiceNumText;
    [SerializeField] GameObject[] ResultPanel = new GameObject[5];//出目のパネル用.
    [SerializeField] GameObject ReasoningPanel;

    [Header("ゲーム終了関連")]
    [SerializeField] GameObject GameEndPanel;
    public int[] Ranks;

    [Header("外部アクセス用変数")]
    //public int[] PlayersHP;//Player側からHPの参照を行う用.
    public static int MaxPlayersNum;//他スクリプトからアクセスする用.
    //public static int WhoseTurn;//誰のターンか（プレイヤーIDを参照してこの変数と比べてターン制御をする）.
    public Text WaitText;   //他の人の行動待ちを表示するテキスト.
    [SerializeField] GameObject PlayersNameGroupPrefab;//右上に表示する名前を生成する用.
    public List<GameObject> Players = new List<GameObject>(); // プレイヤー参照用
    public List<string> PlayersName = new List<string>();//Playerの名前を入れるリスト.
    public bool FinMessage = false;//メッセージの表示が終了したらマスの効果を発動する為のフラグ.
    //public int[] AnimalChildNums;
    public int AnimalChildNum;//選んだキャラクターを保存するための宣言.
    public int DeclarationNum;//宣言番号.
    public bool DiceFinishFlg;//Player側からサイコロを振り終わっているかの参照用.
    public bool DeclarationFlg;//宣言待ちフラグ(Playerから参照する).

    public string[] PlayersRank = new string[4];
    public int[] PlayersRankNum;
    public int Rankcnt;//順位が決まった人数をカウント.

    [SerializeField] GameObject UseBt;    // 早坂優斗(0622)

    [Header("ダウト関連")]
    [SerializeField] GameObject DoubtPanel;
    public bool FailureDoubt;//ダウト失敗フラグ.

    [Header("メッセージ関連")]
    [SerializeField] GameObject MessageWindow;//メッセージを表示するパネル（子供にメッセージ用テキスト）.
    [SerializeField] GameObject MessageLogWindow;//メッセージのログを表示するパネル.
    [Header("チャット関連")]
    [SerializeField] InputField InputChat;
    [SerializeField] Text ChatLog;
    [SerializeField] Text ChatLog2;

    [Header("画像データ")]
    [SerializeField] Sprite[] TutorialSprites = new Sprite[6];
    [SerializeField] Sprite[] HeartSprites = new Sprite[6];//HP用の画像(0番目に死亡用のどくろを入れる).
    [SerializeField] Sprite[] DiceSprites = new Sprite[4]; //サイコロの出目の画像.
    #endregion

    ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
    public Text TestWhoseTurnText;
    bool startflg = false;

    #region Unityイベント(Start・Update)
    // Start is called before the first frame update
    void Start()
    {
        // プレイヤー自身の名前を"Player"に設定する
        PhotonNetwork.NickName = "Player";
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.SetPlayerHP(FirstHP);
        Debug.Log(PhotonNetwork.LocalPlayer.GetPlayerHP());
        Ranks = new int[MaxPlayers];//Playerの人数分HP配列を用意.

        diceBtnFlg = false;
        doubtFlg = false;
        timeflg = false;
        DeclarationFlg = false;
        FailureDoubt = false;
        MaxPlayersNum = MaxPlayers;
        Dice = Instantiate(DicePrefab, DiceCamera.transform.position, Quaternion.identity, this.transform);
        diceCameraPos = DiceCamera.transform.position;
        diceCameraPos.x += 3; //サイコロを振る位置をカメラから少しずらす.
        diceCameraPos.y -= 3;
        Dice.SetActive(false);
        DiceCamera.SetActive(false);
        DiceFlg = false;      //サイコロを振っていない状態にする.
        DiceFinishFlg = false;//サイコロを振り終わっていない状態にする.
        Rankcnt = MaxPlayers;
        PlayersRank = new string[MaxPlayers];
        diceCameradefPos = DiceCamera.transform.position;
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
                if (PhotonNetwork.LocalPlayer.GetReadyNum())
                {
                    CountReadyNum();
                }
                break;
            case GameState.InGame:
                InGameRoop();
                TestWhoseTurnText.text = (string)PhotonNetwork.CurrentRoom.CustomProperties["Turn"].ToString() ;
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

    #region ゲーム状況
    /// <summary>
    /// InGameの際にUpdateで呼び出し続ける関数
    /// 自分のターンの処理・時間の制御をここで行う.
    /// </summary>
    private void InGameRoop()
    {
        int turn = (int)PhotonNetwork.CurrentRoom.CustomProperties["Turn"];
        if (PhotonNetwork.LocalPlayer.ActorNumber == turn)//自分のターンなら
        {
            WaitText.text = "";
            if (PhotonNetwork.LocalPlayer.GetPlayerHP() > 0)//0以上であれば行動可能.
            {
                if (!diceBtnFlg)
                {
                    ShakeDiceButton.SetActive(true);//サイコロを振るボタンを表示.
                    CardButton.SetActive(true);
                }
                WhoseTurnText.color = Color.red;//ターンテキストの色を赤に変更.
                WhoseTurnText.text = "あなたのターン";
            }
            else
            {
                FinishTurn();
            }
        }
        else//自分のターンでないなら.
        {
            WhoseTurnText.color = Color.white;//ターンテキストの色を白に変更.
            WhoseTurnText.text = PlayersName[turn - 1] + "のターン";//誰のターンかをテキストに表示.
            WaitText.text = PlayersName[turn - 1] + "が行動しています";
            ShakeDiceButton.SetActive(false);//サイコロを振るボタンを表示.
            CardButton.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.P))//デバック用タイマー起動.
        {
            photonView.RPC(nameof(StartTimer), RpcTarget.All);
        }
        if (timeflg)
        {
            HaveTime -= Time.deltaTime;
        }
        HaveTimeText.text = HaveTime.ToString("0");//時間の小数点をなくす.
    }

    void EndGame()
    {
        CanvasUI.SetActive(false);
        GameEndPanel.SetActive(true);
        GameEndPanel.transform.GetChild(1).GetComponent<Text>().text = "1位：" + PlayersRank[0];
        GameEndPanel.transform.GetChild(2).GetComponent<Text>().text = "2位：" + PlayersRank[1];
        GameEndPanel.transform.GetChild(3).GetComponent<Text>().text = "3位：" + PlayersRank[2];
        Debug.Log("ゲーム終了");
    }
    #endregion

    #region ターン変更関連関数
    /// <summary>
    /// さいころを振った後にRPCを呼び出し、ターンを変更する関数.
    /// 終わった一人がFinishTurnを呼び出し、他の全員のChangeTurnを呼び出す.
    /// </summary>
    public void FinishTurn()
    {
        Debug.Log("FinishTurn()起動");
        WaitText.text = "";
        //UseBt.SetActive(false);        // 早坂優斗(0622)
        photonView.RPC(nameof(ChangeTurn), RpcTarget.All);//WhoseTurnを増やしてターンを変える.
    }

    /// <summary>
    /// ターンを管理する
    /// WhoseTurnを変更してターンを変更する.
    /// </summary>
    [PunRPC]
    private void ChangeTurn()
    {
        //Debug.Log("ChangeTurn起動");
        int cnt = 0;
        for (int i = 0; i < MaxPlayers; i++)//プレイヤーの数分ループを動かす.
        {
            PlayersNameGroup[i].transform.GetChild(5).gameObject.SetActive(false);
            if (PhotonNetwork.LocalPlayer.GetPlayerHP() == 0)//HPが0のプレイヤーを数える.
            {
                cnt++;
            }
        }
        if (cnt == MaxPlayers - 1)//残り1人になった場合ゲーム終了.
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (PhotonNetwork.LocalPlayer.GetPlayerHP() > 0)
                {
                    PlayersRank[0] = PlayersName[i];
                }
            }
            NowGameState = GameState.EndGame;
        }
        else
        {
            int turn = (int)PhotonNetwork.CurrentRoom.CustomProperties["Turn"];
            if (turn == MaxPlayers)
            {
                //WhoseTurn = FIRST_TURN;//WhoseTurnがプレイヤーの最大数を超えたら最初の人に戻す.
                customProperties["Turn"] = FIRST_TURN;
            }
            else
            {
                turn++;
                customProperties["Turn"] = turn;
                //WhoseTurn++;           //次の人のターンにする.
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
            customProperties.Clear();
            StartCoroutine(TurnMessageWindowCoroutine());
            photonView.RPC(nameof(StartTimer), RpcTarget.All);
        }
    }
    #endregion

    #region HP変更関連
    /// <summary>
    /// 変更のあったプレイヤー一人が代表してHPの同期関数を呼び出す.
    /// </summary>
    public void ChangePlayersHP(int addHP, int subject)//subjectは対象という意味.
    {
        //Debug.Log("ChangeHP起動：対象：" + subject);
        if (subject == PhotonNetwork.LocalPlayer.ActorNumber)//自分自身が対象の場合のみHPを変化させる関数を呼ぶ.
        {
            photonView.RPC(nameof(ChangeHP), RpcTarget.All, addHP, subject);
        }
    }
    public void EnemyAttack(int attackNum, int subject)
    {
        photonView.RPC(nameof(ChangeHP), RpcTarget.All, attackNum, subject);
    }
    /// <summary>
    /// 対象と引数を指定し、全員にHP状態を同期する.
    /// </summary>
    [PunRPC]
    void ChangeHP(int addHP, int subject)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == subject)
        {
            if (PhotonNetwork.LocalPlayer.GetPlayerHP() == 5)
            {
                Debug.Log("HP上限");
            }
            else if (PhotonNetwork.LocalPlayer.GetPlayerHP() == 0)
            {
                return;
            }
            else
            {
                int hp = PhotonNetwork.LocalPlayer.GetPlayerHP() + addHP;
                PhotonNetwork.LocalPlayer.SetPlayerHP(hp);
            }
        }
        PlayersNameGroup[subject - 1].transform.GetChild(PLAYER_HP).GetComponent<Image>().sprite = HeartSprites[PhotonNetwork.LocalPlayer.GetPlayerHP()-1];
        if (PhotonNetwork.LocalPlayer.GetPlayerHP() == 0)
        {
            Debug.Log("HP0になった人" + PlayersName[subject - 1]);
            PlayersRank[Rankcnt - 1] = PlayersName[subject - 1];
            if (PhotonNetwork.LocalPlayer.GetPlayerHP() == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                Debug.Log("MyRank代入");
                Ranks[PhotonNetwork.LocalPlayer.ActorNumber - 1] = Rankcnt;
            }
            Rankcnt--;
        }
    }
    #endregion

    #region 準備完了操作関連

    /// <summary>
    /// 基底人数に達したらプレイヤーを生成する関数.
    /// </summary>
    [PunRPC]
    private void StartGame()
    {
        NowGameState = GameState.InGame;//ゲーム状態をInGameにする.
        StandByCanvas.SetActive(false); //準備完了関連のキャンバスを非表示にする.
        MainCamera.SetActive(true);
        CanvasUI.SetActive(true);       //ゲームに必要なキャンバスを表示する.
        foreach (var player in PhotonNetwork.PlayerList)//プレイヤーの名前を取得.
        {
            PlayersName.Add(player.NickName);//Playerの名前をリストに入れる.
        }
        StartCoroutine(TurnMessageWindowCoroutine());
    }

    /// <summary>
    /// 準備完了ボタンを押した際に呼び出す関数.
    /// </summary>
    public void PushGameStart()
    {
        
        Debug.Log("player.GetReady()直後" + PhotonNetwork.LocalPlayer.GetReadyNum());
        StartButton.SetActive(false);                         //ボタンを押したら非表示にする.
        AnimalChildNum = SelectAnimals.ChildNum;
        PhotonNetwork.NickName = InputNickName.transform.GetChild(INPUT_NAME).GetComponent<Text>().text;// プレイヤー自身の名前を入力された名前に設定する
        PhotonNetwork.LocalPlayer.SetReadyNum(true);
        photonView.RPC(nameof(Readycount), RpcTarget.All);
        StartCoroutine(WaitStart());
    }

    [PunRPC]
    void Readycount()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.GetReadyNum())
            {
                readyccnt++;
            }
        }
        Debug.Log("readycnt数" + readyccnt);
        if (readyccnt != MaxPlayers)//入室した人数が指定した数に満たない場合
        {
            StandByGroup.SetActive(true);                     //待機人数を表示するキャンバスを表示する.
            HelloPlayerText.text = PhotonNetwork.NickName + "さんようこそ！";
            StandByText.text = "あと" + (MaxPlayers - readyccnt)
                                + "人待っています・・・";//最大人数と現在の人数を引いて待っている人数を表示.
            readyccnt = 0;
        }
        else
        {
            startflg = true;
        }
    }

    private void CountReadyNum()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.GetReadyNum())
            {
                readyccnt++;
            }
        }
        if (readyccnt != MaxPlayers)//入室した人数が指定した数に満たない場合
        {
            StandByGroup.SetActive(true);                     //待機人数を表示するキャンバスを表示する.
            HelloPlayerText.text = PhotonNetwork.NickName + "さんようこそ！";
            StandByText.text = "あと" + (MaxPlayers - readyccnt)
                                + "人待っています・・・";//最大人数と現在の人数を引いて待っている人数を表示.
            readyccnt = 0;
        }
        else//指定人数に達したらゲームを始める.
        {
            Debug.Log("ゲームを始めます");
            startflg = true;
            //CreateCharacter(AnimalChildNum);
            //photonView.RPC(nameof(StartGame), RpcTarget.All);
            //for (int i = 0; i < MaxPlayers; i++)
            //{
            //    PlayerInfo = Instantiate(PlayersNameGroupPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, CanvasUI.transform);
            //    PlayerInfo.transform.GetChild(PLAYER_NAME).GetComponent<Text>().text = PlayersName[i];                //名前を表示.
            //    PlayerInfo.transform.GetChild(PLAYER_HP).GetComponent<Image>().sprite = HeartSprites[PhotonNetwork.LocalPlayer.GetPlayerHP()];//初期HPを表示.
            //    PlayerInfo.GetComponent<RectTransform>().localPosition = new Vector3(760f, 465f - 150f * i, 0f);
            //    PlayersNameGroup.Add(PlayerInfo);
            //}
        }

    }

    /// <summary>
    /// 他のプレイヤーの開始を待つコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitStart()
    {
        yield return new WaitUntil(() => startflg == true); // 待機処理
        CreateCharacter(AnimalChildNum);
        photonView.RPC(nameof(StartGame), RpcTarget.All);
        for (int i = 0; i < MaxPlayers; i++)
        {
            PlayerInfo = Instantiate(PlayersNameGroupPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, CanvasUI.transform);
            PlayerInfo.transform.GetChild(PLAYER_NAME).GetComponent<Text>().text = PlayersName[i];                //名前を表示.
            PlayerInfo.transform.GetChild(PLAYER_HP).GetComponent<Image>().sprite = HeartSprites[PhotonNetwork.LocalPlayer.GetPlayerHP()];//初期HPを表示.
            PlayerInfo.GetComponent<RectTransform>().localPosition = new Vector3(760f, 465f - 150f * i, 0f);
            PlayersNameGroup.Add(PlayerInfo);
        }
        yield break;
    }

    /// <summary>
    /// 名前をランダムに決めるボタンを押した際に呼び出す関数.
    /// </summary>
    public void PushOmakaseNameButton()
    {
        int rnd = Random.Range(0, OMAKASE_NAMES.Length);
        InputNickName.text = OMAKASE_NAMES[rnd];
    }

    void CreateCharacter(int num)
    {
        GameObject p = null;
        switch (num)
        {
            case 0:
                p = PhotonNetwork.Instantiate("Colobus", Vector3.zero, Quaternion.identity);//プレイヤーを生成する.
                break;
            case 1:
                p = PhotonNetwork.Instantiate("Gecko", Vector3.zero, Quaternion.identity);//プレイヤーを生成する.
                break;
            case 2:
                p = PhotonNetwork.Instantiate("Herring", Vector3.zero, Quaternion.identity);//プレイヤーを生成する.
                break;
            case 3:
                p = PhotonNetwork.Instantiate("Muskrat", Vector3.zero, Quaternion.identity);//プレイヤーを生成する.
                break;
            case 4:
                p = PhotonNetwork.Instantiate("Pudu", Vector3.zero, Quaternion.identity);//プレイヤーを生成する.
                break;
            case 5:
                p = PhotonNetwork.Instantiate("Sparrow", Vector3.zero, Quaternion.identity);//プレイヤーを生成する.
                break;
            case 6:
                p = PhotonNetwork.Instantiate("Squid", Vector3.zero, Quaternion.identity);//プレイヤーを生成する.
                break;
            case 7:
                p = PhotonNetwork.Instantiate("Taipan", Vector3.zero, Quaternion.identity);//プレイヤーを生成する.
                break;
            case 8:
                p = PhotonNetwork.Instantiate("Colobus", Vector3.zero, Quaternion.identity);//プレイヤーを生成する.
                break;
        }
        Players.Add(p);
    }



    #endregion

    #region チャット機能関連
    public void PushSendChatButton()
    {
        string chat = InputChat.text;
        int master = PhotonNetwork.LocalPlayer.ActorNumber;//送信者の番号(全員がActorNumberを呼ばないために代入).
        InputChat.text = "";//送信したら消す.
        photonView.RPC(nameof(PushChat), RpcTarget.All, master, chat);
    }
    [PunRPC]
    void PushChat(int master, string chat)
    {
        ChatLog2.text = ChatLog.text;
        ChatLog.text = PlayersName[master - 1] + ":" + chat;
    }

    #endregion

    #region 信じるボタン・ダウトボタン関連
    /// <summary>
    /// 信じるボタンを押したときに呼び出す関数.
    /// </summary>
    public void PushBelieveButton()
    {
        int subject = PhotonNetwork.LocalPlayer.ActorNumber;
        photonView.RPC(nameof(AddThroughNum), RpcTarget.All, subject);
        ReasoningPanel.SetActive(false);
        WaitText.text = "他のプレイヤーの宣言をまっています";
    }
    /// <summary>
    /// 信じるボタンを押した人数を増やす関数.
    /// 信じる人数がプレイ人数(サイコロを振った人がいるので-1)と一致したらマスを進ませる.
    /// </summary>
    [PunRPC]
    void AddThroughNum(int subject)
    {
        Debug.Log("AddThroughNum()起動");
        PlayersNameGroup[subject - 1].transform.GetChild(5).gameObject.SetActive(true);
        ThroughNum++;
        if (MaxPlayers - 1 == ThroughNum)
        {
            DeclarationFlg = true;

            ThroughNum = 0;
            //WaitText.text = "";
            DiceInit();
        }
    }

    /// <summary>
    /// ダウトボタンを押した時に呼び出す関数
    /// </summary>
    public void PushDoubtButton()
    {
        int subject = PhotonNetwork.LocalPlayer.ActorNumber;
        photonView.RPC(nameof(DeclarationDoubt), RpcTarget.All, subject);
    }

    /// <summary>
    /// ダウト宣言ボタンを押した際に呼び出す関数
    /// 先着1人が起動でき、起動したら他の人のフラグをONにする
    /// </summary>
    [PunRPC]
    void DeclarationDoubt(int subject)
    {
        int turn = (int)PhotonNetwork.CurrentRoom.CustomProperties["Turn"];
        ReasoningPanel.SetActive(false);//他プレイヤーのパネルを強制的に閉じる.
        Debug.Log("DeclarationDoubt起動");
        Debug.Log("呼び出したPlayer" + PlayersName[subject - 1]);
        DoubtPanel.SetActive(true);
        DoubtPanel.transform.GetChild(0).GetComponent<Text>().text = PlayersName[subject - 1] + "さんがダウト宣言を行いました";
        if (doubtFlg)//嘘をついていた場合の処理.
        {
            FailureDoubt = true;
            DoubtPanel.transform.GetChild(1).GetComponent<Text>().text = PlayersName[turn - 1] + "さんは嘘をついていました";
            DoubtPanel.transform.GetChild(2).GetComponent<Text>().text = "ダウト失敗した" + PlayersName[turn - 1] + "さんに1ダメージ";
            if (turn == PhotonNetwork.LocalPlayer.ActorNumber)//自分自身が対象の場合のみHPを変化させる関数を呼ぶ.
            {
                photonView.RPC(nameof(ChangeHP), RpcTarget.All, -1, turn);
            }
        }
        else//嘘をついていなかった場合の処理.
        {
            DoubtPanel.transform.GetChild(1).GetComponent<Text>().text = PlayersName[turn - 1] + "さんは嘘をついていませんでした";
            DoubtPanel.transform.GetChild(2).GetComponent<Text>().text = "ダウト失敗した" + PlayersName[subject - 1] + "さんに1ダメージ";
            if (subject == PhotonNetwork.LocalPlayer.ActorNumber)//自分自身が対象の場合のみHPを変化させる関数を呼ぶ.
            {
                photonView.RPC(nameof(ChangeHP), RpcTarget.All, -1, subject);
            }
        }
        StartCoroutine(WaitDoubtPanelCoroutine());
    }
    private IEnumerator WaitDoubtPanelCoroutine()
    {
        // 2秒間待つ
        yield return new WaitForSeconds(4);
        Debug.Log("コルーチン呼び出し終了");
        DoubtPanel.SetActive(false);
        DeclarationFlg = true;
        DiceInit();

        yield break;
    }
    #endregion

    #region サイコロ関連関数
    /// <summary>
    /// サイコロを振る用の関数
    /// サイコロのプレファブが無ければ作成し、有れば表示・非表示を繰り返す.
    /// </summary>
    public void ShakeDice()
    {
        DiceCamera.SetActive(true);
        Dice.SetActive(true);
        Dice.transform.position = diceCameraPos;
        diceCameraPos = DiceCamera.transform.position;
        diceCameraPos.x += 3;
        diceCameraPos.y -= 3;
        rotateX = rotateY = rotateZ = Random.Range(0, 360);//回転をランダムに決定.
        Dice.GetComponent<Rigidbody>().AddForce(-transform.right * 300);
        Dice.transform.Rotate(rotateX, rotateY, rotateZ);
        DiceFlg = true;//さいころを振った.
    }

    /// <summary>
    /// 出目をテキストで表示後、少し時間を開けてからパネルを表示するコルーチン本体.
    /// </summary>
    private IEnumerator HiddenDiceCoroutine()
    {
        // 2秒間待つ
        yield return new WaitForSeconds(2);
        Debug.Log("コルーチン呼び出し終了");
        ActivePanel();
        yield break;
    }

    /// <summary>
    /// 出目確定、UIに数値や文字を表示する
    /// </summary>
    public void ConfirmNumber(int num)
    {
        //サイコロの上にカメラを持ってくる処理.
        Vector3 position = Dice.transform.position;
        position.y += 5;

        DiceCamera.transform.position = position;

        if (num == ATTACK)            //攻撃目が出た場合Attackと表示する.
        {
            DiceNumText.text = "Attack";
        }
        else if (num == 5 || num == 6)//ダウト目の場合Doubtと表示.
        {
            DiceNumText.text = "Doubt";
            doubtFlg = true;
            num = DOUBT;
        }
        else                          //ただの数字の場合そのまま出力.
        {
            DiceNumText.text = num.ToString();
        }
        Number = num;                 //パネル表示分岐用に代入する.
        StartCoroutine(HiddenDiceCoroutine());
    }

    /// <summary>
    /// サイコロを振るボタンを押したら呼ばれる関数.
    /// </summary>
    public void PushShakeDiceButton()
    {
        ShakeDiceButton.SetActive(false);
        CardButton.SetActive(false);
        diceBtnFlg = true;//ボタンを押した状態にする.
        if (DiceFlg == false)//サイコロを振っていないなら.
        {
            ShakeDice();
        }
    }

    /// <summary>
    /// 出目によって表示するパネルを変える.
    /// </summary>
    private void ActivePanel()
    {
        ResultPanel[Number - 1].SetActive(true);
    }

    #region 各出目のボタン
    /// <summary>
    /// 1の出目ボタンを押したら.
    /// </summary>
    public void PushOneButton()
    {
        DeclarationNum = 1;
        DeclarationResult();
    }
    /// <summary>
    /// 2の出目ボタンを押したら.
    /// </summary>
    public void PushTwoButton()
    {
        DeclarationNum = 2;
        DeclarationResult();
    }
    /// <summary>
    /// 3の出目ボタンを押したら.
    /// </summary>
    public void PushThreeButton()
    {
        DeclarationNum = 3;
        DeclarationResult();
    }
    /// <summary>
    /// 攻撃出目ボタンを押したら.
    /// </summary>
    public void PushAttackButton()
    {
        DeclarationNum = ATTACK;
        DeclarationResult();
    }
    #endregion

    /// <summary>
    /// 宣言された出目をプレイヤーに返す関数
    /// 終了後、サイコロを初期設定に戻す.
    /// </summary>
    private void DeclarationResult()
    {
        DiceFinishFlg = true;
        for (int i = 0; i < 5; i++)//全てのリザルト画面を非表示にする.
        {
            ResultPanel[i].SetActive(false);
        }
        Debug.Log("出目：" + DeclarationNum);
        DiceNumText.text = " ";
        DiceCamera.SetActive(false);
        Dice.SetActive(false);
        DiceFlg = false;
    }

    /// <summary>
    /// サイコロの初期化関数
    /// サイコロを振り終わったあとに呼び出し、設定を全て初期化する.
    /// </summary>
    private void DiceInit()
    {
        DiceFinishFlg = false;
        DiceNumText.text = " ";
        DeclarationNum = 0;
        ReasoningPanel.SetActive(false);
        Debug.Log("DiceInit起動");
        DiceCamera.SetActive(false);
        Dice.SetActive(false);
        DiceCamera.transform.position = diceCameradefPos;
        DiceFlg = false;
    }

    /// <summary>
    /// 宣言された出目を受け取る関数.
    /// </summary>
    public void ReceiveDeme(int deme)
    {
        if (DeclarationNum == 5 || DeclarationNum == 6)
        {
            Debug.Log("ダウト目が出ている");
        }
        WaitText.text = "他プレイヤーの宣言を待っています";
        photonView.RPC(nameof(ActiveEnemyResult), RpcTarget.Others, deme);//サイコロを振っていない人にパネルを表示する.
    }

    /// <summary>
    /// サイコロの出目を他の人に通知するパネルを表示する関数.
    /// </summary>
    [PunRPC]
    void ActiveEnemyResult(int deme)
    {
        ReasoningPanel.SetActive(true);
        ReasoningPanel.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = DiceSprites[deme - 1];
    }

    #endregion

    #region メッセージ表示関連

    /// <summary>
    /// Playerからメッセージの文字列を受け取って表示する関数.
    /// </summary>
    public void ShowMessage(string message, int num)
    {
        Debug.Log("Shomeesage起動");
        FinMessage = false;
        photonView.RPC(nameof(ShowMessageAll), RpcTarget.All, message, num);
    }

    [PunRPC]
    private void ShowMessageAll(string message, int num)
    {
        MessageWindow.SetActive(true);
        MessageWindow.transform.GetChild(0).GetComponent<Text>().text = PlayersName[num - 1] + "さんが\n" + message;
        StartCoroutine(WaitMessageWindowCoroutine());
    }

    /// <summary>
    /// ターン開始時にメッセージ通知を出すコルーチン.
    /// </summary>
    private IEnumerator TurnMessageWindowCoroutine()
    {
        int turn = (int)PhotonNetwork.CurrentRoom.CustomProperties["Turn"];
        MessageWindow.SetActive(true);
        MessageWindow.transform.GetChild(0).GetComponent<Text>().text = PlayersName[turn - 1] + "さんのターンです";
        // 2秒間待つ
        yield return new WaitForSeconds(2);
        MessageWindow.SetActive(false);
        FinMessage = true;
        doubtFlg = diceBtnFlg = FailureDoubt = false;
        DeclarationFlg = false;    //全員の宣言待ち状態をfalseにする.

        yield break;
    }
    /// <summary>
    /// 3秒経過でメッセージウィンドウを閉じるコルーチン.
    /// </summary>
    private IEnumerator WaitMessageWindowCoroutine()
    {
        // 3秒間待つ
        yield return new WaitForSeconds(2);
        MessageWindow.SetActive(false);
        FinMessage = true;
        yield break;
    }

    #endregion

    #region 持ち時間減少関連

    [PunRPC]
    private void StartTimer()
    {
        //timeflg = true;
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
    #endregion

    #region　遊び方説明関連
    public void PushWakabaTutorial()
    {
        TutorialCanvas.SetActive(true);
    }
    public void PushSkipTutorial()
    {
        TutorialCanvas.SetActive(false);
    }
    public void PushNextTutorial()
    {
        if (NowTutorial < 4)
        {
            NowTutorial++;
        }
        TutorialCanvas.transform.GetChild(1).GetComponent<Image>().sprite = TutorialSprites[NowTutorial];
    }
    public void PushFrontTutorial()
    {
        if (NowTutorial > 0)
        {
            NowTutorial--;
        }
        TutorialCanvas.transform.GetChild(1).GetComponent<Image>().sprite = TutorialSprites[NowTutorial];
    }
    #endregion

    #region プレイヤーにIDを与える関数
    public int Give_ID_Player()
    {
        int ID = 1;
        for (int i = 0; i < UseID.Length; i++)
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

    #endregion

    #region Photon関連(override・変数送信)

    #region Photon関連のoverride関数
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("RoomTNC", new RoomOptions(), TypedLobby.Default);
    }

    // <summary>
    // リモートプレイヤーが入室した際にコールされる
    // </summary>
    public void OnPhotonPlayerConnected()
    {
        Debug.Log("入室");
    }
    public override void OnJoinedRoom()
    {
        NowGameState = GameState.SetGame;

        // カスタムプロパティの設定（数値）
        int turn = FIRST_TURN;
       
        customProperties["Turn"] = turn;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        customProperties.Clear();
    }
    #endregion



    /// <summary>
    /// PUNを使って変数を同期する.
    /// </summary>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(HaveTime);// timeを送信する
            //stream.SendNext(WhoseTurn);
            //stream.SendNext(PlayersHP);
            stream.SendNext(doubtFlg);
            stream.SendNext(Ranks);
            stream.SendNext(AnimalChildNum);
        }
        else
        {
            HaveTime = (float)stream.ReceiveNext(); // timeを受信する
            //WhoseTurn = (int)stream.ReceiveNext();
            //PlayersHP = (int[])stream.ReceiveNext();
            doubtFlg = (bool)stream.ReceiveNext();
            Ranks = (int[])stream.ReceiveNext();
            AnimalChildNum = (int)stream.ReceiveNext();
        }
    }
    #endregion
}

public static class PhotonCustumPropertie
{
    private const string GameStatusKey = "Gs";
    private const string InitStatusKey = "Is";
    private const string ReadyNum = "Rn";
    private const string PlayerHP = "P_HP";

    private static readonly ExitGames.Client.Photon.Hashtable propsToSet = new ExitGames.Client.Photon.Hashtable();

    /// <summary>
    /// 引数でPhotonのプレイヤーを渡すことで
    /// 戻り値でGameStatusが返ってくる。int型で返るので、キャストする
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static int GetGameStatus(this Player player)
    {
        return (player.CustomProperties[GameStatusKey] is int status) ? status : 0;
    }

    /// <summary>
    /// 引数でPhotonのプレイヤーとGameStatusを渡すことで
    /// 他プレイヤーに送信する
    /// </summary>
    /// <param name="player"></param>
    public static void SetGameStatus(this Player player, int status)
    {
        propsToSet[GameStatusKey] = status;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// 引数でPhotonのプレイヤーを渡すことで
    /// 戻り値でそのプレイヤーの初期化情報が返る
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetInitStatus(this Player player)
    {
        return (player.CustomProperties[InitStatusKey] is bool status) ? status : false;
    }

    /// <summary>
    /// 引数でPhotonのプレイヤーと初期化状態を渡すことで
    /// 他プレイヤーに送信する
    /// </summary>
    /// <param name="player"></param>
    public static void SetInitStatus(this Player player, bool status)
    {
        propsToSet[InitStatusKey] = status;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }


    /// <summary>
    /// 引数でPhotonのプレイヤーを渡すことで
    /// 戻り値でそのプレイヤーの初期化情報が返る
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetReadyNum(this Player player)
    {
        return (player.CustomProperties[ReadyNum] is bool status) ? status : false;
    }

    /// <summary>
    /// 引数でPhotonのプレイヤーと初期化状態を渡すことで
    /// 他プレイヤーに送信する
    /// </summary>
    /// <param name="player"></param>
    public static void SetReadyNum(this Player player, bool status)
    {
        propsToSet[ReadyNum] = status;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    public static int GetPlayerHP(this Player player)
    {
        return (player.CustomProperties[PlayerHP] is int status) ? status : 0;
    }

    public static void SetPlayerHP(this Player player, int status)
    {
        propsToSet[PlayerHP] = status;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}