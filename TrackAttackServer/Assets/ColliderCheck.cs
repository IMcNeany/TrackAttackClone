using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheck : MonoBehaviour
{
    public Server server;

    private void Awake()
    {
        server = GameObject.Find("ServerNetworkManager").GetComponent<Server>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            int id = other.GetComponent<PlayerID>().playerID;
            server.SendRespawnRequestToClients(id);
            Debug.Log("Player " + id + " hit collider");
        }
    }
}
