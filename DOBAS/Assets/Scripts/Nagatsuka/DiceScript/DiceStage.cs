using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceStage : MonoBehaviour
{
    [SerializeField] Text DiceNumText;
    [SerializeField] GameObject DiceThrow;
    private int number;

    void Start()
    {
        number = 0;
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.name == "Attack")
            {
                number = 6;
            }
            else if (collider.gameObject.name == "Doubt")
            {
                number = 5;
            }
            else if (collider.gameObject.name == "3")
            {
                number = 4;
            }
            else if (collider.gameObject.name == "4")
            {
                number = 3;
            }
            else if (collider.gameObject.name == "5")
            {
                number = 2;
            }
            else if (collider.gameObject.name == "6")
            {
                number = 1;
            }

        }
    /// <summary>
    /// サイコロ出目確定.
    /// </summary>
    public void ConfirmNumber()
    {
        if (number == 4)
        {
            DiceNumText.text = "Attack";
        }
        else if(number == 5 || number == 6)
        {
            DiceNumText.text = "Doubt";
        }
        else
        {
            DiceNumText.text = number.ToString();
        }
        DiceThrow.GetComponent<DiceThrow>().HiddenDice();
    }
}
