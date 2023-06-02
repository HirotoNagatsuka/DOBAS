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
    const int NAME_UI = 0;
    const int HP_UI = 1;

    const float HEART_POS_X = -200;//�����l.
    const float HEART_POS_Y = 280f;//HP�摜�̕\���ʒu.

    const int ATTACK = 4;//�U���}�X�̐���.

    //��K�̕��Ő錾�����萔
    private const int SHUKAI = 26;//�}�X�̐��iPlayer�Ő錾���Ă���̂Ō�X�ύX�j.
    private const float MOVE_SPEED = 10.0f;      // �}�X��i�ޑ��x.
    #endregion

    #region [SerializeField]�錾
    [Header("[SerializeField]�錾")]
    [SerializeField]
    DiceManager diceManager;//DiceManager�Q�Ɨp.
    MapManager mapManager;
    GameManager gameManager;
    [SerializeField]
    PlayerStatus Player;//Player�̃N���X���C���X�y�N�^�[��Ō����悤�ɂ���.
    #endregion

    private int MoveMasu;                   // Move�}�X�𓥂񂾎��̐i�ރ}�X��
    private bool ActionFlg = true;        // �T�C�R����U�������ǂ���

    GameObject PlayerUI;//�q���̃L�����o�X���擾���邽�߂̕ϐ��錾.

    public int Sum = 0;                     // �o�ڂ̍��v
    private bool DiceTrigger = true;        // �T�C�R����U�������ǂ���

    public int Card = 0;//�����g��Ȃ�(�J�[�h���������̕\��).

    private int deme;

    #region Unity�C�x���g(Start�EUpdate�EOnTrigger)

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        diceManager = GameObject.Find("DiceManager").GetComponent<DiceManager>();
        //�ŏ��̃}�X�ɔz�u.
        transform.position = mapManager.MasumeList[0].position;//�����l0.
        Player.MaxHP = 5;
        Player.ID = gameManager.Give_ID_Player();
        Player.HP = gameManager.PlayersHP[Player.ID - 1];
        PlayerUI = gameObject.transform.GetChild(PLAYER_UI).gameObject;//�q���̃L�����o�X���擾.
        PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.HP - 1];//HP�̕\��.
        //Player.ID = PhotonNetwork.LocalPlayer.UserId;
        //Player.ID = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            Vector3 PlayerPos = transform.position;
            Vector3 TargetPos = mapManager.MasumeList[Sum].position;

            //// �ړ����[�V����
            transform.position = Vector3.MoveTowards(PlayerPos, TargetPos, MOVE_SPEED * Time.deltaTime);
            if (diceManager.FinishFlg) FinishDice();
            Player.HP = gameManager.PlayersHP[Player.ID - 1];
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
            mapManager.GetComponent<MapManager>().ColliderReference(collision);  // MapManager��ColliderReference�֐��������s��
            StartCoroutine("Activation", NowTag);  // Activation�R���[�`�������s
        }
    }

    #endregion

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


    /// <summary>
    /// �T�C�R����U��I�������Ăяo���֐�.
    /// </summary>
    public void FinishDice()
    {
        deme = diceManager.DeclarationNum;
        if (diceManager.DeclarationNum == ATTACK)
        {
           EnemyAttack();
           //photonView.RPC(nameof(EnemyAttack), RpcTarget.All);
        }
        else
        {
            StartDelay(deme);
        }
        diceManager.FinishFlg = false;
    }

    [PunRPC]
    void EnemyAttack()
    {
        Debug.Log("EnemyAttack()�N��");
        int rnd;//�����p.
        while (true)
        {
            rnd = UnityEngine.Random.Range(1, GameManager.MaxPlayersNum + 1);
            //if (PhotonNetwork.LocalPlayer.ActorNumber != rnd)//�������g�łȂ��ꍇ���[�v�𔲂���.
            if (Player.ID != rnd)//�������g�łȂ��ꍇ���[�v�𔲂���.
            {
                break;
            }
        }
        Debug.Log("���[�v�𔲂��܂���");
        //if (PhotonNetwork.LocalPlayer.ActorNumber != rnd)
        //{
        //    photonView.RPC(nameof(ChangeHP), RpcTarget.All, -1);
        //}
        Debug.Log("rnd" + rnd);
        photonView.RPC(nameof(ChangeHP), RpcTarget.All, -1,rnd);
        //if(PhotonNetwork.LocalPlayer.ActorNumber == rnd)
        //if (Player.ID == rnd)
        //{
        //    PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.HP - 1];//HP�̕\��.
        //}
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ���g�̃A�o�^�[��HP�𑗐M����
            stream.SendNext(Player.HP);
        }
        else
        {
            // ���v���C���[�̃A�o�^�[��HP����M����
            Player.HP = (int)stream.ReceiveNext();
        }
    }


    /// <summary>
    /// 1�}�X���i�܂���R���[�`��
    /// num�ɏo�ڂ����Ă��̕��i�܂���
    /// </summary>
    IEnumerator Delay(int num)
    {
        // ��}�X�Âi�܂���
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

        //mapManager.Reference();  // MapManager��Reference�֐��������s��
        DiceTrigger = false; // �T�C�R����U����(�^�[���̏I���)
    }

    // �擾�����^�O���ƂɌ��ʂ𔭓�
    IEnumerator Activation(string tag)
    {
        ActionFlg = true;

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
            photonView.RPC(nameof(ChangeHP), RpcTarget.All, 1,Player.ID);
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


    public void StartDelay(int num)
    {
        Debug.Log("StartDeley�N��");
        // �R���[�`���̊J�n
        StartCoroutine("DelayMove", num);
    }


    /// <summary>
    /// HP���ω�����Ƃ��ɌĂяo���֐�
    /// �ω��ʂ������ɂ��AHP��ς�����UI�ɂ����f����.
    /// �������ɂ͎������g���Ăяo�����̂��𔻒�.
    /// </summary>
    [PunRPC]
    public void ChangeHP(int addHP,int subject)
    {
        gameManager.ChangePlayersHP(addHP, subject);
        Debug.Log("ChangeHP�N�� �Ώ�"+subject);
        //if (Player.ID == subject)
        //{
        //    if (Player.HP == 0)//HP��0�ɂȂ�����.
        //    {
        //        gameManager.PlayersHP[Player.ID - 1] = 0;
        //        Player.HP = 0;
        //        Debug.Log("�Q�[���I�[�o�[");
        //    }
        //    else {
        //        gameManager.PlayersHP[Player.ID - 1] += addHP;
        //        Player.HP = gameManager.PlayersHP[Player.ID - 1];//0�łȂ��Ȃ�ω�������.
        //                            // PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Text>().text = Player.HP.ToString();//HP�̕\��.
        //    }
        //}
        PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.HP - 1];//HP�̕\��.
        //photonView.RPC(nameof(ChangeHPUI), RpcTarget.All);
    }
    [PunRPC]
    public void ChangeHPUI()
    {
        PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.HP - 1];//HP�̕\��.
    }

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
}