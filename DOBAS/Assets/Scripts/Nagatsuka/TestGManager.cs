using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestGManager : MonoBehaviour
{
    [SerializeField] Text HaveTimeText;
    float HaveTime;//各プレイヤーの持ち時間.
    
    // Start is called before the first frame update
    void Start()
    {
        HaveTime = 13;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeHaveTime();
    }

    private void ChangeHaveTime()
    {
        if (HaveTime > 0)//残り時間が残っているなら.
        {
            HaveTime -= Time.deltaTime;
            if (HaveTime <= 10)//10秒以下になったら赤くする.
            {
                HaveTimeText.color = Color.red;
            }
            HaveTimeText.text = HaveTime.ToString("0");//小数点以下を表示しない.
        }
        else//0以下になったら.
        {
            HaveTime = 0;
            Debug.Log("ターン強制終了");
        }        
    }

    /// <summary>
    /// サイコロを振る関数.
    /// </summary>
    public void ShakeDice()
    {

    }
}
