using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    public static MapPlayer ins;            // 参照用
    public int Sum = 0;                     // 出目の合計

    [SerializeField] GameObject MapManager; // MapManager参照
    private int DiceNum;                    // サイコロの出目(1～6)
    private const int Shukai = 20;
    private bool DiceTrigger = true;        // サイコロを振ったかどうか
    private const float Speed = 10.0f;      // マスを進む速度

    public int Card = 0;
    public int Hp = 4;

    // 参照用
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
        //diceManager = GetComponent<DiceManager>();
        //最初のマスに配置
        transform.position = MapManager.GetComponent<MapManager>().MasumeList[Sum].position;

        // コルーチンの開始
        StartCoroutine("Action");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 PlayerPos = transform.position;
        Vector3 TargetPos = MapManager.GetComponent<MapManager>().MasumeList[Sum].position;

        // 移動モーション
        transform.position = Vector3.MoveTowards(PlayerPos, TargetPos, Speed * Time.deltaTime);
    }

    // マスに触れたときタグを参照して効果を呼び出す
    private void OnTriggerStay(Collider collision)
    {
        if (DiceTrigger == false)
        {
            if (collision.gameObject.tag == "Start")
            {
                Debug.Log("Startマス");
            }
            else if (collision.gameObject.tag == "Card")
            {
                Card = MapManager.GetComponent<MapManager>().CardOneUp(Card);  // MapManagerのCardOneUp関数処理を行う
                Debug.Log("カード：" + Card + "枚");
                DiceTrigger = true;
            }
            else if (collision.gameObject.tag == "Move")
            {
                Sum = MapManager.GetComponent<MapManager>().Move(Sum);  // MapManagerのMove関数処理を行う
                Debug.Log("移動完了");
                DiceTrigger = true;
            }
            else if (collision.gameObject.tag == "Hp")
            {
                Hp = MapManager.GetComponent<MapManager>().HpOneUp(Hp);  // MapManagerのHpOneUp関数処理を行う
                Debug.Log("HP：" + Hp);
                DiceTrigger = true;
            }
            else if (collision.gameObject.tag == "Attack")
            {
                MapManager.GetComponent<MapManager>().Attack();  // MapManagerのAttack関数処理を行う
                DiceTrigger = true;
            }
        }
    }

    IEnumerator Action()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);

            // サイコロを振って進む
            //DiceStart();
            yield return new WaitForSeconds(3);

            // サイコロの初期化
            DiceReset();
        }
    }

    IEnumerator Delay(int num)
    {
        // 一マスづつ進ませる
        for (int i = 0; i < num; i++)
        {
            Sum++;  // 現在のマス番号(サイコロ目の合計)

            // スタート地点に戻る
            if (Sum >= Shukai)
            {
                Sum = 0;
            }

            yield return new WaitForSeconds(0.5f); // 0.5秒待つ
        }

        MapManager.GetComponent<MapManager>().Reference();  // MapManagerのReference関数処理を行う
        DiceTrigger = false; // サイコロを振った(ターンの終わり)
    }

    public void DiceStart()
    {
        // サイコロを振っていない時
        if (DiceTrigger == true)
        {
            DiceNum = Random.Range(1, 6);  // サイコロを振る
            Debug.Log("Dice" + DiceNum);
            
            // コルーチンの開始
            StartCoroutine("Delay", DiceNum);
        }
    }

    // ターン事にサイコロの初期化
    public void DiceReset()
    {
        DiceTrigger = true;
    }


    public void StartDelay(int num) {
        // コルーチンの開始
        StartCoroutine("Delay", num);
    }
}
