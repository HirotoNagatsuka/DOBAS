using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardCreateManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> CardObj = new List<GameObject>();

    [SerializeField] public CardManager Card_Manager;

    [SerializeField] GameObject Cards;

    [SerializeField] GameObject CardList;
    // ���X�g�����{�^��
    [SerializeField] GameObject CloseBt;

    [SerializeField] GameObject UseBt;

    [SerializeField] GameObject NotHoldBt;
    private int AddNum = 0;
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetCardID_AddInfo(int id) // �J�[�hID���擾���āA����}��
    {
        Debug.Log(id);
        for (int i = 0; i < Card_Manager.CardLists.Count; i++)
        {
            // ID����v������J�[�h��}��
            if (Card_Manager.CardLists[i].GetId() == id)
            {
                CardObj.Add(Instantiate(Cards, CardList.transform));
                // �v�f�������[�v���A�摜��null��������J�[�h�̏���}��
                for (int ii = AddNum; ii < CardObj.Count; ii++)
                {
                    if (CardObj[ii].GetComponent<Image>().sprite == null)//null����Ȃ���
                    {
                        CardObj[ii].GetComponent<CardParam>().Id = Card_Manager.CardLists[i].GetId();
                        CardObj[ii].GetComponent<CardParam>().Kind = (int)Card_Manager.CardLists[i].GetKindOfCard();
                        CardObj[ii].GetComponent<CardParam>().Icon = Card_Manager.CardLists[i].GetIcon();
                        CardObj[ii].GetComponent<CardParam>().CardName = Card_Manager.CardLists[i].GetCardName();
                        CardObj[ii].GetComponent<CardParam>().Power = Card_Manager.CardLists[i].GetPower();
                        CardObj[ii].GetComponent<CardParam>().Move = Card_Manager.CardLists[i].GetMove();
                        AddNum++;
                        break;
                    }
                }
            }
        }

    }
    // �J�[�h���X�g�\���A��\��
    public void DrawCardList()
    {
        Debug.Log("�{�^����������"+CardObj.Count); 
        if (CardObj.Count == 0)
        {
            //Debug.Log(CardObj);
            NotHoldBt.SetActive(true);
        }
        else
        {
            NotHoldBt.SetActive(false);
        }
        CardList.SetActive(true);
        CloseBt.SetActive(true);
    }
    public void FalseCardList()
    {
        NotHoldBt.SetActive(false);
        CardList.SetActive(false);
        CloseBt.SetActive(false);
    }

    // �g�����J�[�h�������֐�
    public void UseCardDestroy(int id) 
    {
        for (int i = 0; i < CardObj.Count;i++)
        {
            if (CardObj[i].GetComponent<CardParam>().Id == id)
            {               
                Destroy(CardObj[i]);
                CardObj.RemoveAt(i);
                break;
            }
           
        }
        CardList.SetActive(false);
        CloseBt.SetActive(false);
        UseBt.SetActive(true);
    }
}
