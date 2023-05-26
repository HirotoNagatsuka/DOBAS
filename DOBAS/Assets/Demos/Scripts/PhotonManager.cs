using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; // ���ǉ�
using Photon.Realtime; // ���ǉ�

public class PhotonManager : MonoBehaviourPunCallbacks// ���ύX
{
    // �C���X�y�N�^�[����ݒ�
    public GameObject Player;
    private int PlayerNum;//�v���C���[�̐�.
    #region�@�ϐ������e�X�g�p�錾.
    public int num;
    [SerializeField] Text NumText;
    public string _text;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //ConnectUsingSettings���\�b�h���g�����ƂŁA
        //��قǐݒ肵���uPhotonServerSettings�v��p���ăT�[�o�ɐڑ����邱�Ƃ��ł���.
        PhotonNetwork.ConnectUsingSettings();
    }
    #region MonoBehaviourPunCallbacks�N���X���񋟂���4�̃R�[���o�b�N���\�b�h���I�[�o�[���C�h
    /// <summary>
    /// �T�[�o�ւ̐ڑ�����������ƌĂ΂��R�[���o�b�N
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    /// <summary>
    /// ���r�[�ւ̓����������������ɌĂ΂��R�[���o�b�N.
    /// </summary>
    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    // �����Ɏ��s�����ꍇ�ɌĂ΂��R�[���o�b�N
    // �P�l�ڂ͕������Ȃ����ߕK�����s����̂ŕ������쐬����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8; // �ő�8�l�܂œ����\
        PhotonNetwork.CreateRoom(null, roomOptions); //�������̓��[����
    }
    /// <summary>
    /// �����������������ɌĂ΂��R�[���o�b�N.
    /// </summary>
    public override void OnJoinedRoom()
    {
        if (PlayerNum == 0)
        {
            PhotonNetwork.Instantiate(
            Player.name,
            new Vector3(0f, 1f, 1f),    //�|�W�V����
            Quaternion.identity,    //��]
            0
            );
        }
        else if (PlayerNum == 1)
        {
            PhotonNetwork.Instantiate(
            Player.name,
            new Vector3(0f, 1f, -1f),    //�|�W�V����
            Quaternion.identity,    //��]
            0
            );
        }
        //GameObject mainCamera = GameObject.FindWithTag("MainCamera");
        //mainCamera.GetComponent<UnityChan.ThirdPersonCamera>().enabled = true;
    }
    #endregion

    /// <summary>
    /// �ϐ�����
    /// </summary>
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //�ϐ������e�X�g.
            _text = "test";
        }
        //NumText.text = _text.ToString();
    }

    void OnPhotonInstantiate(PhotonMessageInfo info) {
    
    }
}
