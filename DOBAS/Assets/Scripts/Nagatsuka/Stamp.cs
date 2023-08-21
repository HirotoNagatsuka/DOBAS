using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.parent = this.transform;
        this.gameObject.SetActive(false);
    }
}
