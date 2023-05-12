using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //public static MapManager instance;        // �Q�Ɨp
    public List<Transform> MasumeList;        // �}�X�̔z��

    //[SerializeField] GameObject MapPlayer;    // MapManager�Q��
    [SerializeField] private int NowMasume;   // ���ݓ���ł���}�X�̒l

    //private int Hp = 4;            // �̗�
    //private int Card;              // �J�[�h����

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

    }

    //# region �}�X�̌��ʐU�蕪��
    //public void Activate()
    //{
    //    switch(NowMasume)
    //    {
    //        case 0:
    //            // �X�^�[�gor����
    //            Debug.Log("����F" + NowMasume + "�}�X��");
    //            break;

    //        case 1:
    //        case 6:
    //        case 10:
    //        case 13:
    //        case 17:
    //        case 23:
    //            // �J�[�h�}�X
    //            CardOneUp();
    //            break;

    //        case 2:
    //        case 9:
    //        case 15:
    //        case 19:
    //            // �Z�}�X�ړ�
    //            MapPlayer.ins.Sum += 3;
    //            Debug.Log("3�}�X�ړ��F" + NowMasume + "�}�X��");
    //            //Move();
    //            break;

    //        case 3:
    //        case 7:
    //        case 8:
    //        case 14:
    //        case 21:
    //        case 24:
    //            // HP����
    //            HpOneUp();
    //            break;

    //        case 4:
    //        case 11:
    //        case 16:
    //        case 20:
    //        case 25:
    //            // �U���}�X
    //            Attack();
    //            break;

    //        case 5:
    //        case 12:
    //        case 18:
    //        case 22:
    //            // ���ʂȂ�
    //            Debug.Log("���ʂȂ��F" + NowMasume + "�}�X��");
    //            break;

    //        default:
    //            Debug.Log(NowMasume + "�}�X��");
    //            break;
    //    }
    //}
    //#endregion

    #region �v���C���[����̒l�̎��o��
    public void Reference()
    {
        NowMasume = MapPlayer.ins.Sum;

        //Activate();
    }
    #endregion

    #region �J�[�h�𑝂₷
    public int CardOneUp(int card)
    {
        card++;
        Debug.Log("�J�[�h�}�X�F" + NowMasume + "�}�X��");

        return card;
    }
    #endregion


    #region �}�X�̈ړ�
    public int Move(int move)
    {
        if (NowMasume == 2 || NowMasume == 9 || NowMasume == 19)
        {
            for (int i = 0; i < 3; i++)
            {
                move++;
            }
            Debug.Log("3�}�X�ړ��F" + NowMasume + "�}�X��");
        }
        else if (NowMasume == 15)
        {
            move += 1;
            Debug.Log("1�}�X�ړ��F" + NowMasume + "�}�X��");
        }

        return move;
    }
    #endregion

    #region HP�𑝌�
    public int HpOneUp(int hp)
    {
        hp++;

        Debug.Log("HP���}�X�F" + NowMasume + "�}�X��");

        return hp;
    }
    #endregion

    #region �U��
    public void Attack()
    {
        Debug.Log("�U���F" + NowMasume + "�}�X��");
    }
    #endregion

    //#region HP�����擾
    //public int GetHp()
    //{
    //    return Hp;
    //}
    //#endregion

    #region �}�X���Ƃ̍��W���X�g���擾
    public Vector3 GetMasumePos(int masume)
    {
        return MasumeList[masume].position; // �O��������W���A�N�Z�X�\�ɂ���
    }
    #endregion
}
