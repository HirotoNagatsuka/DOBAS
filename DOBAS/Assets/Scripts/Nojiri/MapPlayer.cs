using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    public static MapPlayer ins;            // �Q�Ɨp
    public int Sum = 0;                     // �o�ڂ̍��v

    [SerializeField] GameObject MapManager; // MapManager�Q��
    private int DiceNum;                    // �T�C�R���̏o��(1�`6)
    private const int Shukai = 20;
    private bool DiceTrigger = true;        // �T�C�R����U�������ǂ���
    private const float Speed = 10.0f;      // �}�X��i�ޑ��x
    private int MoveMasu;                   // Move�}�X�𓥂񂾎��̐i�ރ}�X��

    public int Card = 0;
    public int Hp = 4;

    // �Q�Ɨp
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
        //diceManager = GetComponent<DiceManager>();
        //�ŏ��̃}�X�ɔz�u
        transform.position = MapManager.GetComponent<MapManager>().MasumeList[Sum].position;

        // �R���[�`���̊J�n
        StartCoroutine("Action");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 PlayerPos = transform.position;
        Vector3 TargetPos = MapManager.GetComponent<MapManager>().MasumeList[Sum].position;

        // �ړ����[�V����
        transform.position = Vector3.MoveTowards(PlayerPos, TargetPos, Speed * Time.deltaTime);
    }

    // �}�X�ɐG�ꂽ�Ƃ��^�O���Q�Ƃ��Č��ʂ��Ăяo��
    private void OnTriggerStay(Collider collision)
    {

        /* ������
         * �}�X�Ɏ~�܂������Ɍ��ʂ��P�`�Q�b�Ԃ��炢�\��
         * ���̂��ƂɃ}�X�̌��ʂ𔭓�
         * ���Ԃ�OnTriggerStay�œǂ�ł�̂�����
         */

        if (DiceTrigger == false)
        {
            if (collision.gameObject.tag == "Start")
            {
                Debug.Log("����{�[�i�X�Q�b�g�I");
                StartCoroutine("Dis");
            }
            else if (collision.gameObject.tag == "Card")
            {
                Debug.Log("�J�[�h�P���Q�b�g�I");
                StartCoroutine("Dis");

                //Card = MapManager.GetComponent<MapManager>().CardOneUp(Card);  // MapManager��CardOneUp�֐��������s��
                Debug.Log("�J�[�h�F" + Card + "��");
                DiceTrigger = true;
            }
            else if (collision.gameObject.tag == "Move")
            {
                Debug.Log("3�}�X�i�ށI");
                StartCoroutine("Dis");

                MoveMasu = MapManager.GetComponent<MapManager>().Move(MoveMasu);  // MapManager��Move�֐��������s��

                // �P�}�X�Âi��
                StartCoroutine("Delay", MoveMasu);

                Debug.Log("�ړ�����");
                MoveMasu = 0;
                DiceTrigger = true;
            }
            else if (collision.gameObject.tag == "Hp")
            {
                Debug.Log("HP�P�񕜁I�I");
                StartCoroutine("Dis");

                Hp = MapManager.GetComponent<MapManager>().HpOneUp(Hp);  // MapManager��HpOneUp�֐��������s��
                Debug.Log("HP�F" + Hp);
                DiceTrigger = true;
            }
            else if (collision.gameObject.tag == "Attack")
            {
                Debug.Log("���̃v���C���[���U���I");
                StartCoroutine("Dis");

                MapManager.GetComponent<MapManager>().Attack();  // MapManager��Attack�֐��������s��
                DiceTrigger = true;
            }
            else
            {
                Debug.Log("���ʂ̃}�X");
            }
        }
    }

    IEnumerator Dis()
    {
        Debug.Log("�R���[�`������");
        yield return new WaitForSeconds(2);
        Card = MapManager.GetComponent<MapManager>().CardOneUp(Card);  // MapManager��CardOneUp�֐��������s��
    }

    IEnumerator Action()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);

            // �T�C�R����U���Đi��
            //DiceStart();
            yield return new WaitForSeconds(3);

            // �T�C�R���̏�����
            DiceReset();
        }
    }

    IEnumerator Delay(int num)
    {
        // �P�}�X�Âi�܂���
        for (int i = 0; i < num; i++)
        {
            Sum++;  // ���݂̃}�X�ԍ�(�T�C�R���ڂ̍��v)

            // �X�^�[�g�n�_�ɖ߂�
            if (Sum >= Shukai)
            {
                Sum = 0;
            }

            yield return new WaitForSeconds(0.5f); // 0.5�b�҂�
        }

        MapManager.GetComponent<MapManager>().Reference();  // MapManager��Reference�֐��������s��
        DiceTrigger = false; // �T�C�R����U����(�^�[���̏I���)
    }

    public void DiceStart()
    {
        // �T�C�R����U���Ă��Ȃ���
        if (DiceTrigger == true)
        {
            DiceNum = Random.Range(1, 6);  // �T�C�R����U��
            Debug.Log("Dice" + DiceNum);
            
            // �R���[�`���̊J�n
            StartCoroutine("Delay", DiceNum);
        }
    }

    // �^�[�����ɃT�C�R���̏�����
    public void DiceReset()
    {
        DiceTrigger = true;
    }


    public void StartDelay(int num) {
        // �R���[�`���̊J�n
        StartCoroutine("Delay", num);
    }
}
