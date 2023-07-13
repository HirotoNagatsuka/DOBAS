using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("�Q�[�����")]
    [SerializeField] AudioClip charSelect;
    [SerializeField] AudioClip pageFlip;
    [SerializeField] AudioClip pageSkip;
    [SerializeField] AudioClip pageClose;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region �L�����I��SE
    // �L�����Z���N�g�@���̃L���� & �O�̃L������
    public void SelecttButtonPush()
    {
        audioSource.PlayOneShot(charSelect);
    }
    #endregion

    #region �`���[�g���A��������SE
    // �߂��鉹
    public void TutorialFlip()
    {
        audioSource.PlayOneShot(pageFlip);
    }

    // �X�L�b�v��
    public void TutorialSkip()
    {
        audioSource.PlayOneShot(pageSkip);
    }

    // ���鉹
    public void TutorialClose()
    {
        audioSource.PlayOneShot(pageClose);
    }
    #endregion
}
