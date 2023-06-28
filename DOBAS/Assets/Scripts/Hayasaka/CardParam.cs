using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardParam : MonoBehaviour
{
	//�@�J�[�h�̃i���o�[
	public int Id;
	// �J�[�h�̎��
	public int Kind;
	//�@�A�C�e���̃A�C�R��
	public Sprite Icon;
	//�@�A�C�e���̖��O
	public string CardName;
	//�@�A�C�e���U����
	public int Power;
	//�@�ړ���
	public int Move;

	[SerializeField] GameObject CardDetailPanel;

	GameObject ParentPanel;
	// Start is called before the first frame update
	void Start()
    {
		this.gameObject.GetComponent<Image>().sprite = Icon;

		ParentPanel = GameObject.Find("ParentPanel");

		CardDetailPanel = ParentPanel.transform.GetChild(2).gameObject;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
	public void DrawDetailCard() // �J�[�h�̏ڍ׉�ʕ\��
	{
		ParentPanel.transform.GetChild(2).gameObject.SetActive(true);
		SendParam();
	}
	void SendParam() // �J�[�h�̎�ނ���ʂȂǂ̏ڍ׏���}��
    {
        CardDetailPanel.GetComponent<DetailCardManager>().
        GetCardInfo(Icon, CardName, Kind, Id);

        switch (Kind)
        {
            // 0,���A1,�U���A2,�ړ�
            case 0:
                Debug.Log("�����Ȃ�");
                break;
            case 1:
                CardDetailPanel.GetComponent<DetailCardManager>().GetCardAttck(Power);
                break;
            case 2:
                CardDetailPanel.GetComponent<DetailCardManager>().GetCardMove(Move);
                break;
            default:
                break;
        }
    }
}
