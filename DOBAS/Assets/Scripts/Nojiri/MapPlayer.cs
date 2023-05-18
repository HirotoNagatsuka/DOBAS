using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    public static MapPlayer ins;            // 参照用
    public int Sum = 0;                     // 出目の合計

    [SerializeField] GameObject mapManager; // MapManager参照
    private const int Shukai = 20;
    private const float Speed = 10.0f;      // マスを進む速度
    private bool ActionFlg = true;        // サイコロを振ったかどうか
    private int MoveMasu;                   // Moveマスを踏んだ時の進むマス数

    public int Card = 0;
    public int Hp = 4;

    // 後で改良予定
    // MapManagerからの参照用
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
        transform.position = mapManager.GetComponent<MapManager>().MasumeList[Sum].position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 PlayerPos = transform.position;
        Vector3 TargetPos = mapManager.GetComponent<MapManager>().MasumeList[Sum].position;

        // 移動モーション
        transform.position = Vector3.MoveTowards(PlayerPos, TargetPos, Speed * Time.deltaTime);
    }

    IEnumerator DelayMove(int num)
    {
        // １マスづつ進ませる
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
        ActionFlg = false; // プレイヤーの行動終了(マス効果発動前)
    }

    // 取得したタグごとに効果を発動
    IEnumerator Activation(string tag)
    {
        ActionFlg = true;

        // タグごとに分類
        if (tag == "Start") // スタートマス
        {
            Debug.Log("周回ボーナスゲット！");
            yield return new WaitForSeconds(2); // 2秒待つ
        }
        else if (tag == "Card") // カードマス
        {
            Debug.Log("カード１枚ゲット！");
            yield return new WaitForSeconds(2);

            Card = mapManager.GetComponent<MapManager>().CardOneUp(Card);  // MapManagerのCardOneUp関数処理を行う
        }
        else if (tag == "Move") // 移動マス
        {
            Debug.Log("3マス進む！");
            yield return new WaitForSeconds(2);

            MoveMasu = mapManager.GetComponent<MapManager>().Move(MoveMasu, tag);  // MapManagerのMove関数処理を行う
            StartCoroutine("DelayMove", MoveMasu);                            // １マスづつ進む
        }
        else if (tag == "Hp") // HPマス
        {
            Debug.Log("HP１回復！！");
            yield return new WaitForSeconds(2);

            Hp = mapManager.GetComponent<MapManager>().HpOneUp(Hp);  // MapManagerのHpOneUp関数処理を行う
            Debug.Log("HP：" + Hp);
        }
        else if (tag == "Attack") //攻撃マス
        {
            Debug.Log("他のプレイヤーを攻撃！");
            yield return new WaitForSeconds(2);

            mapManager.GetComponent<MapManager>().Attack();  // MapManagerのAttack関数処理を行う
        }
        else // ノーマルマス
        {
            Debug.Log("普通のマス");
            yield return new WaitForSeconds(2);
        }
    }

    // ターンが終わった時に触れているオブジェクトのタグを調べる
    private void OnTriggerStay(Collider collision)
    {
        string NowTag = collision.tag; // タグを取得

        // 行動終了時、マスの効果発動
        if (ActionFlg == false)
        {
            mapManager.GetComponent<MapManager>().ColliderReference(collision);  // MapManagerのColliderReference関数処理を行う
            StartCoroutine("Activation", NowTag);  // Activationコルーチンを実行
        }
    }

    // ターン終了時にサイコロの初期化
    public void DiceReset()
    {
        ActionFlg = true;
    }


    public void StartDelay(int num){
        // コルーチンの開始
        StartCoroutine("DelayMove", num);
    }
}
