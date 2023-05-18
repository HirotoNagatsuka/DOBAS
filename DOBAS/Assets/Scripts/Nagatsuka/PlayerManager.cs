using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
class Player
{
    public string Name; //���O���i�[.
    public int HP;      //HP���i�[.
    public int Attack;  //�U���͂��i�[.
}

public class PlayerManager : MonoBehaviour
{
    #region �萔�錾
    //UI�\���Ɏg�p����萔�l
    const int PLAYER_UI = 0;
    const int NAME_UI = 0;
    const int HP_UI = 1;

    //��K�̕��Ő錾�����萔
    private const int SHUKAI = 26;//�}�X�̐��iPlayer�Ő錾���Ă���̂Ō�X�ύX�j.
    private const float MOVE_SPEED = 10.0f;      // �}�X��i�ޑ��x.
    #endregion

    #region [SerializeField]�錾
    [Header("[SerializeField]�錾")]
    [SerializeField]
    DiceManager diceManager;//DiceManager�Q�Ɨp.
    [SerializeField] 
    MapManager mapManager; // MapManager�Q��.
    [SerializeField]
    Player Player;//Player�̃N���X���C���X�y�N�^�[��Ō����悤�ɂ���.
    #endregion

    GameObject PlayerUI;//�q���̃L�����o�X���擾���邽�߂̕ϐ��錾.

    public static PlayerManager ins;      // �Q�Ɨp 
    public int Sum = 0;                     // �o�ڂ̍��v
    private bool DiceTrigger = true;        // �T�C�R����U�������ǂ���

    public int Card = 0;//�����g��Ȃ�(�J�[�h���������̕\��).

    #region Unity�C�x���g(Awake�EStart�EUpdate�EOnTrigger)

    public void Awake()
    {
        if (ins == null)
        {
            ins = this;
        }
    }
    private void Start()
    {
        PlayerUI = gameObject.transform.GetChild(PLAYER_UI).gameObject;//�q���̃L�����o�X���擾.
        //PlayerUI.gameObject.transform.GetChild(NAME_UI).GetComponent<Text>().text = Player.Name.ToString();//���O�̕\��.
        PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Text>().text = Player.HP.ToString();//HP�̕\��.

        //diceManager = GetComponent<DiceManager>();
        //�ŏ��̃}�X�ɔz�u.
        //transform.position = mapManager.MasumeList[0].position;//�����l0.

        // �R���[�`���̊J�n
        //StartCoroutine("Action");
    }

    private void Update()
    {
        Vector3 PlayerPos = transform.position;
        //Vector3 TargetPos = mapManager.MasumeList[Sum].position;

        //// �ړ����[�V����
        //transform.position = Vector3.MoveTowards(PlayerPos, TargetPos, MOVE_SPEED * Time.deltaTime);
    }

    /// <summary>
    /// �}�X�ɐG�ꂽ�Ƃ��^�O���Q�Ƃ��Č��ʂ��Ăяo��
    /// </summary>
    private void OnTriggerStay(Collider collision)
    {
        if (DiceTrigger == false)
        {
            if (collision.gameObject.tag == "Start")
            {
                Debug.Log("Start�}�X");
            }
            else if (collision.gameObject.tag == "Card")
            {
                Card = mapManager.CardOneUp(Card);  // MapManager��CardOneUp�֐��������s��
                Debug.Log("�J�[�h�F" + Card + "��");
                DiceTrigger = true;
            }
            else if (collision.gameObject.tag == "Move")
            {
                Sum = mapManager.Move(Sum);  // MapManager��Move�֐��������s��
                Debug.Log("�ړ�����");
                DiceTrigger = true;
            }
            else if (collision.gameObject.tag == "Hp")
            {
                Player.HP = mapManager.HpOneUp(Player.HP);  // MapManager��HpOneUp�֐��������s��
                Debug.Log("HP�F" + Player.HP);
                ChangeHP(0);//�����ɂ�HP�̕ω��l�����i��K�̕��ŕω����Ă���̂Ō����0������ɕύX.         
                DiceTrigger = true;
            }
            else if (collision.gameObject.tag == "Attack")
            {
                mapManager.Attack();  // MapManager��Attack�֐��������s��
                DiceTrigger = true;
            }
        }
    }

    #endregion

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

        mapManager.Reference();  // MapManager��Reference�֐��������s��
        DiceTrigger = false; // �T�C�R����U����(�^�[���̏I���)
    }

    public void StartDelay(int num)
    {
        // �R���[�`���̊J�n
        StartCoroutine("Delay", num);
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
