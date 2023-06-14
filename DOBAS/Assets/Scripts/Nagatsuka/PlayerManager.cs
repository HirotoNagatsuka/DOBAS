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
    public Sprite[] HeartSprites;//HP�p�摜�̔z��.
    //public string ID;//�f�o�b�N�p.
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

    // ����
    private int RandomNum;
    private bool NowCardMove = false;

    #region �O���X�N���v�g�Q�Ɨp�錾
    GameManager gameManager;//GameManager�Q�Ɨp.
    DiceManager diceManager;//DiceManager�Q�Ɨp.
    MapManager mapManager; //MapManager�Q�Ɨp.

    [SerializeField]
    CardManager cardManager; // CardListManager�Q�Ɨp(����)
    #endregion

    #region public�ESerializeField�錾
    [Header("[SerializeField]�錾")]
    [SerializeField]
    PlayerStatus Player;//Player�̃N���X���C���X�y�N�^�[��Ō����悤�ɂ���.
    #endregion

    private int MoveMasu;            // Move�}�X�𓥂񂾎��̐i�ރ}�X��
    private bool ActionFlg = true;   // �T�C�R����U�������ǂ���

    GameObject PlayerUI;             //�q���̃L�����o�X���擾���邽�߂̕ϐ��錾.

    public int Sum = 0;              // �o�ڂ̍��v
    private bool DiceTrigger = true; // �T�C�R����U�������ǂ���

    public int Card = 0;//�����g��Ȃ�(�J�[�h���������̕\��).

    #region Unity�C�x���g(Start�EUpdate�EOnTrigger)

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mapManager  = GameObject.Find("MapManager").GetComponent<MapManager>();
        diceManager = GameObject.Find("DiceManager").GetComponent<DiceManager>();
        
        //�ŏ��̃}�X�ɔz�u.
        transform.position = mapManager.MasumeList[0].position;//�����l0.
        Player.ID = gameManager.Give_ID_Player();
        //Player.HP = gameManager.PlayersHP[Player.ID - 1];
        PlayerUI = gameObject.transform.GetChild(PLAYER_UI).gameObject;//�q���̃L�����o�X���擾.
        PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.HP - 1];//HP�̕\��.

        Debug.Log(cardManager);
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            Vector3 PlayerPos = transform.position;
            Vector3 TargetPos = mapManager.MasumeList[Sum].position;

            // �ړ����[�V����
            transform.position = Vector3.MoveTowards(PlayerPos, TargetPos, MOVE_SPEED * Time.deltaTime);
            if (diceManager.FinishFlg) FinishDice();
            //Player.HP = gameManager.PlayersHP[PhotonNetwork.LocalPlayer.ActorNumber - 1];
        }
        //ChangePlayerUI();
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
            mapManager.GetComponent<MapManager>().ColliderReference(collision);  // MapManager��ColliderReference�֐��������s��
            StartCoroutine("Activation", NowTag);  // Activation�R���[�`�������s
        }
    }

    #endregion

    /// <summary>
    /// �v���C���[�̎����Ă���UI(HP�ƍU����)��ύX����֐�.
    /// </summary>
    private void ChangePlayerUI()
    {
        PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.HP - 1];//HP�̕\��.
        PlayerUI.gameObject.transform.GetChild(ATTACK_NUM_UI).GetComponent<Text>().text = Player.Attack.ToString();//HP�̕\��.
    }

    /// <summary>
    /// �T�C�R����U��I�������Ăяo���֐�.
    /// </summary>
    public void FinishDice()
    {
        int deme = diceManager.DeclarationNum;//�o�ڂ��󂯎��.
        if (diceManager.DeclarationNum == ATTACK)
        {
           EnemyAttack(-1);// �����ǉ�(����)
        }
        else
        {
            StartDelay(deme,false);// �������ǉ�(����)
        }
        diceManager.FinishFlg = false;
    }

    /// <summary>
    /// ���̃v���C���[���U�����邽�߂̊֐�
    /// �������擾���A�����ƈ�v����ID�����v���C���[���U������.
    /// </summary>
    [PunRPC]
    void EnemyAttack(int powNum)//�����ǉ�(����)
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
        photonView.RPC(nameof(ChangeHP), RpcTarget.All, powNum,rnd);//�U���l�ύX(����)
    }

    /// <summary>
    /// HP���ω�����Ƃ��ɌĂяo���֐�
    /// �ω��ʂ������ɂ��AHP��ς�����UI�ɂ����f����.
    /// �������ɂ͎������g���Ăяo�����̂��𔻒�.
    /// </summary>
    [PunRPC]
    public void ChangeHP(int addHP, int subject)
    {
        gameManager.ChangePlayersHP(addHP, subject);
        PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.HP - 1];//HP�̕\��.
    }

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

                SendCardList(); // ����
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
                //photonView.RPC(nameof(ChangeHP), RpcTarget.All, 1,Player.ID);
                ChangeHP(1, PhotonNetwork.LocalPlayer.ActorNumber);
                //ChangeHP(1);
            }
            else if (tag == "Attack") //�U���}�X
            {
                Debug.Log("���̃v���C���[���U���I");
                yield return new WaitForSeconds(2);

                mapManager.GetComponent<MapManager>().Attack();  // MapManager��Attack�֐��������s��
            }
            else // �m�[�}���}�X
            {
                Debug.Log("���ʂ̃}�X");
                yield return new WaitForSeconds(2);
            }
        }
    }
    /// <summary>
    /// ���l�̏o�ڂ��󂯎������Ăяo���֐�
    /// �����ɏo�ڂ��󂯎��ړ��p�R���[�`�����N������.
    /// </summary>
    public void StartDelay(int num,bool cardMove) // �������ǉ�(����)
    {
        Debug.Log("StartDeley�N��");
        NowCardMove = cardMove;
        // �R���[�`���̊J�n
        StartCoroutine("DelayMove", num);
    }
    /// <summary>
    /// �����Ŏ󂯎�������}�X���ړ��w����R���[�`��.
    /// </summary>
    IEnumerator DelayMove(int num) //�������ǉ�(����)
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

    #region ���ˍ쐬doubt�v���g�^�C�v
    public void PushBelieveButton()
    {
        Debug.Log("�M����!");
        diceManager.DiceInit();
    }
    public void PushDoubtButton()
    {
        Debug.Log("�_�E�g!");
        diceManager.DiceInit();
    }
    #endregion

    #region ����ǉ�
    public void SendCardList()
    {
        RandomNum = UnityEngine.Random.Range(0, cardManager.GetCardLists().Count);
    }
    #endregion
}