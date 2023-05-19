using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CardManager",menuName ="CreateCardManager")]
public class CardManager : ScriptableObject
{
	[SerializeField]
	public List<CardClass> CardLists = new List<CardClass>();

	//�@�A�C�e�����X�g��Ԃ�
	public List<CardClass> GetCardLists()
	{
		return CardLists;
	}
}