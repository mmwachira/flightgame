using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreCounter : MonoBehaviour 
{
    [SerializeField] private GameObject scoredisplay;
    [SerializeField] private int score;
    private bool adding0is = false;

    void Update()
    {
        if(adding0is == false)
        {
            adding0is = true;
            StartCoroutine(Adding0is());
        }
    }

    IEnumerator Adding0is()
    {
        score += 1;
        scoredisplay.GetComponent<TMP_Text> ().text = "" + score;
        yield return new WaitForSeconds(0.25f);
        adding0is = false;
    }
}