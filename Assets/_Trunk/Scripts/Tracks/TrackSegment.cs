using UnityEngine;

public class TrackSegment : MonoBehaviour
{
    [SerializeField] private Transform _markerRoot;
    [SerializeField] private GameObject[] _possibleObstacles;
    [SerializeField] private float[] _obstaclePositions;

    public GameObject[] PossibleObstacles => _possibleObstacles;
    public float[] ObstaclePositions => _obstaclePositions;
    public float SegmentLength => _segmentLength;
    public Transform ContainerObstacles => _containerObstacles;
    public Transform ContainerCollectables => _containerCollectables;
    
    private float _segmentLength;
    private Transform _containerObstacles;
    private Transform _containerCollectables;

    void OnEnable()
    {
        CalculateLength();

        GameObject obj = new GameObject("ContainerObstacles");
        obj.transform.SetParent(transform);
        _containerObstacles = obj.transform;

        obj = new GameObject("ContainerCollectables");
        obj.transform.SetParent(transform);
        _containerCollectables = obj.transform;
    }

    private void CalculateLength()
    {
        _segmentLength = 0;

        for (int i = 1; i < _markerRoot.childCount; ++i)
        {
            Transform orig = _markerRoot.GetChild(i - 1);
            Transform end = _markerRoot.GetChild(i);

            Vector3 vec = end.position - orig.position;
            _segmentLength += vec.magnitude;
        }
    }

    // Same as GetPointAt but using an interpolation parameter in world units instead of 0 to 1.
    public void GetPointAtInWorldUnit(float wt, out Vector3 pos, out Quaternion rot)
    {
        float t = wt / _segmentLength;
        GetPointAt(t, out pos, out rot);
    }

    // Interpolation parameter t is clamped between 0 and 1.
    public void GetPointAt(float t, out Vector3 pos, out Quaternion rot)
    {
        float clampedT = Mathf.Clamp01(t);
        float scaledT = (_markerRoot.childCount - 1) * clampedT;
        int index = Mathf.FloorToInt(scaledT);
        float segmentT = scaledT - index;

        Transform orig = _markerRoot.GetChild(index);
        if (index == _markerRoot.childCount - 1)
        {
            pos = orig.position;
            rot = orig.rotation;
            return;
        }

        Transform target = _markerRoot.GetChild(index + 1);

        pos = Vector3.Lerp(orig.position, target.position, segmentT);
        rot = Quaternion.Lerp(orig.rotation, target.rotation, segmentT);
    }

    public void Cleanup()
    {
        while (_containerCollectables.childCount > 0)
        {
            Transform t = _containerCollectables.GetChild(0);
            ManagerLevel.Instance.RecyclePoolElement(t);
        }
        Destroy(gameObject);
    }
}
