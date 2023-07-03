using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

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
    //private int ReadyPeople;           //���������l��.
    private bool timeflg;//�������Ԃ����炷���߂̃t���O.
    private bool doubtFlg;//�������낪5��6�̏ꍇ�A�t���O��؂芷����.

    private List<GameObject> PlayersNameGroup = new List<GameObject>();//Player�̏ڍ׏���\������I�u�W�F�N�g.
    private GameObject PlayerInfo;


    #region ���܂���Name�z��
    private static readonly string[] OMAKASE_NAMES = new string[] { "���˂���", "���炠����", "������","����","�˂���","�����",
        "�����Ȃ���","������","�������߂ł�","�I�X�X���ł�","�����Ɨv�`�F�b�N�I","�C����"};
    #endregion

    #endregion

    int readyccnt = 0;
    int NowTutorial = 0;

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
    [SerializeField] GameObject TutorialCanvas;
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

    [Header("�Q�[���I���֘A")]
    [SerializeField] GameObject GameEndPanel;
    //public int[] Ranks;

    [Header("�O���A�N�Z�X�p�ϐ�")]
    //public int[] PlayersHP;//Player������HP�̎Q�Ƃ��s���p.
    public static int MaxPlayersNum;//���X�N���v�g����A�N�Z�X����p.
    //public static int WhoseTurn;//�N�̃^�[�����i�v���C���[ID���Q�Ƃ��Ă��̕ϐ��Ɣ�ׂă^�[�����������j.
    public Text WaitText;   //���̐l�̍s���҂���\������e�L�X�g.
    [SerializeField] GameObject PlayersNameGroupPrefab;//�E��ɕ\�����閼�O�𐶐�����p.
    public List<GameObject> Players = new List<GameObject>(); // �v���C���[�Q�Ɨp
    public List<string> PlayersName = new List<string>();//Player�̖��O�����郊�X�g.
    public bool FinMessage = false;//���b�Z�[�W�̕\�����I��������}�X�̌��ʂ𔭓�����ׂ̃t���O.
    //public int[] AnimalChildNums;
    public int AnimalChildNum;//�I�񂾃L�����N�^�[��ۑ����邽�߂̐錾.
    public int DeclarationNum;//�錾�ԍ�.
    public bool DiceFinishFlg;//Player������T�C�R����U��I����Ă��邩�̎Q�Ɨp.
    public bool DeclarationFlg;//�錾�҂��t���O(Player����Q�Ƃ���).

    public string[] PlayersRank = new string[4];
    public int[] PlayersRankNum;
    public int Rankcnt;//���ʂ����܂����l�����J�E���g.

    [SerializeField] GameObject UseBt;    // ����D�l(0622)

    [Header("�_�E�g�֘A")]
    [SerializeField] GameObject DoubtPanel;
    public bool FailureDoubt;//�_�E�g���s�t���O.

    [Header("���b�Z�[�W�֘A")]
    [SerializeField] GameObject MessageWindow;//���b�Z�[�W��\������p�l���i�q���Ƀ��b�Z�[�W�p�e�L�X�g�j.
    [SerializeField] GameObject MessageLogWindow;//���b�Z�[�W�̃��O��\������p�l��.
    [Header("�`���b�g�֘A")]
    [SerializeField] InputField InputChat;
    [SerializeField] Text ChatLog;
    [SerializeField] Text ChatLog2;

    [Header("�摜�f�[�^")]
    [SerializeField] Sprite[] TutorialSprites = new Sprite[6];
    [SerializeField] Sprite[] HeartSprites = new Sprite[6];//HP�p�̉摜(0�ԖڂɎ��S�p�̂ǂ��������).
    [SerializeField] Sprite[] DiceSprites = new Sprite[4]; //�T�C�R���̏o�ڂ̉摜.
    #endregion

    ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
    public Text TestWhoseTurnText;
    bool startflg = false;

    #region Unity�C�x���g(Start�EUpdate)
    // Start is called before the first frame update
    void Start()
    {
        // �v���C���[���g�̖��O��"Player"�ɐݒ肷��
        PhotonNetwork.NickName = "Player";
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.SetPlayerHP(FirstHP);
        Debug.Log(PhotonNetwork.LocalPlayer.GetPlayerHP());
        //Ranks = new int[MaxPlayers];//Player�̐l����HP�z���p��.

        diceBtnFlg = false;
        doubtFlg = false;
        timeflg = false;
        DeclarationFlg = false;
        FailureDoubt = false;
        MaxPlayersNum = MaxPlayers;
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
                if (PhotonNetwork.LocalPlayer.GetReadyNum())
                {
                    CountReadyNum();
                }
                break;
            case GameState.InGame:
                InGameRoop();
                TestWhoseTurnText.text = (string)PhotonNetwork.CurrentRoom.CustomProperties["Turn"].ToString() ;
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
        int turn = (int)PhotonNetwork.CurrentRoom.CustomProperties["Turn"];
        if (PhotonNetwork.LocalPlayer.ActorNumber == turn)//�����̃^�[���Ȃ�
        {
            WaitText.text = "";
            if (PhotonNetwork.LocalPlayer.GetPlayerHP() > 0)//0�ȏ�ł���΍s���\.
            {
                if (!diceBtnFlg)
                {
                    ShakeDiceButton.SetActive(true);//�T�C�R����U��{�^����\��.
                    CardButton.SetActive(true);
                }
                WhoseTurnText.color = Color.red;//�^�[���e�L�X�g�̐F��ԂɕύX.
                WhoseTurnText.text = "���Ȃ��̃^�[��";
            }
            else
            {
                FinishTurn();
            }
        }
        else//�����̃^�[���łȂ��Ȃ�.
        {
            WhoseTurnText.color = Color.white;//�^�[���e�L�X�g�̐F�𔒂ɕύX.
            WhoseTurnText.text = PlayersName[turn - 1] + "�̃^�[��";//�N�̃^�[�������e�L�X�g�ɕ\��.
            WaitText.text = PlayersName[turn - 1] + "���s�����Ă��܂�";
            ShakeDiceButton.SetActive(false);//�T�C�R����U��{�^����\��.
            CardButton.SetActive(false);
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
        CanvasUI.SetActive(false);
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
        //UseBt.SetActive(false);        // ����D�l(0622)
        photonView.RPC(nameof(ChangeTurn), RpcTarget.All);//WhoseTurn�𑝂₵�ă^�[����ς���.
    }

    /// <summary>
    /// �^�[�����Ǘ�����
    /// WhoseTurn��ύX���ă^�[����ύX����.
    /// </summary>
    [PunRPC]
    private void ChangeTurn()
    {
        //Debug.Log("ChangeTurn�N��");
        int cnt = 0;
        for (int i = 0; i < MaxPlayers; i++)//�v���C���[�̐������[�v�𓮂���.
        {
            PlayersNameGroup[i].transform.GetChild(5).gameObject.SetActive(false);
            if (PhotonNetwork.LocalPlayer.GetPlayerHP() == 0)//HP��0�̃v���C���[�𐔂���.
            {
                cnt++;
            }
        }
        if (cnt == MaxPlayers - 1)//�c��1�l�ɂȂ����ꍇ�Q�[���I��.
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (PhotonNetwork.LocalPlayer.GetPlayerHP() > 0)
                {
                    PlayersRank[0] = PlayersName[i];
                }
            }
            NowGameState = GameState.EndGame;
        }
        else
        {
            int turn = (int)PhotonNetwork.CurrentRoom.CustomProperties["Turn"];
            if (turn == MaxPlayers)
            {
                //WhoseTurn = FIRST_TURN;//WhoseTurn���v���C���[�̍ő吔�𒴂�����ŏ��̐l�ɖ߂�.
                customProperties["Turn"] = FIRST_TURN;
            }
            else
            {
                turn++;
                customProperties["Turn"] = turn;
                //WhoseTurn++;           //���̐l�̃^�[���ɂ���.
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
            customProperties.Clear();
            StartCoroutine(TurnMessageWindowCoroutine());
            photonView.RPC(nameof(StartTimer), RpcTarget.All);
        }
    }
    #endregion

    #region HP�ύX�֘A
    /// <summary>
    /// �ύX�̂������v���C���[��l����\����HP�̓����֐����Ăяo��.
    /// </summary>
    public void ChangePlayersHP(int addHP, int subject)//subject�͑ΏۂƂ����Ӗ�.
    {
        //Debug.Log("ChangeHP�N���F�ΏہF" + subject);
        photonView.RPC(nameof(ChangeHP), RpcTarget.All, addHP, subject);
        //if (subject == PhotonNetwork.LocalPlayer.ActorNumber)//�������g���Ώۂ̏ꍇ�̂�HP��ω�������֐����Ă�.
        //{
        //    photonView.RPC(nameof(ChangeHP), RpcTarget.All, addHP, subject);
        //}
    }
    public void EnemyAttack(int attackNum, int subject)
    {
        photonView.RPC(nameof(ChangeHP), RpcTarget.All, attackNum, subject);
    }
    /// <summary>
    /// �Ώۂƈ������w�肵�A�S����HP��Ԃ𓯊�����.
    /// </summary>
    [PunRPC]
    void ChangeHP(int addHP, int subject)
    {
        //Debug.Log("addHP�̒l" + addHP);
        if (PhotonNetwork.LocalPlayer.ActorNumber == subject)
        {
            if (PhotonNetwork.LocalPlayer.GetPlayerHP() == 5)
            {
                Debug.Log("HP���");
                return;
            }
            else if (PhotonNetwork.LocalPlayer.GetPlayerHP() == 0)
            {
                return;
            }
            else
            {
                int hp = 0;
                hp = PhotonNetwork.LocalPlayer.GetPlayerHP() + addHP;
                Debug.Log("HP�ω���" + hp);
                PhotonNetwork.LocalPlayer.SetPlayerHP(hp);
            }
        }
        foreach (var player in PhotonNetwork.PlayerList)//�v���C���[��HP���擾.
        {
            PlayerInfo.transform.GetChild(PLAYER_HP).GetComponent<Image>().sprite = HeartSprites[player.GetPlayerHP() - 1];
        }
        StartCoroutine("CheckHP");
    }

    /// <summary>
    /// �ʐM�̒x������R���[�`����HP0������m�F
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckHP()
    {
        yield return new WaitForSeconds(1);
        if (PhotonNetwork.LocalPlayer.GetPlayerHP() == 0)
        {
            //PlayersRank[Rankcnt - 1] = PlayersName[subject - 1];
            if (PhotonNetwork.LocalPlayer.GetPlayerHP() == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                Debug.Log("MyRank���");
                PhotonNetwork.LocalPlayer.SetMyRank(Rankcnt);
            }
            Rankcnt--;
        }
        Debug.Log("check�I��");
        yield break;
    }

    #endregion

    #region ������������֘A

    /// <summary>
    /// ���l���ɒB������v���C���[�𐶐�����֐�.
    /// </summary>
    [PunRPC]
    private void StartGame()
    {
        NowGameState = GameState.InGame;//�Q�[����Ԃ�InGame�ɂ���.
        StandByCanvas.SetActive(false); //���������֘A�̃L�����o�X���\���ɂ���.
        MainCamera.SetActive(true);
        CanvasUI.SetActive(true);       //�Q�[���ɕK�v�ȃL�����o�X��\������.
        foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
        {
            PlayersName.Add(player.NickName);//Player�̖��O�����X�g�ɓ����.
        }
        StartCoroutine(TurnMessageWindowCoroutine());
    }

    /// <summary>
    /// ���������{�^�����������ۂɌĂяo���֐�.
    /// </summary>
    public void PushGameStart()
    {
        Debug.Log("player.GetReady()����" + PhotonNetwork.LocalPlayer.GetReadyNum());
        StartButton.SetActive(false);                         //�{�^�������������\���ɂ���.
        AnimalChildNum = SelectAnimals.ChildNum;
        PhotonNetwork.NickName = InputNickName.transform.GetChild(INPUT_NAME).GetComponent<Text>().text;// �v���C���[���g�̖��O����͂��ꂽ���O�ɐݒ肷��
        PhotonNetwork.LocalPlayer.SetReadyNum(true);
        StartCoroutine(WaitStart());
    }


    private void CountReadyNum()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.GetReadyNum())
            {
                readyccnt++;
            }
        }
        if (readyccnt != MaxPlayers)//���������l�����w�肵�����ɖ����Ȃ��ꍇ
        {
            StandByGroup.SetActive(true);                     //�ҋ@�l����\������L�����o�X��\������.
            HelloPlayerText.text = PhotonNetwork.NickName + "����悤�����I";
            StandByText.text = "����" + (MaxPlayers - readyccnt)
                                + "�l�҂��Ă��܂��E�E�E";//�ő�l���ƌ��݂̐l���������đ҂��Ă���l����\��.
            readyccnt = 0;
        }
        else//�w��l���ɒB������Q�[�����n�߂�.
        {
            Debug.Log("�Q�[�����n�߂܂�");
            startflg = true;
        }
    }

    /// <summary>
    /// ���̃v���C���[�̊J�n��҂R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitStart()
    {
        yield return new WaitUntil(() => startflg == true); // �ҋ@����
        CreateCharacter(AnimalChildNum);
        photonView.RPC(nameof(StartGame), RpcTarget.All);
        for (int i = 0; i < MaxPlayers; i++)
        {
            PlayerInfo = Instantiate(PlayersNameGroupPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, CanvasUI.transform);
            PlayerInfo.transform.GetChild(PLAYER_NAME).GetComponent<Text>().text = PlayersName[i];                //���O��\��.
            PlayerInfo.transform.GetChild(PLAYER_HP).GetComponent<Image>().sprite = HeartSprites[PhotonNetwork.LocalPlayer.GetPlayerHP()];//����HP��\��.
            PlayerInfo.GetComponent<RectTransform>().localPosition = new Vector3(760f, 465f - 150f * i, 0f);
            PlayersNameGroup.Add(PlayerInfo);
        }
        yield break;
    }

    /// <summary>
    /// ���O�������_���Ɍ��߂�{�^�����������ۂɌĂяo���֐�.
    /// </summary>
    public void PushOmakaseNameButton()
    {
        int rnd = Random.Range(0, OMAKASE_NAMES.Length);
        InputNickName.text = OMAKASE_NAMES[rnd];
    }

    /// <summary>
    /// �I�������A�o�^�[�𐶐�����.
    /// </summary>
    void CreateCharacter(int num)
    {
        GameObject p = null;
        switch (num)
        {
            case 0:
                p = PhotonNetwork.Instantiate("Colobus", Vector3.zero, Quaternion.identity);//�v���C���[�𐶐�����.
                break;
            case 1:
                p = PhotonNetwork.Instantiate("Gecko", Vector3.zero, Quaternion.identity);//�v���C���[�𐶐�����.
                break;
            case 2:
                p = PhotonNetwork.Instantiate("Herring", Vector3.zero, Quaternion.identity);//�v���C���[�𐶐�����.
                break;
            case 3:
                p = PhotonNetwork.Instantiate("Muskrat", Vector3.zero, Quaternion.identity);//�v���C���[�𐶐�����.
                break;
            case 4:
                p = PhotonNetwork.Instantiate("Pudu", Vector3.zero, Quaternion.identity);//�v���C���[�𐶐�����.
                break;
            case 5:
                p = PhotonNetwork.Instantiate("Sparrow", Vector3.zero, Quaternion.identity);//�v���C���[�𐶐�����.
                break;
            case 6:
                p = PhotonNetwork.Instantiate("Squid", Vector3.zero, Quaternion.identity);//�v���C���[�𐶐�����.
                break;
            case 7:
                p = PhotonNetwork.Instantiate("Taipan", Vector3.zero, Quaternion.identity);//�v���C���[�𐶐�����.
                break;
            case 8:
                p = PhotonNetwork.Instantiate("Colobus", Vector3.zero, Quaternion.identity);//�v���C���[�𐶐�����.
                break;
        }
        Players.Add(p);
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
        int subject = PhotonNetwork.LocalPlayer.ActorNumber;
        photonView.RPC(nameof(AddThroughNum), RpcTarget.All, subject);
        ReasoningPanel.SetActive(false);
        WaitText.text = "���̃v���C���[�̐錾���܂��Ă��܂�";
    }
    /// <summary>
    /// �M����{�^�����������l���𑝂₷�֐�.
    /// �M����l�����v���C�l��(�T�C�R����U�����l������̂�-1)�ƈ�v������}�X��i�܂���.
    /// </summary>
    [PunRPC]
    void AddThroughNum(int subject)
    {
        Debug.Log("AddThroughNum()�N��");
        PlayersNameGroup[subject - 1].transform.GetChild(5).gameObject.SetActive(true);
        ThroughNum++;
        int cnt = 0;
        foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
        {
            if (player.GetPlayerHP() > 0)
            {
                cnt++;
            }
        }
        Debug.Log("cnt��" + cnt);
        if (cnt - 1 == ThroughNum)
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
        int turn = (int)PhotonNetwork.CurrentRoom.CustomProperties["Turn"];
        ReasoningPanel.SetActive(false);//���v���C���[�̃p�l���������I�ɕ���.
        Debug.Log("DeclarationDoubt�N��");
        Debug.Log("�Ăяo����Player" + PlayersName[subject - 1]);
        DoubtPanel.SetActive(true);
        DoubtPanel.transform.GetChild(0).GetComponent<Text>().text = PlayersName[subject - 1] + "���񂪃_�E�g�錾���s���܂���";
        if (doubtFlg)//�R�����Ă����ꍇ�̏���.
        {
            FailureDoubt = true;
            DoubtPanel.transform.GetChild(1).GetComponent<Text>().text = PlayersName[turn - 1] + "����͉R�����Ă��܂���";
            DoubtPanel.transform.GetChild(2).GetComponent<Text>().text = "�_�E�g���s����" + PlayersName[turn - 1] + "�����1�_���[�W";
            if (turn == PhotonNetwork.LocalPlayer.ActorNumber)//�������g���Ώۂ̏ꍇ�̂�HP��ω�������֐����Ă�.
            {
                photonView.RPC(nameof(ChangeHP), RpcTarget.All, -1, turn);
            }
        }
        else//�R�����Ă��Ȃ������ꍇ�̏���.
        {
            DoubtPanel.transform.GetChild(1).GetComponent<Text>().text = PlayersName[turn - 1] + "����͉R�����Ă��܂���ł���";
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
        DiceCamera.SetActive(true);
        Dice.SetActive(true);
        Dice.transform.position = diceCameraPos;
        diceCameraPos = DiceCamera.transform.position;
        diceCameraPos.x += 3;
        diceCameraPos.y -= 3;
        rotateX = rotateY = rotateZ = Random.Range(0, 360);//��]�������_���Ɍ���.
        Dice.GetComponent<Rigidbody>().AddForce(-transform.right * 300);
        Dice.transform.Rotate(rotateX, rotateY, rotateZ);
        DiceFlg = true;//���������U����.
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
        if (DeclarationNum == DOUBT)
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
        int turn = (int)PhotonNetwork.CurrentRoom.CustomProperties["Turn"];
        MessageWindow.SetActive(true);
        MessageWindow.transform.GetChild(0).GetComponent<Text>().text = PlayersName[turn - 1] + "����̃^�[���ł�";
        // 2�b�ԑ҂�
        yield return new WaitForSeconds(2);
        MessageWindow.SetActive(false);
        FinMessage = true;
        doubtFlg = diceBtnFlg = FailureDoubt = false;
        DeclarationFlg = false;    //�S���̐錾�҂���Ԃ�false�ɂ���.

        yield break;
    }
    /// <summary>
    /// 3�b�o�߂Ń��b�Z�[�W�E�B���h�E�����R���[�`��.
    /// </summary>
    private IEnumerator WaitMessageWindowCoroutine()
    {
        // 3�b�ԑ҂�
        yield return new WaitForSeconds(2);
        MessageWindow.SetActive(false);
        FinMessage = true;
        yield break;
    }

    #endregion

    #region �������Ԍ����֘A

    [PunRPC]
    private void StartTimer()
    {
        //timeflg = true;
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

    #region�@�V�ѕ������֘A
    public void PushWakabaTutorial()
    {
        TutorialCanvas.SetActive(true);
    }
    public void PushSkipTutorial()
    {
        TutorialCanvas.SetActive(false);
    }
    public void PushNextTutorial()
    {
        if (NowTutorial < 4)
        {
            NowTutorial++;
        }
        TutorialCanvas.transform.GetChild(1).GetComponent<Image>().sprite = TutorialSprites[NowTutorial];
    }
    public void PushFrontTutorial()
    {
        if (NowTutorial > 0)
        {
            NowTutorial--;
        }
        TutorialCanvas.transform.GetChild(1).GetComponent<Image>().sprite = TutorialSprites[NowTutorial];
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
        PhotonNetwork.JoinOrCreateRoom("RoomTNC", new RoomOptions(), TypedLobby.Default);
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

        // �J�X�^���v���p�e�B�̐ݒ�i���l�j
        int turn = FIRST_TURN;
       
        customProperties["Turn"] = turn;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        customProperties.Clear();
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
            //stream.SendNext(WhoseTurn);
            //stream.SendNext(PlayersHP);
            stream.SendNext(doubtFlg);
            //stream.SendNext(Ranks);
            stream.SendNext(AnimalChildNum);
        }
        else
        {
            HaveTime = (float)stream.ReceiveNext(); // time����M����
            //WhoseTurn = (int)stream.ReceiveNext();
            //PlayersHP = (int[])stream.ReceiveNext();
            doubtFlg = (bool)stream.ReceiveNext();
            //Ranks = (int[])stream.ReceiveNext();
            AnimalChildNum = (int)stream.ReceiveNext();
        }
    }
    #endregion
}

/// <summary>
/// �J�X�^���v���p�e�B��ݒ肷��ÓI�N���X.
/// </summary>
public static class PhotonCustumPropertie
{
    private const string GameStatusKey = "Gs";
    private const string InitStatusKey = "Is";
    private const string ReadyNum = "Rn";
    private const string PlayerHP = "P_HP";
    private const string MyRank = "MyRank";

    private static readonly ExitGames.Client.Photon.Hashtable propsToSet = new ExitGames.Client.Photon.Hashtable();

    /// <summary>
    /// ������Photon�̃v���C���[��n�����Ƃ�
    /// �߂�l��GameStatus���Ԃ��Ă���Bint�^�ŕԂ�̂ŁA�L���X�g����
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static int GetGameStatus(this Player player)
    {
        return (player.CustomProperties[GameStatusKey] is int status) ? status : 0;
    }

    /// <summary>
    /// ������Photon�̃v���C���[��GameStatus��n�����Ƃ�
    /// ���v���C���[�ɑ��M����
    /// </summary>
    /// <param name="player"></param>
    public static void SetGameStatus(this Player player, int status)
    {
        propsToSet[GameStatusKey] = status;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    /// <summary>
    /// ������Photon�̃v���C���[��n�����Ƃ�
    /// �߂�l�ł��̃v���C���[�̏�������񂪕Ԃ�
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetInitStatus(this Player player)
    {
        return (player.CustomProperties[InitStatusKey] is bool status) ? status : false;
    }

    /// <summary>
    /// ������Photon�̃v���C���[�Ə�������Ԃ�n�����Ƃ�
    /// ���v���C���[�ɑ��M����
    /// </summary>
    /// <param name="player"></param>
    public static void SetInitStatus(this Player player, bool status)
    {
        propsToSet[InitStatusKey] = status;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }


    /// <summary>
    /// ������Photon�̃v���C���[��n�����Ƃ�
    /// �߂�l�ł��̃v���C���[�̏�������񂪕Ԃ�
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool GetReadyNum(this Player player)
    {
        return (player.CustomProperties[ReadyNum] is bool status) ? status : false;
    }

    /// <summary>
    /// ������Photon�̃v���C���[�Ə�������Ԃ�n�����Ƃ�
    /// ���v���C���[�ɑ��M����
    /// </summary>
    /// <param name="player"></param>
    public static void SetReadyNum(this Player player, bool status)
    {
        propsToSet[ReadyNum] = status;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    public static int GetPlayerHP(this Player player)
    {
        return (player.CustomProperties[PlayerHP] is int status) ? status : 0;
    }

    public static void SetPlayerHP(this Player player, int status)
    {
        propsToSet[PlayerHP] = status;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
    public static int GetMyRank(this Player player)
    {
        return (player.CustomProperties[MyRank] is int status) ? status : 0;
    }

    public static void SetMyRank(this Player player, int status)
    {
        propsToSet[MyRank] = status;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }
}