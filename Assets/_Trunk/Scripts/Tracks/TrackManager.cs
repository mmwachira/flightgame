using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class TrackManager : MonoBehaviour
{
    [SerializeField] private GameObject[] sectionPrefab;
    private float sectionSpawnInterval = 5f;
    private float sectionDestroyInterval = 15f;

    private GameObject sectionToDestroy;

    private LinkedList<GameObject> sectionsList = new LinkedList<GameObject>();
    private int zPos = 50;
    private int secNum;
    private bool creatingSection = false;



    void Start()
    {
        InvokeRepeating("SpawnSection", 0f, sectionSpawnInterval);
        InvokeRepeating("DestroySection", 15f, sectionDestroyInterval);
        
    }

    void Update()
    {
        if (creatingSection == false)
        {
            creatingSection = true;
        }
        
    }

    // Spawning new random sections every 5 seconds 
    private void SpawnSection()
    {
        secNum = Random.Range(0,2);
        GameObject newSection = Instantiate(sectionPrefab[secNum], new Vector3(0, 0, zPos), Quaternion.identity);
        zPos += 50;
        sectionsList.AddLast(newSection);
        //creatingSection = false;

    }

    // Destroying old sections every 15 seconds
    private void DestroySection()
    {

        sectionToDestroy = sectionsList.FirstOrDefault();
        

        if (sectionsList.Count > 0)
        {
            Debug.Log("Found objects in the list");

            sectionsList.Remove(sectionToDestroy);
            Destroy(sectionToDestroy);
            Debug.Log("Destroyed Section");
        }
        else{
            Debug.LogWarning("Nothing found in the list");
        }

    }

}