using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PlayerInputController: MonoBehaviour 
{

    [SerializeField] public float _forwardSpeed = 5f;
    [SerializeField] public float _maneuverSpeed = 5f;

    private Rigidbody _rigidbody;

    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float laneChangeSpeed = 1.0f;

    private Vector2 _inputVector;
    private Vector2 _inputStartPosition;

    public int maxHealth = 3;

    public int currentHealth;
    public Image[] heartIcons;
    protected int m_CurrentLife;

    [Header("Controls")]
    public float upwardForce = 1f;
    public float sidewaysForce = 5f;

    public bool _IsMoving = true;

    // Start is called before the first frame update

        public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        currentHealth = maxHealth;
        UpdateUI();
    }

    void Update()
    {
        HandleInput();

        // if(Input.GetKey("w")){
        //     player.transform.Translate( 
        //         Vector3.up * upwardForce * Time.deltaTime);
        // }
        // if(Input.GetKey("s")){
        //     if(player.transform.position.y > LevelBoundary.ground)
        //     {
        //        player.transform.Translate(Vector3.down * upwardForce * Time.deltaTime); 
        //     }
            
        // }
        
        // if(Input.GetKey("a")){
        //     if(player.transform.position.x > LevelBoundary.leftSide)
        //     {
        //         player.transform.Translate(Vector3.left * sidewaysForce * Time.deltaTime);
        //     }
        //     //rb.AddForce(-forwardForce,0,0);
        // }
        // if(Input.GetKey("d")){
        //     if(player.transform.position.x < LevelBoundary.rightSide)
        //     {
        //         player.transform.Translate(Vector3.right * sidewaysForce * Time.deltaTime);
        //     }
        //     //rb.AddForce(forwardForce,0,0);
        // }
        
    }

    void FixedUpdate()
    {
        if(_IsMoving)
        {
            _rigidbody.velocity = transform.forward * _forwardSpeed + new Vector3(_inputVector.x * _maneuverSpeed, _inputVector.y * _maneuverSpeed, 0);
            //_rigidbody.velocity = Vector3.forward * _forwardSpeed * Time.deltaTime;
            
            // Increase Speed
            if(_forwardSpeed < maxSpeed)
            {
                _forwardSpeed += 0.2f * Time.deltaTime;
            }
            
        }
    }

    private void HandleInput()
    {
        // Handle input for PC
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            _inputStartPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 currentPos = Input.mousePosition;
            Vector2 delta = currentPos - _inputStartPosition;
            _inputVector = new Vector2(delta.x, delta.y).normalized;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _inputVector = Vector2.zero;
        }
        #else
        // Handle touch input for mobile devices
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _inputStartPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.position - _inputStartPosition;
                _inputVector = new Vector2(delta.x, delta.y).normalized;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _inputVector = Vector2.zero;
            }
        }
        #endif
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
