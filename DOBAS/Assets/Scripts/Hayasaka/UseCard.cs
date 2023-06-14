using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UseCard : MonoBehaviourPunCallbacks
{
    [SerializeField] CardManager Card_Manager;
    [SerializeField] GameObject CardPanel;
    [SerializeField] GameManager gameManager;

    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UseCardMove()
    {
        if (photonView.IsMine)
        {
            gameManager.Players[0].GetComponent<PlayerManager>().StartDelay(Card_Manager.CardLists[3].GetMove(),true);
        }
    }
    public void UseCardAttck()
    {
        if (photonView.IsMine)
        {
            gameManager.Players[0].GetComponent<PlayerManager>().StartDelay(Card_Manager.CardLists[0].GetPower(), true);
        }
    }
}
