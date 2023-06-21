using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardParam : MonoBehaviour
{
	//　カードのナンバー
	public int Id;
	// カードの種類
	public int Kind;
	//　アイテムのアイコン
	public Sprite Icon;
	//　アイテムの名前
	public string CardName;
	//　アイテム攻撃力
	public int Power;
	//　移動数
	public int Move;

	[SerializeField] GameObject CardDetailPanel;

	GameObject CCM;
	// Start is called before the first frame update
	void Start()
    {
		this.gameObject.GetComponent<Image>().sprite = Icon;

		CCM = GameObject.Find("CardListPanel");
	}

    // Update is called once per frame
    void Update()
    {
        
    }
	public void DrawDetailCard() // カードの詳細画面表示
	{
		Instantiate(CardDetailPanel, CCM.transform);

		Invoke("DelaySendParam", 0.5f);
	}
	void DelaySendParam() // カードの種類や効果などの詳細情報を挿入
    {
		CardDetailPanel.GetComponent<DetailCardManager>().GetCardInfo(Icon, CardName, Kind,Id);

		switch (Kind)
		{
			// 0,無、1,攻撃、2,移動
			case 0:
				Debug.Log("何もなし");
				break;
			case 1:
				CardDetailPanel.GetComponent<DetailCardManager>().GetCardAttck(Power);
				break;
			case 2:
				CardDetailPanel.GetComponent<DetailCardManager>().GetCardMove(Move);
				break;
			default:
				break;
		}
	}
}
