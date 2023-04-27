using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;            // 参照用
    public List<Transform> MasumeList;            // マスの配列
    //public int ListNum;    // マス配列の番号
    public int Hp;         // 体力

    [SerializeField] private int NowMasume = 0;   // 現在踏んでいるマスの値
    [SerializeField] private int saikoro;   // サイコロの出目(1～4)
    private bool i = true; 

    // 参照用
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
        //初期位置
        //transform.position = MasumeList[NowMasume].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (i == true)
        {
            NowMasume += saikoro; //現在のマス
            Activate();
            i = false;
        }
    }

    //マスの効果振り分け
    void Activate()
    {
        switch(NowMasume)
        {
            case 0:
                // スタートor周回
                Debug.Log(NowMasume);
                break;
            case 1:
                // カードマス
                Debug.Log(NowMasume);
                break;
            case 2:
                // 〇マス移動
                Debug.Log(NowMasume);
                break;
            case 3:
                // HP増える
                HpOneUp();
                Debug.Log(NowMasume);
                Debug.Log(Hp);
                break;
            case 4:
                // 攻撃マス
                Debug.Log(NowMasume);
                break;
            case 5:
                // 効果なし
                Debug.Log(NowMasume);
                break;
            default:
                break;
        }
    }

    //マスの移動
    void Move()
    {
        transform.position = MasumeList[NowMasume].position;
    }

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
        NowMasume = masume;
        return MasumeList[masume].position; // 外部から座標をアクセス可能にする
    }
    #endregion
}
