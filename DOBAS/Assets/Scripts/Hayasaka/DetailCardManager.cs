using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailCardManager : MonoBehaviour
{
    // 画像、テキストの受け皿
    [SerializeField] Image CardImg;
    [SerializeField] Text CardText;
    // 受け取る値
    public int IdParam;
    public int MoveParam;
    public int AttckParam;
    public int KindParam;
    // 参照用
    GameManager gameManager;
    CardCreateManager CCM;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        CCM = GameObject.Find("CardListPanel").GetComponent<CardCreateManager>();
        Debug.Log(KindParam);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FalseList() // 非表示
    {
        this.gameObject.SetActive(false);
    }
    public void UseCard() // カードの使用
    {
        switch (KindParam)
        {
            // 0,無、1,攻撃、2,移動
            case 0:
                Debug.Log("何もなし");
                break;
            case 1:
                Debug.Log("攻撃");
                //gameManager.Players[0].GetComponent<PlayerManager>().EnemyAttack(AttckParam);
                break;
            case 2:
                Debug.Log("移動");
                gameManager.Players[0].GetComponent<PlayerManager>().StartDelay(MoveParam, true);
                break;
            default:
                break;
        }
        this.gameObject.SetActive(false);
        Invoke("CardInfoDestroy", 0.1f);
    }
    // カード詳細画面の画像などの割り当て
    public void GetCardInfo(Sprite img,string tx,int kind,int id)
    {
        CardImg.GetComponent<Image>().sprite = img;
        CardText.GetComponent<Text>().text = tx;
        KindParam = kind;
        IdParam = id;
    }
    // Moveの値を取得
    public void GetCardMove(int move)
    {
        MoveParam = move;
    }
    // Attck値の取得
    public void GetCardAttck(int power)
    {
        AttckParam = power;
    }
    // 使ったカードの情報を削除
    void CardInfoDestroy()
    {
        CCM.GetComponent<CardCreateManager>().UseCardDestroy(IdParam);

        CardImg.GetComponent<Image>().sprite = null;
        CardText.GetComponent<Text>().text = null;
        KindParam = 0;
        IdParam = 0;
    }
}
