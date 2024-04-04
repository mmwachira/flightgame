using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Movement : MonoBehaviour
{
    public Rigidbody rb;

    public float forwardForce = 5f;
    public float upwardForce = 1f;
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

        if(Input.GetKey("w")){
            transform.Translate(
                //Vector3.Lerp(Vector3.forward, Vector3.up, upwardForce)); 
                Vector3.up * upwardForce * Time.deltaTime);
        }
        if(Input.GetKey("s")){
            if(this.gameObject.transform.position.y > LevelBoundary.ground)
            {
               transform.Translate(Vector3.down * upwardForce * Time.deltaTime); 
            }
            
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
