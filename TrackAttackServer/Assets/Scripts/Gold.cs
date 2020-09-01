using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour
{
    private GoldManager _goldManager;

    private void Start()
    {
        _goldManager = FindObjectOfType<GoldManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _goldManager.SpawnGold();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _goldManager.CollectGold(other.GetComponent<PlayerID>().playerID);
    }
}