using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerTarn : MonoBehaviour
{
    public bool Tarn; // 自身のターン時ON
    public bool TarnEnd; // 自身のターン終了時ON
    public int TestOrder;   // 仮順番割り当て

    string Massge = "のターン";
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
