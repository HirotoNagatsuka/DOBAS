using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceThrow : MonoBehaviour
{
    public GameObject DicePrefab;//サイコロのプレファブを入れる.
    private GameObject Dice;//サイコロ用の表示・非表示を繰り返す用.
    public int num;
    #region ランダムに回転させる用の変数宣言.
    private int rotateX;
    private int rotateY;
    private int rotateZ;
    #endregion


    [SerializeField] GameObject DiceCamera;
    //[SerializeField] Text DiceNumText;
    Vector3 CameraPos;
    public GameObject oya;    //親オブジェクト
    public Transform kodomo; //子オブジェクト.
    private bool DiceFlg;//サイコロ作成フラグ.

    private void Start()
    {
        Dice = GameObject.Instantiate(DicePrefab, CameraPos, Quaternion.identity, kodomo);
        Dice.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShakeDice();
        }
    }

    /// <summary>
    /// サイコロを振る用の関数
    /// サイコロのプレファブが無ければ作成し、有れば表示・非表示を繰り返す.
    /// </summary>
    public void ShakeDice()
    {
        DiceCamera.SetActive(true);
        Dice.SetActive(true);
        Dice.transform.position = CameraPos;
        CameraPos = DiceCamera.transform.position;
        CameraPos.x += 3;
        CameraPos.y -= 3;
        rotateX = Random.Range(0, 360);
        rotateY = Random.Range(0, 360);
        rotateZ = Random.Range(0, 360);
        Dice.GetComponent<Rigidbody>().AddForce(-transform.right * 300);
        Dice.transform.Rotate(rotateX, rotateY, rotateZ);
    }

    /// <summary>
    /// サイコロを非表示にし、サイコロを振る処理を終わらせる.
    /// </summary>
    public void HiddenDice()
    {
        StartCoroutine(HiddenDiceCoroutine());
    }

    // コルーチン本体
    private IEnumerator HiddenDiceCoroutine()
    {
        // 2秒間待つ
        yield return new WaitForSeconds(2);
        DiceCamera.SetActive(false);
        Dice.SetActive(false);
        Debug.Log("コルーチン呼び出し終了");
    }
}
