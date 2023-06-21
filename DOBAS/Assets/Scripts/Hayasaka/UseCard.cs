using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UseCard : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject CardPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void DrawCardList()
    {
        CardPanel.SetActive(true);
    }
}
