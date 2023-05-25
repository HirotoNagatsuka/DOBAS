using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    /// <summary>
    /// �Q�[���̏�.
    /// </summary>
    public enum GameState
    {
        InitGame,//�������.
        SetGame,
        InGame,
        EndGame,
    }
    public GameState NowGameState;//���݂̃Q�[�����[�h.

    [Header("�Q�[���J�n���ɕ\���������")]
    [SerializeField] GameObject CanvasUI;
    [SerializeField] GameObject StartButton;

    TestPlayerTarn[] Players;
    private int WhoseTurn = 0;
    public int Votes = 0;
    public int MaxPlayers;//�v���C���[�̍ő�l��.

    [SerializeField] Text HaveTimeText;
    float HaveTime;//�e�v���C���[�̎�������.
    float DoubtTime;//�_�E�g�錾�̎�������.
    bool DoubtFlg;
    bool timeflg;

    [SerializeField] GameObject ShakeDiceButton;
    public bool IsTurn;//�����̃^�[����.

    #region Unity�C�x���g(Start�EUpdate)
    // Start is called before the first frame update
    void Start()
    {
        HaveTime = 60;
        DoubtFlg = false;
        timeflg = false;

        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        if (NowGameState == GameState.InGame)//�Q�[�����[�h���Q�[�����Ȃ�.
        {
            if (IsTurn) ShakeDiceButton.SetActive(true);
            else ShakeDiceButton.SetActive(false);

            ChangeTurn();
            if (Input.GetKeyDown(KeyCode.P))
            {
                photonView.RPC(nameof(StartTimer), RpcTarget.All);
            }

            if (DoubtFlg) ChangeDoubtTime();
            //else ChangeHaveTime();
            else if (timeflg) HaveTime -= Time.deltaTime;
            HaveTimeText.text = HaveTime.ToString("0");

            // �Q�[���̏󋵊Ǘ�
            if (NowGameState == GameState.InGame)
            {
                MainGame();
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
    /// �}�X�^�[�N���C�A���g�Ȃ�΃^�[�����Ǘ�����.
    /// </summary>
    void ChangeTurn()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)//���g���}�X�^�[�N���C�A���g���𔻒肷��
        {
            Debug.Log("�����}�X�^�[�N���C�A���g�ł�");


        }
    }




    void SetUp()
    {
        // �Q�[���O���
        NowGameState = GameState.InitGame;
        //�v���C���[��T��(��)
        GameObject[] PlayerArray = GameObject.FindGameObjectsWithTag("player");

        Players = new TestPlayerTarn[PlayerArray.Length];

        //�z��Ƀv���C���[�}��
        for (int i = 0; i < PlayerArray.Length; i++)
        {
            Players[i] = PlayerArray[i].GetComponent<TestPlayerTarn>();
        }
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
                Debug.Log("�R��");
                Players[WhoseTurn].DoutFlg = true;
            }
            else
            {
                Debug.Log("�^��");
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
                    Debug.Log(Players[WhoseTurn] + "�͉R���Ă���");
                    break;
                }
            }
            if (!Players[WhoseTurn].DoutFlg)
            {
                if (Players[i].MyDec)
                {
                    Debug.Log(Players[WhoseTurn] + "�͉R���ĂȂ�");
                    break;
                }
            }
            if (!Players[i].MyDec)
            {
                Votes++;
            }
            if (Votes == 4)
            {
                Debug.Log("�X���[");
            }
        }
        Players[WhoseTurn].TarnEnd = true; // ���ԍ��ő���TARNEND���I���ɂȂ��Ă���\��
        Votes = 0;

    }

    

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // time�𑗐M����
            stream.SendNext(HaveTime);
        }
        else
        {
            // time����M����
            HaveTime = (float)stream.ReceiveNext();
        }
    }
    [PunRPC]
    private void StartTimer()
    {
        timeflg = true;
    }

    /// <summary>
    /// ���������{�^�����������ۂɌĂяo���֐�.
    /// </summary>
    public void PushGameStart()
    {
        StartButton.SetActive(false);
        NowGameState = GameState.InGame;
        CanvasUI.SetActive(true);
        var position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
    }


    private void ChangeHaveTime()
    {
        if (HaveTime > 0)//�c�莞�Ԃ��c���Ă���Ȃ�.
        {
            HaveTime -= Time.deltaTime;
            if (HaveTime <= 10)//10�b�ȉ��ɂȂ�����Ԃ�����.
            {
                HaveTimeText.color = Color.red;
            }
            HaveTimeText.text = HaveTime.ToString("0");//�����_�ȉ���\�����Ȃ�.
        }
        else//0�ȉ��ɂȂ�����.
        {
            HaveTime = 0;
            Debug.Log("�^�[�������I��");
        }        
    }
    public void StartDoubtTime()
    {
        DoubtTime = 10;
        DoubtFlg = true;
    }
    private void ChangeDoubtTime()
    {
        if (DoubtTime > 0)//�c�莞�Ԃ��c���Ă���Ȃ�.
        {
            DoubtTime -= Time.deltaTime;
            HaveTimeText.color = Color.red;
            
            HaveTimeText.text = DoubtTime.ToString("0");//�����_�ȉ���\�����Ȃ�.
        }
        else//0�ȉ��ɂȂ�����.
        {
            DoubtTime = 0;
            Debug.Log("�^�[�������I��");
            DoubtFlg = false;
        }
    }
}
