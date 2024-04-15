using UnityEngine;
using System.Collections.Generic;

public class TrackManager : MonoBehaviour
{
    public GameObject[] section;
    public int zPos = 50;
    public int secNum;

    static public TrackManager instance
    {
        get { return s_Instance; }
    }

    static protected TrackManager s_Instance;

    public System.Action<TrackSegment> newSegmentCreated;
    public System.Action<TrackSegment> currentSegementChanged;

    public TrackSegment currentSegment
    {
        get { return m_Segments[0]; }
    }

    public List<TrackSegment> segments
    {
        get { return m_Segments; }
    }


    [Header("Character & Movements")] public PlayerInputController playerController;

    public int trackSeed
    {
        get { return m_TrackSeed; }
        set { m_TrackSeed = value; }
    }

    public float timeToStart
    {
        get { return m_TimeToStart; }
    } // Will return -1 if already started (allow to update UI)

    public int score
    {
        get { return m_Score; }
    }

    public int multiplier
    {
        get { return m_Multiplier; }
    }

    protected float m_TimeToStart = -1.0f;
    protected int m_TrackSeed = -1;

    public float m_CurrentSegmentDistance
    {
        get { return m_CurrentSegmentDistance; }
    }

    public float m_TotalWorldDistance
    {
        get { return m_TotalWorldDistance; }
    }

    protected bool m_IsMoving;
    protected float m_Speed;

    protected float m_TimeSincePowerup; // The higher it goes, the higher the chance of spawning one
    protected float m_TimeSinceLastPremium;

    protected int m_Multiplier;

    protected List<TrackSegment> m_Segments = new List<TrackSegment>();
    protected List<TrackSegment> m_PastSegments = new List<TrackSegment>();
    protected int m_SafeSegementLeft;

    protected ThemeData m_CurrentThemeData;
    protected int m_CurrentZone;
    protected float m_CurrentZoneDistance;
    protected int m_PreviousSegment = -1;

    protected int m_Score;
    protected float m_ScoreAccum;

    protected bool m_Rerun;

    protected const float k_StartingSegmentDistance = 2f;

    private int _segmentsToSpawn = 10;
    private int _segmentsSpawnedCount;

    protected void Awake()
    {
        m_ScoreAccum = 0.0f;
        s_Instance = this;
    }

    void Update()
    {
        if (_segmentsSpawnedCount < _segmentsToSpawn)
        {
            GenerateSection();
        }
    }

    void GenerateSection()
    {
        secNum = Random.Range(0, 3);
        Instantiate(section[secNum], new Vector3(0, 0, zPos), Quaternion.identity);
        zPos += 50;
        _segmentsSpawnedCount++;
    }
}