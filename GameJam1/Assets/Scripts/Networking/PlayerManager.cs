using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Serializable]
    public class Player
    {
        public int playerID; // clientID
        public int score;
        public string name;

        public Player(int playerId, string playerName)
        {
            playerID = playerId;
            name = playerName;
            score = 0;
        }
    }

    public enum PlayerType
    {
        COMPUTER,
        PLAYER // human
    }

    public List<Player> players;
    [SerializeField] private int _clientId;

    public int ClientId
    {
        get => _clientId;
        set => _clientId = value;
    }

    public void AddPlayerToList(int id, string name)
    {
        players.Add(new Player(id, name));
    }

    public void RemovePlayerFromList(int id)
    {
        players.Remove(players.Find(player => player.playerID == id));
    }

    public void UpdatePlayerScore(int id, int score)
    {
        players.Find(player => player.playerID == id).score = score;
    }
}