using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailCardManager : MonoBehaviour
{
    // �摜�A�e�L�X�g�̎󂯎M
    [SerializeField] Image CardImg;
    [SerializeField] Text CardText;
    // �󂯎��l
    public int IdParam;
    public int MoveParam;
    public int AttckParam;
    public int KindParam;
    // �Q�Ɨp
    GameManager gameManager;
    CardCreateManager CCM;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        CCM = GameObject.Find("CardListPanel").GetComponent<CardCreateManager>();
        Debug.Log(KindParam);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FalseList() // ��\��
    {
        this.gameObject.SetActive(false);
    }
    public void UseCard() // �J�[�h�̎g�p
    {
        switch (KindParam)
        {
            // 0,���A1,�U���A2,�ړ�
            case 0:
                Debug.Log("�����Ȃ�");
                break;
            case 1:
                Debug.Log("�U��");
                //gameManager.Players[0].GetComponent<PlayerManager>().EnemyAttack(AttckParam);
                break;
            case 2:
                Debug.Log("�ړ�");
                gameManager.Players[0].GetComponent<PlayerManager>().StartDelay(MoveParam, true);
                break;
            default:
                break;
        }
        this.gameObject.SetActive(false);
        Invoke("CardInfoDestroy", 0.1f);
    }
    // �J�[�h�ڍ׉�ʂ̉摜�Ȃǂ̊��蓖��
    public void GetCardInfo(Sprite img,string tx,int kind,int id)
    {
        CardImg.GetComponent<Image>().sprite = img;
        CardText.GetComponent<Text>().text = tx;
        KindParam = kind;
        IdParam = id;
    }
    // Move�̒l���擾
    public void GetCardMove(int move)
    {
        MoveParam = move;
    }
    // Attck�l�̎擾
    public void GetCardAttck(int power)
    {
        AttckParam = power;
    }
    // �g�����J�[�h�̏����폜
    void CardInfoDestroy()
    {
        CCM.GetComponent<CardCreateManager>().UseCardDestroy(IdParam);

        CardImg.GetComponent<Image>().sprite = null;
        CardText.GetComponent<Text>().text = null;
        KindParam = 0;
        IdParam = 0;
    }
}
