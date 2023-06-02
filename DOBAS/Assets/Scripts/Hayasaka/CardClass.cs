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
	//�@�J�[�h�̃i���o�[
	[SerializeField]
	private int Id;
	//�@�A�C�e���̎��
	[SerializeField]
	private KindOfCard Koc;
	//�@�A�C�e���̃A�C�R��
	[SerializeField]
	private Sprite Icon;
	//�@�A�C�e���̖��O
	[SerializeField]
	private string CardName;
	//�@�A�C�e���U����
	[SerializeField]
	private int Power;

	//�@�ړ���
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
