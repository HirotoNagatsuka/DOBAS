using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class OnlineGameManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
{
    PunTurnManager punTurnManager = default;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    void Start()
    {
        SetupTurnManager();
    }

    void BeginMyTurn()
    {

    }
    void ShareMove(object move)
    {

    }


    void SetupTurnManager()
    {
        punTurnManager = GetComponent<PunTurnManager>();
        punTurnManager.enabled = true;
        punTurnManager.TurnManagerListener = this;
    }
    /// <summary>
    /// プレイヤーの入室を検出し、ゲームスタートを管理する関数
    /// </summary>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom: " + newPlayer.NickName);

        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                punTurnManager.BeginTurn();
            }
        }
    }
    void IPunTurnManagerCallbacks.OnTurnBegins(int turn)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.BeginMyTurn();
        }
    }
    void IPunTurnManagerCallbacks.OnPlayerMove(Photon.Realtime.Player player, int turn, object move)
    {
        this.ShareMove(move);
    }
    void IPunTurnManagerCallbacks.OnPlayerFinished(Photon.Realtime.Player player, int turn, object move)
    {
        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.LocalPlayer.ActorNumber == player.ActorNumber + 1)
        {
            this.BeginMyTurn();
        }
    }
    void IPunTurnManagerCallbacks.OnTurnCompleted(int turn)
    {
        punTurnManager.BeginTurn();
    }
    void IPunTurnManagerCallbacks.OnTurnTimeEnds(int turn)
    {
        punTurnManager.BeginTurn();
    }
}