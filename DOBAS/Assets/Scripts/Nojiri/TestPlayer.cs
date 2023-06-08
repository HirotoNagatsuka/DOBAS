using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public GameObject[] effectObject;
    public float moveSpeed = 5f; // �v���C���[�̈ړ����x
    public float rotationSpeed = 180f;  // �v���C���[�̉�]���x

    private GameObject[] animals;

    bool flg = true;
    bool attackFlg = false;
    Animator anim;
    Vector3 PlayerPos;
    Transform animal_parent;

    // Start is called before the first frame update
    void Start()
    {
        animal_parent = GameObject.Find("Animals").transform;

        int count = animal_parent.childCount;
        animals = new GameObject[count];
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // �v���C���[�̈ʒu�����擾
        PlayerPos = transform.position;

        PlayerMove();
        EffectGenerate();
    }

    IEnumerator Delay()
    {
        // �G�t�F�N�g����
        // ��
        GameObject healObj = Instantiate(effectObject[0], PlayerPos, Quaternion.identity);
        anim.SetTrigger("Jump");
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

        // �U��
        Instantiate(effectObject[3], PlayerPos, Quaternion.identity);
        Instantiate(effectObject[4], PlayerPos, Quaternion.identity);
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(2f);

        flg = true;
    }

    void PlayerMove()
    {
        // ���͂��擾
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // �ړ��x�N�g�����v�Z
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;

        // �v���C���[���ړ�������
        transform.Translate(movement);

        // ���L�[�̓��͂��������ĉ�]������
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotation);
        }
    }

    void EffectGenerate()
    {
        // �G�t�F�N�g�𐶐����Ă悢��
        if (flg)
        {
            // Space�������ꂽ�烉���_���ōU��
            if (Input.GetKey(KeyCode.Space))
            {
                // 
                flg = false;
                StartCoroutine("Delay");
            }
        }
    }
}
