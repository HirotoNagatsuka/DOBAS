using Photon.Pun;
using Photon.Realtime; // ���ǉ�
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private PhotonView photonView;
    /*
    private PhotonView photonView;



    #region�@�ϐ������e�X�g�p�錾.
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
        // �I�[�i�[�̏ꍇ
        if (stream.IsWriting)
        {
            stream.SendNext(this._text);
        }
        // �I�[�i�[�ȊO�̏ꍇ
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
                Debug.LogError("OwnershipTransfer��Request�ɕύX���Ă��������B");
            else
                this.photonView.RequestOwnership();
        }
    }
    private void Start()
    {
        NumText = GameObject.Find("NumText");//NumText���擾����.
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //�ϐ������e�X�g.
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
        NumText = GameObject.Find("NumText");//NumText���擾����.
    }
    // Update is called once per frame
    void Update()
    {
    //    if (photonView.IsMine == false && PhotonNetwork.IsConnected)//���g�̂ݓ��͂��ł���.
    //    {
    //        return;
    //    }
        if (Input.GetKeyDown(KeyCode.P))
        {
            //�ϐ������e�X�g.
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
            //stream.SendNext(value2);//�����������ϐ�����������ꍇ�́ASendNext�𕡐�����
        }
        else if (stream.IsReading)
        {
            _text = (string)stream.ReceiveNext();
            //value = (int)stream.ReceiveNext();
            //value2 = (int)stream.ReceiveNext();//SendNext�𕡐��������ꍇ�A�����������������ReceiveNext�������A�ϐ��̏��Ԃ������ɂ���
        }
    }
}
