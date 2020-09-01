using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeList : MonoBehaviour
{
    public List<Transform> nodes;
    public AudioClip pop;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public int NodeCount()
    {
        return nodes.Count;
    }

    public Transform GetNode(int i)
    {
        return nodes[i];
    }
 
}
