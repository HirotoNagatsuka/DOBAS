using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    public static MapPlayer ins;            // 参照用
    public int Sum = 0;                     // 出目の合計

    [SerializeField] GameObject MapManager; // MapManager参照
    private int DiceNum;                    // サイコロの出目(1～6)
    private bool DiceTrigger = true;        // サイコロを振ったかどうか
    private float Speed = 3.0f;             // マスを進む速度

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
        //最初のマスに配置
        transform.position = MapManager.GetComponent<MapManager>().MasumeList[Sum].position;

        // コルーチンの開始
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
            yield return new WaitForSeconds(3);

            // サイコロを振って進む
            DiceStart();

            yield return new WaitForSeconds(1);

            // サイコロの初期化
            DiceReset();
        }
    }

    public void DiceStart()
    {
        // サイコロを振っていない時
        if (DiceTrigger == true)
        {
            DiceNum = Random.Range(1, 6);  // サイコロを振る
            //Sum += DiceNum;
            Debug.Log("Dice" + DiceNum);

            // 一マスづつ進ませる
            for (int i = 0; i < DiceNum; i++)
            {
                Sum++;  // 現在のマス番号(サイコロ目の合計)

                // スタート地点に戻る
                if (Sum >= 20)
                {
                    Sum = 0;
                }

                transform.position = MapManager.GetComponent<MapManager>().MasumeList[Sum].position; // マスの座標へ移動
            }

            MapManager.GetComponent<MapManager>().Move();  // MapManagerのMove関数処理を行う
            DiceTrigger = false; // サイコロを振った(ターンの終わり)
        }
    }

    // ターン事にサイコロの初期化
    public void DiceReset()
    {
        DiceTrigger = true;
        //Debug.Log(DiceNum);
    }
}
