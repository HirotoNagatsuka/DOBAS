using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CardManager",menuName ="CreateCardManager")]
public class CardManager : ScriptableObject
{
	[SerializeField]
	public List<CardClass> CardLists = new List<CardClass>();
	
	//　アイテムリストを返す(ゲッター?)
	public List<CardClass> GetCardLists()
	{
		return CardLists;
	}
}
