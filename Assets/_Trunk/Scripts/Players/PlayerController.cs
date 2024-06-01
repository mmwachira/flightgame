using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    static int s_HitHash = Animator.StringToHash("Hit");

    [Header("Character & Movements")]
    [SerializeField] private float _forwardSpeed;
    [SerializeField] private float _maneuverSpeed = 5f;
    [SerializeField] private float _acceleration = 0.2f;
    [SerializeField] private float _minSpeed = 5f;
    [SerializeField] private float _maxSpeed = 20f;
    [SerializeField] Animator _animator;
    [SerializeField] private ParticleSystem _collisionParticles;
    [SerializeField] private Renderer _playerRenderer;
    [SerializeField] private Color _flashColor;


    private Rigidbody _rigidbody;
    private Collider[] _colliders;
    private Color _originalColor;

    private Vector2 _inputVector;
    private Vector2 _inputStartPosition;
    private bool _isMoving;
    private int _currentHealth;
    private bool _isInvincible;

    private const float invincibilityDuration = 3.0f;
    private const int _flashCount = 5;
    private const int MaxHealth = 3;
    private const string TagObstacle = "Obstacle";
    private const string TagCollectable = "Collectable";

    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _colliders = GetComponents<Collider>();
        if (_playerRenderer != null)
        {
            _originalColor = _playerRenderer.material.color;
        }
    }

    public void Setup()
    {
        _isMoving = false;
        _isInvincible = false;
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
        _animator.SetFloat(MoveXHash, _inputVector.x);
        _animator.SetFloat(MoveYHash, _inputVector.y);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag(TagObstacle))
        {
            if (!_isInvincible)
            {
                HandleCollision();
            }

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

    private void HandleCollision()
    {
        foreach (Collider collider in _colliders)
        {
            collider.enabled = false;
        }

        _isInvincible = true;

        if (_collisionParticles != null)
        {
            _collisionParticles.Play();
        }

        _forwardSpeed = _minSpeed;
        UpdateHealth(-1);

        StartCoroutine(InvincibilityCoroutine());
        StartCoroutine(FlashCoroutine());

    }

    private IEnumerator InvincibilityCoroutine()
    {
        yield return new WaitForSeconds(invincibilityDuration);

        foreach (Collider collider in _colliders)
        {
            collider.enabled = true;
        }

        _isInvincible = false;
    }

    private IEnumerator FlashCoroutine()
    {
        for (int i = 0; i < _flashCount; i++)
        {
            if (_playerRenderer != null)
            {
                _playerRenderer.material.color = _flashColor;
                yield return new WaitForSeconds(0.1f);
                _playerRenderer.material.color = _originalColor;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}