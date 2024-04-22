using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class TrackManager : MonoBehaviour
{
    //public GameObject[] section;
    public GameObject[] sectionPrefab;
    public float sectionSpawnInterval = 2f;

    private List<GameObject> sectionsList = new List<GameObject>();
    public string parentName;
    public int zPos = 50;
    public int secNum;

    public bool creatingSection = false;



    void Start()
    {
        InvokeRepeating("SpawnSection", 0f, sectionSpawnInterval);
        parentName = transform.name;
        
    }

    void Update()
    {
        if (creatingSection == false)
        {
            creatingSection = true;
            //StartCoroutine(GenerateSection());
        }
        
    }

    void FixedUpdate()
    {
        StartCoroutine(DestroySection(gameObject));
    }

    private void SpawnSection()
    {
        secNum = Random.Range(0,2);
        GameObject newSection = Instantiate(sectionPrefab[secNum], new Vector3(0, 0, zPos), Quaternion.identity);
        zPos += 50;
        sectionsList.Add(newSection);
        if(sectionsList.Contains(newSection))
        {
            Debug.Log("New section added");
        }
        else{
            Debug.Log("Could not add new section");
        }
        
    }

// IEnumerator GenerateSection(){
//     secNum = Random.Range(0, 2);
//     Instantiate(section[secNum], new Vector3(0, 0, zPos), Quaternion.identity);
//     zPos += 50;
//     yield return new WaitForSeconds(3);
//     creatingSection = false;
// }

    IEnumerator DestroySection(GameObject sectionToDestroy)
    {
        sectionToDestroy = sectionsList.FirstOrDefault();
        yield return new WaitForSeconds(20);

        if (sectionsList.Count > 0)
        {
            Debug.Log("Found objects in the list");
            
            Destroy(sectionToDestroy);
            Debug.Log("Destroyed Section");
        }
        else{
            Debug.Log("Nothing found in the list");
        }

        // if (sectionsList.Contains(sectionToDestroy))
        // {
        //     sectionsList.Remove(sectionToDestroy);
        //     Destroy(sectionToDestroy);
        //     Debug.Log("Destroyed section: " + sectionToDestroy.name);
        // }
        // else
        // {
        //     Debug.LogWarning("Section not found in the list!");
        // }
    }

    // IEnumerator DestroyClone()
    // {
    //     yield return new WaitForSeconds(20);
    //     if (parentName == "LevelController")
    //     {
    //         Destroy(sectiontoDestroy);
    //     }
    //     // if (parentName == "Section(Clone)")
    //     // {
    //     //     Destroy(gameObject);
    //     //     Debug.Log("Section destroyed!");
    //     // }
    // }
}