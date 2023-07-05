using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;

[Serializable]
class PlayerStatus
{
    public string Name; //名前を格納.
    public int HP;      //HPを格納.
    public int Attack;  //攻撃力を格納.
    public int ID;//デバック用.
}

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region 定数宣言
    const int ATTACK = 4;//攻撃出目の数字.

    //野尻の方で宣言した定数
    private const int SHUKAI = 26;//マスの数（Playerで宣言しているので後々変更）.
    private const float MOVE_SPEED = 10.0f;      // マスを進む速度.
    #endregion

    #region 外部スクリプト参照用宣言
    GameManager gameManager;//GameManager参照用.
    MapManager mapManager; //MapManager参照用.
    [SerializeField]
    CardManager cardManager; // CardListManager参照用(早坂)
    #endregion

    // 早坂
    private int RandomNum;
    public bool NowCardMove = false;

    #region public・SerializeField宣言
    [Header("[SerializeField]宣言")]
    [SerializeField]
    PlayerStatus Player;//Playerのクラスをインスペクター上で見れるようにする.
    #endregion

    public bool doubtFlg;//嘘をついているか判定.
    public bool CoroutineFlg;//スマートではないので修正したい.

    private int MoveMasu;            // Moveマスを踏んだ時の進むマス数
    private bool ActionFlg = true;   // サイコロを振ったかどうか
    bool FinishFlg = false;

    public int Sum = 0;              // 出目の合計
    public int MyRank;
    public Vector3 NowPos;
    public GameObject ResultCamera;
    Animator animator; // Animator
    Vector3 PlayerPos;      // プレイヤー位置情報
    public GameObject[] effectObject;   // エフェクトのプレハブ配列
    public AnimalsManager animalsManager;

    #region Unityイベント(Start・Update・OnTrigger)

    private void Start()
    {
        animalsManager = this.GetComponent<AnimalsManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mapManager  = GameObject.Find("MapManager").GetComponent<MapManager>();
        animator = GetComponent<Animator>();
        //ResultCamera = GameObject.Find("ResultCamera");
        //最初のマスに配置.
        transform.position = mapManager.MasumeList[0].position;//初期値0.
        Player.ID = gameManager.Give_ID_Player();
        MyRank = 0;
        NamePosSet();
    }

    private void Update()
    {
        NowPos = this.transform.position;
        if (photonView.IsMine)
        {
            if (gameManager.NowGameState == GameManager.GameState.InGame)
            {
                Vector3 PlayerPos = transform.position;
                Vector3 TargetPos = mapManager.MasumeList[Sum].position;

                // 移動モーション
                transform.position = Vector3.MoveTowards(PlayerPos, TargetPos, MOVE_SPEED * Time.deltaTime);
                if (gameManager.DiceFinishFlg && CoroutineFlg == false)
                {
                    CoroutineFlg = true;
                    FinishDice();
                }
            }
            else if (gameManager.NowGameState == GameManager.GameState.EndGame)
            {
                Vector3 position =  Vector3.zero;
                Debug.Log("PlayerEndGame起動");
                switch (gameManager.Ranks[PhotonNetwork.LocalPlayer.ActorNumber-1]) {
                    case 0:
                        Debug.Log("MyRank0");
                        position = new Vector3(11.29004f, 0.3792114f, 9.680443f);
                        transform.position = position;
                        break;
                    case 1:
                        Debug.Log("MyRank1");
                        position = new Vector3(12.96002f, 0.3792114f, 9.680443f);
                        transform.position = position;
                        break;
                    case 2:
                        position = new Vector3(14.78003f, 0.3792114f, 9.680443f);
                        transform.position = position;
                        break;
                }
                // カメラに向かせる
                ResultCamera = GameObject.Find("ResultCamera");
                Vector3 TargetRot = ResultCamera.transform.position - transform.position;
                transform.rotation = Quaternion.LookRotation(TargetRot);
                //Jump();
                switch (gameManager.Ranks[PhotonNetwork.LocalPlayer.ActorNumber - 1])
                {
                    case 0:
                        Jump();
                        break;
                    default:
                        Death();
                        break;
                }

            }
        }
    }

    /// <summary>
    /// マスに触れたときタグを参照して効果を呼び出す
    /// </summary>
    private void OnTriggerStay(Collider collision)
    {
        string NowTag = collision.tag; // タグを取得
        // 行動終了時、マスの効果発動
        if (ActionFlg == false)
        {
            ShowText(NowTag);
            mapManager.GetComponent<MapManager>().ColliderReference(collision);  // MapManagerのColliderReference関数処理を行う
            StartCoroutine("Activation", NowTag);  // Activationコルーチンを実行
        }
    }

    #endregion

    #region アニメーション関連

    #region 外から呼び出される関数用
    // カード取得時
    public void CardGetting()
    {
        StartCoroutine("EffectPreview", "CardGet");
    }

    // 移動するかどうか　true：移動開始　false：移動終了
    public void Moving(bool move)
    {
        if (move)
        {
            animator.SetBool("Walk", true); // Walkアニメ再生
        }
        else
        {
            animator.SetBool("Walk", false); // Walkアニメ再生終了
        }
    }

    // 攻撃時
    public void Attacking()
    {
        if (animator == null) Debug.Log("あるよ");
        animator.SetTrigger("Attack"); // Attackアニメ再生
    }

    // 体力変化時　true：体力増加　false：体力減少
    public void HpChange(bool hpChange)
    {
        if (hpChange)
        {
            StartCoroutine("EffectPreview", "HpUp");
        }
        else
        {
            StartCoroutine("EffectPreview", "HpDown");
        }
    }

    // ダメージを受けたとき生きているかどうか　true：生存　false：死亡
    public void Damage(bool alive)
    {
        if (alive)
        {
            StartCoroutine("EffectPreview", "Hit");
        }
        else
        {
            StartCoroutine("EffectPreview", "Death");
        }
    }

    public void Jump()
    {
        animator.SetTrigger("Jump");
    }
    public void Death()
    {
        animator.SetTrigger("Death"); // Deathアニメ再生
    }
    #endregion

    #region エフェクト生成
    IEnumerator EffectPreview(string AnimName)
    {
        switch (AnimName)
        {
            case "CardGet": // カード取得時
                GameObject cardObj = Instantiate(effectObject[2], PlayerPos, Quaternion.identity);
                animator.SetTrigger("Jump"); // Jumpアニメ再生
                yield return new WaitForSeconds(2f);
                Destroy(cardObj);
                break;

            case "HpUp": // 体力増加
                GameObject healObj = Instantiate(effectObject[0], PlayerPos, Quaternion.identity); // HpUp
                animator.SetTrigger("Jump"); // Jumpアニメ再生
                yield return new WaitForSeconds(2f);
                Destroy(healObj);
                break;

            //case "Move":
            //    break;

            //case "MoveEnd":
            //    break;

            //case "Attack":
            //    break;

            case "HpDown": // 体力減少
                GameObject downObj = Instantiate(effectObject[1], PlayerPos, Quaternion.identity); // HpDown
                yield return new WaitForSeconds(2f);
                Destroy(downObj);
                break;

            case "Attack": // 攻撃
                animator.SetTrigger("Attack"); // Attackアニメ再生
                break;

            case "Hit": // 通常ダメージ
                Instantiate(effectObject[3], PlayerPos, Quaternion.identity); // GetHit
                animator.SetTrigger(AnimName); // Hitアニメ再生
                break;

            case "Death": // 体力０になるとき
                Instantiate(effectObject[3], PlayerPos, Quaternion.identity); // GetHit
                Instantiate(effectObject[4], PlayerPos, Quaternion.identity); // HpBreak

                yield return new WaitForSeconds(0.5f);
                animator.SetTrigger(AnimName); // Deathアニメ再生
                yield return new WaitForSeconds(5f);
                //transform.GetChild(ChildNum).gameObject.SetActive(false); // 5秒後プレイヤーを非アクティブ(死亡)
                break;

            default:
                Debug.LogError("AnimNameエラー");
                break;
        }
    }
    #endregion

    #endregion

    #region メッセージ送信関連
    public void ShowText(string tag)
    {
        string message = "";
        bool noneflg = false;
        switch (tag)
        {
            case "Start":
                message = "周回ボーナスゲット！　攻撃力＋１";
                break;
            case "Card":
                message = "カードを１枚ゲット！";
                break;
            case "Move":
                message = "3マス進む！";
                break;
            case "Hp":
                message = "HPが１回復！";
                break;
            case "Attack":
                message = "他のプレイヤーを攻撃！";
                break;
            default:
                //message = "効果なし";
                noneflg = true;
                break;
        }
        if (!noneflg)
        {
            gameManager.ShowMessage(message, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }
    #endregion

    /// <summary>
    /// サイコロを振り終わったら呼び出す関数.
    /// </summary>
    public void FinishDice()
    {
        int deme = gameManager.DeclarationNum;//出目を受け取る.
        gameManager.ReceiveDeme(deme);
        StartCoroutine("WaitDoubt", deme);
    }

    /// <summary>
    /// 他のプレイヤーのダウト宣言を待つコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitDoubt(int deme)
    {
        //Debug.Log("出目" + deme);
        //Debug.Log("gameManager.DeclarationNum" + gameManager.DeclarationNum);
        //Debug.Log("キー入力待ち");
        //Debug.Log("gameManager.DeclarationFlg" + gameManager.DeclarationFlg);
        yield return new WaitUntil(() => gameManager.DeclarationFlg == true); // 待機処理
        if (!gameManager.FailureDoubt)//嘘をついていた時に指摘されていたら動かさない
        {
            Debug.Log("Deme" + deme);
            if (deme == ATTACK)
            {
                Debug.Log("EnemyAttack()起動用if文中");
                EnemyAttack();
            }
            else
            {
                StartDelay(deme, false);
            }
        }
        else
        {
            StartCoroutine(WaitFinishTurnCoroutine());
            yield break;
        }
        gameManager.DiceFinishFlg = false;
        gameManager.DeclarationFlg = false;
        ResetFlg();
        yield return new WaitUntil(() => FinishFlg == true); // 待機処理
        if (GameManager.WhoseTurn == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            Debug.Log("Player側でのgameManager.FinishTurn()起動");
            gameManager.FinishTurn();
        }
        yield break;
    }
    /// <summary>
    /// 2秒経過でターン終了を送信するコルーチン.
    /// </summary>
    private IEnumerator WaitFinishTurnCoroutine()
    {
        // 3秒間待つ
        yield return new WaitForSeconds(2);
        gameManager.FinishTurn();//行動が終わったらターンを終わらせる.
        yield break;
    }

    /// <summary>
    /// フラグの初期化を行う関数
    /// </summary>
    void ResetFlg()
    {
        CoroutineFlg = doubtFlg = false;
    }

    /// <summary>
    /// 他のプレイヤーを攻撃するための関数
    /// 乱数を取得し、乱数と一致したIDをもつプレイヤーを攻撃する.
    /// </summary>
    void EnemyAttack()
    {
        Debug.Log("EnemyAttack()起動");
        int rnd;//乱数用.
        while (true)
        {
            rnd = UnityEngine.Random.Range(1, GameManager.MaxPlayersNum + 1);
            if (PhotonNetwork.LocalPlayer.ActorNumber != rnd && gameManager.PlayersHP[rnd-1] != 0)//自分自身でない場合ループを抜ける.
            {
                        break;                
            }
        }
        //Debug.Log("ループを抜けました");
        //Debug.Log("取得したrnd" + rnd);
        gameManager.ShowMessage(gameManager.PlayersName[rnd - 1]+"に攻撃！", PhotonNetwork.LocalPlayer.ActorNumber);

        gameManager.EnemyAttack(-1, rnd);
        FinishFlg = true;
    }
    #region EnemyAttackオーバーロード
    /// <summary>
    /// 他のプレイヤーを攻撃するための関数
    /// 乱数を取得し、乱数と一致したIDをもつプレイヤーを攻撃する.
    /// </summary>
    [PunRPC]
    public void EnemyAttack(int powNum)//引数追加(早坂)
    {
        Debug.Log("EnemyAttack()起動");
        int rnd;//乱数用.
        while (true)
        {
            rnd = UnityEngine.Random.Range(1, GameManager.MaxPlayersNum + 1);
            if (PhotonNetwork.LocalPlayer.ActorNumber != rnd)//自分自身でない場合ループを抜ける.
            //if (Player.ID != rnd)//自分自身でない場合ループを抜ける.
            {
                break;
            }
        }
        Debug.Log("ループを抜けました");
        Debug.Log("取得したrnd" + rnd);
        FinishFlg = true;
        photonView.RPC(nameof(ChangeHP), RpcTarget.All, powNum, rnd);
    }
    #endregion

    /// <summary>
    /// HPが変化するときに呼び出す関数
    /// 変化量を引数にし、HPを変えた後UIにも反映する.
    /// 第二引数には自分自身が呼び出したのかを判定.
    /// </summary>
    void ChangeHP(int addHP, int subject)
    {
            gameManager.ChangePlayersHP(addHP, subject);
    }

    #region 移動関連
    /// <summary>
    /// OnTriggerで取得したタグごとに効果を発動するコルーチン
    ///引数に文字列でタグを取得し、分岐する.
    /// </summary>
    IEnumerator Activation(string tag)
    {
        ActionFlg = true;
        if (!NowCardMove) // カード移動時にマスの効果を受けない(早坂)
        {
            yield return new WaitUntil(() => gameManager.FinMessage == true); // 待機処理
            // タグごとに分類
            if (tag == "Start") // スタートマス
            {
                //Debug.Log("周回ボーナスゲット！");
                yield return new WaitForSeconds(2); // 2秒待つ
            }
            else if (tag == "Card") // カードマス
            {
                Debug.Log("カード１枚ゲット！");
                yield return new WaitForSeconds(2);
               // SendCardList(); // 早坂(未完)
            }
            else if (tag == "Move") // 移動マス
            {
                Debug.Log("3マス進む！");
                yield return new WaitForSeconds(2);

                MoveMasu = mapManager.GetComponent<MapManager>().Move(MoveMasu, tag);  // MapManagerのMove関数処理を行う
                StartCoroutine("DelayMove", MoveMasu);                            // １マスづつ進む
            }
            else if (tag == "Hp") // HPマス
            {
                Debug.Log("HP１回復！！");
                yield return new WaitForSeconds(2);

                Debug.Log("HP：" + Player.HP);
                //photonView.RPC(nameof(ChangeHP), RpcTarget.All, 1, Player.ID);
                //ChangeHP(1, PhotonNetwork.LocalPlayer.ActorNumber);
                gameManager.ChangePlayersHP(1, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else if (tag == "HpDown") // HPマス
            {
                //Debug.Log("HP１回復！！");
                yield return new WaitForSeconds(2);
                //photonView.RPC(nameof(ChangeHP), RpcTarget.All, -1, Player.ID);
                //ChangeHP(-1, PhotonNetwork.LocalPlayer.ActorNumber);
                gameManager.ChangePlayersHP(-1, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else if (tag == "Attack") //攻撃マス
            {
                Debug.Log("他のプレイヤーを攻撃！");
                yield return new WaitForSeconds(2);
                EnemyAttack();
            }
            else // ノーマルマス
            {
                Debug.Log("普通のマス");
                yield return new WaitForSeconds(2);
            }
            FinishFlg = true;
        }
    }
    /// <summary>
    /// 数値の出目を受け取ったら呼び出す関数
    /// 引数に出目を受け取り移動用コルーチンを起動する.
    /// </summary>
    public void StartDelay(int num)
    {
        Debug.Log("StartDeley起動");
        // コルーチンの開始
        StartCoroutine("DelayMove", num);
    }
    /// <summary>
    /// 数値の出目を受け取ったら呼び出す関数(オーバーロード)
    /// 引数に出目を受け取り移動用コルーチンを起動する.
    /// </summary>
    public void StartDelay(int num, bool cardMove) // 第二引数追加(早坂)
    {
        Debug.Log("StartDeley(OverLoad)起動");
        NowCardMove = cardMove; //(早坂)
        StartCoroutine("DelayMove", num);        // コルーチンの開始
    }
    /// <summary>
    /// 引数で受け取った分マスを移動指せるコルーチン.
    /// </summary>
    IEnumerator DelayMove(int num)
    {
        // １マスづつ進ませる
        for (int i = 0; i < num; i++)
        {
            Sum++;  // 現在のマス番号(サイコロ目の合計)

            // スタート地点に戻る
            if (Sum >= SHUKAI)
            {
                Sum = 0;
            }

            yield return new WaitForSeconds(0.5f); // 0.5秒待つ
        }
        ActionFlg = false; // プレイヤーの行動終了(マス効果発動前)
    }
    #endregion

    /// <summary>
    /// PUN2を使った変数同期.
    /// ここで同期したい変数を全て送信する.
    /// </summary>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 自身のアバターのHPを送信する
            stream.SendNext(Player.HP);
            stream.SendNext(Sum);
        }
        else
        {
            // 他プレイヤーのアバターのHPを受信する
            Player.HP = (int)stream.ReceiveNext();
            Sum = (int)stream.ReceiveNext();
        }


    }
    #region 早坂追加
    public void SendCardList()
    {
        RandomNum = UnityEngine.Random.Range(0, cardManager.GetCardLists().Count);
    }
    #endregion

    [SerializeField] GameObject nameObject;

    void NamePosSet()
    {
        nameObject.transform.localPosition = new Vector3(0, 0, -1.5f);
        nameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }
}