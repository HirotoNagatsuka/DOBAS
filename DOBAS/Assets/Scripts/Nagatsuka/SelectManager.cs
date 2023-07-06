using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SelectManager : MonoBehaviour
{

    const int INPUT_NAME = 2;//名前入力の子オブジェクトを参照用.
    #region ゲーム開始前（準備完了）関連
    [Header("ゲーム開始前（準備完了）関連")]
    [SerializeField] GameObject TutorialCanvas;   //遊び方説明画面.
    [SerializeField] InputField InputNickName;    //名前を入力する用.
    [SerializeField] GameObject CanvasUI;         //ゲーム開始時にまとめてキャンバスを非表示にする.
    [SerializeField] GameObject StartButton;      //準備完了ボタン.
    [SerializeField] GameObject StandByCanvas;    //準備完了キャンバス.
    [SerializeField] Text HelloPlayerText;        //待機中にプレイヤーを表示するボタン.
    [SerializeField] Text StandByText;            //待機人数を表示するテキスト.
    [SerializeField] AnimalsManager SelectAnimals;//選んだアバターを表示する.
    [SerializeField] GameObject ChangeButtons;    //キャラクター変更用ボタン.
    [SerializeField] GameObject StandByGroup;   //準備完了のグループ.
    #endregion

    public static int AnimalChildNum;//選んだキャラクターを保存するための宣言.
    /// <summary>
    /// 準備完了ボタンを押した際に呼び出す関数.
    /// </summary>
    public void PushGameStart()
    {
        PhotonNetwork.LocalPlayer.SetReadyNum(true);
        Debug.Log("player.GetReady()直後" + PhotonNetwork.LocalPlayer.GetReadyNum());
        StartButton.SetActive(false);                         //ボタンを押したら非表示にする.
        AnimalChildNum = SelectAnimals.ChildNum;
        PhotonNetwork.NickName = InputNickName.transform.GetChild(INPUT_NAME).GetComponent<Text>().text;// プレイヤー自身の名前を入力された名前に設定する
    }
}
