using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = System.Numerics.Vector2;

public class GoldManager : MonoBehaviour
{
    private Server _server;
    private PlayerManager _playerManager;
    public Vector3 goldPosition;
    [SerializeField] private GameObject _goldObject;
    private GameScript _gameScript;

    // Start is called before the first frame update
    void Start()
    {
        if (!_server)
        {
            _server = GameObject.Find("ServerNetworkManager").GetComponent<Server>();
            _playerManager = GameObject.Find("ServerNetworkManager").GetComponent<PlayerManager>();
        }

        _gameScript = gameObject.GetComponent<GameScript>();
        SpawnGold();
        Invoke(nameof(SendGoldPositionUpdate), 1f);
    }


    public void SpawnGold()
    {
        int x = Random.Range(1, _gameScript.grid_x - 1); // so it never spawns in the corners of the board, where is ppl respawn
        int z = Random.Range(1, _gameScript.grid_y - 1); // so it never spawns in the corners of the board, where is ppl respawn
        float y = 0.85f;

        goldPosition = new Vector3(x, y, z);
        _goldObject.transform.position = goldPosition;
    }

    public void CollectGold(int clientId)
    {
        SendGoldPositionUpdate();
        UpdatePlayerScore(clientId);
    }

    public void UpdatePlayerScore(int clientId)
    {
        _playerManager.players.Find(player => player.playerID == clientId).score++;
        _server.SendAllClientsScoreUpdate(_playerManager.players.Find(player => player.playerID == clientId).playerID, _playerManager.players.Find(player => player.playerID == clientId).score);
        
    }


    public void SendGoldPositionUpdate()
    {
        Vector2Int posXZ = new Vector2Int();
        posXZ.x = (int) goldPosition.x;
        posXZ.y = (int) goldPosition.z;
        _server.SendAllClientsGoldPositionUpdate(posXZ);
    }
}