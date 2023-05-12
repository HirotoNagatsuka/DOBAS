using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; // ★追加
using Photon.Realtime; // ★追加

public class PhotonManager : MonoBehaviourPunCallbacks// ★変更
{
    // インスペクターから設定
    public GameObject Player;
    private int PlayerNum;//プレイヤーの数.
    #region　変数同期テスト用宣言.
    public int num;
    [SerializeField] Text NumText;
    public string _text;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //ConnectUsingSettingsメソッドを使うことで、
        //先ほど設定した「PhotonServerSettings」を用いてサーバに接続することができる.
        PhotonNetwork.ConnectUsingSettings();
    }
    #region MonoBehaviourPunCallbacksクラスが提供する4つのコールバックメソッドをオーバーライド
    /// <summary>
    /// サーバへの接続が完了すると呼ばれるコールバック
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    /// <summary>
    /// ロビーへの入室が完了した時に呼ばれるコールバック.
    /// </summary>
    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    // 入室に失敗した場合に呼ばれるコールバック
    // １人目は部屋がないため必ず失敗するので部屋を作成する
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8; // 最大8人まで入室可能
        PhotonNetwork.CreateRoom(null, roomOptions); //第一引数はルーム名
    }
    /// <summary>
    /// 入室が完了した時に呼ばれるコールバック.
    /// </summary>
    public override void OnJoinedRoom()
    {
        if (PlayerNum == 0)
        {
            PhotonNetwork.Instantiate(
            Player.name,
            new Vector3(0f, 1f, 1f),    //ポジション
            Quaternion.identity,    //回転
            0
            );
        }
        else if (PlayerNum == 1)
        {
            PhotonNetwork.Instantiate(
            Player.name,
            new Vector3(0f, 1f, -1f),    //ポジション
            Quaternion.identity,    //回転
            0
            );
        }
        //GameObject mainCamera = GameObject.FindWithTag("MainCamera");
        //mainCamera.GetComponent<UnityChan.ThirdPersonCamera>().enabled = true;
    }
    #endregion

    /// <summary>
    /// 変数同期
    /// </summary>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // オーナーの場合
        if (stream.IsWriting)
        {
            stream.SendNext(this._text);
        }
        // オーナー以外の場合
        else
        {
            this._text = (string)stream.ReceiveNext();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //変数同期テスト.
            _text = "test";
        }
        //NumText.text = _text.ToString();
    }

    void OnPhotonInstantiate(PhotonMessageInfo info) {
    
    }
}
