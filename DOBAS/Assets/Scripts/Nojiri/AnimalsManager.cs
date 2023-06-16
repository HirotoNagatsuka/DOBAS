using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalsManager : MonoBehaviour
{
    public GameObject[] effectObject;   // �G�t�F�N�g�̃v���n�u�z��
    public GameObject Camera;           // �J�����擾
    public float moveSpeed = 5f;        // �v���C���[�̈ړ����x
    public float rotationSpeed = 100f;  // �v���C���[�̉�]���x

    [SerializeField] GameObject ChildObject; // �q�I�u�W�F�N�g�擾
    [SerializeField] int ChildNum = 0;        // �������Ă���q�I�u�W�F�N�g�ԍ�
    bool NowSelect = true;   // �L�����N�^�[�Z���N�gON
    bool EffectPrep = true;  // �G�t�F�N�g�����\��

    Animator ChildAnimator; // �q�I�u�W�F�N�g��
    Vector3 PlayerPos;      // �v���C���[�ʒu

    //Start is called before the first frame update
    void Start()
    {
        CharaSelect();

        //// �A�N�e�B�u�ȃA�o�^�[���擾�ς݂̂Ƃ�
        //if (avatarSet != null)
        //{
        //    // �擾�����A�o�^�[���Z�b�g
        //    animator.avatar = avatarSet;
        //}
        //else
        //{
        //    Debug.LogError("�A�N�e�B�u�Ȏq�I�u�W�F�N�g��Animator��Avatar��������܂���");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        // �v���C���[�̈ʒu�����擾
        PlayerPos = transform.position;

        // �L�����Z���N�g���A��]������
        if (NowSelect)
        {
            float rotation = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotation);
        }

        if(!NowSelect) EffectGenerate();
    }

    #region �G�t�F�N�g����
    void EffectGenerate()
    {
        // �G�t�F�N�g�𐶐����Ă悢��
        if (EffectPrep)
        {
            // Space�������ꂽ�烉���_���ōU��
            if (Input.GetKey(KeyCode.Space))
            {
                EffectPrep = false;
                StartCoroutine("EffectPreview");
            }
        }
    }
    #endregion

    #region �A�o�^�[�؂�ւ�
    //private Avatar FindAvatarInChildren(Transform parent)
    //{
    //    Avatar childAvatar = null;

    //    // �q�I�u�W�F�N�g�̐����[�v
    //    for(int i = 0; i < parent.childCount; i++)
    //    {
    //        Transform child = parent.GetChild(i);  // �q��S�Ď擾
    //        childAvatar = child.GetComponent<Animator>()?.avatar; // Animator��?(null)�łȂ��Ƃ�avatar���擾
    //        Debug.Log(childAvatar);

    //        // avatar���擾���Ă���Ƃ�
    //        if (childAvatar != null)
    //        {
    //            break;
    //        }
    //    }

    //    // avatar��������Ȃ��Ƃ�null��Ԃ�
    //    return null;
    //}
    #endregion

    #region �L��������
    void CharaSelect()
    {
        // �S�Ă̎q�I�u�W�F�N�g���A�N�e�B�u
        for (int i = 0; i < this.transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        // �ŏ��̈���A�N�e�B�u
        transform.GetChild(ChildNum).gameObject.SetActive(true);

    }
    #endregion

    #region �L�����N�^�[�ύX
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

    #region �G�t�F�N�g�����p
    IEnumerator EffectPreview()
    {
        // ��
        GameObject healObj = Instantiate(effectObject[0], PlayerPos, Quaternion.identity); // HpUp
        ChildAnimator.SetTrigger("Jump"); // Jump�A�j���[�V�����Đ�
        yield return new WaitForSeconds(2f);
        Destroy(healObj);

        // �̗͌���
        GameObject downObj = Instantiate(effectObject[1], PlayerPos, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Destroy(downObj);

        // �J�[�h
        GameObject cardObj = Instantiate(effectObject[2], PlayerPos, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Destroy(cardObj);

        // ��U����
        Instantiate(effectObject[3], PlayerPos, Quaternion.identity); // GetHit
        Instantiate(effectObject[4], PlayerPos, Quaternion.identity); // HpBreak
        yield return new WaitForSeconds(0.5f);
        ChildAnimator.SetTrigger("Death"); // Death�A�j���[�V�����Đ�
        yield return new WaitForSeconds(5f);
        transform.GetChild(ChildNum).gameObject.SetActive(false); // �v���C���[���A�N�e�B�u(���S)

        EffectPrep = true;
    }
    #endregion
}
