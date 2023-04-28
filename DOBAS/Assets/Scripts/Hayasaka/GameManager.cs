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

    private int Index = 0;
    public int EndPt = 0;
    //-1:��Q�[����� 1:�Q�[���� 2:�Q�[���I��
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
        // �Q�[���̏󋵊Ǘ�
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
        // �Q�[���O���
        NowGameState = GameState.SetGame;
        //�v���C���[��T��(��)
        GameObject[] PlayerArray = GameObject.FindGameObjectsWithTag("player");
        
        Players = new TestPlayerTarn[PlayerArray.Length];
        //SortPlayers = new TestPlayerTarn[PlayerArray.Length];
        
        //�z��Ƀv���C���[�}��
        for (int i = 0; i < PlayerArray.Length; i++)
        {
            Players[i] = PlayerArray[i].GetComponent<TestPlayerTarn>();
        }
        //UI��T��(��)
        UI = GameObject.Find("PlayerUI");

        //Hp = 4;
        //AtPower = 1;
        //HaveCard = 0;
        //LocaInfo = 0;

        //try
        //{
        //    if (Player)
        //    {
        //        //Debug.Log("�v���C���[����");
        //    }
        //    if (UI)
        //    {
        //        //Debug.Log("UI����");
        //    }
        //}
        //catch (System.NullReferenceException )
        //{
        //    KeyOperation();
        //}
    }
    //void SetNextPlayer()
    //{      
    //    Index++;
    //    for (int i = 0; i < Players.Length; i++)
    //    {
    //        if(Players[i].TestOrder == Index)
    //        {

    //        }
    //    }
    //    if (Index >= Players.Length)
    //    {
    //        Index = 0;
    //    }
    //}
    #region �Q�[����
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
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    Players[Index].TarnEnd = true;
        //    if (Index > Players.Length)
        //    {
        //        Index = 0;
        //    }
        //    else
        //    {
        //        Index++;
        //    }
        //    for(int i = 0;i <= Players.Length; i++)
        //    {
        //        Players[i].Tarn = false; 
        //    }          
        //}
        //if (!Players[Index].Tarn)
        //{ 
        //    Debug.Log(Players[Index]);
        //    //Players[i].Test();

        //    Players[Index].Tarn = true;
        //}
        
        //����
        for (int i = 0; i < Players.Length; i++)
        {
            if (!Players[Index].Tarn)
            {
                Debug.Log(Players[Index]);
                Players[Index].Tarn = true;
            }
            if (Players[Index].TarnEnd)
            {
                Players[Index].Tarn = false;
                Players[Index].TarnEnd = false;
                Index++;
            }
            if (Index >= Players.Length)
            {
                Index = 0;
            }          
        }
    }
    void EndGame()
    {
       
    }
    public void GemeOverJudge()
    {
        EndPt++;
        if(EndPt == Players.Length - 1)
        {
            NowGameState = GameState.EndGame;
        }
    }
    //�����I������
    void KeyOperation()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
        #else
        Application.Quit();//�Q�[���v���C�I��
        #endif
        }
    }
}
