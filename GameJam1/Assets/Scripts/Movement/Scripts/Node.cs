using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Node
{
    public Vector3 position;
    public Vector2 arrayPosition;
    public int player = 0;
    public bool obstacle = false;

    public Node(float x, float y)
    {
        position = new Vector3(x, 0, y);
        arrayPosition = new Vector2(x, y);
    }
}