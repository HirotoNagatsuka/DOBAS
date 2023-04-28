using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    public static MapPlayer ins;
    [SerializeField] GameObject MapManager; // MapManager参照
    public int DiceNum;                     // サイコロの出目(1～6)
    private int Sum = 0;                    // 出目の合計

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
    }

    // Update is called once per frame
    void Update()
    {
        DiceStart();
        //Debug.Log("サイコロの目");
        //Debug.Log(DiceNum);
    }

    public void DiceStart()
    {
        // サイコロが0の時
        if (DiceNum == 0)
        {
            DiceNum = Random.Range(1, 6);  // サイコロを振る
            Sum += DiceNum;
            transform.position = MapManager.GetComponent<MapManager>().MasumeList[Sum].position; // マスの座標へ移動
            MapManager.GetComponent<MapManager>().Move();  // MapManagerのMove関数処理を行う
        }
    }

    // ターン事にサイコロの初期化
    public void DiceReset()
    {
        DiceNum = 0;
    }
}
