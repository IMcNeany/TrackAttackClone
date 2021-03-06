﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Node
{

    public Vector3 position;
    public Vector2 arrayPosition;
    //player claimed tile
    public int player = 0;
    public bool obstacle = false;
    public int width = 8;
    public int height = 8;
    //if gold is on tile
    public bool gold = false;
    //track type on tile
    //none
    //curve
    //straight

    public Node(float x, float y, Vector3 basePoint, float yValue)
    {

        position = new Vector3(-((width / 2) - 1) + x - basePoint.x, yValue, -((height / 2)) + y - basePoint.z);

        //position = new Vector3(x, 0, y);
        //if AR Y value comes from hit pos? 

        // position -= basePoint;
        arrayPosition = new Vector2(x, y);
    }
}