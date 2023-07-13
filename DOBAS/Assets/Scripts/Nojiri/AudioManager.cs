using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("ゲーム状態")]
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

    #region キャラ選択SE
    // キャラセレクト　次のキャラ & 前のキャラへ
    public void SelecttButtonPush()
    {
        audioSource.PlayOneShot(charSelect);
    }
    #endregion

    #region チュートリアル説明書SE
    // めくる音
    public void TutorialFlip()
    {
        audioSource.PlayOneShot(pageFlip);
    }

    // スキップ音
    public void TutorialSkip()
    {
        audioSource.PlayOneShot(pageSkip);
    }

    // 閉じる音
    public void TutorialClose()
    {
        audioSource.PlayOneShot(pageClose);
    }
    #endregion
}
