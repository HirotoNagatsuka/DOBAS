using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public List<Transform> MasumeList;   // �}�X�̔z��
    //[SerializeField]private int nowSqueare = 0;  �@// ���ݓ���ł���}�X�̒l

    public static int moveSqueare; // �����}�X��
    public int hp;                 // �̗�
    private bool yourTurn = false; // �����̃^�[��


    // Start is called before the first frame update
    void Start()
    {
        //�ŏ��̃}�X�ɔz�u
        transform.position = MasumeList[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // �}�X�̈ړ�
    void Move()
    {
        //if(nowSqueare <= Squeares.Length -1)
        //{
        //    return;
        //}
    }

    // HP�̑���
    //void HpInDecrease()
    //{

    //}

    #region �}�X�̈ړ�
    public Vector3 MasumePos(int masume)
    {
        return MasumeList[masume].position; // �O��������W���A�N�Z�X�\�ɂ���
    }
    #endregion
}
