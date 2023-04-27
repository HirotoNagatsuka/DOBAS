using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public List<Transform> MasumeList;   // マスの配列
    //[SerializeField]private int nowSqueare = 0;  　// 現在踏んでいるマスの値

    public static int moveSqueare; // 動くマス数
    public int hp;                 // 体力
    private bool yourTurn = false; // 自分のターン


    // Start is called before the first frame update
    void Start()
    {
        //最初のマスに配置
        transform.position = MasumeList[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // マスの移動
    void Move()
    {
        //if(nowSqueare <= Squeares.Length -1)
        //{
        //    return;
        //}
    }

    // HPの増減
    //void HpInDecrease()
    //{

    //}

    #region マスの移動
    public Vector3 MasumePos(int masume)
    {
        return MasumeList[masume].position; // 外部から座標をアクセス可能にする
    }
    #endregion
}
