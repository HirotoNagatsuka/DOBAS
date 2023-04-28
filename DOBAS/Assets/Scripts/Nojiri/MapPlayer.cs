using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    public static MapPlayer ins;
    [SerializeField] GameObject MapManager; // MapManager�Q��
    public int DiceNum;                     // �T�C�R���̏o��(1�`6)
    private int Sum = 0;                    // �o�ڂ̍��v

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
    }

    // Update is called once per frame
    void Update()
    {
        DiceStart();
        //Debug.Log("�T�C�R���̖�");
        //Debug.Log(DiceNum);
    }

    public void DiceStart()
    {
        // �T�C�R����0�̎�
        if (DiceNum == 0)
        {
            DiceNum = Random.Range(1, 6);  // �T�C�R����U��
            Sum += DiceNum;
            transform.position = MapManager.GetComponent<MapManager>().MasumeList[Sum].position; // �}�X�̍��W�ֈړ�
            MapManager.GetComponent<MapManager>().Move();  // MapManager��Move�֐��������s��
        }
    }

    // �^�[�����ɃT�C�R���̏�����
    public void DiceReset()
    {
        DiceNum = 0;
    }
}
