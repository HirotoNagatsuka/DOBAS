using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;            // �Q�Ɨp
    public List<Transform> MasumeList;            // �}�X�̔z��
    //public int ListNum;    // �}�X�z��̔ԍ�
    public int Hp;         // �̗�

    [SerializeField] private int NowMasume = 0;   // ���ݓ���ł���}�X�̒l
    [SerializeField] private int saikoro;   // �T�C�R���̏o��(1�`4)
    private bool i = true; 

    // �Q�Ɨp
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
        //�����ʒu
        //transform.position = MasumeList[NowMasume].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (i == true)
        {
            NowMasume += saikoro; //���݂̃}�X
            Activate();
            i = false;
        }
    }

    //�}�X�̌��ʐU�蕪��
    void Activate()
    {
        switch(NowMasume)
        {
            case 0:
                // �X�^�[�gor����
                Debug.Log(NowMasume);
                break;
            case 1:
                // �J�[�h�}�X
                Debug.Log(NowMasume);
                break;
            case 2:
                // �Z�}�X�ړ�
                Debug.Log(NowMasume);
                break;
            case 3:
                // HP������
                HpOneUp();
                Debug.Log(NowMasume);
                Debug.Log(Hp);
                break;
            case 4:
                // �U���}�X
                Debug.Log(NowMasume);
                break;
            case 5:
                // ���ʂȂ�
                Debug.Log(NowMasume);
                break;
            default:
                break;
        }
    }

    //�}�X�̈ړ�
    void Move()
    {
        transform.position = MasumeList[NowMasume].position;
    }

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
        NowMasume = masume;
        return MasumeList[masume].position; // �O��������W���A�N�Z�X�\�ɂ���
    }
    #endregion
}
