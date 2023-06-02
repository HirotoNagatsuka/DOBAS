using Photon.Pun;
using TMPro;
using UnityEngine;

// MonoBehaviourPunCallbacks���p�����āAphotonView�v���p�e�B���g����悤�ɂ���
public class PlayerNameDisplay : MonoBehaviourPunCallbacks
{
    private GameObject Oya;
    TextMesh nameLabel;
    private void Start()
    {
        Oya = transform.parent.gameObject;
        nameLabel= GetComponent<TextMesh>();
        // �v���C���[���ƃv���C���[ID��\������
        nameLabel.text = $"{photonView.Owner.NickName}({photonView.OwnerActorNr})";
    }
    private void Update()
    {
        nameLabel.text = $"{photonView.Owner.NickName}({photonView.OwnerActorNr})";
    }
}