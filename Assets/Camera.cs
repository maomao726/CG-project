using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("w")){transform.Translate(0,0,0.07f, Space.Self);}
        if(Input.GetKey("s")){transform.Translate(0,0,-0.07f, Space.Self);}
        if(Input.GetKey("d")){transform.Translate(0.07f,0,0, Space.Self);}
        if(Input.GetKey("a")){transform.Translate(-0.07f,0,0, Space.Self);}
    }
}
