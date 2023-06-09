using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerTarn : MonoBehaviour
{
    public bool Tarn;    // 自身のターン時ON
    public bool TarnEnd; // 自身のターン終了時ON
    public bool GameEnd;
    public bool DoutFlg;
    public bool DoutDec; // Decは指摘的な意味
    public bool MyDec;
    public int  Saikoro;
    public int TestOrder;   // 仮順番割り当て

    public float Count = 5.0f;//ダウト宣言時間.

    [SerializeField] GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        Tarn = false;
        TarnEnd = false;
        DoutFlg = false;
        DoutDec = false;
        MyDec = false;
        Saikoro = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Tarn)
        {
            //Debug.Log(OrderLog + Massge);
            // ダウと目出すか(仮)
            if (Input.GetKeyDown(KeyCode.R))
            {
                Saikoro = Random.Range(2, 7);
                Debug.Log(Saikoro);
            }            
            // 才衣目送信？
            if (Input.GetKeyDown(KeyCode.F) && !TarnEnd)
            {
               // gameManager.DoutDis(Saikoro);
            }
            // ターン終了
            if (Input.GetKeyDown(KeyCode.Return)　&& TarnEnd)
            {
                TarnEnd = false;
                Tarn = false;
                Saikoro = 0;
                DoutFlg = false;
                //gameManager.EndJudge(true);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
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
    //ダウト指摘(仮)
    public void DoutNoted()
    {
        Count -= Time.deltaTime;
        if (Count >= 0.0f)
        {
            if (Input.GetKeyDown(KeyCode.P) && DoutDec)
            {
                MyDec = true; // 嘘ついてるだろ宣言
               // gameManager.DoutJudge();
                DoutDec = false;
            }
        }
        if (Count <= 0.0f)
        {
            //gameManager.DoutJudge();
        }
    }
}
