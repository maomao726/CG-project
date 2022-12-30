using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materialColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
