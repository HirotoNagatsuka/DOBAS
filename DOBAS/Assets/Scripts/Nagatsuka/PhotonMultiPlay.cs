using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonMultiPlay : MonoBehaviourPunCallbacks
{
    public string Name;//プレイヤーの名前.

    [SerializeField] GameManager gameManager; // MapManager参照
    // Start is called before the first frame update
    void Start()
    {
        // プレイヤー自身の名前を"Player"に設定する
        PhotonNetwork.NickName = Name;

        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // <summary>
    // リモートプレイヤーが入室した際にコールされる
    // </summary>
    public void OnPhotonPlayerConnected()
    {
        TurnManager.PlayerNum++;
    }
    public override void OnJoinedRoom()
    {
        gameManager.NowGameState = GameManager.GameState.SetGame;
        if (gameManager.NowGameState == GameManager.GameState.InGame)
        {
            var position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            //PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
        }
    }
}
