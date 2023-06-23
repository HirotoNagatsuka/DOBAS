using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonMultiPlay : MonoBehaviourPunCallbacks
{
    public string Name;//�v���C���[�̖��O.
    public int ID;

    [SerializeField] GameManager gameManager; // MapManager�Q��
    // Start is called before the first frame update
    void Start()
    {
        // �v���C���[���g�̖��O��"Player"�ɐݒ肷��
        PhotonNetwork.NickName = "Player";
        PhotonNetwork.ConnectUsingSettings();
    }
    #region Photon�֘A��override�֐�
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // <summary>
    // �����[�g�v���C���[�����������ۂɃR�[�������
    // </summary>
    public void OnPhotonPlayerConnected()
    {
        Debug.Log("����");

    }
    public override void OnJoinedRoom()
    {
        gameManager.NowGameState = GameManager.GameState.SetGame;
    }
    #endregion
}
