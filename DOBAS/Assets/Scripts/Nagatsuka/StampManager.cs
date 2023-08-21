using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class StampManager : MonoBehaviourPunCallbacks
{
    GameObject StampImage;
    public Sprite[] Stamp = new Sprite[2];

    /// <summary>
    /// ï\é¶Ç≥ÇÍÇΩÇÁìÆÇ≠ä÷êî.
    /// </summary>
    private void OnEnable()
    {
        StampImage = PhotonNetwork.Instantiate("StampImage", Vector3.zero, Quaternion.identity);
        StampImage.transform.parent = this.transform;
        StampImage.transform.GetChild(1).GetComponent<Text>().text = PhotonNetwork.NickName;
        StampImage.SetActive(false);
    }

    public void PushStamp()
    {
        int id = 0;
        float x = Random.Range(100f, 1200f);
        float y = Random.Range(100f, 1200f);
        photonView.RPC(nameof(ShowStamp), RpcTarget.All, x, y, id);
    }

    [PunRPC]
    void ShowStamp(float x,float y, int id)
    {
        StampImage.SetActive(true);
        Vector2 position = new Vector2(x, y);
        StampImage.transform.position = position;
        StampImage.GetComponent<Image>().sprite = Stamp[id];
        StartCoroutine("WaitStamp");
    }

    private IEnumerator WaitStamp()
    {
        // 3ïbä‘ë“Ç¬
        yield return new WaitForSeconds(3);

        StampImage.SetActive(false);
        yield break;
    }
}
