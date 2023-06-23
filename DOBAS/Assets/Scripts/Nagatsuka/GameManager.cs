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

    const int ATTACK = 4;//�U���o�ڂ̐���.
    const int DOUBT = 5; //�_�E�g�ڂ̐���(5��6�̏ꍇ).
    //�v���C���[�̏ڍ׃p�l���̎Q�Ɨp.
    const int PLAYER_NAME = 1;
    const int PLAYER_HP = 2;
    const int PLAYER_ATTACK = 4;

    const int SAIKORO_RESULT = 1;//�T�C�R���̏o�ڂ𑼂̐l�ɒʒm����p�l���p.
    #endregion

    #region private�ϐ�
    private GameObject Dice;//�T�C�R���p�̕\���E��\�����J��Ԃ��p.
    Vector3 diceCameradefPos;//�T�C�R���J�����̏����l.
    Vector3 diceCameraPos;//�T�C�R����U�鏉���ʒu.
    private bool DiceFlg;//���������U�������̃t���O.
    private int Number;       //�T�C�R���̏o�ڂ����U���g�p�l���ɕ\������p�̕ϐ�.
    private bool diceBtnFlg;//��������{�^�������������̔���p.
    #region �����_���ɂ����������]������p�̕ϐ��錾.
    private int rotateX;
    private int rotateY;
    private int rotateZ;
    #endregion
    private int ThroughNum;//�M����(�X���[����)�l��.
    private bool[] UseID = new bool[4];//�v���C���[�Ɋ��蓖�Ă�ID�̎g�p��.
    private int ReadyPeople;           //���������l��.
    private bool timeflg;//�������Ԃ����炷���߂̃t���O.
    private bool doubtFlg;//�������낪5��6�̏ꍇ�A�t���O��؂芷����.

    private List<GameObject> PlayersNameGroup = new List<GameObject>();//Player�̏ڍ׏���\������I�u�W�F�N�g.



    #region ���܂���Name�z��
    private static readonly string[] OMAKASE_NAMES = new string[] { "���˂���", "���炠����", "������",
        "�������߂ł�","�I�X�X���ł�","�����Ɨv�`�F�b�N�I","�C����"};
    #endregion

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
    [SerializeField] Text StandByText;          //�ҋ@�l����\������e�L�X�g.
    [SerializeField] GameObject MainCamera;
    [SerializeField] AnimalsManager SelectAnimals;
    [SerializeField] GameObject ChangeButtons;  //�L�����N�^�[�ύX�p�{�^��.

    [SerializeField] Text WhoseTurnText;        //�N�̃^�[�����̃e�L�X�g.
    [SerializeField] Text HaveTimeText;         //�������ԃe�L�X�g.
    [SerializeField] GameObject StandByGroup;   //���������̃O���[�v.
    [SerializeField] GameObject CardButton;

    [Header("��������֘A")]
    [SerializeField] GameObject DicePrefab;//�T�C�R���̃v���t�@�u������.
    [SerializeField] GameObject ShakeDiceButton;//���������U��{�^��.
    [SerializeField] GameObject DiceCamera;
    [SerializeField] Text DiceNumText;
    [SerializeField] GameObject[] ResultPanel = new GameObject[5];//�o�ڂ̃p�l���p.
    [SerializeField] GameObject ReasoningPanel;
    [SerializeField] GameObject DiceShakeButton;//�T�C�R����U��{�^��.
    //public GameObject EnemyResult;//����̏o�ڂ�����.

    [Header("�Q�[���I���֘A")]
    [SerializeField] GameObject GameEndPanel;
    public int MyRank;

    [Header("�O���A�N�Z�X�p�ϐ�")]
    public int[] PlayersHP;//Player������HP�̎Q�Ƃ��s���p.
    public static int MaxPlayersNum;//���X�N���v�g����A�N�Z�X����p.
    public static int WhoseTurn;//�N�̃^�[�����i�v���C���[ID���Q�Ƃ��Ă��̕ϐ��Ɣ�ׂă^�[�����������j.
    public Text WaitText;   //���̐l�̍s���҂���\������e�L�X�g.
    [SerializeField] GameObject PlayersNameGroupPrefab;//�E��ɕ\�����閼�O�𐶐�����p.
    public List<GameObject> Players = new List<GameObject>(); // �v���C���[�Q�Ɨp
    public List<string> PlayersName = new List<string>();//Player�̖��O�����郊�X�g.
    public bool FinMessage = false;//���b�Z�[�W�̕\�����I��������}�X�̌��ʂ𔭓�����ׂ̃t���O.
    public bool PlayerFinTurn = false;//�v���C���[���^�[�����I�����Ă��邩�̔���.
    public int AnimalChildNum;//�I�񂾃L�����N�^�[��ۑ����邽�߂̐錾.
    public int DeclarationNum;//�錾�ԍ�.
    public bool DiceFinishFlg;//Player������T�C�R����U��I����Ă��邩�̎Q�Ɨp.
    public bool DeclarationFlg;//�錾�҂��t���O(Player����Q�Ƃ���).
    public bool FailureDoubt;//�_�E�g���s�t���O.
    public string[] PlayersRank;
    public int Rankcnt;//���ʂ����܂����l�����J�E���g.
    [Header("�_�E�g�֘A")]
    [SerializeField] GameObject DoubtPanel;

    [Header("���b�Z�[�W�֘A")]
    [SerializeField] GameObject MessageWindow;//���b�Z�[�W��\������p�l���i�q���Ƀ��b�Z�[�W�p�e�L�X�g�j.
    [SerializeField] GameObject MessageLogWindow;//���b�Z�[�W�̃��O��\������p�l��.
    [Header("�`���b�g�֘A")]

    [SerializeField] InputField InputChat;
    [SerializeField] Text ChatLog;
    [SerializeField] Text ChatLog2;

    [Header("�摜�f�[�^")]
    [SerializeField] Sprite[] HeartSprites = new Sprite[6];//HP�p�̉摜(0�ԖڂɎ��S�p�̂ǂ��������).
    [SerializeField] Sprite[] DiceSprites = new Sprite[4]; //�T�C�R���̏o�ڂ̉摜.
    #endregion

    #region Unity�C�x���g(Start�EUpdate)
    // Start is called before the first frame update
    void Start()
    {
        // �v���C���[���g�̖��O��"Player"�ɐݒ肷��
        PhotonNetwork.NickName = "Player";
        PhotonNetwork.ConnectUsingSettings();

        WhoseTurn = FIRST_TURN;
        PlayersHP = new int[MaxPlayers];//Player�̐l����HP�z���p��.
        for (int i = 0; i < MaxPlayers; i++)
        {
            PlayersHP[i] = FirstHP;//HP�̏����l����.
        }
        diceBtnFlg = false;
        doubtFlg = false;
        timeflg = false;
        DeclarationFlg = false;
        FailureDoubt = false;
        MaxPlayersNum = MaxPlayers;
        WaitText.text = "";
        Dice = Instantiate(DicePrefab, DiceCamera.transform.position, Quaternion.identity, this.transform);
        diceCameraPos = DiceCamera.transform.position;
        diceCameraPos.x += 3; //�T�C�R����U��ʒu���J�������班�����炷.
        diceCameraPos.y -= 3;
        Dice.SetActive(false);
        DiceCamera.SetActive(false);
        DiceFlg = false;      //�T�C�R����U���Ă��Ȃ���Ԃɂ���.
        DiceFinishFlg = false;//�T�C�R����U��I����Ă��Ȃ���Ԃɂ���.
        Rankcnt = MaxPlayers;
        PlayersRank = new string[MaxPlayers];
        diceCameradefPos = DiceCamera.transform.position;
        MyRank = 0;
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

    #region �Q�[����
    /// <summary>
    /// InGame�̍ۂ�Update�ŌĂяo��������֐�
    /// �����̃^�[���̏����E���Ԃ̐���������ōs��.
    /// </summary>
    private void InGameRoop()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == WhoseTurn)//�����̃^�[���Ȃ�
        {
            if (PlayersHP[WhoseTurn - 1] > 0)//0�ȏ�ł���΍s���\.
            {
                if (!diceBtnFlg)
                {
                    ShakeDiceButton.SetActive(true);//�T�C�R����U��{�^����\��.
                    CardButton.SetActive(true);
                }
                WhoseTurnText.color = Color.red;//�^�[���e�L�X�g�̐F��ԂɕύX.
                WhoseTurnText.text = "���Ȃ��̃^�[��";
                //WaitText.text = "";
            }
            else
            {
                WaitText.text = "���O�͂�������ł���";
                FinishTurn();
                Debug.Log("���O�͂�������ł���");
            }
        }
        else//�����̃^�[���łȂ��Ȃ�.
        {
            WhoseTurnText.color = Color.white;//�^�[���e�L�X�g�̐F�𔒂ɕύX.
            WhoseTurnText.text = PlayersName[WhoseTurn - 1] + "�̃^�[��";//�N�̃^�[�������e�L�X�g�ɕ\��.
            WaitText.text = PlayersName[WhoseTurn - 1] + "���s�����Ă��܂�";
        }

        if (Input.GetKeyDown(KeyCode.P))//�f�o�b�N�p�^�C�}�[�N��.
        {
            photonView.RPC(nameof(StartTimer), RpcTarget.All);
        }
        if (timeflg)
        {
            HaveTime -= Time.deltaTime;
        }
        HaveTimeText.text = HaveTime.ToString("0");//���Ԃ̏����_���Ȃ���.
    }

    void EndGame()
    {
        GameEndPanel.SetActive(true);
        GameEndPanel.transform.GetChild(1).GetComponent<Text>().text = "1�ʁF" + PlayersRank[0];
        GameEndPanel.transform.GetChild(2).GetComponent<Text>().text = "2�ʁF" + PlayersRank[1];
        GameEndPanel.transform.GetChild(3).GetComponent<Text>().text = "3�ʁF" + PlayersRank[2];
        Debug.Log("�Q�[���I��");
    }
    #endregion

    #region �^�[���ύX�֘A�֐�
    /// <summary>
    /// ���������U�������RPC���Ăяo���A�^�[����ύX����֐�.
    /// �I�������l��FinishTurn���Ăяo���A���̑S����ChangeTurn���Ăяo��.
    /// </summary>
    public void FinishTurn()
    {
        Debug.Log("FinishTurn()�N��");
        WaitText.text = "";
        photonView.RPC(nameof(ChangeTurn), RpcTarget.All);//WhoseTurn�𑝂₵�ă^�[����ς���.
    }

    /// <summary>
    /// �^�[�����Ǘ�����
    /// WhoseTurn��ύX���ă^�[����ύX����.
    /// </summary>
    [PunRPC]
    public void ChangeTurn()
    {
        int cnt = 0;
        for (int i = 0; i < MaxPlayers; i++)//�v���C���[�̐������[�v�𓮂���.
        {
            if (PlayersHP[i] == 0)//HP��0�̃v���C���[�𐔂���.
            {
                cnt++;
            }
        }
        if (cnt == MaxPlayers - 1)//�c��1�l�ɂȂ����ꍇ�Q�[���I��.
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (PlayersHP[i] > 0)
                {
                    PlayersRank[0] = PlayersName[i];
                }
            }
            NowGameState = GameState.EndGame;
        }
        if (WhoseTurn == MaxPlayers)
        {
            WhoseTurn = FIRST_TURN;//WhoseTurn���v���C���[�̍ő吔�𒴂�����ŏ��̐l�ɖ߂�.
        }
        else
        {
            WhoseTurn++;           //���̐l�̃^�[���ɂ���.
        }
        StartCoroutine(TurnMessageWindowCoroutine());
        photonView.RPC(nameof(StartTimer), RpcTarget.All);
    }
    #endregion

    #region HP�ύX�֘A
    /// <summary>
    /// �ύX�̂������v���C���[��l����\����HP�̓����֐����Ăяo��.
    /// </summary>
    public void ChangePlayersHP(int addHP, int subject)//subject�͑ΏۂƂ����Ӗ�.
    {
        if (subject == PhotonNetwork.LocalPlayer.ActorNumber)//�������g���Ώۂ̏ꍇ�̂�HP��ω�������֐����Ă�.
        {
            photonView.RPC(nameof(ChangeHP), RpcTarget.All, addHP, subject);
        }
    }
    /// <summary>
    /// �Ώۂƈ������w�肵�A�S����HP��Ԃ𓯊�����.
    /// </summary>
    [PunRPC]
    void ChangeHP(int addHP, int subject)
    {
        if (PlayersHP[subject - 1] == 5)
        {
            Debug.Log("HP���");
        }
        else
        {
            PlayersHP[subject - 1] += addHP;
        }
        PlayersNameGroup[subject - 1].transform.GetChild(PLAYER_HP).GetComponent<Image>().sprite = HeartSprites[PlayersHP[subject - 1]];
        if (PlayersHP[subject - 1] == 0)
        {
            PlayersRank[Rankcnt - 1] = PlayersName[subject - 1];
            if (PlayersHP[subject - 1] == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                MyRank = Rankcnt;
            }
            Rankcnt++;
        }
    }
    #endregion

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
        MainCamera.SetActive(true);
        CanvasUI.SetActive(true);       //�Q�[���ɕK�v�ȃL�����o�X��\������.
        //GameObject p = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);//�v���C���[�𐶐�����.
        GameObject p = PhotonNetwork.Instantiate("Animals", Vector3.zero, Quaternion.identity);//�v���C���[�𐶐�����.
        Players.Add(p);

        int i = 0;                        //�ڍ׏���Y���W��ύX���邽�߂Ƀ��[�J���ȕϐ�i��p��.
        foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
        {
            PlayersName.Add(player.NickName);//Player�̖��O�����X�g�ɓ����.
            //Player�̏ڍ׏���\��������̂𐶐�����
            var obj = Instantiate(PlayersNameGroupPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, CanvasUI.transform);
            obj.GetComponent<RectTransform>().localPosition = new Vector3(760f, 465f - 150f * i, 0f);        //�ʒu���E��ɐݒ�.
            obj.transform.GetChild(PLAYER_NAME).GetComponent<Text>().text = player.NickName;                //���O��\��.
            obj.transform.GetChild(PLAYER_HP).GetComponent<Image>().sprite = HeartSprites[PlayersHP[i]];//����HP��\��.
            PlayersNameGroup.Add(obj);//�ڍ׏�����ꂽ�I�u�W�F�N�g�����X�g�ɓ����.
            i++;                      //Y���W��ύX���邽�߂�i���v���X����.
        }
    }

    /// <summary>
    /// ���������{�^�����������ۂɌĂяo���֐�.
    /// </summary>
    public void PushGameStart()
    {
        photonView.RPC(nameof(AddReadyPeaple), RpcTarget.All);//���������l���𑝂₷.
        StartButton.SetActive(false);                         //�{�^�������������\���ɂ���.
        AnimalChildNum = SelectAnimals.ChildNum;
        PhotonNetwork.NickName = InputNickName.transform.GetChild(INPUT_NAME).GetComponent<Text>().text;// �v���C���[���g�̖��O����͂��ꂽ���O�ɐݒ肷��
        if (ReadyPeople != MaxPlayers)                        //���������l�����w�肵�����ɖ����Ȃ��ꍇ
        {
            StandByGroup.SetActive(true);                     //�ҋ@�l����\������L�����o�X��\������.
            HelloPlayerText.text = PhotonNetwork.NickName + "����悤�����I";
            StandByText.text = "����" + (MaxPlayers - ReadyPeople)
                                + "�l�҂��Ă��܂��E�E�E";//�ő�l���ƌ��݂̐l���������đ҂��Ă���l����\��.
        }
        else//�w��l���ɒB������Q�[�����n�߂�.
        {
            photonView.RPC(nameof(StartGame), RpcTarget.All);
        }
    }

    /// <summary>
    /// ���O�������_���Ɍ��߂�{�^�����������ۂɌĂяo���֐�.
    /// </summary>
    public void PushOmakaseNameButton()
    {
        int rnd = Random.Range(0, OMAKASE_NAMES.Length);
        InputNickName.text = OMAKASE_NAMES[rnd];
    }
    #endregion

    #region �`���b�g�@�\�֘A
    public void PushSendChatButton()
    {
        string chat = InputChat.text;
        int master = PhotonNetwork.LocalPlayer.ActorNumber;//���M�҂̔ԍ�(�S����ActorNumber���Ă΂Ȃ����߂ɑ��).
        InputChat.text = "";//���M���������.
        photonView.RPC(nameof(PushChat), RpcTarget.All, master, chat);
    }
    [PunRPC]
    void PushChat(int master, string chat)
    {
        ChatLog2.text = ChatLog.text;
        ChatLog.text = PlayersName[master - 1] + ":" + chat;
    }

    #endregion

    #region �M����{�^���E�_�E�g�{�^���֘A
    /// <summary>
    /// �M����{�^�����������Ƃ��ɌĂяo���֐�.
    /// </summary>
    public void PushBelieveButton()
    {
        photonView.RPC(nameof(AddThroughNum), RpcTarget.All);
    }
    /// <summary>
    /// �M����{�^�����������l���𑝂₷�֐�.
    /// �M����l�����v���C�l��(�T�C�R����U�����l������̂�-1)�ƈ�v������}�X��i�܂���.
    /// </summary>
    [PunRPC]
    void AddThroughNum()
    {
        Debug.Log("AddThroughNum()�N��");
        ThroughNum++;
        if (MaxPlayers - 1 == ThroughNum)
        {
            DeclarationFlg = true;

            ThroughNum = 0;
            //WaitText.text = "";
            DiceInit();
        }
    }

    /// <summary>
    /// �_�E�g�{�^�������������ɌĂяo���֐�
    /// </summary>
    public void PushDoubtButton()
    {
        int subject = PhotonNetwork.LocalPlayer.ActorNumber;
        photonView.RPC(nameof(DeclarationDoubt), RpcTarget.All, subject);
    }

    /// <summary>
    /// �_�E�g�錾�{�^�����������ۂɌĂяo���֐�
    /// �撅1�l���N���ł��A�N�������瑼�̐l�̃t���O��ON�ɂ���
    /// </summary>
    [PunRPC]
    void DeclarationDoubt(int subject)
    {
        ReasoningPanel.SetActive(false);//���v���C���[�̃p�l���������I�ɕ���.
        Debug.Log("DeclarationDoubt�N��");
        Debug.Log("�Ăяo����Player" + PlayersName[subject - 1]);
        DoubtPanel.SetActive(true);
        DoubtPanel.transform.GetChild(0).GetComponent<Text>().text = PlayersName[subject - 1] + "���񂪃_�E�g�錾���s���܂���";
        if (doubtFlg)//�R�����Ă����ꍇ�̏���.
        {
            FailureDoubt = true;
            DoubtPanel.transform.GetChild(1).GetComponent<Text>().text = PlayersName[WhoseTurn - 1] + "����͉R�����Ă��܂���";
            DoubtPanel.transform.GetChild(2).GetComponent<Text>().text = "�_�E�g���s����" + PlayersName[WhoseTurn - 1] + "�����1�_���[�W";
            if (WhoseTurn == PhotonNetwork.LocalPlayer.ActorNumber)//�������g���Ώۂ̏ꍇ�̂�HP��ω�������֐����Ă�.
            {
                photonView.RPC(nameof(ChangeHP), RpcTarget.All, -1, WhoseTurn);
            }
        }
        else//�R�����Ă��Ȃ������ꍇ�̏���.
        {
            DoubtPanel.transform.GetChild(1).GetComponent<Text>().text = PlayersName[WhoseTurn - 1] + "����͉R�����Ă��܂���ł���";
            DoubtPanel.transform.GetChild(2).GetComponent<Text>().text = "�_�E�g���s����" + PlayersName[subject - 1] + "�����1�_���[�W";
            if (subject == PhotonNetwork.LocalPlayer.ActorNumber)//�������g���Ώۂ̏ꍇ�̂�HP��ω�������֐����Ă�.
            {
                photonView.RPC(nameof(ChangeHP), RpcTarget.All, -1, subject);
            }
        }
        StartCoroutine(WaitDoubtPanelCoroutine());
    }
    private IEnumerator WaitDoubtPanelCoroutine()
    {
        // 2�b�ԑ҂�
        yield return new WaitForSeconds(4);
        Debug.Log("�R���[�`���Ăяo���I��");
        DoubtPanel.SetActive(false);
        DeclarationFlg = true;
        DiceInit();

        yield break;
    }
    #endregion

    #region �T�C�R���֘A�֐�
    /// <summary>
    /// �T�C�R����U��p�̊֐�
    /// �T�C�R���̃v���t�@�u��������΍쐬���A�L��Ε\���E��\�����J��Ԃ�.
    /// </summary>
    public void ShakeDice()
    {
        DiceShakeButton.SetActive(false);
        DiceCamera.SetActive(true);
        Dice.SetActive(true);
        Dice.transform.position = diceCameraPos;
        diceCameraPos = DiceCamera.transform.position;
        diceCameraPos.x += 3;
        diceCameraPos.y -= 3;
        rotateX = Random.Range(0, 360);
        rotateY = Random.Range(0, 360);
        rotateZ = Random.Range(0, 360);
        Dice.GetComponent<Rigidbody>().AddForce(-transform.right * 300);
        Dice.transform.Rotate(rotateX, rotateY, rotateZ);
        DiceFlg = true;//��������������.
    }

    /// <summary>
    /// �o�ڂ��e�L�X�g�ŕ\����A�������Ԃ��J���Ă���p�l����\������R���[�`���{��.
    /// </summary>
    private IEnumerator HiddenDiceCoroutine()
    {
        // 2�b�ԑ҂�
        yield return new WaitForSeconds(2);
        Debug.Log("�R���[�`���Ăяo���I��");
        ActivePanel();
        yield break;
    }

    /// <summary>
    /// �o�ڊm��AUI�ɐ��l�╶����\������
    /// </summary>
    public void ConfirmNumber(int num)
    {
        //�T�C�R���̏�ɃJ�����������Ă��鏈��.
        Vector3 position = Dice.transform.position;
        position.y += 5;

        DiceCamera.transform.position = position;

        if (num == ATTACK)            //�U���ڂ��o���ꍇAttack�ƕ\������.
        {
            DiceNumText.text = "Attack";
        }
        else if (num == 5 || num == 6)//�_�E�g�ڂ̏ꍇDoubt�ƕ\��.
        {
            DiceNumText.text = "Doubt";
            doubtFlg = true;
            num = DOUBT;
        }
        else                          //�����̐����̏ꍇ���̂܂܏o��.
        {
            DiceNumText.text = num.ToString();
        }
        Number = num;                 //�p�l���\������p�ɑ������.
        StartCoroutine(HiddenDiceCoroutine());
    }

    /// <summary>
    /// �T�C�R����U��{�^������������Ă΂��֐�.
    /// </summary>
    public void PushShakeDiceButton()
    {
        ShakeDiceButton.SetActive(false);
        CardButton.SetActive(false);
        diceBtnFlg = true;//�{�^������������Ԃɂ���.
        if (DiceFlg == false)//�T�C�R����U���Ă��Ȃ��Ȃ�.
        {
            ShakeDice();
        }
    }

    /// <summary>
    /// �o�ڂɂ���ĕ\������p�l����ς���.
    /// </summary>
    private void ActivePanel()
    {
        ResultPanel[Number - 1].SetActive(true);
    }

    #region �e�o�ڂ̃{�^��
    /// <summary>
    /// 1�̏o�ڃ{�^������������.
    /// </summary>
    public void PushOneButton()
    {
        DeclarationNum = 1;
        DeclarationResult();
    }
    /// <summary>
    /// 2�̏o�ڃ{�^������������.
    /// </summary>
    public void PushTwoButton()
    {
        DeclarationNum = 2;
        DeclarationResult();
    }
    /// <summary>
    /// 3�̏o�ڃ{�^������������.
    /// </summary>
    public void PushThreeButton()
    {
        DeclarationNum = 3;
        DeclarationResult();
    }
    /// <summary>
    /// �U���o�ڃ{�^������������.
    /// </summary>
    public void PushAttackButton()
    {
        DeclarationNum = ATTACK;
        DeclarationResult();
    }
    #endregion

    /// <summary>
    /// �錾���ꂽ�o�ڂ��v���C���[�ɕԂ��֐�
    /// �I����A�T�C�R���������ݒ�ɖ߂�.
    /// </summary>
    private void DeclarationResult()
    {
        DiceFinishFlg = true;
        for (int i = 0; i < 5; i++)//�S�Ẵ��U���g��ʂ��\���ɂ���.
        {
            ResultPanel[i].SetActive(false);
        }
        Debug.Log("�o�ځF" + DeclarationNum);
        DiceNumText.text = " ";
        DiceCamera.SetActive(false);
        Dice.SetActive(false);
        DiceFlg = false;
    }

    /// <summary>
    /// �T�C�R���̏������֐�
    /// �T�C�R����U��I��������ƂɌĂяo���A�ݒ��S�ď���������.
    /// </summary>
    private void DiceInit()
    {
        DiceFinishFlg = false;
        DiceNumText.text = " ";
        DeclarationNum = 0;
        ReasoningPanel.SetActive(false);
        Debug.Log("DiceInit�N��");
        DiceCamera.SetActive(false);
        Dice.SetActive(false);
        DiceCamera.transform.position = diceCameradefPos;
        DiceFlg = false;
    }

    /// <summary>
    /// �錾���ꂽ�o�ڂ��󂯎��֐�.
    /// </summary>
    public void ReceiveDeme(int deme)
    {
        if (DeclarationNum == 5 || DeclarationNum == 6)
        {
            Debug.Log("�_�E�g�ڂ��o�Ă���");
        }
        WaitText.text = "���v���C���[�̐錾��҂��Ă��܂�";
        photonView.RPC(nameof(ActiveEnemyResult), RpcTarget.Others, deme);//�T�C�R����U���Ă��Ȃ��l�Ƀp�l����\������.
    }

    /// <summary>
    /// �T�C�R���̏o�ڂ𑼂̐l�ɒʒm����p�l����\������֐�.
    /// </summary>
    [PunRPC]
    void ActiveEnemyResult(int deme)
    {
        ReasoningPanel.SetActive(true);
        ReasoningPanel.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = DiceSprites[deme - 1];
    }

    #endregion

    #region ���b�Z�[�W�\���֘A

    /// <summary>
    /// Player���烁�b�Z�[�W�̕�������󂯎���ĕ\������֐�.
    /// </summary>
    public void ShowMessage(string message, int num)
    {
        Debug.Log("Shomeesage�N��");
        FinMessage = false;
        photonView.RPC(nameof(ShowMessageAll), RpcTarget.All, message, num);
    }

    [PunRPC]
    private void ShowMessageAll(string message, int num)
    {
        MessageWindow.SetActive(true);
        MessageWindow.transform.GetChild(0).GetComponent<Text>().text = PlayersName[num - 1] + "����\n" + message;
        StartCoroutine(WaitMessageWindowCoroutine());
    }

    /// <summary>
    /// �^�[���J�n���Ƀ��b�Z�[�W�ʒm���o���R���[�`��.
    /// </summary>
    private IEnumerator TurnMessageWindowCoroutine()
    {
        MessageWindow.SetActive(true);
        MessageWindow.transform.GetChild(0).GetComponent<Text>().text = PlayersName[WhoseTurn - 1] + "����̃^�[���ł�";
        // 3�b�ԑ҂�
        yield return new WaitForSeconds(2);
        MessageWindow.SetActive(false);
        FinMessage = true;
        doubtFlg = diceBtnFlg = FailureDoubt = PlayerFinTurn = false;
        DeclarationFlg = false;    //�S���̐錾�҂���Ԃ�false�ɂ���.
        
        yield break;
    }
    /// <summary>
    /// 3�b�o�߂Ń��b�Z�[�W�E�B���h�E�����R���[�`��.
    /// </summary>
    private IEnumerator WaitMessageWindowCoroutine()
    {
        // 3�b�ԑ҂�
        yield return new WaitForSeconds(3);
        MessageWindow.SetActive(false);
        FinMessage = true;
        FinishTurn();
        yield break;
    }

    #endregion

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
    #endregion

    #region �v���C���[��ID��^����֐�
    public int Give_ID_Player()
    {
        int ID = 1;
        for (int i = 0; i < UseID.Length; i++)
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

    #endregion

    #region Photon�֘A(override�E�ϐ����M)

    #region Photon�֘A��override�֐�
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // <summary>
    // �����[�g�v���C���[�����������ۂɃR�[�������
    // </summary>
    public void OnPhotonPlayerConnected()
    {
        Debug.Log("����");

    }
    public override void OnJoinedRoom()
    {
        NowGameState = GameState.SetGame;
    }
    #endregion

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
            stream.SendNext(doubtFlg);
        }
        else
        {
            HaveTime = (float)stream.ReceiveNext(); // time����M����
            WhoseTurn = (int)stream.ReceiveNext();
            PlayersHP = (int[])stream.ReceiveNext();
            doubtFlg = (bool)stream.ReceiveNext();
        }
    }
    #endregion
}