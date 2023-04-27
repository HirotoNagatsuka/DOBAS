using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Dice : MonoBehaviour
{
    #region 定数宣言
    const int DICE_UI = 0;
    #endregion
    Rigidbody rbody;
    private int number;//出目を入れる.
    [SerializeField] GameObject Stage;//ステージに当たった出目を判定するための変数宣言.
    GameObject DiceUI;//子供のキャンバスを取得するための変数宣言.
    #region Start・Update関数
    void Start()
    {
        number = 0;//初期化.
        rbody = GetComponent<Rigidbody>();
        Stage = GameObject.Find("DiceStage");//ステージを取得する.
        DiceUI = gameObject.transform.GetChild(DICE_UI).gameObject;//子供のキャンバスを取得.
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    Destroy(gameObject);
        //}
    }
    #endregion

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Stage")//当たっているものがステージか判定.
        {
            if (rbody.velocity.magnitude == 0)//マグニチュードが0の場合（垂直判定）.
            {
                Stage.GetComponent<DiceStage>().ConfirmNumber();//ステージで行っている出目判定を返す.
                //DiceUI.gameObject.transform.GetChild(0).GetComponent<Text>().text = number.ToString();//出目のUI表示.
                //switch (number)//出目によってUIの表示を変更する.
                //{
                //    case 1:
                //        DiceUI.transform.localPosition = new Vector3(0, 0, 3);
                //        break;
                //    case 2:
                //        DiceUI.transform.localPosition = new Vector3(0, 3, 0);
                //        break;
                //    case 3:
                //        DiceUI.transform.localPosition = new Vector3(-3, 0, 0);
                //        break;
                //    case 4:
                //        DiceUI.transform.localPosition = new Vector3(3, 0, 0);
                //        break;
                //    case 5:
                //        DiceUI.transform.localPosition = new Vector3(0, -3, 0);
                //        break;
                //    case 6:
                //        DiceUI.transform.localPosition = new Vector3(0, 0, -3);
                //        break;
                //    default:
                //        break;
                //}
            }
        }
    }
}
