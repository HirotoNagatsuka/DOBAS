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
    public int HP;      //HP���i�[.
    public int Attack;  //�U���͂��i�[.
    public int ID;//�f�o�b�N�p.
}

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region �萔�錾
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

    public int Sum = 0;              // �o�ڂ̍��v
    public int MyRank;
    public Vector3 NowPos;
    public GameObject ResultCamera;
    Animator animator; // Animator
    Vector3 PlayerPos;      // �v���C���[�ʒu���
    public GameObject[] effectObject;   // �G�t�F�N�g�̃v���n�u�z��
    public AnimalsManager animalsManager;

    #region Unity�C�x���g(Start�EUpdate�EOnTrigger)

    private void Start()
    {
        animalsManager = this.GetComponent<AnimalsManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mapManager  = GameObject.Find("MapManager").GetComponent<MapManager>();
        animator = GetComponent<Animator>();
        //ResultCamera = GameObject.Find("ResultCamera");
        //�ŏ��̃}�X�ɔz�u.
        transform.position = mapManager.MasumeList[0].position;//�����l0.
        Player.ID = gameManager.Give_ID_Player();
        MyRank = 0;
        NamePosSet();
    }

    private void Update()
    {
        NowPos = this.transform.position;
        if (photonView.IsMine)
        {
            if (gameManager.NowGameState == GameManager.GameState.InGame)
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
            }
            else if (gameManager.NowGameState == GameManager.GameState.EndGame)
            {
                Vector3 position =  Vector3.zero;
                Debug.Log("PlayerEndGame�N��");
                switch (gameManager.Ranks[PhotonNetwork.LocalPlayer.ActorNumber-1]) {
                    case 0:
                        Debug.Log("MyRank0");
                        position = new Vector3(11.29004f, 0.3792114f, 9.680443f);
                        transform.position = position;
                        break;
                    case 1:
                        Debug.Log("MyRank1");
                        position = new Vector3(12.96002f, 0.3792114f, 9.680443f);
                        transform.position = position;
                        break;
                    case 2:
                        position = new Vector3(14.78003f, 0.3792114f, 9.680443f);
                        transform.position = position;
                        break;
                }
                // �J�����Ɍ�������
                ResultCamera = GameObject.Find("ResultCamera");
                Vector3 TargetRot = ResultCamera.transform.position - transform.position;
                transform.rotation = Quaternion.LookRotation(TargetRot);
                //Jump();
                switch (gameManager.Ranks[PhotonNetwork.LocalPlayer.ActorNumber - 1])
                {
                    case 0:
                        Jump();
                        break;
                    default:
                        Death();
                        break;
                }

            }
        }
    }

    /// <summary>
    /// �}�X�ɐG�ꂽ�Ƃ��^�O���Q�Ƃ��Č��ʂ��Ăяo��
    /// </summary>
    private void OnTriggerStay(Collider collision)
    {
        string NowTag = collision.tag; // �^�O���擾
        // �s���I�����A�}�X�̌��ʔ���
        if (ActionFlg == false)
        {
            ShowText(NowTag);
            mapManager.GetComponent<MapManager>().ColliderReference(collision);  // MapManager��ColliderReference�֐��������s��
            StartCoroutine("Activation", NowTag);  // Activation�R���[�`�������s
        }
    }

    #endregion

    #region �A�j���[�V�����֘A

    #region �O����Ăяo�����֐��p
    // �J�[�h�擾��
    public void CardGetting()
    {
        StartCoroutine("EffectPreview", "CardGet");
    }

    // �ړ����邩�ǂ����@true�F�ړ��J�n�@false�F�ړ��I��
    public void Moving(bool move)
    {
        if (move)
        {
            animator.SetBool("Walk", true); // Walk�A�j���Đ�
        }
        else
        {
            animator.SetBool("Walk", false); // Walk�A�j���Đ��I��
        }
    }

    // �U����
    public void Attacking()
    {
        if (animator == null) Debug.Log("�����");
        animator.SetTrigger("Attack"); // Attack�A�j���Đ�
    }

    // �̗͕ω����@true�F�̗͑����@false�F�̗͌���
    public void HpChange(bool hpChange)
    {
        if (hpChange)
        {
            StartCoroutine("EffectPreview", "HpUp");
        }
        else
        {
            StartCoroutine("EffectPreview", "HpDown");
        }
    }

    // �_���[�W���󂯂��Ƃ������Ă��邩�ǂ����@true�F�����@false�F���S
    public void Damage(bool alive)
    {
        if (alive)
        {
            StartCoroutine("EffectPreview", "Hit");
        }
        else
        {
            StartCoroutine("EffectPreview", "Death");
        }
    }

    public void Jump()
    {
        animator.SetTrigger("Jump");
    }
    public void Death()
    {
        animator.SetTrigger("Death"); // Death�A�j���Đ�
    }
    #endregion

    #region �G�t�F�N�g����
    IEnumerator EffectPreview(string AnimName)
    {
        switch (AnimName)
        {
            case "CardGet": // �J�[�h�擾��
                GameObject cardObj = Instantiate(effectObject[2], PlayerPos, Quaternion.identity);
                animator.SetTrigger("Jump"); // Jump�A�j���Đ�
                yield return new WaitForSeconds(2f);
                Destroy(cardObj);
                break;

            case "HpUp": // �̗͑���
                GameObject healObj = Instantiate(effectObject[0], PlayerPos, Quaternion.identity); // HpUp
                animator.SetTrigger("Jump"); // Jump�A�j���Đ�
                yield return new WaitForSeconds(2f);
                Destroy(healObj);
                break;

            //case "Move":
            //    break;

            //case "MoveEnd":
            //    break;

            //case "Attack":
            //    break;

            case "HpDown": // �̗͌���
                GameObject downObj = Instantiate(effectObject[1], PlayerPos, Quaternion.identity); // HpDown
                yield return new WaitForSeconds(2f);
                Destroy(downObj);
                break;

            case "Attack": // �U��
                animator.SetTrigger("Attack"); // Attack�A�j���Đ�
                break;

            case "Hit": // �ʏ�_���[�W
                Instantiate(effectObject[3], PlayerPos, Quaternion.identity); // GetHit
                animator.SetTrigger(AnimName); // Hit�A�j���Đ�
                break;

            case "Death": // �̗͂O�ɂȂ�Ƃ�
                Instantiate(effectObject[3], PlayerPos, Quaternion.identity); // GetHit
                Instantiate(effectObject[4], PlayerPos, Quaternion.identity); // HpBreak

                yield return new WaitForSeconds(0.5f);
                animator.SetTrigger(AnimName); // Death�A�j���Đ�
                yield return new WaitForSeconds(5f);
                //transform.GetChild(ChildNum).gameObject.SetActive(false); // 5�b��v���C���[���A�N�e�B�u(���S)
                break;

            default:
                Debug.LogError("AnimName�G���[");
                break;
        }
    }
    #endregion

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
        //Debug.Log("�o��" + deme);
        //Debug.Log("gameManager.DeclarationNum" + gameManager.DeclarationNum);
        //Debug.Log("�L�[���͑҂�");
        //Debug.Log("gameManager.DeclarationFlg" + gameManager.DeclarationFlg);
        yield return new WaitUntil(() => gameManager.DeclarationFlg == true); // �ҋ@����
        if (!gameManager.FailureDoubt)//�R�����Ă������Ɏw�E����Ă����瓮�����Ȃ�
        {
            Debug.Log("Deme" + deme);
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
            yield break;
        }
        gameManager.DiceFinishFlg = false;
        gameManager.DeclarationFlg = false;
        ResetFlg();
        yield return new WaitUntil(() => FinishFlg == true); // �ҋ@����
        if (GameManager.WhoseTurn == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            Debug.Log("Player���ł�gameManager.FinishTurn()�N��");
            gameManager.FinishTurn();
        }
        yield break;
    }
    /// <summary>
    /// 2�b�o�߂Ń^�[���I���𑗐M����R���[�`��.
    /// </summary>
    private IEnumerator WaitFinishTurnCoroutine()
    {
        // 3�b�ԑ҂�
        yield return new WaitForSeconds(2);
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
    void EnemyAttack()
    {
        Debug.Log("EnemyAttack()�N��");
        int rnd;//�����p.
        while (true)
        {
            rnd = UnityEngine.Random.Range(1, GameManager.MaxPlayersNum + 1);
            if (PhotonNetwork.LocalPlayer.ActorNumber != rnd && gameManager.PlayersHP[rnd-1] != 0)//�������g�łȂ��ꍇ���[�v�𔲂���.
            {
                        break;                
            }
        }
        //Debug.Log("���[�v�𔲂��܂���");
        //Debug.Log("�擾����rnd" + rnd);
        gameManager.ShowMessage(gameManager.PlayersName[rnd - 1]+"�ɍU���I", PhotonNetwork.LocalPlayer.ActorNumber);

        gameManager.EnemyAttack(-1, rnd);
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
    void ChangeHP(int addHP, int subject)
    {
            gameManager.ChangePlayersHP(addHP, subject);
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
                //Debug.Log("����{�[�i�X�Q�b�g�I");
                yield return new WaitForSeconds(2); // 2�b�҂�
            }
            else if (tag == "Card") // �J�[�h�}�X
            {
                Debug.Log("�J�[�h�P���Q�b�g�I");
                yield return new WaitForSeconds(2);
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
                //photonView.RPC(nameof(ChangeHP), RpcTarget.All, 1, Player.ID);
                //ChangeHP(1, PhotonNetwork.LocalPlayer.ActorNumber);
                gameManager.ChangePlayersHP(1, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else if (tag == "HpDown") // HP�}�X
            {
                //Debug.Log("HP�P�񕜁I�I");
                yield return new WaitForSeconds(2);
                //photonView.RPC(nameof(ChangeHP), RpcTarget.All, -1, Player.ID);
                //ChangeHP(-1, PhotonNetwork.LocalPlayer.ActorNumber);
                gameManager.ChangePlayersHP(-1, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else if (tag == "Attack") //�U���}�X
            {
                Debug.Log("���̃v���C���[���U���I");
                yield return new WaitForSeconds(2);
                EnemyAttack();
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
    /// ���l�̏o�ڂ��󂯎������Ăяo���֐�(�I�[�o�[���[�h)
    /// �����ɏo�ڂ��󂯎��ړ��p�R���[�`�����N������.
    /// </summary>
    public void StartDelay(int num, bool cardMove) // �������ǉ�(����)
    {
        Debug.Log("StartDeley(OverLoad)�N��");
        NowCardMove = cardMove; //(����)
        StartCoroutine("DelayMove", num);        // �R���[�`���̊J�n
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

    [SerializeField] GameObject nameObject;

    void NamePosSet()
    {
        nameObject.transform.localPosition = new Vector3(0, 0, -1.5f);
        nameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }
}