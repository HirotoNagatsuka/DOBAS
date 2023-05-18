using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    public static MapPlayer ins;            // �Q�Ɨp
    public int Sum = 0;                     // �o�ڂ̍��v

    [SerializeField] GameObject mapManager; // MapManager�Q��
    private const int Shukai = 20;
    private const float Speed = 10.0f;      // �}�X��i�ޑ��x
    private bool ActionFlg = true;        // �T�C�R����U�������ǂ���
    private int MoveMasu;                   // Move�}�X�𓥂񂾎��̐i�ރ}�X��

    public int Card = 0;
    public int Hp = 4;

    // ��ŉ��Ǘ\��
    // MapManager����̎Q�Ɨp
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
        transform.position = mapManager.GetComponent<MapManager>().MasumeList[Sum].position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 PlayerPos = transform.position;
        Vector3 TargetPos = mapManager.GetComponent<MapManager>().MasumeList[Sum].position;

        // �ړ����[�V����
        transform.position = Vector3.MoveTowards(PlayerPos, TargetPos, Speed * Time.deltaTime);
    }

    IEnumerator DelayMove(int num)
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
        ActionFlg = false; // �v���C���[�̍s���I��(�}�X���ʔ����O)
    }

    // �擾�����^�O���ƂɌ��ʂ𔭓�
    IEnumerator Activation(string tag)
    {
        ActionFlg = true;

        // �^�O���Ƃɕ���
        if (tag == "Start") // �X�^�[�g�}�X
        {
            Debug.Log("����{�[�i�X�Q�b�g�I");
            yield return new WaitForSeconds(2); // 2�b�҂�
        }
        else if (tag == "Card") // �J�[�h�}�X
        {
            Debug.Log("�J�[�h�P���Q�b�g�I");
            yield return new WaitForSeconds(2);

            Card = mapManager.GetComponent<MapManager>().CardOneUp(Card);  // MapManager��CardOneUp�֐��������s��
        }
        else if (tag == "Move") // �ړ��}�X
        {
            Debug.Log("3�}�X�i�ށI");
            yield return new WaitForSeconds(2);

            MoveMasu = mapManager.GetComponent<MapManager>().Move(MoveMasu, tag);  // MapManager��Move�֐��������s��
            StartCoroutine("DelayMove", MoveMasu);                            // �P�}�X�Âi��
        }
        else if (tag == "Hp") // HP�}�X
        {
            Debug.Log("HP�P�񕜁I�I");
            yield return new WaitForSeconds(2);

            Hp = mapManager.GetComponent<MapManager>().HpOneUp(Hp);  // MapManager��HpOneUp�֐��������s��
            Debug.Log("HP�F" + Hp);
        }
        else if (tag == "Attack") //�U���}�X
        {
            Debug.Log("���̃v���C���[���U���I");
            yield return new WaitForSeconds(2);

            mapManager.GetComponent<MapManager>().Attack();  // MapManager��Attack�֐��������s��
        }
        else // �m�[�}���}�X
        {
            Debug.Log("���ʂ̃}�X");
            yield return new WaitForSeconds(2);
        }
    }

    // �^�[�����I��������ɐG��Ă���I�u�W�F�N�g�̃^�O�𒲂ׂ�
    private void OnTriggerStay(Collider collision)
    {
        string NowTag = collision.tag; // �^�O���擾

        // �s���I�����A�}�X�̌��ʔ���
        if (ActionFlg == false)
        {
            mapManager.GetComponent<MapManager>().ColliderReference(collision);  // MapManager��ColliderReference�֐��������s��
            StartCoroutine("Activation", NowTag);  // Activation�R���[�`�������s
        }
    }

    // �^�[���I�����ɃT�C�R���̏�����
    public void DiceReset()
    {
        ActionFlg = true;
    }


    public void StartDelay(int num){
        // �R���[�`���̊J�n
        StartCoroutine("DelayMove", num);
    }
}
