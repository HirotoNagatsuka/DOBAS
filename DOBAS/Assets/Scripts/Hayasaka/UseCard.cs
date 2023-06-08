using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseCard : MonoBehaviour
{
    [SerializeField] CardManager Card_Manager;
    [SerializeField] GameObject CardPanel;
    //[SerializeField] PlayerManager Player_Manager;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UseCardMove()
    {
        Debug.Log(Card_Manager.CardLists[3].GetMove());
        CardPanel.SetActive(true);
        //Player_Manager.StartDelay(Card_Manager.CardLists[0].GetMove());
    }
}
