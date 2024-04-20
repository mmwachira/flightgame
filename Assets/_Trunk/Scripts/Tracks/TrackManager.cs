using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TrackManager : MonoBehaviour
{
    //public GameObject[] section;
    public GameObject sectionPrefab;
    public float sectionSpawnInterval = 2f;

    private List<GameObject> sectionsList = new List<GameObject>();
    //public GameObject sectiontoDestroy;
    public string parentName;
    public int zPos = 50;
    public int secNum;

    public bool creatingSection = false;



    void Start()
    {
        InvokeRepeating("SpawnSection", 0f, sectionSpawnInterval);
        parentName = transform.name;
        StartCoroutine(DestroySection(gameObject));
    }

    void Update()
    {
        if (creatingSection == false)
        {
            creatingSection = true;
            //StartCoroutine(GenerateSection());
        }
        
    }

    private void SpawnSection()
    {
        GameObject newSection = Instantiate(sectionPrefab, new Vector3(0, 0, zPos), Quaternion.identity);
        zPos += 50;
        sectionsList.Add(newSection);
        
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
        yield return new WaitForSeconds(3);
        if (sectionsList.Contains(sectionToDestroy))
        {
            sectionsList.Remove(sectionToDestroy);
            Destroy(sectionToDestroy);
            Debug.Log("Destroyed section: " + sectionToDestroy.name);
        }
        else
        {
            Debug.LogWarning("Section not found in the list!");
        }
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