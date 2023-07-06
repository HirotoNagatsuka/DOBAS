using Photon.Pun;
using TMPro;
using UnityEngine;

// MonoBehaviourPunCallbacksを継承して、photonViewプロパティを使えるようにする
public class PlayerNameDisplay : MonoBehaviourPunCallbacks
{
    private GameObject Oya;
    TextMesh nameLabel;
    private void Start()
    {
        Oya = transform.parent.gameObject;
        nameLabel= GetComponent<TextMesh>();
        // プレイヤー名とプレイヤーIDを表示する
        nameLabel.text = $"{photonView.Owner.NickName}({photonView.OwnerActorNr})";
    }
    private void Update()
    {
        nameLabel.text = $"{photonView.Owner.NickName}({photonView.OwnerActorNr})";
    }
}