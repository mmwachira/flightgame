using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Movement : MonoBehaviour
{
    public Rigidbody rb;

    public float forwardForce = 5f;
    public float upwardForce = 50f;
    public float backwardForce = 5f;
    public float sidewaysForce = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
     GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * forwardForce * Time.deltaTime, Space.World);

        if(Input.GetKey("space")){
            rb.AddForce(0,forwardForce,0);
        }
        
        if(Input.GetKey("a")){
            if(this.gameObject.transform.position.x > LevelBoundary.leftSide)
            {
                transform.Translate(Vector3.left * sidewaysForce * Time.deltaTime);
            }
            //rb.AddForce(-forwardForce,0,0);
        }
        if(Input.GetKey("d")){
            if(this.gameObject.transform.position.x < LevelBoundary.rightSide)
            {
                transform.Translate(Vector3.right * sidewaysForce * Time.deltaTime);
            }
            //rb.AddForce(forwardForce,0,0);
        }
        
    }

}
