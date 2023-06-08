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

    const int ATTACK = 4;//攻撃出目の数字.
    //プレイヤーの詳細パネルの参照用.
    const int PLAYER_NAME = 1;
    const int PLAYER_HP = 2;
    const int PLAYER_ATTACK = 4;
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

    [Header("さいころ関連")]
    [SerializeField] GameObject DicePrefab;//サイコロのプレファブを入れる.
    [SerializeField] GameObject DiceCamera;
    [SerializeField] Text DiceNumText;
    [SerializeField] GameObject[] ResultPanel = new GameObject[5];//出目のパネル用.
    [SerializeField] GameObject ReasoningPanel;
    [SerializeField] GameObject DiceShakeButton;//サイコロを振るボタン.
    public GameObject EnemyResult;//相手の出目を入れる.


    public List<string> PlayersName = new List<string>();
    public int[] PlayersHP;
    public static int MaxPlayersNum;//他スクリプトからアクセスする用.
    public static int WhoseTurn;//誰のターンか（プレイヤーIDを参照してこの変数と比べてターン制御をする）.
    public Text WaitText;
    [SerializeField] GameObject PlayersNameGroupPrefab;//右上に表示する名前を生成する用.
    [SerializeField] Sprite[] HeartSprites = new Sprite[5];
    [SerializeField] Sprite[] DiceSprites = new Sprite[4];

    //[SerializeField] DiceManager diceManager;
    #endregion

    bool doubtFlg;
    private GameObject Dice;//サイコロ用の表示・非表示を繰り返す用.

    #region ランダムに回転させる用の変数宣言.
    private int rotateX;
    private int rotateY;
    private int rotateZ;
    #endregion


    Vector3 diceCameraPos;
    private bool DiceFlg;//サイコロ作成フラグ.

    private int Number;
    public int DeclarationNum;//宣言番号.

    public bool DiceFinishFlg;

    private bool[] UseID = new bool[4];//プレイヤーに割り当てるIDの使用状況.
    private int ReadyPeople;//準備完了人数.
    string TurnName;//誰のターンかの名前用.

    public bool DeclarationFlg;//宣言待ちフラグ.

    bool timeflg;
    public List<GameObject> PlayersNameGroup = new List<GameObject>();
    public List<GameObject> Players = new List<GameObject>();
    public int testTurn;
    #region Unityイベント(Start・Update)
    // Start is called before the first frame update
    void Start()
    {
        WhoseTurn = FIRST_TURN;
        PlayersHP = new int[MaxPlayers];//Playerの人数分HP配列を用意.
        for (int i = 0; i < MaxPlayers; i++)
        {
            PlayersHP[i] = FirstHP;//HPの初期値を代入.
        }

        timeflg = false;
        DeclarationFlg = false;
        MaxPlayersNum = MaxPlayers;
        WaitText.text = "";
        DiceStart();
    }

    void DiceStart()
    {
        Dice = Instantiate(DicePrefab, DiceCamera.transform.position, Quaternion.identity, this.transform);
        Dice.transform.position = diceCameraPos;
        diceCameraPos = DiceCamera.transform.position;
        diceCameraPos.x += 3;
        diceCameraPos.y -= 3;
        Dice.SetActive(false);
        DiceCamera.SetActive(false);
        DiceFlg = false;
        DiceFinishFlg = false;
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
        testTurn = WhoseTurn;
    }
    #endregion

    #region ターン変更関連関数
    /// <summary>
    /// さいころを振った後にRPCを呼び出し、ターンを変更する関数.
    /// </summary>
    public void FinishTurn()
    {
        DeclarationFlg = false;
        photonView.RPC(nameof(ChangeTurn), RpcTarget.All);//WhoseTurnを増やしてターンを変える.
    }

    /// <summary>
    /// ターンを管理する
    /// WhoseTurnを変更してターンを変更する.
    /// </summary>
    [PunRPC]
    public void ChangeTurn()
    {
        Debug.Log("ChangeTurn()起動");
        if (WhoseTurn == MaxPlayers) WhoseTurn = FIRST_TURN;//WhoseTurnがプレイヤーの最大数を超えたら最初の人に戻す.
        else WhoseTurn++;                                   //次の人のターンにする.
        DeclarationFlg = false;
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
            WhoseTurnText.text = PlayersName[WhoseTurn - 1] + "のターン";
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            photonView.RPC(nameof(StartTimer), RpcTarget.All);
        }
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
        PlayersNameGroup[subject - 1].transform.GetChild(PLAYER_HP).GetComponent<Image>().sprite = HeartSprites[PlayersHP[subject - 1] -1];
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
        int i=0;           // ループを使用して、全てのプレイヤーに対して名前を表示する例
        foreach (var player in PhotonNetwork.PlayerList)//プレイヤーの名前を取得.
        {
            PlayersName.Add(player.NickName);
            var obj = Instantiate(PlayersNameGroupPrefab, new Vector3(0f,0f,0f), Quaternion.identity,CanvasUI.transform);
            obj.GetComponent<RectTransform>().localPosition = new Vector3(760f, 465f -150f * i, 0f);
            obj.transform.GetChild(PLAYER_NAME).GetComponent<Text>().text = player.NickName;
            obj.transform.GetChild(PLAYER_HP).GetComponent<Image>().sprite = HeartSprites[PlayersHP[i] - 1];
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

        if (ReadyPeople != MaxPlayers)                        //入室した人数が指定した数に満たない場合
        {
            StandByGroup.SetActive(true);                     //待機人数を表示するキャンバスを表示する.
            HelloPlayerText.text = PhotonNetwork.NickName + "さんようこそ！";
            StandByText.text = "あと" + (MaxPlayers - ReadyPeople)
                                + "人待っています・・・";//最大人数と現在の人数を引いて待っている人数を表示.
        }
        else
        {
            photonView.RPC(nameof(StartGame), RpcTarget.All);
        }
    }
    #endregion

    /// <summary>
    /// 宣言された出目を受け取る関数.
    /// </summary>
    public void ReceiveDeme(int deme)
    {
        photonView.RPC(nameof(ActiveEnemyResult), RpcTarget.Others,deme);
        WaitText.text = "他プレイヤーの宣言を待っています";
    }

    [PunRPC]
    void ActiveEnemyResult(int deme)
    {
        EnemyResult.SetActive(true);
        EnemyResult.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = DiceSprites[deme - 1];
    }

    public void PushBelieveButton()
    {
        photonView.RPC(nameof(AddThroughNum), RpcTarget.All);
    }
    public void PushDoubtButton()
    {
        DeclarationFlg = true;
        DiceInit();
        FinishTurn();
    }
    public int ThroughNum;//信じた人数.
    /// <summary>
    /// 信じるボタンを押した人数を増やす関数.
    /// 信じる人数がプレイ人数(サイコロを振った人がいるので-1)と一致したらマスを進ませる.
    /// </summary>
    [PunRPC]
    void AddThroughNum()
    {
        Debug.Log("AddThroughNum()起動");
        ThroughNum++;
        if (MaxPlayers - 1 == ThroughNum)
        {
            DeclarationFlg = true;
            
            ThroughNum = 0;
            WaitText.text = "";
            DiceInit();
        }
    }
    
    void ResetFlg()
    {

    }


    /// <summary>
    /// サイコロを振る用の関数
    /// サイコロのプレファブが無ければ作成し、有れば表示・非表示を繰り返す.
    /// </summary>
    public void ShakeDice()
    {
        DiceShakeButton.SetActive(false);
        DiceCamera.SetActive(true);
        Dice.SetActive(true);
        Dice.transform.position = diceCameraPos;
        diceCameraPos = DiceCamera.transform.position;
        diceCameraPos.x += 3;
        diceCameraPos.y -= 3;
        rotateX = Random.Range(0, 360);
        rotateY = Random.Range(0, 360);
        rotateZ = Random.Range(0, 360);
        Dice.GetComponent<Rigidbody>().AddForce(-transform.right * 300);
        Dice.transform.Rotate(rotateX, rotateY, rotateZ);
        DiceFlg = true;
    }

    /// <summary>
    /// コルーチン本体
    /// 出目をテキストで表示後、少し時間を開けてからパネルを表示する.
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
        Number = num;
        if (num == 4)
        {
            DiceNumText.text = "Attack";
        }
        else if (num == 5 || num == 6)
        {
            DiceNumText.text = "Doubt";
        }
        else
        {
            DiceNumText.text = num.ToString();
        }
        StartCoroutine(HiddenDiceCoroutine());
    }

    /// <summary>
    /// サイコロを振るボタンを押したら呼ばれる関数.
    /// </summary>
    public void PushShakeDiceButton()
    {
        if (DiceFlg == false)
        {
            ShakeDice();
        }
    }

    /// <summary>
    /// 出目によって表示するパネルを変える.
    /// </summary>
    private void ActivePanel()
    {
        if (Number == 1)
        {
            ResultPanel[0].SetActive(true);
        }
        else if (Number == 2)
        {
            ResultPanel[1].SetActive(true);
        }
        else if (Number == 3)
        {
            ResultPanel[2].SetActive(true);
        }
        else if (Number == 4)
        {
            ResultPanel[3].SetActive(true);
        }
        else//ダウトの場合フラグをOnにする.
        {
            ResultPanel[4].SetActive(true);
            doubtFlg = true;
        }
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
        DeclarationNum = 4;
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
        //後々修正.
        ResultPanel[0].SetActive(false);
        ResultPanel[1].SetActive(false);
        ResultPanel[2].SetActive(false);
        ResultPanel[3].SetActive(false);
        ResultPanel[4].SetActive(false);

        Debug.Log("出目：" + DeclarationNum);
        DiceNumText.text = " ";
        DiceCamera.SetActive(false);
        Dice.SetActive(false);
        DiceFlg = false;
    }

    public void DiceInit()
    {
        DiceFinishFlg = false;
        DiceNumText.text = " ";
        DeclarationNum = 0;
        ReasoningPanel.SetActive(false);
        DiceCamera.SetActive(false);
        Dice.SetActive(false);
        DiceFlg = false;
        DiceShakeButton.SetActive(true);
    }

    /// <summary>
    /// 宣言された出目を返す関数
    /// </summary>
    public int ReturnDeclarationNum()
    {
        return DeclarationNum;
    }

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
    #endregion
}