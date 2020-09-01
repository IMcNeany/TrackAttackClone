using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
// A class to store player info, such as clientID, player type and score
    [Serializable]
    public class Player
    {
        public int playerID; // clientID
        public PlayerType type;
        public int score;

        public Player(int playerId, PlayerType type)
        {
            playerID = playerId;
            this.type = type;
            score = 0;
        }
    }

    public enum PlayerType
    {
        COMPUTER,
        PLAYER // human
    }

    public List<Player> players;
    private Server _server;

    private void Start()
    {
        _server = FindObjectOfType<Server>();
    }

    public void AddPlayerToList(int id, PlayerType playerType)
    {
        players.Add(new Player(id, playerType));
        // the name is in the player infos as name
   //     _server.SendAllClientsPlayerIDsUpdate(id, true);
    }

    public void RemovePlayerFromList(int id)
    {
        players.Remove(players.Find(player => player.playerID == id));
     //   _server.SendAllClientsPlayerIDsUpdate(id, false);
    }

    public void SendPlayersListToClients()
    {
        for (int i = 0; i < players.Count; i++)
        {
            _server.SendAllClientsPlayerIDsUpdate(players[i].playerID, _server.lobby.PlayerInfos[i].name);

        }
    }
}