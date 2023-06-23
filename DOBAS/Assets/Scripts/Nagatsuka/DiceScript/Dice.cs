using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Dice : MonoBehaviour
{
    #region 定数宣言
    const int DICE_UI = 0;
    #endregion
    Rigidbody rbody;
    private int number;//出目を入れる.
    private GameObject Stage;//ステージに当たった出目を判定するための変数宣言.
    private bool flg;
    private bool Hitflg;//サイコロを回転させるときに一度ステージに当たったら回転を止める.

    #region Start・Update関数
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        Stage = GameObject.Find("DiceStage");//ステージを取得する.
    }
    private void Update()
    {
        if (!Hitflg)
        {
            transform.Rotate(-3f, 0, 3f);
        }
    }
    #endregion

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Stage")//当たっているものがステージか判定.
        {
            Hitflg = true;
            if (rbody.velocity.magnitude == 0 && flg==false)//マグニチュードが0の場合（垂直判定）.
            {
                Stage.GetComponent<DiceStage>().ReturnNumber();//ステージで行っている出目判定を返す.
                flg = true;

            }
        }
    }
    /// <summary>
    /// サイコロが表示されたら動く関数.
    /// </summary>
    private void OnEnable()
    {
        flg = false;
        Hitflg = false;
    }
}
