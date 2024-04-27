using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PlayerInputController: MonoBehaviour 
{
    public TrackManager trackManager;

    [Header("Character & Movements")]
    [SerializeField] private float _forwardSpeed;
    private const float _acceleration = 0.2f;
    [SerializeField] private float _maneuverSpeed = 5f;
    private const float minSpeed = 5.0f;
    private const float maxSpeed = 45f;
    [SerializeField] private float laneChangeSpeed = 1.0f;

    private Rigidbody _rigidbody;

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
    public float scaledSpeed;

    // Start is called before the first frame update

        public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        currentHealth = maxHealth;
        _forwardSpeed = minSpeed;
        UpdateUI();
    }

    void Update()
    {
        HandleInput();
        scaledSpeed = _forwardSpeed * Time.deltaTime;
    }

    

    void FixedUpdate()
    {
        if(_IsMoving)
        {
            _rigidbody.velocity = transform.forward * _forwardSpeed + new Vector3(_inputVector.x * _maneuverSpeed, _inputVector.y * _maneuverSpeed, 0);
            
            // Increase Speed
            if(_forwardSpeed < maxSpeed)
            {
                _forwardSpeed += _acceleration * Time.deltaTime;
            }
            else
                _forwardSpeed = maxSpeed;
            
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
