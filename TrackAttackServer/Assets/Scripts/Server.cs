using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Server : MonoBehaviour
{
    private const int MAX_USER = 4;
    private const int PORT = 26000;
    private const int WEBPORT = 26001;
    private const int BYTE_SIZE = 1024;
    private const float tick_rate = 0.1f;
    private float cart_tick = 0.0f;
    private float grid_tick = 0.0f;

    private int host_id;
    private int web_host_id;
    private int reliable;
    private int unreliable;
    private int reliable_ordered;
    private int current_user_amount = 0;

    private bool server_started = false;

    private byte error;
    public LobbyScript lobby;
    public GameScript game;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        CheckScene();
        if (lobby != null && server_started == false)
        {
            StartServer();
        }
    }

    private void CheckScene()
    {
        if (GameObject.Find("Lobby"))
        {
            lobby = GameObject.Find("Lobby").GetComponent<LobbyScript>();
            game = null;
        }
        else if (GameObject.Find("GameManager"))
        {
            game = GameObject.Find("GameManager").GetComponent<GameScript>();
            lobby = null;
        }
    }

    public int GetPlayerCount()
    {
        if (current_user_amount == 0)
        {
            return 4; //if 0, then we're probably in debug mode, so tell the system theres 4 players
        }
        else
        {
            return current_user_amount;
        }
    }

    private void Update()
    {
        CheckScene();
        UpdateMessagePump();
        CheckInputs();
    }

    public void CheckInputs()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void StartServer()
    {
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();

        unreliable = cc.AddChannel(QosType.Unreliable);
        reliable = cc.AddChannel(QosType.Reliable);
        reliable_ordered = cc.AddChannel(QosType.ReliableSequenced);
        HostTopology topo = new HostTopology(cc, MAX_USER);

        //server only 

        host_id = NetworkTransport.AddHost(topo, PORT, null);
        web_host_id = NetworkTransport.AddWebsocketHost(topo, WEBPORT, null);

        server_started = true;
        Debug.Log("Opening connection on port: " + PORT + " and webport: " + WEBPORT);
        lobby.CreateLobby();
    }

    public void StopServer()
    {
        server_started = false;
        NetworkTransport.Shutdown();
    }

    //send message to clients
    public void SendClientMessage(int rec_host, int client_id, int channel_id, NetworkMessage message)
    {
        byte[] byte_buffer = new byte[BYTE_SIZE];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memory_stream = new MemoryStream(byte_buffer);
        formatter.Serialize(memory_stream, message);
        if (rec_host == 0)
        {
            NetworkTransport.Send(host_id, client_id, channel_id, byte_buffer, BYTE_SIZE, out error);
        }
        else
        {
            NetworkTransport.Send(web_host_id, client_id, channel_id, byte_buffer, BYTE_SIZE, out error);
        }
    }

    public void SendAllClientsMessage(int rec_host, int channel_id, NetworkMessage message)
    {
        byte[] byte_buffer = new byte[BYTE_SIZE];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memory_stream = new MemoryStream(byte_buffer);
        formatter.Serialize(memory_stream, message);
        for (int i = 1; i < current_user_amount + 1; i++)
        {
            if (rec_host == 0)
            {
                NetworkTransport.Send(host_id, i, channel_id, byte_buffer, BYTE_SIZE, out error);
            }
            else
            {
                NetworkTransport.Send(web_host_id, i, channel_id, byte_buffer, BYTE_SIZE, out error);
            }
        }
    }


    public void UpdateMessagePump()
    {
        if (!server_started)
        {
            return;
        }

        int rec_host_id; //standalone vs web
        int client_id; //user
        int channel_id; //lane

        byte[] byte_buffer = new byte[BYTE_SIZE];
        int data_size;

        NetworkEventType type = NetworkTransport.Receive(out rec_host_id, out client_id, out channel_id, byte_buffer, BYTE_SIZE, out data_size, out error);

        if (type != NetworkEventType.Nothing && type != NetworkEventType.BroadcastEvent)
        {
            //Debug.Log("Receive client ID: " + client_id);
        }

        switch (type)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.DataEvent:
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream memory_stream = new MemoryStream(byte_buffer);
                NetworkMessage message = (NetworkMessage) formatter.Deserialize(memory_stream);
                OnDataRecieved(client_id, channel_id, host_id, message);
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("User connected with ID: " + client_id);
                current_user_amount++;
                AssignClientID(client_id);
                if (lobby)
                {
                    gameObject.GetComponent<PlayerManager>().AddPlayerToList(client_id, PlayerManager.PlayerType.PLAYER); // add the player info to the list of players
                    lobby.SetPlayerConnection(client_id, connection_status.connected);
                }

                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("User Disconnected with ID: " + client_id);

                if (lobby)
                {
                    gameObject.GetComponent<PlayerManager>().RemovePlayerFromList(client_id); // remove player from the list of players
                    lobby.SetPlayerConnection(client_id, connection_status.disconnected);
                }

                current_user_amount--;
                break;
            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Incorrect Data type");
                break;
        }
    }

    private void AssignClientID(int client_id)
    {
        NWMAssignPlayerNum assigner = new NWMAssignPlayerNum();
        assigner.num = client_id;
        SendClientMessage(host_id, client_id, reliable_ordered, assigner);
    }

    private void OnDataRecieved(int client_id, int channel_id, int host_id, NetworkMessage message)
    {
        switch (message.operation)
        {
            case NetworkOperation.none:

                break;

            case NetworkOperation.set_ready:
                NWMPlayerReady ready_message = (NWMPlayerReady) message;
                if (lobby)
                {
                    lobby.SetPlayerReadyState(client_id, ready_message.ready_state);
                    lobby.SetPlayerName(client_id, ready_message.player_name);
                }
                else if (game)
                {
                    game.ClientReady(client_id, ready_message.ready_state);
                }

                break;
            case NetworkOperation.cart_data:
                NWMCartData cart_message = (NWMCartData) message;
                game.SetPlayerCartData(cart_message);
                break;
            case NetworkOperation.grid_single:
                NWMGridData grid_message = (NWMGridData) message;
                game.SetGridTile(grid_message);
                break;
            case NetworkOperation.cart_off_tracks:
                ReceiveOffTrackInfo(client_id);
                break;
        }
    }

    public void UpdateClientGrids(NWMGridData grid_data)
    {
        SendAllClientsMessage(host_id, reliable, grid_data);
    }

    public void UpdateClientCarts(NWMCartData cart_data)
    {
        cart_tick += 1 * Time.deltaTime;
        if (cart_tick > tick_rate)
        {
            SendAllClientsMessage(host_id, unreliable, cart_data);
            cart_tick = 0.0f;
        }
    }

    public void SendGameStarted(NWMGameStart game_start)
    {
        SendAllClientsMessage(host_id, reliable, game_start);
    }

    public void SendAllClientsUserCount()
    {
        NWMUserCount user_counter = new NWMUserCount();
        user_counter.count = current_user_amount;
        SendAllClientsMessage(host_id, reliable, user_counter);
    }

    public void SendSceneToLoad(string scene_name)
    {
        NWMSceneLoad scene_load = new NWMSceneLoad();
        scene_load.scene_name = scene_name;
        SendAllClientsMessage(host_id, reliable, scene_load);
    }

    public void SendAllClientsGoldPositionUpdate(Vector2Int newGoldPosition)
    {
        NWMGoldUpdate _goldUpdate = new NWMGoldUpdate();
        _goldUpdate.posX = newGoldPosition.x;
        _goldUpdate.posZ = newGoldPosition.y;
        SendAllClientsMessage(host_id,reliable, _goldUpdate);
    }
    public void SendAllClientsScoreUpdate(int playerId, int playerScore)
    {
        NWMScoreUpdate _playerUpdate = new NWMScoreUpdate();
        _playerUpdate.playerId = playerId;
        _playerUpdate.playerIdScore = playerScore;
        SendAllClientsMessage(host_id,reliable, _playerUpdate);
    }
    public void SendAllClientsPlayerIDsUpdate(int playerId, string playerName)
    {
        NWMPlayerIDUpdate _playerUpdate = new NWMPlayerIDUpdate();
        _playerUpdate.playerId = playerId;
        _playerUpdate.playerName = playerName;
        SendAllClientsMessage(host_id,reliable, _playerUpdate);
    }

    public void SendRespawnRequestToClients(int id)
    {
        NWMPlayerRespawn respawn_data = new NWMPlayerRespawn();
        respawn_data.player_id = id;
        SendAllClientsMessage(host_id, reliable, respawn_data);
    }
    public void SendTimerUpdateToAllClients(int seconds)
    {
        NWMTimerUpdate timer_data = new NWMTimerUpdate();
        timer_data.seconds = seconds;
        SendAllClientsMessage(host_id, reliable, timer_data);
    }    
    
    public void SendGameFinishedNotificationToAllClients(bool gameFinished)
    {
        NWMGameFinishedSendInfo game_finished_data = new NWMGameFinishedSendInfo();
        game_finished_data.isGameFinished = gameFinished;
        SendAllClientsMessage(host_id, reliable, game_finished_data);
    }
    
    public void ReceiveOffTrackInfo(int id)
    {
        SendRespawnRequestToClients(id);
    }
}