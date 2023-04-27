using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
class Player
{
    public string Name;//名前を格納.
    public int HP;//HPを格納.
    public int Attack;//攻撃力を格納.
}

public class PlayerManager : MonoBehaviour
{
    #region 定数宣言
    const int PLAYER_UI = 0;
    const int NAME_UI = 0;
    const int HP_UI = 1;
    #endregion

    public int TestAddHP;

    [SerializeField]
    Player Player;//Playerのクラスをインスペクター上で見れるようにする.

    GameObject PlayerUI;//子供のキャンバスを取得するための変数宣言.

    #region Unityイベント(Start・Update)
    private void Start()
    {
        PlayerUI = gameObject.transform.GetChild(PLAYER_UI).gameObject;//子供のキャンバスを取得.
        PlayerUI.gameObject.transform.GetChild(NAME_UI).GetComponent<Text>().text = Player.Name.ToString();//名前の表示.
        PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Text>().text = Player.HP.ToString();//HPの表示.
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))//テスト用にHPを増減.
        {
            ChangeHP(TestAddHP);
        }
    }

    #endregion

    /// <summary>
    /// HPが変化するときに呼び出す関数
    /// 変化量を引数にし、HPを変えた後UIにも反映する.
    /// </summary>
    public void ChangeHP(int addHP)
    {
        if(Player.HP == 0)//HPが0になったら.
        {
            Player.HP = 0;
            Debug.Log("ゲームオーバー");
        }
        else Player.HP += addHP;//0でないなら変化させる.
        PlayerUI.gameObject.transform.GetChild(HP_UI).GetComponent<Text>().text = Player.HP.ToString();//HPの表示.
    }

}
