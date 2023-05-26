using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region �萔�錾(Const)

    private const int FIRST_TURN = 1;//�ŏ��̐l�̃^�[��.
    #endregion


    #region public�ESerializeField�錾

    /// <summary>
    /// �Q�[���̏�.
    /// </summary>
    public enum GameState
    {
        InitGame,   //�������(Photon�Ɛڑ�����O�ɃJ�E���g�֐����Ă΂�Ȃ��悤�ɏ����Ə����𕪂���).
        SetGame,    //�Q�[���������.
        InGame,     //�Q�[���v���C���.
        EndGame,    //�Q�[���I�����.
    }
    [Header("�Q�[�����[�h")]
    public GameState NowGameState;//���݂̃Q�[�����[�h.

    [Header("SerializeField�錾")]
    [SerializeField] GameObject CanvasUI;       //�Q�[���J�n���ɂ܂Ƃ߂ăL�����o�X���\���ɂ���.
    [SerializeField] GameObject StartButton;    //���������{�^��.
    [SerializeField] GameObject StandByCanvas;  //���������L�����o�X.
    [SerializeField] Text StandByText;          //�ҋ@�l����\������{�^��.
    [SerializeField] Text WhoseTurnText;        //�N�̃^�[�����̃e�L�X�g.
    [SerializeField] Text HaveTimeText;         //�������ԃe�L�X�g.
    [SerializeField] GameObject StandByGroup;   //���������̃O���[�v.
    [SerializeField] GameObject ShakeDiceButton;//���������U��{�^��.

    [Header("public�錾")]
    public static int WhoseTurn;//�N�̃^�[�����i�v���C���[ID���Q�Ƃ��Ă��̕ϐ��Ɣ�ׂă^�[�����������j.
    public int MaxPlayers;//�v���C���[�̍ő�l��.

    #endregion

    int ReadyPeople;//���������l��.
    string TurnName;//�N�̃^�[�����̖��O�p.

    float HaveTime;//�e�v���C���[�̎�������.
    float DoubtTime;//�_�E�g�錾�̎�������.
    bool DoubtFlg;
    bool timeflg;

    #region Unity�C�x���g(Start�EUpdate)
    // Start is called before the first frame update
    void Start()
    {
        WhoseTurn = FIRST_TURN;
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
            if (PhotonNetwork.LocalPlayer.ActorNumber == WhoseTurn)
            {
                ShakeDiceButton.SetActive(true);
                WhoseTurnText.text = "���Ȃ��̃^�[��";
            }
            else
            {
                ShakeDiceButton.SetActive(false);
                WhoseTurnText.text = "";
            }

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
            StartButton.SetActive(true);
            StandByText.text = "����" +
                                (MaxPlayers - ReadyPeople)
                                + "�l�҂��Ă��܂��E�E�E";//�ő�l���ƌ��݂̐l���������đ҂��Ă���l����\��.
            //int playercnt = PhotonNetwork.CurrentRoom.PlayerCount;
            //if (playercnt >= MaxPlayers)
            //{
            //    StartButton.SetActive(true);
            //}
        }
    }
    #endregion


    #region �^�[���ύX�֘A�֐�
    /// <summary>
    /// ���������U�������RPC���Ăяo���A�^�[����ύX����֐�.
    /// </summary>
    public void FinishDice()
    {
        photonView.RPC(nameof(ChangeTurn), RpcTarget.All);//WhoseTurn�𑝂₵�ă^�[����ς���.
    }

    /// <summary>
    /// �^�[�����Ǘ�����
    /// WhoseTurn��ύX���ă^�[����ύX����.
    /// </summary>
    [PunRPC]
    public void ChangeTurn()
    {
        if (WhoseTurn == MaxPlayers) WhoseTurn = FIRST_TURN;//WhoseTurn���v���C���[�̍ő吔�𒴂�����ŏ��̐l�ɖ߂�.
        else WhoseTurn++;                                   //���̐l�̃^�[���ɂ���.

        //if(PhotonNetwork.LocalPlayer.ActorNumber == WhoseTurn)//�����̃^�[���ɂȂ����疼�O�𑗂�.
        //{
        //    TurnName = PhotonNetwork.NickName;
        //    WhoseTurnText.text = PhotonNetwork.NickName + "�̃^�[��";//�N�̃^�[�����e�L�X�g�ŕ\������.
        //}
    }
    #endregion

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

    /// <summary>
    /// PUN���g���ĕϐ��𓯊�����.
    /// </summary>
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

    #region ������������֘A

    /// <summary>
    /// �ҋ@�l���𑝂₷�֐�.
    /// </summary>
    [PunRPC]
    private void AddReadyPeaple()
    {
        ReadyPeople++;
    }
    /// <summary>
    /// ���l���ɒB������v���C���[�𐶐�����֐�.
    /// </summary>
    [PunRPC]
    private void StartGame()
    {
        StandByCanvas.SetActive(false); //���������֘A�̃L�����o�X���\���ɂ���.
        NowGameState = GameState.InGame;//�Q�[�����[�h��InGame�ɂ���.
        CanvasUI.SetActive(true);       //�Q�[���ɕK�v�ȃL�����o�X��\������.
        var position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);//�v���C���[�𐶐�����.
    }

    /// <summary>
    /// ���������{�^�����������ۂɌĂяo���֐�.
    /// </summary>
    public void PushGameStart()
    {
        photonView.RPC(nameof(AddReadyPeaple), RpcTarget.All);//���������l���𑝂₷.
        StartButton.SetActive(false);                         //�{�^�������������\���ɂ���.
        if (ReadyPeople != MaxPlayers)                        //���������l�����w�肵�����ɖ����Ȃ��ꍇ
        {
            StandByGroup.SetActive(true);                     //�ҋ@�l����\������L�����o�X��\������.
            StandByText.text = "����" + (MaxPlayers - ReadyPeople )
                                + "�l�҂��Ă��܂��E�E�E";//�ő�l���ƌ��݂̐l���������đ҂��Ă���l����\��.
        }
        else
        {
            photonView.RPC(nameof(StartGame), RpcTarget.All);//���������l���𑝂₷.
        }
    }
    #endregion

    /// <summary>
    /// �������Ԃ����炷�֐�
    /// </summary>
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
        DoubtTime = 10;//�_�E�g�w�E���Ԃ�ݒ�
        DoubtFlg = true;
    }

    /// <summary>
    /// �_�E�g�w�E���Ԃ̌����֐�.
    /// </summary>
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
