using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rotate : MonoBehaviour {
    private int rotateSpeed = 1;

    void Update()
    {
        transform.Rotate(0, rotateSpeed, 0, Space.World);
    }
}

