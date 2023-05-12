using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        SetGame,
        InGame,
        EndGame,
    }
    public GameState NowGameState;

    public int Hp;
    public int AtPower;
    public int HaveCard;
    public int LocaInfo;
    public int Votes = 0;

    public int Index = 0;
    public int EndPt = 0;
    //-1:非ゲーム状態 1:ゲーム中 2:ゲーム終了
    private int GameFlg;

    private bool TarnFlg;
    GameObject UI;
    
    TestPlayerTarn[] Players;

    //TestPlayerTarn[] SortPlayers;

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
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
    void SetUp()
    {
        // ゲーム前状態
        NowGameState = GameState.SetGame;
        //プレイヤーを探す(仮)
        GameObject[] PlayerArray = GameObject.FindGameObjectsWithTag("player");
        
        Players = new TestPlayerTarn[PlayerArray.Length];
        //SortPlayers = new TestPlayerTarn[PlayerArray.Length];
        
        //配列にプレイヤー挿入
        for (int i = 0; i < PlayerArray.Length; i++)
        {
            Players[i] = PlayerArray[i].GetComponent<TestPlayerTarn>();
        }
        //UIを探す(仮)
        UI = GameObject.Find("PlayerUI");

        //try
        //{
        //    if (Player)
        //    {
        //        //Debug.Log("プレイヤー存在");
        //    }
        //    if (UI)
        //    {
        //        //Debug.Log("UI存在");
        //    }
        //}
        //catch (System.NullReferenceException )
        //{
        //    KeyOperation();
        //}
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
            if (Index >= Players.Length)
            {
                Index = 0;
            }
            if (!Players[Index].Tarn)
            {
                Debug.Log(Players[Index]);
                Players[Index].Tarn = true;
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
            Index++;
        }
    }
    
    public void DoutDis(int Number)
    {
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].DoutDec = true;
        }
        if (Players[Index].Tarn)
        {
            if (Number == 5 || Number == 6)
            {
                Debug.Log("嘘目");
                Players[Index].DoutFlg = true;
            }
            else
            {
                Debug.Log("真目");
                Players[Index].DoutFlg = false; 
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
            if (Players[Index].DoutFlg) 
            {
                if (Players[i].MyDec)
                {
                    Debug.Log(Players[Index] + "は嘘ついてたわ");
                    break;
                }
            }
            if (!Players[Index].DoutFlg)
            {
                if (Players[i].MyDec)
                {
                    Debug.Log(Players[Index] + "は嘘ついてない");
                    break;
                }
            }
            if (!Players[i].MyDec)
            {
                Votes++;
            }
            if(Votes == 4)
            {
                Debug.Log("スルー");
            }
        }  
        Players[Index].TarnEnd = true; // 時間差で他のTARNENDがオンになっている可能性
        Votes = 0;

    }
    public void GemeOverJudge()
    {
        EndPt++;
        if(EndPt == Players.Length - 1)
        {
            NowGameState = GameState.EndGame;
        }
    }
    //強制終了処理
    void KeyOperation()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
        #else
        Application.Quit();//ゲームプレイ終了
        #endif
        }
    }
}
