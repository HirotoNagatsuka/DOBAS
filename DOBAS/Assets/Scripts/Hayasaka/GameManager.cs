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
    //-1:非ゲーム状態 1:ゲーム中 2:ゲーム終了
    private int GameFlg;

    GameObject UI;
    public GameObject[] Players;
    TestPlayerTarn[] PlayersTpt;
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
        Players = new GameObject[PlayerArray.Length];
        PlayersTpt = new TestPlayerTarn[PlayerArray.Length];
        for (int i = 0; i < PlayerArray.Length; i++)
        {
            Players[i] = PlayerArray[i];
            PlayersTpt[i] = Players[i].GetComponent<TestPlayerTarn>();
        }
        //UIを探す(仮)
        UI = GameObject.Find("PlayerUI");

        //Hp = 4;
        //AtPower = 1;
        //HaveCard = 0;
        //LocaInfo = 0;

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
        for (int i = 0; i < 4; i++)
        {
            Debug.Log(Players[i]);
            PlayersTpt[i].Test();
           if(PlayersTpt[i].TestOrder == i+1 && !PlayersTpt[i].TarnEnd)
           {
              PlayersTpt[i].Tarn = true;
           }
           
        }
        
    }
    void EndGame()
    {
       
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
