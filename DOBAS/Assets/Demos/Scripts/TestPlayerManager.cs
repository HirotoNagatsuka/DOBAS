using Photon.Pun;
using Photon.Realtime; // ★追加
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private PhotonView photonView;
    /*
    private PhotonView photonView;



    #region　変数同期テスト用宣言.
    public int num;
    public string _text;
    public string TestText;
    GameObject NumText;
    #endregion

    public string Text
    {
        get { return _text; }
        set { _text = value; RequestOwner(); }
    }

    void Awake()
    {
        this.photonView = GetComponent<PhotonView>();
        
    }
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

    private void RequestOwner()
    {
        if (this.photonView.IsMine == false)
        {
            if (this.photonView.OwnershipTransfer != OwnershipOption.Request)
                Debug.LogError("OwnershipTransferをRequestに変更してください。");
            else
                this.photonView.RequestOwnership();
        }
    }
    private void Start()
    {
        NumText = GameObject.Find("NumText");//NumTextを取得する.
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //変数同期テスト.
            _text = TestText;
        }
        NumText.GetComponent<Text>().text = _text.ToString();
    }*/
    int value;
    int value2;
    public string _text;
    public string TestText;
    GameObject NumText;
    

    private void Start()
    {
        NumText = GameObject.Find("NumText");//NumTextを取得する.
    }
    // Update is called once per frame
    void Update()
    {
    //    if (photonView.IsMine == false && PhotonNetwork.IsConnected)//自身のみ入力ができる.
    //    {
    //        return;
    //    }
        if (Input.GetKeyDown(KeyCode.P))
        {
            //変数同期テスト.
            _text = TestText;
        }
        NumText.GetComponent<Text>().text = _text.ToString();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_text);
            //stream.SendNext(value);
            //stream.SendNext(value2);//同期したい変数が複数ある場合は、SendNextを複数書く
        }
        else if (stream.IsReading)
        {
            _text = (string)stream.ReceiveNext();
            //value = (int)stream.ReceiveNext();
            //value2 = (int)stream.ReceiveNext();//SendNextを複数書いた場合、こちらも同じ数だけReceiveNextを書き、変数の順番も同じにする
        }
    }
}
