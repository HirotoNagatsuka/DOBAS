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

    public int WhoseTurn = 1;
    public int Votes = 0;
    public int MaxPlayers;//�v���C���[�̍ő�l��.

    [SerializeField] Text HaveTimeText;
    float HaveTime;//�e�v���C���[�̎�������.
    float DoubtTime;//�_�E�g�錾�̎�������.
    bool DoubtFlg;
    bool timeflg;

    [SerializeField] GameObject ShakeDiceButton;
    public int IsTurnNum;//���ԖڂɃ^�[�������邩.

    #region Unity�C�x���g(Start�EUpdate)
    // Start is called before the first frame update
    void Start()
    {
        HaveTime = 60;
        DoubtFlg = false;
        timeflg = false;
       // SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        if (NowGameState == GameState.InGame)//�Q�[�����[�h���Q�[�����Ȃ�.
        {
            Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
            if (PhotonNetwork.LocalPlayer.ActorNumber == WhoseTurn) ShakeDiceButton.SetActive(true);
            else ShakeDiceButton.SetActive(false);

            //ChangeTurn();
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
               // MainGame();
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

    public void FinishDice()
    {
        photonView.RPC(nameof(ChangeTurn), RpcTarget.All);
    }
    /// <summary>
    /// �^�[�����Ǘ�����.
    /// </summary>
    [PunRPC]
    public void ChangeTurn()
    {
        if (WhoseTurn == MaxPlayers) WhoseTurn = 1;
        else WhoseTurn++;
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


    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(HaveTime);// time�𑗐M����
            stream.SendNext(WhoseTurn);
        }
        else
        {
            HaveTime = (float)stream.ReceiveNext(); // time����M����
            WhoseTurn = (int)stream.ReceiveNext();
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
    #region ����_�E�g����
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
    //            Debug.Log("�R��");
    //            Players[WhoseTurn].DoutFlg = true;
    //        }
    //        else
    //        {
    //            Debug.Log("�^��");
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
    //                Debug.Log(Players[WhoseTurn] + "�͉R���Ă���");
    //                break;
    //            }
    //        }
    //        if (!Players[WhoseTurn].DoutFlg)
    //        {
    //            if (Players[i].MyDec)
    //            {
    //                Debug.Log(Players[WhoseTurn] + "�͉R���ĂȂ�");
    //                break;
    //            }
    //        }
    //        if (!Players[i].MyDec)
    //        {
    //            Votes++;
    //        }
    //        if (Votes == 4)
    //        {
    //            Debug.Log("�X���[");
    //        }
    //    }
    //    Players[WhoseTurn].TarnEnd = true; // ���ԍ��ő���TARNEND���I���ɂȂ��Ă���\��
    //    Votes = 0;

    //}

    #endregion
}
