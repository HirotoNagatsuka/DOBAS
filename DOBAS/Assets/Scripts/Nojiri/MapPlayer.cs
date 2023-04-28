using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    public static MapPlayer ins;
    [SerializeField] GameObject MapManager;
    public int Num;   // �T�C�R���̏o��(1�`6)
    private int start = 0;

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
        transform.position = MapManager.GetComponent<MapManager>().MasumeList[start].position;
    }

    // Update is called once per frame
    void Update()
    {
        DiceStart();
        //Debug.Log("�T�C�R���̖�");
        //Debug.Log(Num);
    }

    public void DiceStart()
    {
        if (Num == 0)
        {
            Num = Random.Range(1, 6);
            start += Num;
            transform.position = MapManager.GetComponent<MapManager>().MasumeList[start].position;
            MapManager.GetComponent<MapManager>().Move();
        }
    }

    public void DiceReset()
    {
        Num = 0;
    }
}
