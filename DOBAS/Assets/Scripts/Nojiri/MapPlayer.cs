using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    public static MapPlayer ins;
    [SerializeField] GameObject MapManager; // MapManager�Q��
    private int DiceNum;                     // �T�C�R���̏o��(1�`6)
    public int Sum = 0;                    // �o�ڂ̍��v
    private bool DiceTrigger = true;

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
            // 3�b�҂�
            yield return new WaitForSeconds(3);

            DiceStart();

            yield return new WaitForSeconds(1);

            DiceReset();
        }
    }

    public void DiceStart()
    {
        // �T�C�R����0�̎�
        if (DiceTrigger == true)
        {
            DiceNum = Random.Range(1, 6);  // �T�C�R����U��
            Sum += DiceNum;
            transform.position = MapManager.GetComponent<MapManager>().MasumeList[Sum].position; // �}�X�̍��W�ֈړ�
            MapManager.GetComponent<MapManager>().Move();  // MapManager��Move�֐��������s��
            DiceTrigger = false;
        }
    }

    // �^�[�����ɃT�C�R���̏�����
    public void DiceReset()
    {
        DiceTrigger = true;
        //Debug.Log(DiceNum);
    }
}
