using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //public static MapManager instance;        // 参照用
    public List<Transform> MasumeList;        // マスの配列

    //[SerializeField] GameObject MapPlayer;    // MapManager参照
    [SerializeField] private int NowMasume;   // 現在踏んでいるマスの値

    //private int Hp = 4;            // 体力
    //private int Card;              // カード枚数

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

    }

    //# region マスの効果振り分け
    //public void Activate()
    //{
    //    switch(NowMasume)
    //    {
    //        case 0:
    //            // スタートor周回
    //            Debug.Log("周回：" + NowMasume + "マス目");
    //            break;

    //        case 1:
    //        case 6:
    //        case 10:
    //        case 13:
    //        case 17:
    //        case 23:
    //            // カードマス
    //            CardOneUp();
    //            break;

    //        case 2:
    //        case 9:
    //        case 15:
    //        case 19:
    //            // 〇マス移動
    //            MapPlayer.ins.Sum += 3;
    //            Debug.Log("3マス移動：" + NowMasume + "マス目");
    //            //Move();
    //            break;

    //        case 3:
    //        case 7:
    //        case 8:
    //        case 14:
    //        case 21:
    //        case 24:
    //            // HP増減
    //            HpOneUp();
    //            break;

    //        case 4:
    //        case 11:
    //        case 16:
    //        case 20:
    //        case 25:
    //            // 攻撃マス
    //            Attack();
    //            break;

    //        case 5:
    //        case 12:
    //        case 18:
    //        case 22:
    //            // 効果なし
    //            Debug.Log("効果なし：" + NowMasume + "マス目");
    //            break;

    //        default:
    //            Debug.Log(NowMasume + "マス目");
    //            break;
    //    }
    //}
    //#endregion

    #region プレイヤーからの値の取り出し
    public void Reference()
    {
        NowMasume = MapPlayer.ins.Sum;

        //Activate();
    }
    #endregion

    #region カードを増やす
    public int CardOneUp(int card)
    {
        card++;
        Debug.Log("カードマス：" + NowMasume + "マス目");

        return card;
    }
    #endregion


    #region マスの移動
    public int Move(int move)
    {
        if (NowMasume == 2 || NowMasume == 9 || NowMasume == 19)
        {
            for (int i = 0; i < 3; i++)
            {
                move++;
            }
            Debug.Log("3マス移動：" + NowMasume + "マス目");
        }
        else if (NowMasume == 15)
        {
            move += 1;
            Debug.Log("1マス移動：" + NowMasume + "マス目");
        }

        return move;
    }
    #endregion

    #region HPを増減
    public int HpOneUp(int hp)
    {
        hp++;

        Debug.Log("HP増マス：" + NowMasume + "マス目");

        return hp;
    }
    #endregion

    #region 攻撃
    public void Attack()
    {
        Debug.Log("攻撃：" + NowMasume + "マス目");
    }
    #endregion

    //#region HP情報を取得
    //public int GetHp()
    //{
    //    return Hp;
    //}
    //#endregion

    #region マスごとの座標リストを取得
    public Vector3 GetMasumePos(int masume)
    {
        return MasumeList[masume].position; // 外部から座標をアクセス可能にする
    }
    #endregion
}
