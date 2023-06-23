using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalsManager : MonoBehaviour
{
    public GameObject[] effectObject;   // エフェクトのプレハブ配列
    public GameObject Camera;           // カメラ取得
    public float moveSpeed = 5f;        // プレイヤー移動速度
    public float rotationSpeed = 100f;  // プレイヤー回転速度

    [SerializeField] GameObject ChildObject; // 子オブジェクト取得
    public int ChildNum = 0;       // 生成している子オブジェクト番号
    private bool NowSelect = true;   // キャラクターセレクトON/OFF
    private bool EffectPrep = true;  // エフェクト生成可能か

    Animator ChildAnimator; // 子オブジェクトAnimator
    Vector3 PlayerPos;      // プレイヤー位置情報

    GameManager gameManager;
    //Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        CharaSelect(); // キャラ生成
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーの位置情報を取得
        PlayerPos = transform.position;
        if(gameManager.NowGameState== GameManager.GameState.SetGame)
        {
            // キャラセレクト時に、キャラを回転させる
            if (NowSelect)
            {
                float rotation = rotationSpeed * Time.deltaTime;
                transform.Rotate(Vector3.up, rotation);
            }
        }
    }

    #region キャラ生成
    void CharaSelect()
    {
        // 全ての子オブジェクトを非アクティブ
        for (int i = 0; i < this.transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        if (gameManager.NowGameState != GameManager.GameState.InGame)
        {
            // 最初の一つをアクティブ
            transform.GetChild(ChildNum).gameObject.SetActive(true);
        }
        else
        {
            // 選択したものをアクティブ
            transform.GetChild(gameManager.AnimalChildNum).gameObject.SetActive(true);
            ChildObject = transform.GetChild(gameManager.AnimalChildNum).gameObject;
            ChildAnimator = ChildObject.GetComponent<Animator>();
        }
    }
    #endregion

    #region キャラクター変更(ボタン処理)
    // 次のキャラへボタン押下
    public void NextButtonPush()
    {
        CharaSet(true);
    }

    // 前のキャラへボタン押下
    public void ReturnButtonPush()
    {
        CharaSet(false);
    }

    void CharaSet(bool flg)
    {
        // 現在のアクティブな子オブジェクトを非アクティブ
        transform.GetChild(ChildNum).gameObject.SetActive(false);

        if (flg)
        {
            ChildNum++;
            // 最後まで切り替えたら最初のオブジェクトに戻る
            if (ChildNum >= this.transform.childCount) { ChildNum = 0; }
        }
        else
        {
            ChildNum--;
            // 最初まで切り替えたら最後のオブジェクトに戻る
            if (ChildNum < 0) { ChildNum = transform.childCount - 1; }
        }

        // 次の子オブジェクトをアクティブ
        transform.GetChild(ChildNum).gameObject.SetActive(true);
    }

    // 決定ボタン押下
    public void DecisionButtonPush()
    {
        // 回転を止めてカメラに向かせる
        Vector3 TargetRot = Camera.transform.position - transform.position;
        TargetRot.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(TargetRot);
        transform.rotation = targetRotation;

        // 選択したキャラを取得
        ChildObject = transform.GetChild(ChildNum).gameObject;
        ChildAnimator = ChildObject.GetComponent<Animator>();
        ChildAnimator.SetTrigger("Jump"); // 決定時Jumpアニメーション再生

        // キャラクターセレクトOFF
        NowSelect = false;
    }
    #endregion

    #region 外から呼び出される関数用
    // カード取得時
    public void CardGetting()
    {
        StartCoroutine("EffectPreview", "CardGet");
    }

    // 移動するかどうか　true：移動開始　false：移動終了
    public void Moving(bool move)
    {
        if (move)
        {
            ChildAnimator.SetBool("Walk", true); // Walkアニメ再生
        }
        else
        {
            ChildAnimator.SetBool("Walk", false); // Walkアニメ再生終了
        }
    }

    // 攻撃時
    public void Attacking()
    {
        if (ChildAnimator == null) Debug.Log("あるよ");
        ChildAnimator.SetTrigger("Attack"); // Attackアニメ再生
    }

    // 体力変化時　true：体力増加　false：体力減少
    public void HpChange(bool hpChange)
    {
        if (hpChange)
        {
            StartCoroutine("EffectPreview", "HpUp");
        }
        else
        {
            StartCoroutine("EffectPreview", "HpDown");
        }
    }

    // ダメージを受けたとき生きているかどうか　true：生存　false：死亡
    public void Damage(bool alive)
    {
        if (alive)
        {
            StartCoroutine("EffectPreview", "Hit");
        }
        else
        {
            StartCoroutine("EffectPreview", "Death");
        }
    }
    #endregion

    #region エフェクト生成
    IEnumerator EffectPreview(string AnimName)
    {
        switch (AnimName)
        {
            case "CardGet": // カード取得時
                GameObject cardObj = Instantiate(effectObject[2], PlayerPos, Quaternion.identity);
                ChildAnimator.SetTrigger("Jump"); // Jumpアニメ再生
                yield return new WaitForSeconds(2f);
                Destroy(cardObj);
                break;

            case "HpUp": // 体力増加
                GameObject healObj = Instantiate(effectObject[0], PlayerPos, Quaternion.identity); // HpUp
                ChildAnimator.SetTrigger("Jump"); // Jumpアニメ再生
                yield return new WaitForSeconds(2f);
                Destroy(healObj);
                break;

            //case "Move":
            //    break;

            //case "MoveEnd":
            //    break;

            //case "Attack":
            //    break;

            case "HpDown": // 体力減少
                GameObject downObj = Instantiate(effectObject[1], PlayerPos, Quaternion.identity); // HpDown
                yield return new WaitForSeconds(2f);
                Destroy(downObj);
                break;

            case "Attack": // 攻撃
                ChildAnimator.SetTrigger("Attack"); // Attackアニメ再生
                break;

            case "Hit": // 通常ダメージ
                Instantiate(effectObject[3], PlayerPos, Quaternion.identity); // GetHit
                ChildAnimator.SetTrigger(AnimName); // Hitアニメ再生
                break;

            case "Death": // 体力０になるとき
                Instantiate(effectObject[3], PlayerPos, Quaternion.identity); // GetHit
                Instantiate(effectObject[4], PlayerPos, Quaternion.identity); // HpBreak

                yield return new WaitForSeconds(0.5f);
                ChildAnimator.SetTrigger(AnimName); // Deathアニメ再生
                yield return new WaitForSeconds(5f);
                transform.GetChild(ChildNum).gameObject.SetActive(false); // 5秒後プレイヤーを非アクティブ(死亡)
                break;

            default:
                Debug.LogError("AnimNameエラー");
                break;
        }
    }
    #endregion
}
