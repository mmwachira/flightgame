using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackManager: MonoBehaviour
{
    public GameObject[] section;
    public int zPos = 50;
    public bool creatingSection = false;
    public int secNum;
    static public TrackManager instance { get { return s_Instance; } }
    static protected TrackManager s_Instance;

    public System.Action<TrackSegment> newSegmentCreated;
    public System.Action<TrackSegment> currentSegementChanged;

    public TrackSegment currentSegment { get { return m_Segments[0]; } }
    public List<TrackSegment> segments { get { return m_Segments; } }


    [Header("Character & Movements")]
    public PlayerInputController playerController; 

    public int trackSeed { get { return m_TrackSeed; } set { m_TrackSeed = value; } }

    public float timeToStart { get { return m_TimeToStart; } }  // Will return -1 if already started (allow to update UI)

    public int score { get { return m_Score; } }
    public int multiplier { get { return m_Multiplier; } }
    protected float m_TimeToStart = -1.0f;
    protected int m_TrackSeed = -1;
    public float m_CurrentSegmentDistance {get {return m_CurrentSegmentDistance;}}
    public float m_TotalWorldDistance {get {return m_TotalWorldDistance;}}
    protected bool m_IsMoving;
    protected float m_Speed;

    protected float m_TimeSincePowerup;     // The higher it goes, the higher the chance of spawning one
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

    protected void Awake()
    {
        m_ScoreAccum = 0.0f;
        s_Instance = this;
    }

    void Update()
    {
        if (creatingSection == false)
        {
            creatingSection = true;
            StartCoroutine(GenerateSection());
        }
    }

    IEnumerator GenerateSection()
    {
        secNum = Random.Range(0, 3);
        Instantiate(section[secNum], new Vector3(0,0,zPos), Quaternion.identity);
        zPos += 50;
        yield return new WaitForSeconds(2);
        creatingSection = false;
    }

    // public IEnumerator Begin()
    // {
    //     if (!m_Rerun)
    //     {
    //         if (m_TrackSeed != -1)
    //             Random.InitState(m_TrackSeed);
    //         else
    //             Random.InitState((int)System.DateTime.Now.Ticks);

    //         m_CurrentSegmentDistance = k_StartingSegmentDistance;
    //         m_TotalWorldDistance = 0.0f;

    //         playerController.gameObject.SetActive(true);

    //         yield return null;

    //     }
        
    // }

    // public IEnumerator SpawnNewSegment()
    // {
    //     if (!m_IsTutorial)
    //     {
    //         if (m_CurrentThemeData.zones[m_CurrentZone].length < m_CurrentZoneDistance)
    //             ChangeZone();
    //     }

    //     int segmentUse = Random.Range(0, m_CurrentThemeData.zones[m_CurrentZone].prefabList.Length);
    //     if (segmentUse == m_PreviousSegment) segmentUse = (segmentUse + 1) % m_CurrentThemeData.zones[m_CurrentZone].prefabList.Length;

    //     AsyncOperationHandle segmentToUseOp = m_CurrentThemeData.zones[m_CurrentZone].prefabList[segmentUse].InstantiateAsync(_offScreenSpawnPos, Quaternion.identity);
    //     yield return segmentToUseOp;
    //     if (segmentToUseOp.Result == null || !(segmentToUseOp.Result is GameObject))
    //     {
    //         Debug.LogWarning(string.Format("Unable to load segment {0}.", m_CurrentThemeData.zones[m_CurrentZone].prefabList[segmentUse].Asset.name));
    //         yield break;
    //     }
    //     TrackSegment newSegment = (segmentToUseOp.Result as GameObject).GetComponent<TrackSegment>();

    //     Vector3 currentExitPoint;
    //     Quaternion currentExitRotation;
    //     if (m_Segments.Count > 0)
    //     {
    //         m_Segments[m_Segments.Count - 1].GetPointAt(1.0f, out currentExitPoint, out currentExitRotation);
    //     }
    //     else
    //     {
    //         currentExitPoint = transform.position;
    //         currentExitRotation = transform.rotation;
    //     }

    //     newSegment.transform.rotation = currentExitRotation;

    //     Vector3 entryPoint;
    //     Quaternion entryRotation;
    //     newSegment.GetPointAt(0.0f, out entryPoint, out entryRotation);


    //     Vector3 pos = currentExitPoint + (newSegment.transform.position - entryPoint);
    //     newSegment.transform.position = pos;
    //     newSegment.manager = this;

    //     newSegment.transform.localScale = new Vector3((Random.value > 0.5f ? -1 : 1), 1, 1);
    //     newSegment.objectRoot.localScale = new Vector3(1.0f / newSegment.transform.localScale.x, 1, 1);

    //     if (m_SafeSegementLeft <= 0)
    //     {
    //         SpawnObstacle(newSegment);
    //     }
    //     else
    //         m_SafeSegementLeft -= 1;

    //     m_Segments.Add(newSegment);

    //     if (newSegmentCreated != null) newSegmentCreated.Invoke(newSegment);
    // }
}