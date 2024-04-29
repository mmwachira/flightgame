using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Collectible : MonoBehaviour {
    [SerializeField] private GameObject coinText;

    [SerializeField] private int coins;

    [SerializeField] private AudioSource coinCollectSound;


    void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Collectibles")
        {
            coinCollectSound.Play();
            coins += 1;
            Debug.Log("Picked up a collectible");
            coinText.GetComponent<TMP_Text>().text = "" + coins;

            Destroy(collision.gameObject);
        }
    }
}