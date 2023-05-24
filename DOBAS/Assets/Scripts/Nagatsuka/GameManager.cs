using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    /// <summary>
    /// ゲームの状況.
    /// </summary>
    public enum GameState
    {
        SetGame,
        InGame,
        EndGame,
    }
    public GameState NowGameState;//現在のゲームモード.

    TestPlayerTarn[] Players;
    private int WhoseTurn = 0;
    public int Votes = 0;

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

        SetUp();
    }

    void SetUp()
    {
        // ゲーム前状態
        NowGameState = GameState.SetGame;
        //プレイヤーを探す(仮)
        GameObject[] PlayerArray = GameObject.FindGameObjectsWithTag("player");
        //if (PlayerArray != null) Debug.Log("中身ある");

        Players = new TestPlayerTarn[PlayerArray.Length];

        //配列にプレイヤー挿入
        for (int i = 0; i < PlayerArray.Length; i++)
        {
            Players[i] = PlayerArray[i].GetComponent<TestPlayerTarn>();
        }
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
    void MainGame()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (WhoseTurn >= Players.Length)
            {
                WhoseTurn = 0;
            }
            if (!Players[WhoseTurn].Tarn)
            {
                Debug.Log(Players[WhoseTurn]);
                Players[WhoseTurn].Tarn = true;
            }

        }
    }
    void EndGame()
    {

    }
    public void EndJudge(bool Flg)
    {
        if (Flg)
        {
            WhoseTurn++;
        }
    }

    public void DoutDis(int Number)
    {
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].DoutDec = true;
        }
        if (Players[WhoseTurn].Tarn)
        {
            if (Number == 5 || Number == 6)
            {
                Debug.Log("嘘目");
                Players[WhoseTurn].DoutFlg = true;
            }
            else
            {
                Debug.Log("真目");
                Players[WhoseTurn].DoutFlg = false;
            }
        }
    }
    public void DoutJudge()
    {
        Debug.Log(Votes);
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].DoutDec = false;
            Players[i].Count = 5.0f;
        }
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[WhoseTurn].DoutFlg)
            {
                if (Players[i].MyDec)
                {
                    Debug.Log(Players[WhoseTurn] + "は嘘ついてたわ");
                    break;
                }
            }
            if (!Players[WhoseTurn].DoutFlg)
            {
                if (Players[i].MyDec)
                {
                    Debug.Log(Players[WhoseTurn] + "は嘘ついてない");
                    break;
                }
            }
            if (!Players[i].MyDec)
            {
                Votes++;
            }
            if (Votes == 4)
            {
                Debug.Log("スルー");
            }
        }
        Players[WhoseTurn].TarnEnd = true; // 時間差で他のTARNENDがオンになっている可能性
        Votes = 0;

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

        // ゲームの状況管理
        if (NowGameState == GameState.InGame)
        {
            //SceneManager.LoadScene("Main");
            MainGame();
        }
        if (NowGameState == GameState.EndGame)
        {
            //SceneManager.LoadScene("End");
            EndGame();
        }
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
