using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector3 pWIND = new Vector3();
    Vector3 step;

    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Cloth>().enabled = true;
        //GetComponent<Cloth>().externalAcceleration = new Vector3(external_x, external_y, external_z);
        //GetComponent<Cloth>().randomAcceleration = new Vector3(10.0f, 2.0f, 10.0f);
        pWIND = new Vector3();
        step = new Vector3(0.05f, 0.05f, 0.05f);

    }

    // Update is called once per frame
    void Update()
    {
        print(pWIND);
        
        if(Input.GetKey("u")){
            pWIND += step;
        }
        else if(Input.GetKey("i")){
            pWIND -= step;
        }
    }
}
