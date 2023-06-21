using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardCreateManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> CardObj = new List<GameObject>();

    [SerializeField] CardManager Card_Manager;

    [SerializeField] GameObject Cards;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i < 5; i++)
        {
            GetCardID_AddInfo(i);// ���Ăяo��
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GetCardID_AddInfo(int id) // �J�[�hID���擾���āA����}��
    {
        for (int i = 0; i < Card_Manager.CardLists.Count; i++)
        {
            // ID����v������J�[�h��}��
            if (Card_Manager.CardLists[i].GetId() == id) 
            {
                CardObj.Add(Instantiate(Cards,this.transform));

                // �v�f�������[�v���A�摜��null��������J�[�h�̏���}��
                for (int ii = 0; ii < CardObj.Count; ii++)
                {
                    if(CardObj[ii].GetComponent<Image>().sprite == null)
                    {
                        CardObj[ii].GetComponent<CardParam>().Id = Card_Manager.CardLists[i].GetId();
                        CardObj[ii].GetComponent<CardParam>().Kind = (int)Card_Manager.CardLists[i].GetKindOfCard();
                        CardObj[ii].GetComponent<CardParam>().Icon = Card_Manager.CardLists[i].GetIcon();
                        CardObj[ii].GetComponent<CardParam>().CardName = Card_Manager.CardLists[i].GetCardName();
                        CardObj[ii].GetComponent<CardParam>().Power = Card_Manager.CardLists[i].GetPower();
                        CardObj[ii].GetComponent<CardParam>().Move = Card_Manager.CardLists[i].GetMove();
                        break;
                    }
                }
            }
        }
    }
    public void DrawCardList()
    {
        this.gameObject.SetActive(true);
    }
    // �g�����J�[�h�������֐�
    public void UseCardDestroy(int id) 
    {
        for (int i = 0; i < CardObj.Count;i++)
        {
            // ID����v������J�[�h��}��
            if (CardObj[i].GetComponent<CardParam>().Id == id)
            {
                Destroy(CardObj[i]);
                CardObj.RemoveAt(i);
                break;
            }
            else
            {
                break;
            }
        }
    }
}
