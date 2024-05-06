using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class TrackManager : MonoBehaviour
{
    public PlayerController playerController;
    static protected TrackManager s_Instance;
    [SerializeField] private GameObject[] sectionPrefab;
    private int previousSectionIndex = -1;
    private GameObject previousSection;

    private float sectionSpawnInterval;
    private float sectionDestroyInterval;

    private GameObject sectionToDestroy;
    [SerializeField] private Transform playerTransform;

    private List<GameObject> li_spawnedSections = new List<GameObject>();

    [SerializeField] private float m_currentSectionDistance;
    private float sectionSpawnDistance = 50f;
    [SerializeField] private float nextSectionDistance = 0f;

    private const float k_StartingSectionDistance = 0f;
    private const float k_SectionRemovalDistance = 50f;
    const float _WorldThreshold = 10000f;

    private Vector3 currentPos;
    private Quaternion currentRot;
    private int secNum;
    private bool creatingSection = false;

    private Transform _levelSegmentsContainer;
    private int _spawnedSectionNameCount;

    private const int DesiredSectionsToSpawn = 10;
    private const int SafeSections = 5;
    private int _spawnedSectionsCount = 0;

    protected float m_Score;
    protected float m_ScoreAccum;
    

    void Start()
    {
        m_ScoreAccum = 0;
        s_Instance = this;
        m_currentSectionDistance = k_StartingSectionDistance;

        GameObject obj = new GameObject("Level Segments");
        _levelSegmentsContainer = obj.transform;

         while (_spawnedSections < DesiredSectionsToSpawn)
         {
             SpawnInitialSections();
             
             _spawnedSections++;
         }

         nextSectionDistance+=sectionSpawnDistance;
        
    }

    [SerializeField] private int _spawnedSections = 0;

    void Update()
    {

        // Spawn new sections

            while(_spawnedSections < SafeSections)
            {
                SpawnNewSection();
        
            }
        

        // Check and destroy past sections
        DestroyPastSections();
  
    }

    public void SpawnInitialSections()
    {
                do
                {
                    secNum = Random.Range(0, 3);
                }
                while (secNum == previousSectionIndex);

                _spawnedSectionNameCount++;

                //secNum = Random.Range(0,2);
                GameObject newSection = Instantiate(sectionPrefab[secNum], new Vector3(0, 0, sectionSpawnDistance), Quaternion.identity);
                newSection.name = $"Section {_spawnedSectionNameCount}";
                newSection.transform.SetParent(_levelSegmentsContainer);

                sectionSpawnDistance += 50;
                li_spawnedSections.Add(newSection);
                previousSectionIndex = secNum;
    }

    // Spawning new random sections once player distance is greater than the spawning distance for the next section 
    public void SpawnNewSection()
    {   

            for(int i = 0; i < DesiredSectionsToSpawn; i++)
            {
                do
                {
                    secNum = Random.Range(0,3);
                }
                while (secNum == previousSectionIndex);

                _spawnedSectionNameCount++;
                //secNum = Random.Range(0,2);

                GameObject newSection = Instantiate(sectionPrefab[secNum], new Vector3(0, 0, nextSectionDistance), Quaternion.identity);
                newSection.name = $"Section {_spawnedSectionNameCount}";
                newSection.transform.SetParent(_levelSegmentsContainer);

                nextSectionDistance += 50;
                li_spawnedSections.Add(newSection);

                _spawnedSections++;

                previousSectionIndex = secNum;
            }



    }
    // Destroy old sections once player is some distance from the past section
    private void DestroyPastSections()
    {
        
        for (int i = 0; i < li_spawnedSections.Count; i++)
        {

             if(playerTransform.position.z > li_spawnedSections[i].transform.position.z + (260f * playerController.scaledSpeed))
             {
                 if (li_spawnedSections.Count > 0)
                 {

                     Destroy(li_spawnedSections[i]);
                     li_spawnedSections.RemoveAt(i);
                     Debug.Log("Destroyed Section");
                     i--;

                     _spawnedSections--;
                 }
          else{
              Debug.LogWarning("Nothing found in the list");
          }
             }
        }
         

    }

}