using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalsManager : MonoBehaviour
{
    public GameObject[] effectObject;   // エフェクトのプレハブ配列
    public GameObject Camera;           // カメラ取得
    public float moveSpeed = 5f;        // プレイヤーの移動速度
    public float rotationSpeed = 100f;  // プレイヤーの回転速度

    [SerializeField] GameObject ChildObject; // 子オブジェクト取得
    [SerializeField] int ChildNum = 0;        // 生成している子オブジェクト番号
    bool NowSelect = true;   // キャラクターセレクトON
    bool EffectPrep = true;  // エフェクト生成可能か

    Animator ChildAnimator; // 子オブジェクトの
    Vector3 PlayerPos;      // プレイヤー位置

    //Start is called before the first frame update
    void Start()
    {
        CharaSelect();

        //// アクティブなアバターを取得済みのとき
        //if (avatarSet != null)
        //{
        //    // 取得したアバターをセット
        //    animator.avatar = avatarSet;
        //}
        //else
        //{
        //    Debug.LogError("アクティブな子オブジェクトにAnimatorのAvatarが見つかりません");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーの位置情報を取得
        PlayerPos = transform.position;

        // キャラセレクト時、回転させる
        if (NowSelect)
        {
            float rotation = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotation);
        }

        if(!NowSelect) EffectGenerate();
    }

    #region エフェクト生成
    void EffectGenerate()
    {
        // エフェクトを生成してよいか
        if (EffectPrep)
        {
            // Spaceが押されたらランダムで攻撃
            if (Input.GetKey(KeyCode.Space))
            {
                EffectPrep = false;
                StartCoroutine("EffectPreview");
            }
        }
    }
    #endregion

    #region アバター切り替え
    //private Avatar FindAvatarInChildren(Transform parent)
    //{
    //    Avatar childAvatar = null;

    //    // 子オブジェクトの数ループ
    //    for(int i = 0; i < parent.childCount; i++)
    //    {
    //        Transform child = parent.GetChild(i);  // 子を全て取得
    //        childAvatar = child.GetComponent<Animator>()?.avatar; // Animatorが?(null)でないときavatarを取得
    //        Debug.Log(childAvatar);

    //        // avatarを取得しているとき
    //        if (childAvatar != null)
    //        {
    //            break;
    //        }
    //    }

    //    // avatarが見つからないときnullを返す
    //    return null;
    //}
    #endregion

    #region キャラ生成
    void CharaSelect()
    {
        // 全ての子オブジェクトを非アクティブ
        for (int i = 0; i < this.transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        // 最初の一つをアクティブ
        transform.GetChild(ChildNum).gameObject.SetActive(true);

    }
    #endregion

    #region キャラクター変更
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

    #region エフェクト調整用
    IEnumerator EffectPreview()
    {
        // 回復
        GameObject healObj = Instantiate(effectObject[0], PlayerPos, Quaternion.identity); // HpUp
        ChildAnimator.SetTrigger("Jump"); // Jumpアニメーション再生
        yield return new WaitForSeconds(2f);
        Destroy(healObj);

        // 体力減少
        GameObject downObj = Instantiate(effectObject[1], PlayerPos, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Destroy(downObj);

        // カード
        GameObject cardObj = Instantiate(effectObject[2], PlayerPos, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Destroy(cardObj);

        // 被攻撃時
        Instantiate(effectObject[3], PlayerPos, Quaternion.identity); // GetHit
        Instantiate(effectObject[4], PlayerPos, Quaternion.identity); // HpBreak
        yield return new WaitForSeconds(0.5f);
        ChildAnimator.SetTrigger("Death"); // Deathアニメーション再生
        yield return new WaitForSeconds(5f);
        transform.GetChild(ChildNum).gameObject.SetActive(false); // プレイヤーを非アクティブ(死亡)

        EffectPrep = true;
    }
    #endregion
}
