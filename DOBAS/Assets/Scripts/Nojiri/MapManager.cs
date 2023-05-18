using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public List<Transform> MasumeList;        // �}�X�̔z��

    //[SerializeField] GameObject MapPlayer;    // MapManager�Q��
    [SerializeField] private int NowMasume;   // ���ݓ���ł���}�X�̒l

    // Start is called before the first frame update
    void Start()
    {
        NowMasume = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region �v���C���[�ɏՓ˂��Ă���I�u�W�F�N�g�̃^�O���擾
    public void ColliderReference(Collider plyCollision)
    {
        string Now = plyCollision.tag; // �^�O���擾

        Debug.Log("NowTag�F" + Now);

        NowMasume = MapPlayer.ins.Sum; // �����\��
    }
    #endregion

    #region �J�[�h�𑝂₷
    public int CardOneUp(int card)
    {
        card++;
        Debug.Log("�J�[�h�}�X�F" + NowMasume + "�}�X��");
        Debug.Log("�J�[�h�F" + card + "��");

        return card;
    }
    #endregion


    #region �}�X�̈ړ�
    public int Move(int move, string tag)
    {
        if (tag == "Move")
        {
            move += 3;
            Debug.Log("3�}�X�ړ��F" + NowMasume + "�}�X��");
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

    //#region �}�X���Ƃ̍��W���X�g���擾
    //public Vector3 GetMasumePos(int masume)
    //{
    //    return MasumeList[masume].position; // �O��������W���A�N�Z�X�\�ɂ���
    //}
    //#endregion
}
