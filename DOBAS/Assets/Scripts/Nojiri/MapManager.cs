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

    #region �v���C���[���ŏՓ˂��Ă���I�u�W�F�N�g�̃^�O���擾
    public void ColliderReference(Collider plyCollision)
    {
        string Now = plyCollision.tag; // �^�O���擾

        Debug.Log("NowTag�F" + Now);
    }
    #endregion

    #region �J�[�h�𑝂₷
    public int CardOneUp(int card)
    {
        card++;
        Debug.Log("�J�[�h�F" + card + "��");

        return card;
    }
    #endregion


    #region �}�X�̈ړ�
    public int Move(int move, string tag)
    {
        if (tag == "Move")
        {
            move = 3;
        }

        return move;
    }
    #endregion

    #region HP�𑝌�
    public int HpOneUp(int hp)
    {
        hp++;

        return hp;
    }
    #endregion

    #region �U��(��)
    public void Attack()
    {

    }
    #endregion

    //���������g���\��
    //#region �U��
    //public int Attack(int hp)
    //{
    //    hp--;

    //    return hp;
    //}
    //#endregion

    //#region �}�X���Ƃ̍��W���X�g���擾
    //public Vector3 GetMasumePos(int masume)
    //{
    //    return MasumeList[masume].position; // �O��������W���A�N�Z�X�\�ɂ���
    //}
    //#endregion
}
