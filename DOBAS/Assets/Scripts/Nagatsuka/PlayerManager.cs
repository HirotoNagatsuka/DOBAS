using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;

[Serializable]
class Player
{
    public string Name; //名前を格納.
    public int MaxHP;   //HPの最大値;
    public int HP;      //HPを格納.
    public int Attack;  //攻撃力を格納.
    public Sprite[] HeartSprites;//HP用画像の配列.
}

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region 定数宣言
    //UI表示に使用する定数値
    const int PLAYER_UI = 0;
    const int NAME_UI = 0;
    const int HP_UI = 1;

    const float HEART_POS_X = -200;//初期値.
    const float HEART_POS_Y = 280f;//HP画像の表示位置.

    //野尻の方で宣言した定数
    private const int SHUKAI = 26;//マスの数（Playerで宣言しているので後々変更）.
    private const float MOVE_SPEED = 10.0f;      // マスを進む速度.
    #endregion

    #region [SerializeField]宣言
    [Header("[SerializeField]宣言")]
    [SerializeField]
    DiceManager diceManager;//DiceManager参照用.
    MapManager mapManager;
    [SerializeField]
    Player Player;//Playerのクラスをインスペクター上で見れるようにする.
    #endregion

    private int MoveMasu;                   // Moveマスを踏んだ時の進むマス数
    private bool ActionFlg = true;        // サイコロを振ったかどうか

    GameObject PlayerUI;//子供のキャンバスを取得するための変数宣言.

    public static PlayerManager ins;      // 参照用 
    public int Sum = 0;                     // 出目の合計
    private bool DiceTrigger = true;        // サイコロを振ったかどうか

    public int Card = 0;//多分使わない(カード枚数だけの表示).

    private int deme;

    #region Unityイベント(Awake・Start・Update・OnTrigger)

    public void Awake()
    {
    //    if (ins == null)
    //    {
    //        ins = this;
    //    }
    }
    private void Start()
    {
        PlayerUI = gameObject.transform.GetChild(PLAYER_UI).gameObject;//子供のキャンバスを取得.
        //PlayerUI.gameObject.transform.GetChild(NAME_UI).GetComponent<Text>().text = Player.Name.ToString();//名前の表示.
        //PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Text>().text = Player.HP.ToString();//HPの表示.
        PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.HP -1];//HPの表示.

        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        diceManager = GameObject.Find("DiceManager").GetComponent<DiceManager>();
        //diceManager = GetComponent<DiceManager>();
        //最初のマスに配置.
        transform.position = mapManager.MasumeList[0].position;//初期値0.
        Player.MaxHP = 5;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            Vector3 PlayerPos = transform.position;
            Vector3 TargetPos = mapManager.MasumeList[Sum].position;

            //// 移動モーション
            transform.position = Vector3.MoveTowards(PlayerPos, TargetPos, MOVE_SPEED * Time.deltaTime);
            if (diceManager.FinishFlg) FinishDice();
            if (Player.HP <= 0){
                PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = null;
            }
            else if(Player.HP > Player.MaxHP)
            {
                PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.MaxHP -1];
            }
            else
            {
                PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Image>().sprite = Player.HeartSprites[Player.HP -1];
            }
        }
    }

    /// <summary>
    /// マスに触れたときタグを参照して効果を呼び出す
    /// </summary>
    private void OnTriggerStay(Collider collision)
    {
        string NowTag = collision.tag; // タグを取得
        // 行動終了時、マスの効果発動
        if (ActionFlg == false)
        {
            GetComponent<Message>().ShowText(NowTag); //止まっているマス効果をテキストに表示(追記)
            mapManager.GetComponent<MapManager>().ColliderReference(collision);  // MapManagerのColliderReference関数処理を行う
            StartCoroutine("Activation", NowTag);  // Activationコルーチンを実行
        }
    }

    #endregion



    IEnumerator DelayMove(int num)
    {
        // １マスづつ進ませる
        for (int i = 0; i < num; i++)
        {
            Sum++;  // 現在のマス番号(サイコロ目の合計)

            // スタート地点に戻る
            if (Sum >= SHUKAI)
            {
                Sum = 0;
            }

            yield return new WaitForSeconds(0.5f); // 0.5秒待つ
        }
        ActionFlg = false; // プレイヤーの行動終了(マス効果発動前)
    }


    /// <summary>
    /// サイコロを振り終わったら呼び出す関数.
    /// </summary>
    public void FinishDice()
    {
        deme = diceManager.DeclarationNum;
        if (diceManager.DeclarationNum == 4)
        {
            photonView.RPC(nameof(EnemyAttack), RpcTarget.All);
            //EnemyAttack();
        }
        else
        {
            StartDelay(deme);
        }
        diceManager.FinishFlg = false;
    }

    [PunRPC]
    void EnemyAttack()
    {
        Player.HP--;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 自身のアバターのスタミナを送信する
            stream.SendNext(Player.HP);
        }
        else
        {
            // 他プレイヤーのアバターのスタミナを受信する
            Player.HP = (int)stream.ReceiveNext();
        }
    }


    /// <summary>
    /// 1マスずつ進ませるコルーチン
    /// numに出目を入れてその分進ませる
    /// </summary>
    IEnumerator Delay(int num)
    {
        // 一マスづつ進ませる
        for (int i = 0; i < num; i++)
        {
            Sum++;  // 現在のマス番号(サイコロ目の合計)

            // スタート地点に戻る
            if (Sum >= SHUKAI)
            {
                Sum = 0;
            }
            yield return new WaitForSeconds(0.5f); // 0.5秒待つ
        }

        //mapManager.Reference();  // MapManagerのReference関数処理を行う
        DiceTrigger = false; // サイコロを振った(ターンの終わり)
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

            Player.HP = mapManager.GetComponent<MapManager>().HpOneUp(Player.HP);  // MapManagerのHpOneUp関数処理を行う
            Debug.Log("HP：" + Player.HP);
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


    public void StartDelay(int num)
    {
        Debug.Log("StartDeley起動");
        // コルーチンの開始
        StartCoroutine("DelayMove", num);
    }


    /// <summary>
    /// HPが変化するときに呼び出す関数
    /// 変化量を引数にし、HPを変えた後UIにも反映する.
    /// </summary>
    public void ChangeHP(int addHP)
    {
        if(Player.HP == 0)//HPが0になったら.
        {
            Player.HP = 0;
            Debug.Log("ゲームオーバー");
        }
        else Player.HP += addHP;//0でないなら変化させる.
        PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Text>().text = Player.HP.ToString();//HPの表示.
    }

    public void PushBelieveButton()
    {
        Debug.Log("信じる!");
        diceManager.DiceInit();
    }
    public void PushDoubtButton()
    {
        Debug.Log("ダウト!");
        diceManager.DiceInit();
    }

    #region 野尻のソースで必要かわからないもの
    IEnumerator Action()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);

            // サイコロを振って進む

            yield return new WaitForSeconds(3);

            // サイコロの初期化
            DiceReset();
        }
    }

    // ターン事にサイコロの初期化
    public void DiceReset()
    {
        DiceTrigger = true;
    }
    #endregion
}