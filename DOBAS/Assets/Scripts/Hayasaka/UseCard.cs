using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UseCard : MonoBehaviourPunCallbacks
{
    [SerializeField] CardManager Card_Manager;
    [SerializeField] GameObject CardPanel;
     GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void DrawCardList()
    {
        CardPanel.SetActive(true);
    }
    public void UseCardMove()
    {
        gameManager.Players[0].GetComponent<PlayerManager>().StartDelay(Card_Manager.CardLists[3].GetMove(),true);
    }
    public void UseCardAttck()
    {
        gameManager.Players[0].GetComponent<PlayerManager>().EnemyAttack(Card_Manager.CardLists[0].GetPower());       
    }
}
