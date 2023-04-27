using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public List<Transform> MasumeList;            // マスの配列
    [SerializeField]private int nowMasume = 0;  　// 現在踏んでいるマスの値

    public static int MoveSqueare; // 動くマス数
    public int Hp;                 // 体力
    private bool YourTurn = false; // 自分のターン


    //void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // マスの移動
    //void Move()
    //{
    //    //if(nowSqueare <= Squeares.Length -1)
    //    //{
    //    //    return;
    //    //}
    //}

    #region HPを増やす
    public void HpOneUp()
    {
        Hp++;
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
