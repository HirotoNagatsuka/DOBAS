using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Dice : MonoBehaviour
{
    #region �萔�錾
    const int DICE_UI = 0;
    #endregion
    Rigidbody rbody;
    private int number;//�o�ڂ�����.
    private GameObject Stage;//�X�e�[�W�ɓ��������o�ڂ𔻒肷�邽�߂̕ϐ��錾.
    private bool flg;
    private bool Hitflg;//�T�C�R������]������Ƃ��Ɉ�x�X�e�[�W�ɓ����������]���~�߂�.

    #region Start�EUpdate�֐�
    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        Stage = GameObject.Find("DiceStage");//�X�e�[�W���擾����.
    }
    private void Update()
    {
        if (!Hitflg)
        {
            transform.Rotate(-3f, 0, 3f);
        }
    }
    #endregion

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Stage")//�������Ă�����̂��X�e�[�W������.
        {
            Hitflg = true;
            if (rbody.velocity.magnitude == 0 && flg==false)//�}�O�j�`���[�h��0�̏ꍇ�i��������j.
            {
                Stage.GetComponent<DiceStage>().ReturnNumber();//�X�e�[�W�ōs���Ă���o�ڔ����Ԃ�.
                flg = true;

            }
        }
    }
    /// <summary>
    /// �T�C�R�����\�����ꂽ�瓮���֐�.
    /// </summary>
    private void OnEnable()
    {
        flg = false;
        Hitflg = false;
    }
}
