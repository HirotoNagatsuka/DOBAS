using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Cards", menuName = "CreateCard")]
public class CardClass : ScriptableObject
{
    public enum KindOfCard
    {
        Atack,
        Move,
        CardMix,
        PosMix,
        UseCard
    }
	//　カードのナンバー
	[SerializeField]
	private int Id;
	//　アイテムの種類
	[SerializeField]
	private KindOfCard Koc;
	//　アイテムのアイコン
	[SerializeField]
	private Sprite Icon;
	//　アイテムの名前
	[SerializeField]
	private string CardName;
	//　アイテム攻撃力
	[SerializeField]
	private int Power;

	//　移動数
	[SerializeField]
	private int Move;

	public int GetId()
	{
		return Id;
	}
	public KindOfCard GetKindOfCard()
	{
		return Koc;
	}

	public Sprite GetIcon()
	{
		return Icon;
	}

	public string GetCardName()
	{
		return CardName;
	}
	public int GetPower()
	{
		return Power;
	}
	public int GetMove()
	{
		return Move;
	}
}
