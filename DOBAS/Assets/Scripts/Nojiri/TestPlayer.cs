using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public GameObject[] effectObject;
    public float moveSpeed = 5f; // プレイヤーの移動速度
    public float rotationSpeed = 180f;  // プレイヤーの回転速度

    private GameObject[] animals;

    bool flg = true;
    bool attackFlg = false;
    Animator anim;
    Vector3 PlayerPos;
    Transform animal_parent;

    // Start is called before the first frame update
    void Start()
    {
        animal_parent = GameObject.Find("Animals").transform;

        int count = animal_parent.childCount;
        animals = new GameObject[count];
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーの位置情報を取得
        PlayerPos = transform.position;

        PlayerMove();
        EffectGenerate();
    }

    IEnumerator Delay()
    {
        // エフェクト生成
        // 回復
        GameObject healObj = Instantiate(effectObject[0], PlayerPos, Quaternion.identity);
        anim.SetTrigger("Jump");
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

        // 攻撃
        Instantiate(effectObject[3], PlayerPos, Quaternion.identity);
        Instantiate(effectObject[4], PlayerPos, Quaternion.identity);
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(2f);

        flg = true;
    }

    void PlayerMove()
    {
        // 入力を取得
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 移動ベクトルを計算
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;

        // プレイヤーを移動させる
        transform.Translate(movement);

        // 矢印キーの入力を処理して回転させる
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotation);
        }
    }

    void EffectGenerate()
    {
        // エフェクトを生成してよいか
        if (flg)
        {
            // Spaceが押されたらランダムで攻撃
            if (Input.GetKey(KeyCode.Space))
            {
                // 
                flg = false;
                StartCoroutine("Delay");
            }
        }
    }
}
