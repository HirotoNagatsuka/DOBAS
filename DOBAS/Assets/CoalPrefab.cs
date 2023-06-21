using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalPrefab : MonoBehaviour
{
    [SerializeField] GameObject CardPanel;
    [SerializeField] GameObject CardDetailPanel;

    // Start is called before the first frame update
    void Start()
    {
        GameObject CardPanelObj  = Instantiate(CardPanel,this.gameObject.transform);
        GameObject CardDetailObj = Instantiate(CardDetailPanel,this.gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
