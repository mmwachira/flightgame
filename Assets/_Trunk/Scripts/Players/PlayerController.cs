using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Character & Movements")] 
    [SerializeField] private float _forwardSpeed;
    [SerializeField] private float _maneuverSpeed = 5f;
    [SerializeField] private float _acceleration = 0.2f;
    [SerializeField] private float _minSpeed = 5f;
    [SerializeField] private float _maxSpeed = 20f;
    [SerializeField] Animator _animator;

    private Rigidbody _rigidbody;
    private Vector2 _inputVector;
    private Vector2 _inputStartPosition;
    private bool _isMoving;
    private int _currentHealth;

    private const int MaxHealth = 3;
    private const string TagObstacle = "Obstacle";
    private const string TagCollectable = "Collectable";

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Setup()
    {
        _isMoving = false;
        _currentHealth = MaxHealth;
        _forwardSpeed = _minSpeed;
        ManagerUI.Instance.UpdateLivesDisplay(_currentHealth);
    }

    void Update()
    {
        if (!_isMoving) return;

        HandleInput();

        //TODO: We should also check the results of increasing _maneuverSpeed with some proportion to the _forwardSpeed so the reaction of the plane is higher too
        if (_forwardSpeed < _maxSpeed)
            _forwardSpeed += _acceleration * Time.deltaTime;
        else
            _forwardSpeed = _maxSpeed;
    }

    void FixedUpdate()
    {
        if (!_isMoving) return;
    
        _rigidbody.velocity = transform.forward * _forwardSpeed + new Vector3(_inputVector.x * _maneuverSpeed, _inputVector.y * _maneuverSpeed, 0);
    }

    public void StartMoving()
    {
        _isMoving = true;
    }

    public void StopMoving()
    {
        _isMoving = false;
    }

    private void HandleInput()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            _inputStartPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 currentPosition = Input.mousePosition;
            Vector2 delta = currentPosition - _inputStartPosition;
            _inputVector = new Vector2(delta.x, delta.y).normalized;

            if (Mathf.Abs(_inputVector.x) > 0.2f)
            {
                _animator.Play(_inputVector.x > 0 ? "RightMov_anim" : "LeftMov_anim");
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _inputVector = Vector2.zero;
            _animator.Play("stationary_anim");
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
        if (collision.CompareTag(TagObstacle))
        {
            UpdateHealth(-1);
        }
        else if (collision.CompareTag(TagCollectable))
        {
            ManagerLevel.Instance.CollectItem(collision.transform);
            
        }
    }
    
    private void UpdateHealth(int value)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + value, 0, 3);
        ManagerUI.Instance.UpdateLivesDisplay(_currentHealth);
        if (_currentHealth < 1)
        {
            ManagerGame.Instance.GameOver();
        }
    }
}