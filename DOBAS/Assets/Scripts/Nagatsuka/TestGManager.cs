using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class TestGManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] Text HaveTimeText;
    float HaveTime;//�e�v���C���[�̎�������.
    float DoubtTime;//�_�E�g�錾�̎�������.
    bool DoubtFlg;
    bool timeflg;

    // Start is called before the first frame update
    void Start()
    {
        HaveTime = 60;
        DoubtFlg = false;
        timeflg = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //photonView.RPC(nameof(RpcSendMessage), RpcTarget.All, "����ɂ���");
            photonView.RPC(nameof(StartTimer), RpcTarget.All);
        }

        if (DoubtFlg) ChangeDoubtTime();
        //else ChangeHaveTime();
        else if (timeflg) HaveTime -= Time.deltaTime;
        HaveTimeText.text = HaveTime.ToString("0");
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // time�𑗐M����
            stream.SendNext(HaveTime);
        }
        else
        {
            // time����M����
            HaveTime = (float)stream.ReceiveNext();
        }
    }
    [PunRPC]
    private void StartTimer()
    {
        timeflg = true;
    }

    [PunRPC]
    private void RpcSendMessage(string message)
    {
        timeflg = true;
        Debug.Log(message);
    }




    private void ChangeHaveTime()
    {
        if (HaveTime > 0)//�c�莞�Ԃ��c���Ă���Ȃ�.
        {
            HaveTime -= Time.deltaTime;
            if (HaveTime <= 10)//10�b�ȉ��ɂȂ�����Ԃ�����.
            {
                HaveTimeText.color = Color.red;
            }
            HaveTimeText.text = HaveTime.ToString("0");//�����_�ȉ���\�����Ȃ�.
        }
        else//0�ȉ��ɂȂ�����.
        {
            HaveTime = 0;
            Debug.Log("�^�[�������I��");
        }        
    }
    public void StartDoubtTime()
    {
        DoubtTime = 10;
        DoubtFlg = true;
    }
    private void ChangeDoubtTime()
    {
        if (DoubtTime > 0)//�c�莞�Ԃ��c���Ă���Ȃ�.
        {
            DoubtTime -= Time.deltaTime;
            HaveTimeText.color = Color.red;
            
            HaveTimeText.text = DoubtTime.ToString("0");//�����_�ȉ���\�����Ȃ�.
        }
        else//0�ȉ��ɂȂ�����.
        {
            DoubtTime = 0;
            Debug.Log("�^�[�������I��");
            DoubtFlg = false;
        }
    }
}
