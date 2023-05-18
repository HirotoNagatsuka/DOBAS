using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DiceManager : MonoBehaviour
{
    [SerializeField] TestGManager testGManager;
    public GameObject DicePrefab;//�T�C�R���̃v���t�@�u������.
    private GameObject Dice;//�T�C�R���p�̕\���E��\�����J��Ԃ��p.
    public int num;
    #region �����_���ɉ�]������p�̕ϐ��錾.
    private int rotateX;
    private int rotateY;
    private int rotateZ;
    #endregion

    [SerializeField] Sprite[] SaikoroSprite = new Sprite[4];
    [SerializeField] GameObject DiceCamera;
    [SerializeField] Text DiceNumText;
    [SerializeField] GameObject[] ResultPanel=new GameObject[5];//�o�ڂ̃p�l���p.
    [SerializeField] GameObject ReasoningPanel;
    Vector3 CameraPos;
    public Transform kodomo; //�q�I�u�W�F�N�g.
    private bool DiceFlg;//�T�C�R���쐬�t���O.
    [SerializeField] GameObject DiceShakeButton;
    private int Number;
    public int DeclarationNum;//�錾�ԍ�.
    public bool Doubt;
    [SerializeField] MapPlayer mapPlayer;
    [SerializeField] PlayerManager playerManager;

    private void Start()
    {
        //mapPlayer = GetComponent<MapPlayer>();
        Dice = Instantiate(DicePrefab, DiceCamera.transform.position, Quaternion.identity, kodomo);
        Dice.transform.position = CameraPos;
        CameraPos = DiceCamera.transform.position;
        CameraPos.x += 3;
        CameraPos.y -= 3;
        Dice.SetActive(false);
        DiceCamera.SetActive(false);
        DiceFlg = false;
    }

    /// <summary>
    /// �T�C�R����U��p�̊֐�
    /// �T�C�R���̃v���t�@�u��������΍쐬���A�L��Ε\���E��\�����J��Ԃ�.
    /// </summary>
    public void ShakeDice()
    {
        DiceCamera.SetActive(true);
        Dice.SetActive(true);
        Dice.transform.position = CameraPos;
        CameraPos = DiceCamera.transform.position;
        CameraPos.x += 3;
        CameraPos.y -= 3;
        rotateX = Random.Range(0, 360);
        rotateY = Random.Range(0, 360);
        rotateZ = Random.Range(0, 360);
        Dice.GetComponent<Rigidbody>().AddForce(-transform.right * 300);
        Dice.transform.Rotate(rotateX, rotateY, rotateZ);
        DiceFlg = true;
        DiceShakeButton.SetActive(false);
    }

    // �R���[�`���{��
    private IEnumerator HiddenDiceCoroutine()
    {
        // 2�b�ԑ҂�
        yield return new WaitForSeconds(2);
        Debug.Log("�R���[�`���Ăяo���I��");
        ActivePanel();
        yield break;
    }

    /// <summary>
    /// �o�ڊm��AUI�ɐ��l�╶����\������
    /// </summary>
    public void ConfirmNumber(int num)
    {
        Number = num;
        if (num == 4)
        {
            DiceNumText.text = "Attack";
        }
        else if (num == 5 || num == 6)
        {
            DiceNumText.text = "Doubt";
        }
        else
        {
            DiceNumText.text = num.ToString();
        }
        StartCoroutine(HiddenDiceCoroutine());
    }

    public void PushShakeDiceButton()
    {
        if (DiceFlg == false)
        {
            ShakeDice();
        }
    }

    /// <summary>
    /// �o�ڂɂ���ĕ\������p�l����ς���.
    /// </summary>
    private void ActivePanel()
    {
        if (Number == 1)
        {
            ResultPanel[0].SetActive(true);
        }
        else if (Number == 2)
        {
            ResultPanel[1].SetActive(true);
        }
        else if (Number == 3)
        {
            ResultPanel[2].SetActive(true);
        }
        else if (Number == 4)
        {
            ResultPanel[3].SetActive(true);
        }
        else//�_�E�g�̏ꍇ�t���O��On�ɂ���.
        {
            ResultPanel[4].SetActive(true);
            Doubt = true;
        }
    }

    #region �e�o�ڂ̃{�^��
    /// <summary>
    /// 1�̏o�ڃ{�^������������.
    /// </summary>
    public void PushOneButton()
    {
        DeclarationNum = 1;
        DeclarationResult();
    }
    /// <summary>
    /// 2�̏o�ڃ{�^������������.
    /// </summary>
    public void PushTwoButton()
    {
        DeclarationNum = 2;
        DeclarationResult();
    }
    /// <summary>
    /// 3�̏o�ڃ{�^������������.
    /// </summary>
    public void PushThreeButton()
    {
        DeclarationNum = 3;
        DeclarationResult();
    }
    /// <summary>
    /// �U���o�ڃ{�^������������.
    /// </summary>
    public void PushAttackButton()
    {
        DeclarationNum = 4;
        DeclarationResult();
    }
    #endregion

    private void DeclarationResult()
    {
        playerManager.StartDelay(DeclarationNum);
        // mapPlayer.StartDelay(DeclarationNum);
        /*
        if (Doubt)
        {
            ResultPanel[4].SetActive(false);
        }
        else
        {
            ResultPanel[DeclarationNum - 1].SetActive(false);
        }*/
        //��X�C��.
        ResultPanel[0].SetActive(false);
        ResultPanel[1].SetActive(false);
        ResultPanel[2].SetActive(false);
        ResultPanel[3].SetActive(false);
        ResultPanel[4].SetActive(false);


        Debug.Log("�o�ځF" + DeclarationNum);
        DiceNumText.text = " ";
        DiceCamera.SetActive(false);
        Dice.SetActive(false);
        DiceFlg = false;
        DiceInit();
        /*
        ReasoningPanel.SetActive(true);
        ReasoningPanel.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = SaikoroSprite[DeclarationNum-1];
        testGManager.StartDoubtTime();*/
    }

    public void DiceInit()
    {
        DiceNumText.text = " ";
        DeclarationNum = 0;
        ReasoningPanel.SetActive(false);
        DiceCamera.SetActive(false);
        Dice.SetActive(false);
        DiceFlg = false;
        DiceShakeButton.SetActive(true);
    }

    /// <summary>
    /// �錾���ꂽ�o�ڂ�Ԃ��֐�
    /// </summary>
    public int ReturnDeclarationNum()
    {
        return DeclarationNum;
    }
}
