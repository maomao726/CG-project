using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public float external_x = 10.0f;
    public float external_y = 2.0f;
    public float external_z = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Cloth>().enabled = true;
        GetComponent<Cloth>().externalAcceleration = new Vector3(external_x, external_y, external_z);
        GetComponent<Cloth>().randomAcceleration = new Vector3(10.0f, 2.0f, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("u")){
            external_x += 0.05f;
        }
        if(Input.GetKey("j")){
            external_x -= 0.05f;
        }
        if(Input.GetKey("i")){
            external_y += 0.05f;
        }
        if(Input.GetKey("k")){
            external_y -= 0.05f;
        }
        if(Input.GetKey("o")){
            external_z += 0.05f;
        }
        if(Input.GetKey("l")){
            external_z -= 0.05f;
        }
        GetComponent<Cloth>().externalAcceleration = 
            new Vector3(external_x, external_y, external_z);
    }
}
