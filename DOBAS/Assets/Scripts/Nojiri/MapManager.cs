using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //public static MapManager instance;        // 参照用
    public List<Transform> MasumeList;        // マスの配列

    //[SerializeField] GameObject MapPlayer;    // MapManager参照
    [SerializeField] private int NowMasume;   // 現在踏んでいるマスの値

    private int Hp = 4;          // 体力
    private int Card;            // カード枚数
    private int Saikoro;         // サイコロの出目(1～6)
    private bool MyTurn = true;  // 自分のターン

    //// 参照用
    //public void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
        NowMasume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (MyTurn == true)
        //{
        //    //NowMasume += Saikoro; //現在のマス
        //    Activate();
        //    MyTurn = false;
        //}
    }

    # region マスの効果振り分け
    public void Activate()
    {
        switch(NowMasume)
        {
            case 0:
                // スタートor周回
                Debug.Log("周回：" + NowMasume + "マス目");
                break;
            case 1:
            case 6:
                // カードマス
                CardOneUp();
                break;
            case 2:
            case 9:
                // 〇マス移動
                Debug.Log("移動：" + NowMasume + "マス目");
                break;
            case 3:
            case 7:
                // HP増える
                HpOneUp();
                break;
            case 4:
            case 10:
                // 攻撃マス
                Attack();
                break;
            case 5:
                // 効果なし
                Debug.Log("効果なし：" + NowMasume + "マス目");
                break;
            case 8:
                // HP減る
                Debug.Log("HP減る：" + NowMasume + "マス目");
                break;
            default:
                Debug.Log(NowMasume + "マス目");
                break;
        }
    }
    #endregion

    #region マスの移動
    public void Move()
    {
        //Saikoro = MapPlayer.GetComponent<MapPlayer>().DiceNum;
        NowMasume = MapPlayer.ins.Sum;
        Activate();
    }
    #endregion

    #region HPを増やす
    public void HpOneUp()
    {
        Hp++;

        Debug.Log("HP増マス：" + NowMasume + "マス目");
        Debug.Log("HP" + Hp);
    }
    #endregion

    #region カードを増やす
    public void CardOneUp()
    {
        Card++;

        Debug.Log("カードマス：" + NowMasume + "マス目");
        Debug.Log(Card + "枚");
    }
    #endregion

    #region 攻撃
    public void Attack()
    {
        Debug.Log("攻撃：" + NowMasume + "マス目");
    }
    #endregion

    #region HP情報を取得
    public int GetHp()
    {
        return Hp;
    }
    #endregion

    #region マスごとの座標リストを取得
    public Vector3 GetMasumePos(int masume)
    {
        return MasumeList[masume].position; // 外部から座標をアクセス可能にする
    }
    #endregion
}
