using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SelectManager : MonoBehaviour
{

    const int INPUT_NAME = 2;//���O���͂̎q�I�u�W�F�N�g���Q�Ɨp.
    #region �Q�[���J�n�O�i���������j�֘A
    [Header("�Q�[���J�n�O�i���������j�֘A")]
    [SerializeField] GameObject TutorialCanvas;   //�V�ѕ��������.
    [SerializeField] InputField InputNickName;    //���O����͂���p.
    [SerializeField] GameObject CanvasUI;         //�Q�[���J�n���ɂ܂Ƃ߂ăL�����o�X���\���ɂ���.
    [SerializeField] GameObject StartButton;      //���������{�^��.
    [SerializeField] GameObject StandByCanvas;    //���������L�����o�X.
    [SerializeField] Text HelloPlayerText;        //�ҋ@���Ƀv���C���[��\������{�^��.
    [SerializeField] Text StandByText;            //�ҋ@�l����\������e�L�X�g.
    [SerializeField] AnimalsManager SelectAnimals;//�I�񂾃A�o�^�[��\������.
    [SerializeField] GameObject ChangeButtons;    //�L�����N�^�[�ύX�p�{�^��.
    [SerializeField] GameObject StandByGroup;   //���������̃O���[�v.
    #endregion

    public static int AnimalChildNum;//�I�񂾃L�����N�^�[��ۑ����邽�߂̐錾.
    /// <summary>
    /// ���������{�^�����������ۂɌĂяo���֐�.
    /// </summary>
    public void PushGameStart()
    {
        PhotonNetwork.LocalPlayer.SetReadyNum(true);
        Debug.Log("player.GetReady()����" + PhotonNetwork.LocalPlayer.GetReadyNum());
        StartButton.SetActive(false);                         //�{�^�������������\���ɂ���.
        AnimalChildNum = SelectAnimals.ChildNum;
        PhotonNetwork.NickName = InputNickName.transform.GetChild(INPUT_NAME).GetComponent<Text>().text;// �v���C���[���g�̖��O����͂��ꂽ���O�ɐݒ肷��
    }
}
