using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerTarn : MonoBehaviour
{
    public bool Tarn; // ���g�̃^�[����ON
    public bool TarnEnd; // ���g�̃^�[���I����ON
    public int TestOrder;   // �����Ԋ��蓖��

    string Massge = "�̃^�[��";
    string OrderLog;
    // Start is called before the first frame update
    void Start()
    {
        Tarn = false;
        TarnEnd = false;
        Massge+= TestOrder.ToString();
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
        }   
    }
    public void Test()
    {
        Debug.Log(TestOrder);
    }
}
