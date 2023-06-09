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

    [Header("��������֘A")]
    [SerializeField] GameObject DicePrefab;//�T�C�R���̃v���t�@�u������.
    [SerializeField] GameObject DiceCamera;
    [SerializeField] Text DiceNumText;
    [SerializeField] GameObject[] ResultPanel = new GameObject[5];//�o�ڂ̃p�l���p.
    [SerializeField] GameObject ReasoningPanel;
    [SerializeField] GameObject DiceShakeButton;//�T�C�R����U��{�^��.
    public GameObject EnemyResult;//����̏o�ڂ�����.


    public List<string> PlayersName = new List<string>();
    public int[] PlayersHP;
    public static int MaxPlayersNum;//���X�N���v�g����A�N�Z�X����p.
    public static int WhoseTurn;//�N�̃^�[�����i�v���C���[ID���Q�Ƃ��Ă��̕ϐ��Ɣ�ׂă^�[�����������j.
    public Text WaitText;
    [SerializeField] GameObject PlayersNameGroupPrefab;//�E��ɕ\�����閼�O�𐶐�����p.
    [SerializeField] Sprite[] HeartSprites = new Sprite[5];
    [SerializeField] Sprite[] DiceSprites = new Sprite[4];

    //[SerializeField] DiceManager diceManager;
    #endregion

    #region private�ϐ�
    private bool doubtFlg;//�������낪5��6�̏ꍇ�A�t���O��؂芷����.
    private GameObject Dice;//�T�C�R���p�̕\���E��\�����J��Ԃ��p.
    Vector3 diceCameraPos;//�T�C�R����U�鏉���ʒu.
    private bool DiceFlg;//���������U�������̃t���O.
    private int ThroughNum;//�M����(�X���[����)�l��.

    #endregion

    #region �����_���ɂ����������]������p�̕ϐ��錾.
    private int rotateX;
    private int rotateY;
    private int rotateZ;
    #endregion


    private int Number;
    public int DeclarationNum;//�錾�ԍ�.

    public bool DiceFinishFlg;

    private bool[] UseID = new bool[4];//�v���C���[�Ɋ��蓖�Ă�ID�̎g�p��.
    private int ReadyPeople;//���������l��.

    public bool DeclarationFlg;//�錾�҂��t���O.

    bool timeflg;
    public List<GameObject> PlayersNameGroup = new List<GameObject>();
    public List<GameObject> Players = new List<GameObject>();
    #region Unity�C�x���g(Start�EUpdate)
    // Start is called before the first frame update
    void Start()
    {
        WhoseTurn = FIRST_TURN;
        PlayersHP = new int[MaxPlayers];//Player�̐l����HP�z���p��.
        for (int i = 0; i < MaxPlayers; i++)
        {
            PlayersHP[i] = FirstHP;//HP�̏����l����.
        }

        timeflg = false;
        DeclarationFlg = false;
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
    /// �I�������l��FinishTurn���Ăяo���A���̑S����ChangeTurn���Ăяo��.
    /// </summary>
    public void FinishTurn()
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
        Debug.Log("ChangeTurn()�N��");
        if (WhoseTurn == MaxPlayers)
        {
            WhoseTurn = FIRST_TURN;//WhoseTurn���v���C���[�̍ő吔�𒴂�����ŏ��̐l�ɖ߂�.
        }
        else
        {
            WhoseTurn++;           //���̐l�̃^�[���ɂ���.
        }
        DeclarationFlg = false;    //�S���̐錾�҂���Ԃ�false�ɂ���.
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
            ShakeDiceButton.SetActive(true);//�T�C�R����U��{�^����\��.
            WhoseTurnText.color = Color.red;//�^�[���e�L�X�g�̐F��ԂɕύX.
            WhoseTurnText.text = "���Ȃ��̃^�[��";
        }
        else//�����̃^�[���łȂ��Ȃ�.
        {
            ShakeDiceButton.SetActive(false);//�T�C�R����U��{�^�����\��.
            WhoseTurnText.color = Color.white;//�^�[���e�L�X�g�̐F�𔒂ɕύX.
            WhoseTurnText.text = PlayersName[WhoseTurn - 1] + "�̃^�[��";//�N�̃^�[�������e�L�X�g�ɕ\��.
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

    }
    #endregion

    #region HP�ύX�֘A
    /// <summary>
    /// �ύX�̂������v���C���[��l����\����HP�̓����֐����Ăяo��.
    /// </summary>
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
        PlayersNameGroup[subject - 1].transform.GetChild(PLAYER_HP).GetComponent<Image>().sprite = HeartSprites[PlayersHP[subject - 1] -1];
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
        PhotonNetwork.Instantiate("Player", new Vector3(0f, 0f, 0f), Quaternion.identity);//�v���C���[�𐶐�����.
        int i=0;                        //�ڍ׏���Y���W��ύX���邽�߂Ƀ��[�J���ȕϐ�i��p��.
        foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
        {
            PlayersName.Add(player.NickName);//Player�̖��O�����X�g�ɓ����.
            //Player�̏ڍ׏���\��������̂𐶐�����
            var obj = Instantiate(PlayersNameGroupPrefab, new Vector3(0f,0f,0f), Quaternion.identity,CanvasUI.transform);
            obj.GetComponent<RectTransform>().localPosition = new Vector3(760f, 465f -150f * i, 0f);        //�ʒu���E��ɐݒ�.
            obj.transform.GetChild(PLAYER_NAME).GetComponent<Text>().text = player.NickName;                //���O��\��.
            obj.transform.GetChild(PLAYER_HP).GetComponent<Image>().sprite = HeartSprites[PlayersHP[i] - 1];//����HP��\��.
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
    #endregion

    /// <summary>
    /// �錾���ꂽ�o�ڂ��󂯎��֐�.
    /// </summary>
    public void ReceiveDeme(int deme)
    {
        photonView.RPC(nameof(ActiveEnemyResult), RpcTarget.Others,deme);//�T�C�R����U���Ă��Ȃ��l�Ƀp�l����\������.
        WaitText.text = "���v���C���[�̐錾��҂��Ă��܂�";
    }

    /// <summary>
    /// �T�C�R���̏o�ڂ𑼂̐l�ɒʒm����p�l����\������֐�.
    /// </summary>
    [PunRPC]
    void ActiveEnemyResult(int deme)
    {
        EnemyResult.SetActive(true);
        EnemyResult.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = DiceSprites[deme - 1];
    }

    /// <summary>
    /// �M����{�^�����������Ƃ��ɌĂяo���֐�.
    /// </summary>
    public void PushBelieveButton()
    {
        photonView.RPC(nameof(AddThroughNum), RpcTarget.All);
    }
    public void PushDoubtButton()
    {
        DeclarationFlg = true;
        DiceInit();
        FinishTurn();
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
            WaitText.text = "";
            DiceInit();
        }
    }

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
        if (num == ATTACK)            //�U���ڂ��o���ꍇAttack�ƕ\������.
        {
            DiceNumText.text = "Attack";
        }
        else if (num == 5 || num == 6)//�_�E�g�ڂ̏ꍇDoubt�ƕ\��.
        {
            DiceNumText.text = "Doubt";
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
        if (Number == DOUBT)
        {
            doubtFlg = true;
        }
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
        //��X�C��.
        ResultPanel[0].SetActive(false);
        ResultPanel[1].SetActive(false);
        ResultPanel[2].SetActive(false);
        ResultPanel[3].SetActive(false);
        ResultPanel[4].SetActive(false);

        Debug.Log("�o�ځF" + DeclarationNum);
        DiceNumText.text = " ";
        DiceCamera.SetActive(false);
        Dice.SetActive(false);
        DiceFlg = false;
    }

    /// <summary>
    /// �T�C�R���̏������֐�
    /// �T�C�R����U��I��������ƂɌĂяo��.
    /// </summary>
    public void DiceInit()
    {
        DiceFinishFlg = false;
        DiceNumText.text = " ";
        DeclarationNum = 0;
        ReasoningPanel.SetActive(false);
        DiceCamera.SetActive(false);
        Dice.SetActive(false);
        DiceFlg = false;
        DiceShakeButton.SetActive(true);
    }

    /// <summary>
    /// �v���C���[��ID��^����֐�
    /// </summary>
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
}