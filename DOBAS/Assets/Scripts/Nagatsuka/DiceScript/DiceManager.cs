using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DiceManager : MonoBehaviour
{
    [SerializeField] TestGManager testGManager;
    public GameObject DicePrefab;//サイコロのプレファブを入れる.
    private GameObject Dice;//サイコロ用の表示・非表示を繰り返す用.
    public int num;
    #region ランダムに回転させる用の変数宣言.
    private int rotateX;
    private int rotateY;
    private int rotateZ;
    #endregion

    [SerializeField] Sprite[] SaikoroSprite = new Sprite[4];
    [SerializeField] GameObject DiceCamera;
    [SerializeField] Text DiceNumText;
    [SerializeField] GameObject[] ResultPanel=new GameObject[5];//出目のパネル用.
    [SerializeField] GameObject ReasoningPanel;
    Vector3 CameraPos;
    public Transform kodomo; //子オブジェクト.
    private bool DiceFlg;//サイコロ作成フラグ.
    [SerializeField] GameObject DiceShakeButton;
    private int Number;
    public int DeclarationNum;//宣言番号.
    public bool Doubt;
    [SerializeField] MapPlayer mapPlayer;
    [SerializeField] PlayerManager playerManager;

    private void Start()
    {
        //mapPlayer = GetComponent<MapPlayer>();
        Dice = Instantiate(DicePrefab, DiceCamera.transform.position, Quaternion.identity, kodomo);
        Dice.transform.position = CameraPos;
        CameraPos = DiceCamera.transform.position;
        CameraPos.x += 3;
        CameraPos.y -= 3;
        Dice.SetActive(false);
        DiceCamera.SetActive(false);
        DiceFlg = false;
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
        DiceFlg = true;
        DiceShakeButton.SetActive(false);
    }

    // コルーチン本体
    private IEnumerator HiddenDiceCoroutine()
    {
        // 2秒間待つ
        yield return new WaitForSeconds(2);
        Debug.Log("コルーチン呼び出し終了");
        ActivePanel();
        yield break;
    }

    /// <summary>
    /// 出目確定、UIに数値や文字を表示する
    /// </summary>
    public void ConfirmNumber(int num)
    {
        Number = num;
        if (num == 4)
        {
            DiceNumText.text = "Attack";
        }
        else if (num == 5 || num == 6)
        {
            DiceNumText.text = "Doubt";
        }
        else
        {
            DiceNumText.text = num.ToString();
        }
        StartCoroutine(HiddenDiceCoroutine());
    }

    public void PushShakeDiceButton()
    {
        if (DiceFlg == false)
        {
            ShakeDice();
        }
    }

    /// <summary>
    /// 出目によって表示するパネルを変える.
    /// </summary>
    private void ActivePanel()
    {
        if (Number == 1)
        {
            ResultPanel[0].SetActive(true);
        }
        else if (Number == 2)
        {
            ResultPanel[1].SetActive(true);
        }
        else if (Number == 3)
        {
            ResultPanel[2].SetActive(true);
        }
        else if (Number == 4)
        {
            ResultPanel[3].SetActive(true);
        }
        else//ダウトの場合フラグをOnにする.
        {
            ResultPanel[4].SetActive(true);
            Doubt = true;
        }
    }

    #region 各出目のボタン
    /// <summary>
    /// 1の出目ボタンを押したら.
    /// </summary>
    public void PushOneButton()
    {
        DeclarationNum = 1;
        DeclarationResult();
    }
    /// <summary>
    /// 2の出目ボタンを押したら.
    /// </summary>
    public void PushTwoButton()
    {
        DeclarationNum = 2;
        DeclarationResult();
    }
    /// <summary>
    /// 3の出目ボタンを押したら.
    /// </summary>
    public void PushThreeButton()
    {
        DeclarationNum = 3;
        DeclarationResult();
    }
    /// <summary>
    /// 攻撃出目ボタンを押したら.
    /// </summary>
    public void PushAttackButton()
    {
        DeclarationNum = 4;
        DeclarationResult();
    }
    #endregion

    private void DeclarationResult()
    {
        playerManager.StartDelay(DeclarationNum);
        // mapPlayer.StartDelay(DeclarationNum);
        /*
        if (Doubt)
        {
            ResultPanel[4].SetActive(false);
        }
        else
        {
            ResultPanel[DeclarationNum - 1].SetActive(false);
        }*/
        //後々修正.
        ResultPanel[0].SetActive(false);
        ResultPanel[1].SetActive(false);
        ResultPanel[2].SetActive(false);
        ResultPanel[3].SetActive(false);
        ResultPanel[4].SetActive(false);


        Debug.Log("出目：" + DeclarationNum);
        DiceNumText.text = " ";
        DiceCamera.SetActive(false);
        Dice.SetActive(false);
        DiceFlg = false;
        DiceInit();
        /*
        ReasoningPanel.SetActive(true);
        ReasoningPanel.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = SaikoroSprite[DeclarationNum-1];
        testGManager.StartDoubtTime();*/
    }

    public void DiceInit()
    {
        DiceNumText.text = " ";
        DeclarationNum = 0;
        ReasoningPanel.SetActive(false);
        DiceCamera.SetActive(false);
        Dice.SetActive(false);
        DiceFlg = false;
        DiceShakeButton.SetActive(true);
    }

    /// <summary>
    /// 宣言された出目を返す関数
    /// </summary>
    public int ReturnDeclarationNum()
    {
        return DeclarationNum;
    }
}
