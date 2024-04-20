using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PlayerInputController: MonoBehaviour 
{
    public GameObject player;
    public float laneChangeSpeed = 1.0f;

    public int maxHealth = 3;

    public int currentHealth;
    public Image[] heartIcons;
    protected int m_CurrentLife;

    [Header("Controls")]
    public float jumpHeight = 1.2f;

    public float forwardForce = 5f;
    public float upwardForce = 1f;
    public float sidewaysForce = 5f;

    public bool _IsMoving = true;
    public Rigidbody rb;

    // Start is called before the first frame update

        public void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    void Update()
    {
        

        if(_IsMoving)
        {
            player.transform.Translate(Vector3.forward * forwardForce * Time.deltaTime, Space.World);
        }
        

        if(Input.GetKey("w")){
            player.transform.Translate(
                //Vector3.Lerp(Vector3.forward, Vector3.up, upwardForce)); 
                Vector3.up * upwardForce * Time.deltaTime);
        }
        if(Input.GetKey("s")){
            if(player.transform.position.y > LevelBoundary.ground)
            {
               player.transform.Translate(Vector3.down * upwardForce * Time.deltaTime); 
            }
            
        }
        
        if(Input.GetKey("a")){
            if(player.transform.position.x > LevelBoundary.leftSide)
            {
                player.transform.Translate(Vector3.left * sidewaysForce * Time.deltaTime);
            }
            //rb.AddForce(-forwardForce,0,0);
        }
        if(Input.GetKey("d")){
            if(player.transform.position.x < LevelBoundary.rightSide)
            {
                player.transform.Translate(Vector3.right * sidewaysForce * Time.deltaTime);
            }
            //rb.AddForce(forwardForce,0,0);
        }
        
    }
    void OnTriggerEnter(Collider collision) 
    {
        if(collision.tag == "Obstacle")
        {
            Debug.Log("Obstacle hit");
            //_IsMoving = false;
            TakeDamage(1);
            
            
        }
    }
    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if(i < currentHealth)
            {
                heartIcons[i].enabled = true;
            }
            else{
                heartIcons[i].enabled = false;
            }
        }

    }
    
}
