using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerTarn : MonoBehaviour
{
    public bool Tarn;    // ���g�̃^�[����ON
    public bool TarnEnd; // ���g�̃^�[���I����ON
    public bool GameEnd;
    public bool DoutFlg;
    public bool DoutDec; // Dec�͎w�E�I�ȈӖ�
    public bool MyDec;
    public int  Saikoro;
    public int TestOrder;   // �����Ԋ��蓖��

    public float Count = 5.0f;
    string Massge = "�̃^�[��";
    string OrderLog;
    public GameObject GmObj;
    GameManager Gm;
    // Start is called before the first frame update
    void Start()
    {
        Gm = GmObj.GetComponent<GameManager>();
        Tarn = false;
        TarnEnd = false;
        DoutFlg = false;
        DoutDec = false;
        MyDec = false;
        Saikoro = 0;
        Massge += TestOrder.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Tarn)
        {
            Debug.Log(OrderLog + Massge);
            // �_�E�Ɩڏo����(��)
            if (Input.GetKeyDown(KeyCode.R))
            {
                Saikoro = Random.Range(2, 7);
                Debug.Log(Saikoro);
            }            
            // �ˈߖڑ��M�H
            if (Input.GetKeyDown(KeyCode.F) && !TarnEnd)
            {
                Gm.DoutDis(Saikoro);
            }
            // �^�[���I��
            if (Input.GetKeyDown(KeyCode.Return)�@&& TarnEnd)
            {
                TarnEnd = false;
                Tarn = false;
                Saikoro = 0;
                DoutFlg = false;
                Gm.EndJudge(true);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Gm.GemeOverJudge();
                TarnEnd = true;
            }
        }
        if (DoutDec)
        {
            DoutNoted();
        }
        else
        {
            MyDec = false;
        }
    }
    //�_�E�g�w�E(��)
    public void DoutNoted()
    {
        Count -= Time.deltaTime;
        if (Count >= 0.0f)
        {
            if (Input.GetKeyDown(KeyCode.P) && DoutDec)
            {
                MyDec = true; // �R���Ă邾��錾
                Gm.DoutJudge();
                DoutDec = false;
            }
        }
        if (Count <= 0.0f)
        {
            Gm.DoutJudge();
        }
    }
    public void Test()
    {
        Debug.Log(TestOrder);
    }
}
