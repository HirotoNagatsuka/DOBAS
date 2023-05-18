using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public List<Transform> MasumeList;        // マスの配列

    //[SerializeField] GameObject MapPlayer;    // MapManager参照
    [SerializeField] private int NowMasume;   // 現在踏んでいるマスの値

    // Start is called before the first frame update
    void Start()
    {
        NowMasume = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region プレイヤーに衝突しているオブジェクトのタグを取得
    public void ColliderReference(Collider plyCollision)
    {
        string Now = plyCollision.tag; // タグを取得

        Debug.Log("NowTag：" + Now);

        NowMasume = MapPlayer.ins.Sum; // 消す予定
    }
    #endregion

    #region カードを増やす
    public int CardOneUp(int card)
    {
        card++;
        Debug.Log("カードマス：" + NowMasume + "マス目");
        Debug.Log("カード：" + card + "枚");

        return card;
    }
    #endregion


    #region マスの移動
    public int Move(int move, string tag)
    {
        if (tag == "Move")
        {
            move += 3;
            Debug.Log("3マス移動：" + NowMasume + "マス目");
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

    //#region マスごとの座標リストを取得
    //public Vector3 GetMasumePos(int masume)
    //{
    //    return MasumeList[masume].position; // 外部から座標をアクセス可能にする
    //}
    //#endregion
}
