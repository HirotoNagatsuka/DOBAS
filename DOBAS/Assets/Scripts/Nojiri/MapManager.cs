using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //public static MapManager instance;        // �Q�Ɨp
    public List<Transform> MasumeList;        // �}�X�̔z��

    //[SerializeField] GameObject MapPlayer;    // MapManager�Q��
    [SerializeField] private int NowMasume;   // ���ݓ���ł���}�X�̒l

    private int Hp = 4;          // �̗�
    private int Card;            // �J�[�h����
    private int Saikoro;         // �T�C�R���̏o��(1�`6)
    private bool MyTurn = true;  // �����̃^�[��

    //// �Q�Ɨp
    //public void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
        NowMasume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (MyTurn == true)
        //{
        //    //NowMasume += Saikoro; //���݂̃}�X
        //    Activate();
        //    MyTurn = false;
        //}
    }

    //�}�X�̌��ʐU�蕪��
    public void Activate()
    {
        switch(NowMasume)
        {
            case 0:
                // �X�^�[�gor����
                Debug.Log("����" + NowMasume);
                break;
            case 1:
                // �J�[�h�}�X
                CardOneUp();
                break;
            case 2:
            case 6:
                // �Z�}�X�ړ�
                Debug.Log("�ړ�" + NowMasume);
                break;
            case 3:
                // HP������
                HpOneUp();
                break;
            case 4:
                // �U���}�X
                Attack();
                break;
            case 5:
                // ���ʂȂ�
                Debug.Log("���ʂȂ�" + NowMasume);
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
            default:
                break;
        }
    }

    #region �}�X�̈ړ�
    public void Move()
    {
        //Saikoro = MapPlayer.GetComponent<MapPlayer>().DiceNum;
        NowMasume = MapPlayer.ins.Sum;
        Activate();
    }
    #endregion

    #region HP�𑝂₷
    public void HpOneUp()
    {
        Hp++;

        Debug.Log("HP���}�X" + NowMasume);
        Debug.Log("HP" + Hp);
    }
    #endregion

    #region �J�[�h�𑝂₷
    public void CardOneUp()
    {
        Card++;

        Debug.Log("�J�[�h�}�X" + NowMasume);
        Debug.Log(Card + "��");
    }
    #endregion

    #region �U��
    public void Attack()
    {
        Debug.Log("�U��" + NowMasume);
    }
    #endregion

    #region HP�����擾
    public int GetHp()
    {
        return Hp;
    }
    #endregion

    #region �}�X���Ƃ̍��W���X�g���擾
    public Vector3 GetMasumePos(int masume)
    {
        return MasumeList[masume].position; // �O��������W���A�N�Z�X�\�ɂ���
    }
    #endregion
}
