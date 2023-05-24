using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStore : MonoBehaviour
{
    //　カードマネージャー
    [SerializeField]
    private CardManager CardMg;
    [SerializeField]
    public List<int> RandomCardLists = new List<int>();


    // Start is called before the first frame update
    void Start()
    {
        RandomGetCard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   void RandomGetCard()
    {
        while(CardMg.GetCardLists().Count > 0)
        {
            int index = Random.Range(0, CardMg.GetCardLists().Count);
            int random = CardMg.CardLists[index].GetId();
            Debug.Log(random);

        }
    }
}
