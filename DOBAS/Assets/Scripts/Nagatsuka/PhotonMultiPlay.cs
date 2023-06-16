using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonMultiPlay : MonoBehaviourPunCallbacks
{
    public string Name;//プレイヤーの名前.
    public int ID;

    [SerializeField] GameManager gameManager; // MapManager参照
    // Start is called before the first frame update
    void Start()
    {
        // プレイヤー自身の名前を"Player"に設定する
        PhotonNetwork.NickName = "Player";
        PhotonNetwork.ConnectUsingSettings();
    }
    #region Photon関連のoverride関数
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // <summary>
    // リモートプレイヤーが入室した際にコールされる
    // </summary>
    public void OnPhotonPlayerConnected()
    {
        Debug.Log("入室");

    }
    public override void OnJoinedRoom()
    {
        gameManager.NowGameState = GameManager.GameState.SetGame;
    }
    #endregion
}
