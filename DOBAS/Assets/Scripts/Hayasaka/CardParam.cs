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

	GameObject CCM;
	// Start is called before the first frame update
	void Start()
    {
		this.gameObject.GetComponent<Image>().sprite = Icon;

		CCM = GameObject.Find("CardListPanel");
	}

    // Update is called once per frame
    void Update()
    {
        
    }
	public void DrawDetailCard() // �J�[�h�̏ڍ׉�ʕ\��
	{
		Instantiate(CardDetailPanel, CCM.transform);

		Invoke("DelaySendParam", 0.5f);
	}
	void DelaySendParam() // �J�[�h�̎�ނ���ʂȂǂ̏ڍ׏���}��
    {
		CardDetailPanel.GetComponent<DetailCardManager>().GetCardInfo(Icon, CardName, Kind,Id);

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
