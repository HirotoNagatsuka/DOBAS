using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;

[Serializable]
class Player
{
    public string Name; //���O���i�[.
    public int MaxHP;   //HP�̍ő�l;
    public int HP;      //HP���i�[.
    public int Attack;  //�U���͂��i�[.
    public Sprite[] HeartSprites;//HP�p�摜�̔z��.
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

    //��K�̕��Ő錾�����萔
    private const int SHUKAI = 26;//�}�X�̐��iPlayer�Ő錾���Ă���̂Ō�X�ύX�j.
    private const float MOVE_SPEED = 10.0f;      // �}�X��i�ޑ��x.
    #endregion

    #region [SerializeField]�錾
    [Header("[SerializeField]�錾")]
    [SerializeField]
    DiceManager diceManager;//DiceManager�Q�Ɨp.
    MapManager mapManager;
    [SerializeField]
    Player Player;//Player�̃N���X���C���X�y�N�^�[��Ō����悤�ɂ���.
    #endregion

    private int MoveMasu;                   // Move�}�X�𓥂񂾎��̐i�ރ}�X��
    private bool ActionFlg = true;        // �T�C�R����U�������ǂ���

    GameObject PlayerUI;//�q���̃L�����o�X���擾���邽�߂̕ϐ��錾.

    public static PlayerManager ins;      // �Q�Ɨp 
    public int Sum = 0;                     // �o�ڂ̍��v
    private bool DiceTrigger = true;        // �T�C�R����U�������ǂ���

    public int Card = 0;//�����g��Ȃ�(�J�[�h���������̕\��).

    private int deme;

    #region Unity�C�x���g(Awake�EStart�EUpdate�EOnTrigger)

    public void Awake()
    {
    //    if (ins == null)
    //    {
    //        ins = this;
    //    }
    }
    private void Start()
    {
        PlayerUI = gameObject.transform.GetChild(PLAYER_UI).gameObject;//�q���̃L�����o�X���擾.
        //PlayerUI.gameObject.transform.GetChild(NAME_UI).GetComponent<Text>().text = Player.Name.ToString();//���O�̕\��.
        //PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Text>().text = Player.HP.ToString();//HP�̕\��.
        PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.HP -1];//HP�̕\��.

        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        diceManager = GameObject.Find("DiceManager").GetComponent<DiceManager>();
        //diceManager = GetComponent<DiceManager>();
        //�ŏ��̃}�X�ɔz�u.
        transform.position = mapManager.MasumeList[0].position;//�����l0.
        Player.MaxHP = 5;
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
            if (Player.HP <= 0){
                PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = null;
            }
            else if(Player.HP > Player.MaxHP)
            {
                PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.MaxHP -1];
            }
            else
            {
                PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.HP -1];
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
            GetComponent<Message>().ShowText(NowTag); //�~�܂��Ă���}�X���ʂ��e�L�X�g�ɕ\��(�ǋL)
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
        if (diceManager.DeclarationNum == 4)
        {
            photonView.RPC(nameof(EnemyAttack), RpcTarget.All);
            //EnemyAttack();
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
        Player.HP--;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ���g�̃A�o�^�[�̃X�^�~�i�𑗐M����
            stream.SendNext(Player.HP);
        }
        else
        {
            // ���v���C���[�̃A�o�^�[�̃X�^�~�i����M����
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

            Player.HP = mapManager.GetComponent<MapManager>().HpOneUp(Player.HP);  // MapManager��HpOneUp�֐��������s��
            Debug.Log("HP�F" + Player.HP);
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
    /// </summary>
    public void ChangeHP(int addHP)
    {
        if(Player.HP == 0)//HP��0�ɂȂ�����.
        {
            Player.HP = 0;
            Debug.Log("�Q�[���I�[�o�[");
        }
        else Player.HP += addHP;//0�łȂ��Ȃ�ω�������.
        PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Text>().text = Player.HP.ToString();//HP�̕\��.
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

    #region ��K�̃\�[�X�ŕK�v���킩��Ȃ�����
    IEnumerator Action()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);

            // �T�C�R����U���Đi��

            yield return new WaitForSeconds(3);

            // �T�C�R���̏�����
            DiceReset();
        }
    }

    // �^�[�����ɃT�C�R���̏�����
    public void DiceReset()
    {
        DiceTrigger = true;
    }
    #endregion
}