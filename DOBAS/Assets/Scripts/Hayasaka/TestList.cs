using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestList : MonoBehaviour
{
    [SerializeField]
    public List<int> PlayerLists = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GetCard(int CardNum)
    {
        PlayerLists.Add(CardNum);
    }
}
