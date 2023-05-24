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
        SetGame,
        InGame,
        EndGame,
    }
    public GameState NowGameState;//���݂̃Q�[�����[�h.

    TestPlayerTarn[] Players;
    private int WhoseTurn = 0;
    public int Votes = 0;

    [SerializeField] Text HaveTimeText;
    float HaveTime;//�e�v���C���[�̎�������.
    float DoubtTime;//�_�E�g�錾�̎�������.
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
        // �Q�[���O���
        NowGameState = GameState.SetGame;
        //�v���C���[��T��(��)
        GameObject[] PlayerArray = GameObject.FindGameObjectsWithTag("player");
        //if (PlayerArray != null) Debug.Log("���g����");

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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //photonView.RPC(nameof(RpcSendMessage), RpcTarget.All, "����ɂ���");
            photonView.RPC(nameof(StartTimer), RpcTarget.All);
        }

        if (DoubtFlg) ChangeDoubtTime();
        //else ChangeHaveTime();
        else if (timeflg) HaveTime -= Time.deltaTime;
        HaveTimeText.text = HaveTime.ToString("0");

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

    [PunRPC]
    private void RpcSendMessage(string message)
    {
        timeflg = true;
        Debug.Log(message);
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
