using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCard : MonoBehaviour
{
    [SerializeField] CardManager Card_Manager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetCard(int id)
    {
        switch (id)
        {
            case 1:
                Debug.Log(Card_Manager.CardLists[0].GetCardName());
                break;
            default:
                break;
        }
    }
}
