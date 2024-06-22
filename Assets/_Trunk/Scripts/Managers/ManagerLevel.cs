using System.Collections;
using System.Collections.Generic;
using FlightGame.Players;
using FlightGame.Tracks;
using FlightGame.Utils;
//using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlightGame.Managers
{
    public class ManagerLevel : MonoBehaviour
    {
        public static ManagerLevel Instance { get; private set; }

        public bool IsGameplay => _isGameplay;
        public float LaneOffset => _laneOffset;
        public float TotalRunDistance => _totalRunDistance;
        public float m_Score => _currentScore;
        public TrackSegment CurrentSegment => _currentTrackSegment;
        public float CollectedCoins => _collectedCoins;
        public PlayerController playerController => _playerController;

        [SerializeField] private PlayerController _playerController;
        [SerializeField] private FlyingCollectable _collectableTemplate;
        [SerializeField] private List<TrackSegment> _trackSegmentsTemplates;
        [SerializeField] private float _laneOffset = 1.4f;

        [SerializeField] private bool _randomizeY = true; // Whether to randomize Y position
        [SerializeField] private float _collectableYOffset = 2.0f; // Default Y offset
        [SerializeField] private Vector2 _yRandomizationRange = new Vector2(2.0f, 4.0f); // Y randomization range

        [SerializeField] private GameObject _ringQuestionsTemplate;

        private readonly List<TrackSegment> _trackSegmentsSpawn = new();
        private readonly List<TrackSegment> _trackSegmentsToRemove = new();
        private int _spawnedTrackSegments;
        private int _totalSpawnedTrackSegments;
        private int _collectedCoins;
        private int _currentScore;
        private float _segmentRunDistance;
        private float _totalRunDistance;

        private Vector3 _previousPosition;
        private Transform _levelContainer;
        private PoolByPrefab _collectablesPool;
        private TrackSegment _currentTrackSegment;
        private bool _isGameOver;
        private bool _isGameplay;
        private bool _isCorrect;
        private int _questionSpawnedCount;
        private int _nextQuestionSpawnSegment;

        private const int QuestionSpawnOffset = 4;
        private const int CollectablePoolSize = 200;
        private const int MaxSegmentCount = 8;
        private const float StartingSegmentDistance = 2f;
        private const float SegmentRemovalDistance = -40f;
        private const float CenterResetThreshold = 10000f;
        private const int ObstacleLayer = 6; // Change it according to layer setup
        private static readonly int ObstacleLayerMask = 1 << ObstacleLayer;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            DontDestroyOnLoad(gameObject);
            _nextQuestionSpawnSegment = QuestionSpawnOffset;
        }

        public void Setup()
        {
            GameObject obj = new GameObject("Level Segments");
            _levelContainer = obj.transform;

            _collectablesPool = new PoolByPrefab(_levelContainer)
            {
                AutoExpand = false
            };
            _collectablesPool.AddPrefab(_collectableTemplate, CollectablePoolSize);
        }

        public void StartGame()
        {
            _segmentRunDistance = StartingSegmentDistance;
            _totalRunDistance = 0;
            _collectedCoins = 0;
            ManagerUI.Instance.Reset();
            _playerController.Setup();
            _isGameplay = true;
            _isGameOver = false;
            _previousPosition = _playerController.transform.position;
            _playerController.StartMoving();

            //StartCoroutine(ShowQuestions());

        }

        public void EndGame()
        {
            _isGameplay = false;
            _isGameOver = true;
            _playerController.StopMoving();
        }

        private void Update()
        {
            while (_spawnedTrackSegments < MaxSegmentCount)
            {
                SpawnNewSegment();
            }

            if (_isGameOver)//Quick hack to clean first 3 segments after game over
            {
                _trackSegmentsSpawn[0].CleanObstaclesAndCollectables();
                _trackSegmentsSpawn[1].CleanObstaclesAndCollectables();
            }

            if (!_isGameplay || _isGameOver)
            {
                return;
            }


            Vector3 currentPosition = _playerController.transform.position;
            float forwardDistance = currentPosition.z - _previousPosition.z;

            // Ensure forward distance is positive to count only forward movement
            if (forwardDistance > 0)
            {
                _segmentRunDistance += forwardDistance;
                _totalRunDistance += forwardDistance;
                ManagerUI.Instance.UpdateDistance((int)_totalRunDistance);
                _currentScore = (int)TotalRunDistance;

            }

            _previousPosition = currentPosition;

            if (_segmentRunDistance > _trackSegmentsSpawn[0].SegmentLength)
            {
                _segmentRunDistance -= _trackSegmentsSpawn[0].SegmentLength;

                // _trackSegmentsToRemove are segment we already passed, we keep them to move them and destroy them later, but they aren't part of the game anymore 
                _trackSegmentsToRemove.Add(_trackSegmentsSpawn[0]);
                _trackSegmentsSpawn.RemoveAt(0);
                _spawnedTrackSegments--;
            }

            _trackSegmentsSpawn[0].GetPointAtInWorldUnit(_segmentRunDistance, out var currentPos, out var currentRot);

            // Floating origin implementation
            // Move the whole world back to 0,0,0 when we get too far away to avoid going out of bounds of the variable.
            bool needRecenter = currentPos.sqrMagnitude > CenterResetThreshold;
            if (needRecenter)
            {
                int count = _trackSegmentsSpawn.Count;
                for (int i = 0; i < count; i++)
                {
                    _trackSegmentsSpawn[i].transform.position -= currentPos;
                }

                count = _trackSegmentsToRemove.Count;
                for (int i = 0; i < count; i++)
                {
                    _trackSegmentsToRemove[i].transform.position -= currentPos;
                }
                Transform characterTransform = _playerController.transform;
                characterTransform.position = new Vector3(characterTransform.position.x, characterTransform.position.y, characterTransform.position.z - currentPos.z);
            }


            // Still move past segment until they aren't visible anymore.
            for (int i = 0; i < _trackSegmentsToRemove.Count; ++i)
            {
                if ((_trackSegmentsToRemove[i].transform.position - currentPos).z < SegmentRemovalDistance)
                {
                    _trackSegmentsToRemove[i].Cleanup();
                    _trackSegmentsToRemove.RemoveAt(i);
                    i--;
                }
            }
        }

        private void SpawnNewSegment()
        {
            int randomSegment = Random.Range(0, _trackSegmentsTemplates.Count);
            TrackSegment newSegment = Instantiate(_trackSegmentsTemplates[randomSegment], _levelContainer);


            Vector3 currentExitPoint;
            Quaternion currentExitRotation;
            if (_trackSegmentsSpawn.Count > 0)
            {
                _trackSegmentsSpawn[_trackSegmentsSpawn.Count - 1].GetPointAt(1.0f, out currentExitPoint, out currentExitRotation);
            }
            else
            {
                currentExitPoint = transform.position;
                currentExitRotation = transform.rotation;
            }

            newSegment.transform.rotation = currentExitRotation;

            newSegment.GetPointAt(0.0f, out var entryPoint, out _);


            Vector3 pos = currentExitPoint + (newSegment.transform.position - entryPoint);
            newSegment.transform.position = pos;
            //newSegment.transform.localScale = new Vector3((Random.value > 0.5f ? -1 : 1), 1, 1);//TODO: Need to fix the text alignment of the questions later, for now disabling the flipping

            if (_spawnedTrackSegments > 1) //We spawn obstacles from the second segment onwards
            {
                SpawnObstacle(newSegment);

            }
            _trackSegmentsSpawn.Add(newSegment);
            _spawnedTrackSegments++;
            _totalSpawnedTrackSegments++;
            _currentTrackSegment = newSegment;
        }

        private void SpawnObstacle(TrackSegment segment)
        {
            if (segment.PossibleObstacles.Length != 0)
            {
                for (int i = 0; i < segment.ObstaclePositions.Length; ++i)
                {
                    GameObject obstacle;
                    if (_totalSpawnedTrackSegments == _nextQuestionSpawnSegment)
                    {
                        obstacle = _ringQuestionsTemplate;
                        _questionSpawnedCount++;
                        _nextQuestionSpawnSegment = QuestionSpawnOffset * (1 + _questionSpawnedCount);
                    }
                    else
                    {
                        obstacle = segment.PossibleObstacles[Random.Range(0, segment.PossibleObstacles.Length)];
                    }

                    SpawnFromObjectInSegment(obstacle, segment, i);
                }
            }

            SpawnCollectables(segment);
        }

        private void SpawnFromObjectInSegment(GameObject obj, TrackSegment segment, int posIndex)
        {
            if (obj != null)
            {
                Obstacle obstacle = obj.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    obstacle.Spawn(segment, segment.ObstaclePositions[posIndex]);
                }
            }
        }

        private void SpawnCollectables(TrackSegment segment)
        {
            const float increment = 3.5f;
            float currentWorldPosition = 0.0f;
            int currentLane = Random.Range(0, 3);

            // Initialize the first Y offset
            float currentYOffset = Random.Range(_yRandomizationRange.x, _yRandomizationRange.y);

            while (currentWorldPosition < segment.SegmentLength)
            {
                segment.GetPointAtInWorldUnit(currentWorldPosition, out var position, out var rotation);

                bool laneValid = true;
                int testedLane = currentLane;
                while (Physics.CheckSphere(position + ((testedLane - 1) * _laneOffset * (rotation * Vector3.right)), 0.4f, ObstacleLayerMask))
                {
                    testedLane = (testedLane + 1) % 3;
                    if (currentLane == testedLane)
                    {
                        // Couldn't find a valid lane.
                        laneValid = false;
                        break;
                    }
                }

                if (currentLane != testedLane)
                {
                    // Switch to a new valid lane, update Y offset
                    currentYOffset = Random.Range(_yRandomizationRange.x, _yRandomizationRange.y);
                    currentLane = testedLane;
                }

                if (laneValid)
                {
                    position = position + ((currentLane - 1) * _laneOffset * (rotation * Vector3.right));
                    position.y += currentYOffset;

                    FlyingCollectable collectable = _collectablesPool.GetAvailableObject<FlyingCollectable>();
                    collectable.transform.SetParent(segment.ContainerCollectables, true);
                    collectable.transform.position = position;
                    collectable.transform.rotation = rotation;
                }

                currentWorldPosition += increment;
            }
        }

        public void RecyclePoolElement(Transform item)
        {
            _collectablesPool.RecycleObject(item);
        }

        public void CollectItem(Transform item)
        {

            RecyclePoolElement(item);
            PlayerData.instance.coins += 1;
            _collectedCoins++;
            ManagerUI.Instance.UpdateCollected(_collectedCoins);

            ManagerSounds.Instance.PlaySingle(ManagerSounds.Instance.SfxCollect, true);
        }

        public void AddCollectedCoins(int amount)
        {
            _collectedCoins += amount;
            ManagerUI.Instance.UpdateCollected(_collectedCoins);
        }

        private IEnumerator ShowQuestions()
        {
            while (!_isGameOver)
            {
                yield return new WaitForSeconds(20f);
                _playerController.StartSlowDown();
                ManagerQuestions.Instance.AskQuestion(CurrentSegment);

                yield return new WaitForSeconds(5f);
                //ManagerUI.Instance.UpdateAnswer();
                playerController.ResumeMoving();
            }

        }
    }
}