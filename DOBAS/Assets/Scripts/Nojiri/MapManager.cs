using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;        // 参照用
    public List<Transform> MasumeList;        // マスの配列

    private int Hp = 4;    // 体力
    private int Card;

    [SerializeField] private int NowMasume;   // 現在踏んでいるマスの値
    private int Saikoro;                      // サイコロの出目(1～6)
    private bool MyTurn = true;

    // 参照用
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        NowMasume = 0;

        //初期位置
        //transform.position = MasumeList[NowMasume].position;
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

    //マスの効果振り分け
    public void Activate()
    {
        switch(NowMasume)
        {
            case 0:
                // スタートor周回
                Debug.Log("周回" + NowMasume);
                break;
            case 1:
                // カードマス
                CardOneUp();
                break;
            case 2:
                // 〇マス移動
                Debug.Log("移動" + NowMasume);
                break;
            case 3:
                // HP増える
                HpOneUp();
                break;
            case 4:
                // 攻撃マス
                Attack();
                break;
            case 5:
                // 効果なし
                Debug.Log("効果なし" + NowMasume);
                break;
            default:
                break;
        }
    }

    #region マスの移動
    public void Move()
    {
        Saikoro = MapPlayer.ins.Num;
        NowMasume += Saikoro;
        transform.position = MasumeList[NowMasume].position;
        Activate();
    }
    #endregion

    #region HPを増やす
    public void HpOneUp()
    {
        Hp++;

        Debug.Log("HP増マス" + NowMasume);
        Debug.Log("HP" + Hp);
    }
    #endregion

    #region カードを増やす
    public void CardOneUp()
    {
        Card++;

        Debug.Log("カードマス" + NowMasume);
        Debug.Log(Card + "枚");
    }
    #endregion

    #region 攻撃
    public void Attack()
    {
        Debug.Log("攻撃" + NowMasume);
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
        NowMasume = masume;
        return MasumeList[masume].position; // 外部から座標をアクセス可能にする
    }
    #endregion
}
