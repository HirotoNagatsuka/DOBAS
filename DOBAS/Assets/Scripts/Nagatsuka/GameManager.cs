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
    const int INPUT_NAME = 2;//���O���͂̎q�I�u�W�F�N�g���Q�Ɨp.
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
    [Header("�Q�[�����")]
    public GameState NowGameState;//���݂̃Q�[�����.

    [Header("�ʐM�v���C�l��")]
    public int MaxPlayers;//�v���C���[�̍ő�l��.

    [Header("�v���C���[�̏���HP")]
    public int FirstHP;

    [Header("�v���C���[�̎�������")]
    public float HaveTime;//�e�v���C���[�̎�������.

    [Header("public�ESerializeField�錾")]
    [Header("�Q�[���J�n�O�i���������j�֘A")]
    [SerializeField] InputField InputNickName;  //���O����͂���p.
    [SerializeField] GameObject CanvasUI;       //�Q�[���J�n���ɂ܂Ƃ߂ăL�����o�X���\���ɂ���.
    [SerializeField] GameObject StartButton;    //���������{�^��.
    [SerializeField] GameObject StandByCanvas;  //���������L�����o�X.
    [SerializeField] Text HelloPlayerText;      //�ҋ@���Ƀv���C���[��\������{�^��.
    [SerializeField] Text StandByText;          //�ҋ@�l����\������{�^��.


    [SerializeField] Text WhoseTurnText;        //�N�̃^�[�����̃e�L�X�g.
    [SerializeField] Text HaveTimeText;         //�������ԃe�L�X�g.
    [SerializeField] GameObject StandByGroup;   //���������̃O���[�v.
    [SerializeField] GameObject ShakeDiceButton;//���������U��{�^��.

    public List<string> PlayersName = new List<string>();
    public int[] PlayersHP;
    public static int MaxPlayersNum;//���X�N���v�g����A�N�Z�X����p.
    public static int WhoseTurn;//�N�̃^�[�����i�v���C���[ID���Q�Ƃ��Ă��̕ϐ��Ɣ�ׂă^�[�����������j.

    [SerializeField] GameObject PlayersNameGroupPrefab;//�E��ɕ\�����閼�O�𐶐�����p.
    [SerializeField] Sprite[] Hearts = new Sprite[5];
    #endregion

    private bool[] UseID = new bool[4];//�v���C���[�Ɋ��蓖�Ă�ID�̎g�p��.
    private int ReadyPeople;//���������l��.
    string TurnName;//�N�̃^�[�����̖��O�p.

    float DoubtTime;//�_�E�g�錾�̎�������.
    bool DoubtFlg;
    bool timeflg;
    public List<GameObject> PlayersNameGroup = new List<GameObject>();

    #region Unity�C�x���g(Start�EUpdate)
    // Start is called before the first frame update
    void Start()
    {
        WhoseTurn = FIRST_TURN;
        DoubtFlg = false;
        timeflg = false;
        PlayersHP = new int[MaxPlayers];//Player�̐l����HP�z���p��.
        for (int i = 0; i < MaxPlayers; i++)
        {
            PlayersHP[i] = FirstHP;//HP�̏����l����.
        }
        MaxPlayersNum = MaxPlayers;
    }

    // Update is called once per frame
    void Update()
    {
        switch (NowGameState)//�Q�[�����[�h�ɂ���ď����𕪊򂷂�.
        {
            case GameState.InitGame:
                break;
            case GameState.SetGame:
                StartButton.SetActive(true);
                StandByText.text = "����" + (MaxPlayers - ReadyPeople)
                                    + "�l�҂��Ă��܂��E�E�E";//�ő�l���ƌ��݂̐l���������đ҂��Ă���l����\��.
                break;
            case GameState.InGame:
                InGameRoop();
                break;
            case GameState.EndGame:
                    EndGame();
                break;
            default:
                Debug.Log("�G���[:�\�����ʃQ�[�����[�h");
                break;
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
    }
    #endregion

    #region �Q�[����
    /// <summary>
    /// InGame�̍ۂ�Update�ŌĂяo��������֐�
    /// �����̃^�[���̏����E���Ԃ̐���������ōs��.
    /// </summary>
    private void InGameRoop()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == WhoseTurn)
        {
            ShakeDiceButton.SetActive(true);
            WhoseTurnText.color = Color.red;
            WhoseTurnText.text = "���Ȃ��̃^�[��";
        }
        else
        {
            ShakeDiceButton.SetActive(false);
            WhoseTurnText.color = Color.white;
            WhoseTurnText.text = PlayersName[WhoseTurn-1]+ "�̃^�[��";
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            photonView.RPC(nameof(StartTimer), RpcTarget.All);
        }

        if (DoubtFlg) ChangeDoubtTime();
        //else ChangeHaveTime();
        else if (timeflg) HaveTime -= Time.deltaTime;
        HaveTimeText.text = HaveTime.ToString("0");
    }

    void EndGame()
    {

    }
    #endregion


    public void ChangePlayersHP(int addHP,int subject)//subject�͑ΏۂƂ����Ӗ�.
    {
        if(subject== PhotonNetwork.LocalPlayer.ActorNumber)//�������g���Ώۂ̏ꍇ�̂�HP��ω�������֐����Ă�.
        {
            photonView.RPC(nameof(ChangeHP), RpcTarget.All, addHP, subject);
        }
    }
    /// <summary>
    /// �Ώۂƈ������w�肵�A�S����HP��Ԃ𓯊�����.
    /// </summary>
    [PunRPC]
    void ChangeHP(int addHP,int subject)
    {
        PlayersHP[subject - 1] += addHP;
        PlayersNameGroup[subject - 1].transform.GetChild(2).GetComponent<Image>().sprite = Hearts[PlayersHP[subject - 1]];
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
            stream.SendNext(PlayersHP);
        }
        else
        {
            HaveTime = (float)stream.ReceiveNext(); // time����M����
            WhoseTurn = (int)stream.ReceiveNext();
            PlayersHP = (int[])stream.ReceiveNext();
        }
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
        NowGameState = GameState.InGame;//�Q�[����Ԃ�InGame�ɂ���.
        CanvasUI.SetActive(true);       //�Q�[���ɕK�v�ȃL�����o�X��\������.
        var position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);//�v���C���[�𐶐�����.
        int i=0;                                                             // ���[�v���g�p���āA�S�Ẵv���C���[�ɑ΂��Ė��O��\�������
        foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
        {
            PlayersName.Add(player.NickName);
            var obj = Instantiate(PlayersNameGroupPrefab, new Vector3(0f,0f,0f), Quaternion.identity,CanvasUI.transform);
            obj.GetComponent<RectTransform>().localPosition = new Vector3(760f, 465f -150f * i, 0f);
            obj.transform.GetChild(1).GetComponent<Text>().text = player.NickName;
            obj.transform.GetChild(2).GetComponent<Image>().sprite = Hearts[PlayersHP[i] - 1];
            PlayersNameGroup.Add(obj);
            i++;
        }
    }

    /// <summary>
    /// ���������{�^�����������ۂɌĂяo���֐�.
    /// </summary>
    public void PushGameStart()
    {
        photonView.RPC(nameof(AddReadyPeaple), RpcTarget.All);//���������l���𑝂₷.
        StartButton.SetActive(false);                         //�{�^�������������\���ɂ���.

        PhotonNetwork.NickName = InputNickName.transform.GetChild(INPUT_NAME).GetComponent<Text>().text;// �v���C���[���g�̖��O����͂��ꂽ���O�ɐݒ肷��
        //Debug.Log(InputNickName.transform.GetChild(2).GetComponent<Text>().text);

        if (ReadyPeople != MaxPlayers)                        //���������l�����w�肵�����ɖ����Ȃ��ꍇ
        {
            StandByGroup.SetActive(true);                     //�ҋ@�l����\������L�����o�X��\������.
            HelloPlayerText.text = PhotonNetwork.NickName + "����悤�����I";
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
    /// �v���C���[��ID��^����֐�
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
    #region �������Ԍ����֘A

    [PunRPC]
    private void StartTimer()
    {
        timeflg = true;
    }

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
    #endregion

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
