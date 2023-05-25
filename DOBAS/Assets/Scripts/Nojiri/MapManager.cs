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

    #region プレイヤー側で衝突しているオブジェクトのタグを取得
    public void ColliderReference(Collider plyCollision)
    {
        string Now = plyCollision.tag; // タグを取得

        Debug.Log("NowTag：" + Now);
    }
    #endregion

    #region カードを増やす
    public int CardOneUp(int card)
    {
        card++;
        Debug.Log("カード：" + card + "枚");

        return card;
    }
    #endregion


    #region マスの移動
    public int Move(int move, string tag)
    {
        if (tag == "Move")
        {
            move = 3;
        }

        return move;
    }
    #endregion

    #region HPを増減
    public int HpOneUp(int hp)
    {
        hp++;

        return hp;
    }
    #endregion

    #region 攻撃(仮)
    public void Attack()
    {

    }
    #endregion

    //こっちを使う予定
    //#region 攻撃
    //public int Attack(int hp)
    //{
    //    hp--;

    //    return hp;
    //}
    //#endregion

    //#region マスごとの座標リストを取得
    //public Vector3 GetMasumePos(int masume)
    //{
    //    return MasumeList[masume].position; // 外部から座標をアクセス可能にする
    //}
    //#endregion
}
