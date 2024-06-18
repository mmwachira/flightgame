using System.Collections;
using FlightGame.Managers;
using FlightGame.Questions;
using FlightGame.Tracks;
using I2.Loc;
using UnityEngine;

namespace FlightGame.Players
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        static int s_HitHash = Animator.StringToHash("Hit");

        public string tagOption => TagOption;

        [Header("Character & Movements")]
        [SerializeField] private float _forwardSpeed;
        [SerializeField] private float _previousForwardSpeed;
        private float originalTimeScale;
        private float _originalForwardSpeed;
        [SerializeField] private float _maneuverSpeed = 5f;
        [SerializeField] private float _acceleration = 0.2f;
        [SerializeField] private float _minSpeed = 5f;
        [SerializeField] private float _maxSpeed = 20f;
        [SerializeField] Animator _animator;
        [SerializeField] private ParticleSystem _collisionParticles;
        [SerializeField] private Renderer _playerRenderer;
        [SerializeField] private Color _flashColor;


        private Rigidbody _rigidbody;

        private Color _originalColor;


        private Vector2 _inputVector;
        private Vector2 _inputStartPosition;
        private bool _isMoving;
        private int _currentHealth;
        private bool _isInvincible;
        private bool isSlowingDown;

        private const float InvincibilityDuration = 1.5f;
        private const float slowDownTime = 2f;
        private const int MaxHealth = 3;
        private const string TagObstacle = "Obstacle";
        private const string TagCollectable = "Collectable";
        private const string TagQuestion = "Question";
        private const string TagQuestionHide = "QuestionHide";
        private const string TagOption = "Option";

        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveYHash = Animator.StringToHash("MoveY");

        public void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();

            if (_playerRenderer != null)
            {
                _originalColor = _playerRenderer.material.color;
            }
            _originalForwardSpeed = _forwardSpeed;
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

            if (isSlowingDown)
            {
                return;
            }

            //TODO: We should also check the results of increasing _maneuverSpeed with some proportion to the _forwardSpeed so the reaction of the plane is higher too
            if (_forwardSpeed < _maxSpeed)
                _forwardSpeed += _acceleration * Time.deltaTime;
            else
                _forwardSpeed = _maxSpeed;
        }

        void FixedUpdate()
        {
            if (!_isMoving)
            {
                _rigidbody.velocity = Vector3.zero;
                return;
            }

            _rigidbody.velocity = transform.forward * _forwardSpeed + new Vector3(_inputVector.x * _maneuverSpeed, _inputVector.y * _maneuverSpeed, 0);
        }

        public void StartMoving()
        {
            _isMoving = true;
        }

        public void ResumeMoving()
        {
            _isMoving = true;
            StartCoroutine(AccelerationCoroutine(2.0f));

        }

        public void StartSlowDown()
        {
            _previousForwardSpeed = _originalForwardSpeed;
            StartCoroutine(SlowDownCoroutine());

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
            else if (collision.CompareTag(TagQuestion))
            {
                //ManagerTime.Instance.DoSlowMotion();
                ObstacleQuestion question = collision.GetComponent<ObstacleQuestion>();
                if (question != null)
                {
                    ManagerUI.Instance.ShowQuestion(LocalizationManager.GetTranslation(question.QuestionData.QuestionKey));
                }
                ManagerTime.Instance.StartSlowMotion(0.09f);

            }
            else if (collision.CompareTag(TagQuestionHide))
            {
                ManagerUI.Instance.HideQuestion();
            }
            else if (collision.CompareTag(TagOption))
            {
                AnswerRing answerRing = collision.GetComponent<AnswerRing>();
                if (answerRing != null)
                {
                    HandleAnswerRing(answerRing);
                }

            }
        }

        private void UpdateHealth(int value)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + value, 0, 3);
            ManagerUI.Instance.UpdateLivesDisplay(_currentHealth);
            if (_currentHealth < 1)
            {
                ManagerGame.Instance.GameOver();
                return;
            }
            StartCoroutine(InvincibilityAndFlashCoroutine());
        }

        private void HandleCollision()
        {
            _isInvincible = true;

            if (_collisionParticles != null)
            {
                _collisionParticles.Play();
            }

            _forwardSpeed = _minSpeed;
            UpdateHealth(-1);
        }

        void HandleAnswerRing(AnswerRing answerRing)
        {
            ManagerQuestions.Instance.CheckAnswer(answerRing.AnswerIndex);
            if (answerRing.IsCorrect)
            {
                ManagerUI.Instance.UpdateAnswer("Correct! +3");
                ManagerLevel.Instance.AddCollectedCoins(1);
                Destroy(answerRing.gameObject);
            }
            else
            {
                ManagerUI.Instance.UpdateAnswer("Wrong! Try again.");
            }



        }



        private IEnumerator InvincibilityAndFlashCoroutine()
        {
            float flashInterval = 0.2f;
            float elapsedTime = 0f;
            WaitForSeconds delay = new WaitForSeconds(flashInterval);

            while (elapsedTime < InvincibilityDuration)
            {
                if (_playerRenderer != null)
                {
                    _originalColor = _playerRenderer.material.color;
                    _playerRenderer.material.color = _flashColor;
                    yield return delay;
                    _playerRenderer.material.color = _originalColor;
                    yield return delay;
                    elapsedTime += flashInterval * 2;
                }
            }

            if (_playerRenderer != null)
            {
                _playerRenderer.material.color = _originalColor; // Ensure it returns to the original color at the end
            }
            _isInvincible = false;
        }

        public IEnumerator SlowDownCoroutine()
        {

            isSlowingDown = true;
            float s_elapsedTime = 0f;
            originalTimeScale = Time.timeScale;


            while (s_elapsedTime < slowDownTime)
            {

                _forwardSpeed = Mathf.Lerp(_originalForwardSpeed, 0, s_elapsedTime / slowDownTime);
                s_elapsedTime += Time.deltaTime;

                yield return null;
            }

            _forwardSpeed = 0;
            isSlowingDown = false;
            _isMoving = false;

            yield return new WaitForSeconds(5f);
            ResumeMoving();

        }

        public IEnumerator AccelerationCoroutine(float accelerationTime)
        {
            float a_elapsedTime = 0f;

            while (a_elapsedTime < accelerationTime)
            {

                _forwardSpeed = Mathf.Lerp(0, _previousForwardSpeed, a_elapsedTime / accelerationTime);
                a_elapsedTime += Time.deltaTime;
                yield return null;
            }


            //_forwardSpeed = _previousForwardSpeed;
        }

    }
}