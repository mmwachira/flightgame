using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlaneController : MonoBehaviour
{
    [SerializeField] private float _forwardSpeed = 10f;
    [SerializeField] private float _maneuverSpeed = 5f;

    private Rigidbody _rigidbody;
    private Vector2 _inputStartPosition;
    private Vector2 _inputVector;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        // Always move the plane forward
        _rigidbody.velocity = transform.forward * _forwardSpeed + new Vector3(_inputVector.x * _maneuverSpeed, _inputVector.y * _maneuverSpeed, 0);
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
}