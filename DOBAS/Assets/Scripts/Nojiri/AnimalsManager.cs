using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalsManager : MonoBehaviour
{
    public GameObject[] effectObject;   // �G�t�F�N�g�̃v���n�u�z��
    public GameObject Camera;           // �J�����擾
    public float moveSpeed = 5f;        // �v���C���[�ړ����x
    public float rotationSpeed = 100f;  // �v���C���[��]���x

    [SerializeField] GameObject ChildObject; // �q�I�u�W�F�N�g�擾
    public int ChildNum = 0;       // �������Ă���q�I�u�W�F�N�g�ԍ�
    private bool NowSelect = true;   // �L�����N�^�[�Z���N�gON/OFF
    private bool EffectPrep = true;  // �G�t�F�N�g�����\��

    Animator ChildAnimator; // �q�I�u�W�F�N�gAnimator
    Vector3 PlayerPos;      // �v���C���[�ʒu���

    GameManager gameManager;
    //Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        CharaSelect(); // �L��������
    }

    // Update is called once per frame
    void Update()
    {
        // �v���C���[�̈ʒu�����擾
        PlayerPos = transform.position;
        if(gameManager.NowGameState== GameManager.GameState.SetGame)
        {
            // �L�����Z���N�g���ɁA�L��������]������
            if (NowSelect)
            {
                float rotation = rotationSpeed * Time.deltaTime;
                transform.Rotate(Vector3.up, rotation);
            }
        }
    }

    #region �L��������
    void CharaSelect()
    {
        // �S�Ă̎q�I�u�W�F�N�g���A�N�e�B�u
        for (int i = 0; i < this.transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        if (gameManager.NowGameState != GameManager.GameState.InGame)
        {
            // �ŏ��̈���A�N�e�B�u
            transform.GetChild(ChildNum).gameObject.SetActive(true);
        }
        else
        {
            // �I���������̂��A�N�e�B�u
            transform.GetChild(gameManager.AnimalChildNum).gameObject.SetActive(true);
            ChildObject = transform.GetChild(gameManager.AnimalChildNum).gameObject;
            ChildAnimator = ChildObject.GetComponent<Animator>();
        }
    }
    #endregion

    #region �L�����N�^�[�ύX(�{�^������)
    // ���̃L�����փ{�^������
    public void NextButtonPush()
    {
        CharaSet(true);
    }

    // �O�̃L�����փ{�^������
    public void ReturnButtonPush()
    {
        CharaSet(false);
    }

    void CharaSet(bool flg)
    {
        // ���݂̃A�N�e�B�u�Ȏq�I�u�W�F�N�g���A�N�e�B�u
        transform.GetChild(ChildNum).gameObject.SetActive(false);

        if (flg)
        {
            ChildNum++;
            // �Ō�܂Ő؂�ւ�����ŏ��̃I�u�W�F�N�g�ɖ߂�
            if (ChildNum >= this.transform.childCount) { ChildNum = 0; }
        }
        else
        {
            ChildNum--;
            // �ŏ��܂Ő؂�ւ�����Ō�̃I�u�W�F�N�g�ɖ߂�
            if (ChildNum < 0) { ChildNum = transform.childCount - 1; }
        }

        // ���̎q�I�u�W�F�N�g���A�N�e�B�u
        transform.GetChild(ChildNum).gameObject.SetActive(true);
    }

    // ����{�^������
    public void DecisionButtonPush()
    {
        // ��]���~�߂ăJ�����Ɍ�������
        Vector3 TargetRot = Camera.transform.position - transform.position;
        TargetRot.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(TargetRot);
        transform.rotation = targetRotation;

        // �I�������L�������擾
        ChildObject = transform.GetChild(ChildNum).gameObject;
        ChildAnimator = ChildObject.GetComponent<Animator>();
        ChildAnimator.SetTrigger("Jump"); // ���莞Jump�A�j���[�V�����Đ�

        // �L�����N�^�[�Z���N�gOFF
        NowSelect = false;
    }
    #endregion

    #region �O����Ăяo�����֐��p
    // �J�[�h�擾��
    public void CardGetting()
    {
        StartCoroutine("EffectPreview", "CardGet");
    }

    // �ړ����邩�ǂ����@true�F�ړ��J�n�@false�F�ړ��I��
    public void Moving(bool move)
    {
        if (move)
        {
            ChildAnimator.SetBool("Walk", true); // Walk�A�j���Đ�
        }
        else
        {
            ChildAnimator.SetBool("Walk", false); // Walk�A�j���Đ��I��
        }
    }

    // �U����
    public void Attacking()
    {
        if (ChildAnimator == null) Debug.Log("�����");
        ChildAnimator.SetTrigger("Attack"); // Attack�A�j���Đ�
    }

    // �̗͕ω����@true�F�̗͑����@false�F�̗͌���
    public void HpChange(bool hpChange)
    {
        if (hpChange)
        {
            StartCoroutine("EffectPreview", "HpUp");
        }
        else
        {
            StartCoroutine("EffectPreview", "HpDown");
        }
    }

    // �_���[�W���󂯂��Ƃ������Ă��邩�ǂ����@true�F�����@false�F���S
    public void Damage(bool alive)
    {
        if (alive)
        {
            StartCoroutine("EffectPreview", "Hit");
        }
        else
        {
            StartCoroutine("EffectPreview", "Death");
        }
    }
    #endregion

    #region �G�t�F�N�g����
    IEnumerator EffectPreview(string AnimName)
    {
        switch (AnimName)
        {
            case "CardGet": // �J�[�h�擾��
                GameObject cardObj = Instantiate(effectObject[2], PlayerPos, Quaternion.identity);
                ChildAnimator.SetTrigger("Jump"); // Jump�A�j���Đ�
                yield return new WaitForSeconds(2f);
                Destroy(cardObj);
                break;

            case "HpUp": // �̗͑���
                GameObject healObj = Instantiate(effectObject[0], PlayerPos, Quaternion.identity); // HpUp
                ChildAnimator.SetTrigger("Jump"); // Jump�A�j���Đ�
                yield return new WaitForSeconds(2f);
                Destroy(healObj);
                break;

            //case "Move":
            //    break;

            //case "MoveEnd":
            //    break;

            //case "Attack":
            //    break;

            case "HpDown": // �̗͌���
                GameObject downObj = Instantiate(effectObject[1], PlayerPos, Quaternion.identity); // HpDown
                yield return new WaitForSeconds(2f);
                Destroy(downObj);
                break;

            case "Attack": // �U��
                ChildAnimator.SetTrigger("Attack"); // Attack�A�j���Đ�
                break;

            case "Hit": // �ʏ�_���[�W
                Instantiate(effectObject[3], PlayerPos, Quaternion.identity); // GetHit
                ChildAnimator.SetTrigger(AnimName); // Hit�A�j���Đ�
                break;

            case "Death": // �̗͂O�ɂȂ�Ƃ�
                Instantiate(effectObject[3], PlayerPos, Quaternion.identity); // GetHit
                Instantiate(effectObject[4], PlayerPos, Quaternion.identity); // HpBreak

                yield return new WaitForSeconds(0.5f);
                ChildAnimator.SetTrigger(AnimName); // Death�A�j���Đ�
                yield return new WaitForSeconds(5f);
                transform.GetChild(ChildNum).gameObject.SetActive(false); // 5�b��v���C���[���A�N�e�B�u(���S)
                break;

            default:
                Debug.LogError("AnimName�G���[");
                break;
        }
    }
    #endregion
}
