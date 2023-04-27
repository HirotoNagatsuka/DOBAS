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
    //-1:��Q�[����� 1:�Q�[���� 2:�Q�[���I��
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
        Players = new GameObject[PlayerArray.Length];
        PlayersTpt = new TestPlayerTarn[PlayerArray.Length];
        for (int i = 0; i < PlayerArray.Length; i++)
        {
            Players[i] = PlayerArray[i];
            PlayersTpt[i] = Players[i].GetComponent<TestPlayerTarn>();
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
