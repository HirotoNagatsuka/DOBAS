using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;

[Serializable]
class PlayerStatus
{
    public string Name; //���O���i�[.
    public int MaxHP;   //HP�̍ő�l;
    public int HP;      //HP���i�[.
    public int Attack;  //�U���͂��i�[.
   // public Sprite[] HeartSprites;//HP�p�摜�̔z��.
    public int ID;//�f�o�b�N�p.
}

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region �萔�錾
    //UI�\���Ɏg�p����萔�l
    const int PLAYER_UI = 0;
    const int HP_UI = 0;
    const int ATTACK_NUM_UI = 2;

    const float HEART_POS_X = -200;//�����l.
    const float HEART_POS_Y = 280f;//HP�摜�̕\���ʒu.

    const int ATTACK = 4;//�U���o�ڂ̐���.

    //��K�̕��Ő錾�����萔
    private const int SHUKAI = 26;//�}�X�̐��iPlayer�Ő錾���Ă���̂Ō�X�ύX�j.
    private const float MOVE_SPEED = 10.0f;      // �}�X��i�ޑ��x.
    #endregion

    #region �O���X�N���v�g�Q�Ɨp�錾
    GameManager gameManager;//GameManager�Q�Ɨp.
    MapManager mapManager; //MapManager�Q�Ɨp.
    [SerializeField]
    CardManager cardManager; // CardListManager�Q�Ɨp(����)
    #endregion

    // ����
    private int RandomNum;
    public bool NowCardMove = false;

    #region public�ESerializeField�錾
    [Header("[SerializeField]�錾")]
    [SerializeField]
    PlayerStatus Player;//Player�̃N���X���C���X�y�N�^�[��Ō����悤�ɂ���.
    #endregion

    public bool doubtFlg;//�R�����Ă��邩����.
    public bool CoroutineFlg;//�X�}�[�g�ł͂Ȃ��̂ŏC��������.

    private int MoveMasu;            // Move�}�X�𓥂񂾎��̐i�ރ}�X��
    private bool ActionFlg = true;   // �T�C�R����U�������ǂ���
    bool FinishFlg = false;
    GameObject PlayerUI;             //�q���̃L�����o�X���擾���邽�߂̕ϐ��錾.

    public int Sum = 0;              // �o�ڂ̍��v
    private bool DiceTrigger = true; // �T�C�R����U�������ǂ���

    public int Card = 0;//�����g��Ȃ�(�J�[�h���������̕\��).

    #region Unity�C�x���g(Start�EUpdate�EOnTrigger)

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mapManager  = GameObject.Find("MapManager").GetComponent<MapManager>();
        //gameManager = GameObject.Find("DiceManager").GetComponent<DiceManager>();
        //�ŏ��̃}�X�ɔz�u.
        transform.position = mapManager.MasumeList[0].position;//�����l0.
        Player.ID = gameManager.Give_ID_Player();
        Player.HP = gameManager.PlayersHP[Player.ID - 1];
        PlayerUI = gameObject.transform.GetChild(PLAYER_UI).gameObject;//�q���̃L�����o�X���擾.
       // PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.HP - 1];//HP�̕\��.
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            Vector3 PlayerPos = transform.position;
            Vector3 TargetPos = mapManager.MasumeList[Sum].position;

            // �ړ����[�V����
            transform.position = Vector3.MoveTowards(PlayerPos, TargetPos, MOVE_SPEED * Time.deltaTime);
            if (gameManager.DiceFinishFlg && CoroutineFlg == false)
            {
                    CoroutineFlg = true;
                    FinishDice();
            }
            Player.HP = gameManager.PlayersHP[PhotonNetwork.LocalPlayer.ActorNumber - 1];
        }
    }

    /// <summary>
    /// �}�X�ɐG�ꂽ�Ƃ��^�O���Q�Ƃ��Č��ʂ��Ăяo��
    /// </summary>
    private void OnTriggerStay(Collider collision)
    {
        string NowTag = collision.tag; // �^�O���擾
        //Debug.Log("OnTrigger�N��");
        // �s���I�����A�}�X�̌��ʔ���
        if (ActionFlg == false)
        {
            ShowText(NowTag);
            //GetComponent<Message>().ShowText(NowTag); //�~�܂��Ă���}�X���ʂ��e�L�X�g�ɕ\��(�ǋL)
            mapManager.GetComponent<MapManager>().ColliderReference(collision);  // MapManager��ColliderReference�֐��������s��
            StartCoroutine("Activation", NowTag);  // Activation�R���[�`�������s
        }
    }

    #endregion

    #region ���b�Z�[�W���M�֘A
    public void ShowText(string tag)
    {
        string message = "";
        bool noneflg = false;
        switch (tag)
        {
            case "Start":
                message = "����{�[�i�X�Q�b�g�I�@�U���́{�P";
                break;
            case "Card":
                message = "�J�[�h���P���Q�b�g�I";
                break;
            case "Move":
                message = "3�}�X�i�ށI";
                break;
            case "Hp":
                message = "HP���P�񕜁I";
                break;
            case "Attack":
                message = "���̃v���C���[���U���I";
                break;
            default:
                //message = "���ʂȂ�";
                noneflg = true;
                break;
        }
        if (!noneflg)
        {
            gameManager.ShowMessage(message, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }
        #endregion

        /// <summary>
        /// �v���C���[�̎����Ă���UI(HP�ƍU����)��ύX����֐�.
        /// </summary>
        private void ChangePlayerUI()
    {
        //PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.HP - 1];//HP�̕\��.
        PlayerUI.gameObject.transform.GetChild(ATTACK_NUM_UI).GetComponent<Text>().text = Player.Attack.ToString();//HP�̕\��.
    }

    /// <summary>
    /// �T�C�R����U��I�������Ăяo���֐�.
    /// </summary>
    public void FinishDice()
    {
        int deme = gameManager.DeclarationNum;//�o�ڂ��󂯎��.
        gameManager.ReceiveDeme(deme);
        StartCoroutine("WaitDoubt", deme);
    }

    /// <summary>
    /// ���̃v���C���[�̃_�E�g�錾��҂R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitDoubt(int deme)
    {
        Debug.Log("�o��" + deme);
        Debug.Log("gameManager.DeclarationNum" + gameManager.DeclarationNum);
        Debug.Log("�L�[���͑҂�");
        Debug.Log("gameManager.DeclarationFlg" + gameManager.DeclarationFlg);
        yield return new WaitUntil(() => gameManager.DeclarationFlg == true); // �ҋ@����
        if (!gameManager.FailureDoubt)//�R�����Ă������Ɏw�E����Ă����瓮�����Ȃ�
        {
            if (deme == ATTACK)
            {
                Debug.Log("EnemyAttack()�N���pif����");
                EnemyAttack();
            }
            else
            {
                StartDelay(deme, false);
            }
        }
        else
        {
            StartCoroutine(WaitFinishTurnCoroutine());
        }
        gameManager.DiceFinishFlg = false;
        gameManager.DeclarationFlg = false;
        ResetFlg();
        yield return new WaitUntil(() => FinishFlg == true); // �ҋ@����
        gameManager.FinishTurn();
        yield break;
    }
    /// <summary>
    /// 3�b�o�߂Ń^�[���I���𑗐M����R���[�`��.
    /// </summary>
    private IEnumerator WaitFinishTurnCoroutine()
    {
        // 3�b�ԑ҂�
        yield return new WaitForSeconds(3);
        gameManager.FinishTurn();//�s�����I�������^�[�����I��点��.
        yield break;
    }

    /// <summary>
    /// �t���O�̏��������s���֐�
    /// </summary>
    void ResetFlg()
    {
        CoroutineFlg = doubtFlg = false;
    }

    /// <summary>
    /// ���̃v���C���[���U�����邽�߂̊֐�
    /// �������擾���A�����ƈ�v����ID�����v���C���[���U������.
    /// </summary>
    [PunRPC]
    void EnemyAttack()
    {
        Debug.Log("EnemyAttack()�N��");
        int rnd;//�����p.
        while (true)
        {
            rnd = UnityEngine.Random.Range(1, GameManager.MaxPlayersNum + 1);
            if (PhotonNetwork.LocalPlayer.ActorNumber != rnd)//�������g�łȂ��ꍇ���[�v�𔲂���.
            {
                break;
            }
        }
        Debug.Log("���[�v�𔲂��܂���");
        Debug.Log("�擾����rnd" + rnd);
        gameManager.ShowMessage(gameManager.PlayersName[rnd - 1]+"�ɍU���I", PhotonNetwork.LocalPlayer.ActorNumber);

        photonView.RPC(nameof(ChangeHP), RpcTarget.All, -1,rnd);
        FinishFlg = true;
    }
    #region EnemyAttack�I�[�o�[���[�h
    /// <summary>
    /// ���̃v���C���[���U�����邽�߂̊֐�
    /// �������擾���A�����ƈ�v����ID�����v���C���[���U������.
    /// </summary>
    [PunRPC]
    public void EnemyAttack(int powNum)//�����ǉ�(����)
    {
        Debug.Log("EnemyAttack()�N��");
        int rnd;//�����p.
        while (true)
        {
            rnd = UnityEngine.Random.Range(1, GameManager.MaxPlayersNum + 1);
            if (PhotonNetwork.LocalPlayer.ActorNumber != rnd)//�������g�łȂ��ꍇ���[�v�𔲂���.
            //if (Player.ID != rnd)//�������g�łȂ��ꍇ���[�v�𔲂���.
            {
                break;
            }
        }
        Debug.Log("���[�v�𔲂��܂���");
        Debug.Log("�擾����rnd" + rnd);
        FinishFlg = true;
        photonView.RPC(nameof(ChangeHP), RpcTarget.All, powNum, rnd);
    }
    #endregion

    /// <summary>
    /// HP���ω�����Ƃ��ɌĂяo���֐�
    /// �ω��ʂ������ɂ��AHP��ς�����UI�ɂ����f����.
    /// �������ɂ͎������g���Ăяo�����̂��𔻒�.
    /// </summary>
    [PunRPC]
    public void ChangeHP(int addHP, int subject)
    {
        if (subject == PhotonNetwork.LocalPlayer.ActorNumber)//�������g���Ώۂ̏ꍇ�̂�HP��ω�������֐����Ă�.
        {
            gameManager.ChangePlayersHP(addHP, subject);
        }
    }

    #region �ړ��֘A
    /// <summary>
    /// OnTrigger�Ŏ擾�����^�O���ƂɌ��ʂ𔭓�����R���[�`��
    ///�����ɕ�����Ń^�O���擾���A���򂷂�.
    /// </summary>
    IEnumerator Activation(string tag)
    {
        ActionFlg = true;
        if (!NowCardMove) // �J�[�h�ړ����Ƀ}�X�̌��ʂ��󂯂Ȃ�(����)
        {
            yield return new WaitUntil(() => gameManager.FinMessage == true); // �ҋ@����
            // �^�O���Ƃɕ���
            if (tag == "Start") // �X�^�[�g�}�X
            {
                Debug.Log("����{�[�i�X�Q�b�g�I");
                yield return new WaitForSeconds(2); // 2�b�҂�
            }
            else if (tag == "Card") // �J�[�h�}�X
            {
                Debug.Log("�J�[�h�P���Q�b�g�I");
                yield return new WaitForSeconds(2);

                Card = mapManager.GetComponent<MapManager>().CardOneUp(Card);  // MapManager��CardOneUp�֐��������s��

               // SendCardList(); // ����(����)
            }
            else if (tag == "Move") // �ړ��}�X
            {
                Debug.Log("3�}�X�i�ށI");
                yield return new WaitForSeconds(2);

                MoveMasu = mapManager.GetComponent<MapManager>().Move(MoveMasu, tag);  // MapManager��Move�֐��������s��
                StartCoroutine("DelayMove", MoveMasu);                            // �P�}�X�Âi��
            }
            else if (tag == "Hp") // HP�}�X
            {
                Debug.Log("HP�P�񕜁I�I");
                yield return new WaitForSeconds(2);

                Debug.Log("HP�F" + Player.HP);
                photonView.RPC(nameof(ChangeHP), RpcTarget.All, 1, Player.ID);
                //ChangeHP(1, PhotonNetwork.LocalPlayer.ActorNumber);
                //ChangeHP(1);
            }
            else if (tag == "HpDown") // HP�}�X
            {
                //Debug.Log("HP�P�񕜁I�I");
                yield return new WaitForSeconds(2);

                //Debug.Log("HP�F" + Player.HP);
                photonView.RPC(nameof(ChangeHP), RpcTarget.All, -1, Player.ID);
                //ChangeHP(1, PhotonNetwork.LocalPlayer.ActorNumber);
                //ChangeHP(1);
            }
            else if (tag == "Attack") //�U���}�X
            {
                Debug.Log("���̃v���C���[���U���I");
                yield return new WaitForSeconds(2);
                EnemyAttack();
                //mapManager.GetComponent<MapManager>().Attack();  // MapManager��Attack�֐��������s��
            }
            else // �m�[�}���}�X
            {
                Debug.Log("���ʂ̃}�X");
                yield return new WaitForSeconds(2);
            }
            FinishFlg = true;
        }
    }
    /// <summary>
    /// ���l�̏o�ڂ��󂯎������Ăяo���֐�
    /// �����ɏo�ڂ��󂯎��ړ��p�R���[�`�����N������.
    /// </summary>
    public void StartDelay(int num)
    {
        Debug.Log("StartDeley�N��");
        // �R���[�`���̊J�n
        StartCoroutine("DelayMove", num);
    }
    /// <summary>
    /// ���l�̏o�ڂ��󂯎������Ăяo���֐�
    /// �����ɏo�ڂ��󂯎��ړ��p�R���[�`�����N������.
    /// </summary>
    public void StartDelay(int num, bool cardMove) // �������ǉ�(����)
    {
        Debug.Log("StartDeley(OverLoad)�N��");
        NowCardMove = cardMove; //(����)
        // �R���[�`���̊J�n
        StartCoroutine("DelayMove", num);
    }
    /// <summary>
    /// �����Ŏ󂯎�������}�X���ړ��w����R���[�`��.
    /// </summary>
    IEnumerator DelayMove(int num)
    {
        // �P�}�X�Âi�܂���
        for (int i = 0; i < num; i++)
        {
            Sum++;  // ���݂̃}�X�ԍ�(�T�C�R���ڂ̍��v)

            // �X�^�[�g�n�_�ɖ߂�
            if (Sum >= SHUKAI)
            {
                Sum = 0;
            }

            yield return new WaitForSeconds(0.5f); // 0.5�b�҂�
        }
        ActionFlg = false; // �v���C���[�̍s���I��(�}�X���ʔ����O)
    }
    #endregion

    /// <summary>
    /// PUN2���g�����ϐ�����.
    /// �����œ����������ϐ���S�đ��M����.
    /// </summary>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ���g�̃A�o�^�[��HP�𑗐M����
            stream.SendNext(Player.HP);
            stream.SendNext(Sum);
        }
        else
        {
            // ���v���C���[�̃A�o�^�[��HP����M����
            Player.HP = (int)stream.ReceiveNext();
            Sum = (int)stream.ReceiveNext();
        }


    }
    #region ����ǉ�
    public void SendCardList()
    {
        RandomNum = UnityEngine.Random.Range(0, cardManager.GetCardLists().Count);
    }
    #endregion
}