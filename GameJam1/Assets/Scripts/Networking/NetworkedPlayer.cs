using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedPlayer : MonoBehaviour
{
    public ClientNetwork client;
    public int player_id = -1; //-1 for computer, 1-4 for players
    public NetworkGameManager nwgm;
    public void Update()
    {
        if(!nwgm)
        {
            nwgm = GameObject.Find("NetworkGameManager").GetComponent<NetworkGameManager>();
        }
        if(!client && !nwgm.game_finished)
        {
            client = GameObject.Find("ClientNetworkManager").GetComponent<ClientNetwork>();
        }
    }

    public void SetPlayerID(int id)
    {
        player_id = id;
    }
    public int GetPlayerID()
    {
        return player_id;
    }
    public void SetCartValues(Vector3 pos, Vector3 rot)
    {
        transform.localPosition = pos;
        transform.localRotation = Quaternion.Euler(rot);
    }

}
