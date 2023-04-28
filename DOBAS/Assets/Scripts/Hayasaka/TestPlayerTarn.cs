using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerTarn : MonoBehaviour
{
    public bool Tarn; // ���g�̃^�[����ON
    public bool TarnEnd; // ���g�̃^�[���I����ON
    public bool GameEnd;
    public int TestOrder;   // �����Ԋ��蓖��

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
        
        Massge += TestOrder.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Tarn)
        {
            Debug.Log(OrderLog + Massge);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                TarnEnd = true;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Gm.GemeOverJudge();
                TarnEnd = true;
            }
        }   
    }
    public void Test()
    {
        Debug.Log(TestOrder);
    }
}
