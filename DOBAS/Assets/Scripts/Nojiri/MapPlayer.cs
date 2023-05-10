using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    public static MapPlayer ins;            // �Q�Ɨp
    public int Sum = 0;                     // �o�ڂ̍��v

    [SerializeField] GameObject MapManager; // MapManager�Q��
    private int DiceNum;                    // �T�C�R���̏o��(1�`6)
    private bool DiceTrigger = true;        // �T�C�R����U�������ǂ���
    private float Speed = 3.0f;             // �}�X��i�ޑ��x

    public void Awake()
    {
        if(ins == null)
        {
            ins = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //�ŏ��̃}�X�ɔz�u
        transform.position = MapManager.GetComponent<MapManager>().MasumeList[Sum].position;

        // �R���[�`���̊J�n
        StartCoroutine("Delay");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Delay()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);

            // �T�C�R����U���Đi��
            DiceStart();

            yield return new WaitForSeconds(1);

            // �T�C�R���̏�����
            DiceReset();
        }
    }

    public void DiceStart()
    {
        // �T�C�R����U���Ă��Ȃ���
        if (DiceTrigger == true)
        {
            DiceNum = Random.Range(1, 6);  // �T�C�R����U��
            //Sum += DiceNum;
            Debug.Log("Dice" + DiceNum);

            // ��}�X�Âi�܂���
            for (int i = 0; i < DiceNum; i++)
            {
                Sum++;  // ���݂̃}�X�ԍ�(�T�C�R���ڂ̍��v)

                // �X�^�[�g�n�_�ɖ߂�
                if (Sum >= 20)
                {
                    Sum = 0;
                }

                transform.position = MapManager.GetComponent<MapManager>().MasumeList[Sum].position; // �}�X�̍��W�ֈړ�
            }

            MapManager.GetComponent<MapManager>().Move();  // MapManager��Move�֐��������s��
            DiceTrigger = false; // �T�C�R����U����(�^�[���̏I���)
        }
    }

    // �^�[�����ɃT�C�R���̏�����
    public void DiceReset()
    {
        DiceTrigger = true;
        //Debug.Log(DiceNum);
    }
}
