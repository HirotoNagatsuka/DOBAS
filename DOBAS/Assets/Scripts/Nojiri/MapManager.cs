using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public List<Transform> MasumeList;            // �}�X�̔z��
    [SerializeField]private int nowMasume = 0;  �@// ���ݓ���ł���}�X�̒l

    public static int MoveSqueare; // �����}�X��
    public int Hp;                 // �̗�
    private bool YourTurn = false; // �����̃^�[��


    //void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // �}�X�̈ړ�
    //void Move()
    //{
    //    //if(nowSqueare <= Squeares.Length -1)
    //    //{
    //    //    return;
    //    //}
    //}

    #region HP�𑝂₷
    public void HpOneUp()
    {
        Hp++;
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
