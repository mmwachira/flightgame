using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBoundary: MonoBehaviour {

    public static float leftSide = -20f;
    public static float rightSide = 20f;
    public static float ground = 1.0f;
    public float internalLeft;
    public float internalRight;
    public float generalGround;

    void Update()
    {
        internalLeft = leftSide;
        internalRight = rightSide;
        generalGround = ground;
    }
}