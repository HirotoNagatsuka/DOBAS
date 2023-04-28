using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    public static MapPlayer ins;
    [SerializeField] GameObject MapManager; // MapManager参照
    private int DiceNum;                     // サイコロの出目(1～6)
    public int Sum = 0;                    // 出目の合計
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
            // 3秒待つ
            yield return new WaitForSeconds(3);

            DiceStart();

            yield return new WaitForSeconds(1);

            DiceReset();
        }
    }

    public void DiceStart()
    {
        // サイコロが0の時
        if (DiceTrigger == true)
        {
            DiceNum = Random.Range(1, 6);  // サイコロを振る
            Sum += DiceNum;
            transform.position = MapManager.GetComponent<MapManager>().MasumeList[Sum].position; // マスの座標へ移動
            MapManager.GetComponent<MapManager>().Move();  // MapManagerのMove関数処理を行う
            DiceTrigger = false;
        }
    }

    // ターン事にサイコロの初期化
    public void DiceReset()
    {
        DiceTrigger = true;
        //Debug.Log(DiceNum);
    }
}
