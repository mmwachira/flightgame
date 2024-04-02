using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBoundary: MonoBehaviour {

    public static float leftSide = -4.5f;
    public static float rightSide = 4.5f;
    public float internalLeft;
    public float internalRight;

    void Update()
    {
        internalLeft = leftSide;
        internalRight = rightSide;
    }
}