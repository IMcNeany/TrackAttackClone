using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class ClientNetwork : MonoBehaviour
{
    public InputField user_input;
    public InputField user_name;
    public GameObject lobby;
    public GameObject menu;
    public Image ready_button;
    public NetworkGameManager network_GM;
    public PlayerManager playerManager;
    public Image ARImage;
    public Sprite ARON;
    public Sprite AROFF;
    public Text ARText;

    private bool AR = false;
    public bool AR_avaliable = false;

    private const int MAX_USER = 4;
    private const int PORT = 26000;
    private const int WEBPORT = 26001;
    private const int BYTE_SIZE = 1024;
    private const float tick_rate = 0.1f;
    private float current_tick = 0.0f;
    private string SERVER_IP = "127.0.0.1";
    private int host_id;
    private int client_id = -1;
    public int player_id = -1;
    private bool player_ready = false;
    public int connected_user_count = -1;

    private int reliable;
    private int unreliable;
    private int reliable_ordered;
    public bool in_lobby = true;
    public bool in_game = false;
    public bool attempting_connection = false;
    public bool client_connected = false;

    private byte error;

    private bool client_server_started = false;


    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartClientServer();
        playerManager = FindObjectOfType<PlayerManager>();
    }

    private void Update()
    {
        current_tick += 1 * Time.deltaTime;
        UpdateMessage();
        if (in_lobby)
        {
            UpdateClientLobbyScene();
        }
        else if (in_game)
        {
            if (network_GM == null)
            {
                network_GM = GameObject.Find("NetworkGameManager").GetComponent<NetworkGameManager>();
            }
        }
    }

    public void StartClientServer()
    {
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();

        unreliable = cc.AddChannel(QosType.Unreliable);
        reliable = cc.AddChannel(QosType.Reliable);
        reliable_ordered = cc.AddChannel(QosType.ReliableSequenced);
        HostTopology topo = new HostTopology(cc, MAX_USER);
        host_id = NetworkTransport.AddHost(topo, 0);
        client_server_started = true;
    }

    public void ConnectToServer()
    {
        if (!client_connected && !attempting_connection)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
                //web client
                client_id = NetworkTransport.Connect(host_id, SERVER_IP, WEBPORT, 0, out error);
                Debug.Log(client_id);
                Debug.Log("Attempting to connect from web to IP: " + SERVER_IP);
#else
            //standalone client
            client_id = NetworkTransport.Connect(host_id, SERVER_IP, PORT, 0, out error);
            Debug.Log(client_id);
            Debug.Log("Attempting to connect from standalone to IP: " + SERVER_IP + " Port: " + PORT);

#endif
            attempting_connection = true;
        }
    }

    public void DisconnectFromServer()
    {
        NetworkTransport.Disconnect(host_id, client_id, out error);
    }

    public void StopClientServer()
    {
        DisconnectFromServer();
        client_server_started = false;
        NetworkTransport.Shutdown();
    }

    //send message to the server
    public void SendServerMessage(NetworkMessage message, int channel_id)
    {
        if (channel_id == unreliable)
        {
            if (current_tick < tick_rate)
            {
                //trying not to send too many messages
                return;
            }
            else
            {
                current_tick = 0.0f;
            }
        }

        byte[] byte_buffer = new byte[BYTE_SIZE];

        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream memory_stream = new MemoryStream(byte_buffer);
        formatter.Serialize(memory_stream, message);

        NetworkTransport.Send(host_id, client_id, channel_id, byte_buffer, BYTE_SIZE, out error);
    }


    public void UpdateMessage()
    {
        if (!client_server_started)
        {
            return;
        }

        int rec_host_id; //standalone vs web
        int connection_id; //which user
        int channel_id; //the lane
        byte[] byte_buffer = new byte[BYTE_SIZE];
        int data_size;

        NetworkEventType type = NetworkTransport.Receive(out rec_host_id, out connection_id, out channel_id, byte_buffer, BYTE_SIZE, out data_size, out error);
        if (client_id == connection_id)
        {
            switch (type)
            {
                case NetworkEventType.Nothing:
                    break;
                case NetworkEventType.DataEvent:
                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream memory_stream = new MemoryStream(byte_buffer);
                    NetworkMessage message = (NetworkMessage) formatter.Deserialize(memory_stream);
                    OnDataReceived(channel_id, host_id, message);
                    break;
                case NetworkEventType.ConnectEvent:
                    Debug.Log("Connected to server, ID: " + client_id);
                    client_connected = true;
                    break;
                case NetworkEventType.DisconnectEvent:
                    Debug.Log("Disconnected from server");
                    player_ready = false;
                    attempting_connection = false;
                    client_connected = false;
                    player_id = -1;
                    break;
                default:
                case NetworkEventType.BroadcastEvent:
                    Debug.Log("Incorrect Data type");
                    break;
            }

            if (type == NetworkEventType.DataEvent)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream memory_stream = new MemoryStream(byte_buffer);
                NetworkMessage message = (NetworkMessage) formatter.Deserialize(memory_stream);
                if (message.operation == NetworkOperation.assign_num)
                {
                    NWMAssignPlayerNum assigner = (NWMAssignPlayerNum) message;
                    player_id = assigner.num;
                    playerManager.ClientId = player_id;
                    Debug.Log("Player num: " + player_id);
                }
            }
        }
    }

    private void OnDataReceived(int channel_id, int host_id, NetworkMessage message)
    {
        switch (message.operation)
        {
            case NetworkOperation.none:

                break;

            case NetworkOperation.set_ready:
                break;
            case NetworkOperation.cart_data:
                NWMCartData cart_data = (NWMCartData) message;
                ReceiveCartData(cart_data);
                break;
            case NetworkOperation.grid_single:
                NWMGridData grid_data = (NWMGridData) message;
                ReceiveSingleGridData(grid_data);
                break;
            case NetworkOperation.user_counter:
                NWMUserCount user_counter = (NWMUserCount) message;
                ReceiveUserCount(user_counter);
                break;
            case NetworkOperation.scene_load:
                NWMSceneLoad scene_load = (NWMSceneLoad) message;
                ReceiveSceneToLoad(scene_load);
                break;
            case NetworkOperation.assign_num:
                NWMAssignPlayerNum assigner = (NWMAssignPlayerNum) message;
                player_id = assigner.num;
                Debug.Log(client_id);
                break;
            case NetworkOperation.gold_pos:
                NWMGoldUpdate goldUpdate = (NWMGoldUpdate) message;
                ReceiveGoldPositionUpdate(goldUpdate);
                break;
            case NetworkOperation.player_score:
                NWMScoreUpdate scoreUpdate = (NWMScoreUpdate) message;
                ReceiveScoreUpdate(scoreUpdate);
                break;
            case NetworkOperation.player_info:
                NWMPlayerIDUpdate idUpdate = (NWMPlayerIDUpdate) message;
                ReceivePlayerDataUpdate(idUpdate);
                break;
            case NetworkOperation.game_start:
                NWMGameStart start_msg = (NWMGameStart) message;
                SetClientGameStarted(start_msg);

                break;
            case NetworkOperation.player_respawn:
                NWMPlayerRespawn respawn_data = (NWMPlayerRespawn) message;
                ReceivePlayerRepsawnData(respawn_data);
                break;
            case NetworkOperation.timer_seconds:
                NWMTimerUpdate timer_data = (NWMTimerUpdate) message;
                ReceiveUpdateTimer(timer_data);
                break; 
            case NetworkOperation.game_finished:
                NWMGameFinishedSendInfo game_finished_info = (NWMGameFinishedSendInfo) message;
                ReceiveGameFinishedInfo(game_finished_info);
                break;
        }
    }

    public void SetClientGameStarted(NWMGameStart start_message)
    {
        network_GM.game_started = start_message.started;
    }

    public void SetARMode()
    {
        if(AR_avaliable)
        {
            if (AR)
            {
                AR = false;
                ARImage.sprite = AROFF;
                ARText.text = "AR OFF";
            }
            else
            {
                AR = true;
                ARImage.sprite = ARON;
                ARText.text = "AR ON";
            }
        }
        else
        {
            AR = false;
            ARImage.sprite = AROFF;
            ARText.text = "AR OFF";
        }
       
    }

    public void ReceiveSceneToLoad(NWMSceneLoad scene_load)
    {
        if(AR)
        {
            SceneManager.LoadScene(scene_load.scene_name + "AR");
        }
        else
        {
            SceneManager.LoadScene(scene_load.scene_name);
        }
        in_lobby = false;
        in_game = false;
    }

    public void ReceiveUserCount(NWMUserCount user_counter)
    {
        if (connected_user_count == -1)
        {
            connected_user_count = user_counter.count;
            Debug.Log("NetworkCalled");
            if (!network_GM)
            {
                network_GM = GameObject.Find("NetworkGameManager").GetComponent<NetworkGameManager>();
            }
            if(!AR)
            {
                network_GM.SetupScene();
                network_GM.AssignClientPlayer(player_id);
                network_GM.AssignNetworkPlayer(connected_user_count);
            }
            
        }
    }

    public void UpdateClientLobbyScene()
    {
        SERVER_IP = user_input.text;

        if (client_connected)
        {
            menu.SetActive(false);
            lobby.SetActive(true);
        }
        else
        {
            menu.SetActive(true);
            lobby.SetActive(false);
        }

        if (player_ready)
        {
            user_name.transform.parent.gameObject.SetActive(false);
            ready_button.color = Color.green;
        }
        else
        {
            user_name.transform.parent.gameObject.SetActive(true);
            ready_button.color = Color.red;
        }
    }

    public void PlayerReady()
    {
        player_ready = !player_ready;
        NWMPlayerReady msg = new NWMPlayerReady();
        msg.ready_state = player_ready;
        msg.player_name = user_name.text;
        SendServerMessage(msg, reliable);
    }

    public void ReceiveCartData(NWMCartData cart_data)
    {
        //go through cart data for each user
        for (int i = 0; i < MAX_USER; i++)
        {
            if ((i + 1) == player_id) // dont want to receive our own data
            {
                continue;
            }

            Vector3 cart_position = new Vector3(cart_data.POSX[i], cart_data.POSY[i], cart_data.POSZ[i]);
            Vector3 cart_rotation = new Vector3(cart_data.ROTX[i], cart_data.ROTY[i], cart_data.ROTZ[i]);

            //check the networked players ids with data data indexes
            for (int j = 0; j < network_GM.networked_players.Count; j++)
            {
                if ((i + 1) == network_GM.networked_players[j].player_id)
                {
                    network_GM.networked_players[j].SetCartValues(cart_position, cart_rotation);
                }
            }
        }
    }

    public void SendCartData(int player_id, Vector3 pos, Vector3 rot)
    {
        int id = player_id - 1;
        NWMCartData cart_data = new NWMCartData();
        cart_data.SetArrays();
        cart_data.player_ID = player_id;
        cart_data.POSX[id] = pos.x;
        cart_data.POSY[id] = pos.y;
        cart_data.POSZ[id] = pos.z;
        cart_data.ROTX[id] = rot.x;
        cart_data.ROTY[id] = rot.y;
        cart_data.ROTZ[id] = rot.z;
        SendServerMessage(cart_data, unreliable);
    }

    //receive update of one grid tile
    public void ReceiveSingleGridData(NWMGridData grid_single)
    {
        network_GM.ReceiveGridUpdateFromServer(grid_single);
    }

    public void SendGridData(NWMGridData grid_single)
    {
        SendServerMessage(grid_single, reliable);
    }

    public void ReceiveGoldPositionUpdate(NWMGoldUpdate goldUpdate)
    {
        network_GM.clientGoldManager.UpdateGoldPosition(goldUpdate.posX, goldUpdate.posZ);
    }

    public void ReceiveScoreUpdate(NWMScoreUpdate scoreUpdate)
    {
        // go throug the player manager and update the current list of players
        playerManager.UpdatePlayerScore(scoreUpdate.playerId, scoreUpdate.playerIdScore);
        network_GM.clientGoldManager.UpdateLeaderboard();
    }

    public void ReceivePlayerDataUpdate(NWMPlayerIDUpdate playerIdUpdate)
    {
        playerManager.AddPlayerToList(playerIdUpdate.playerId, playerIdUpdate.playerName);
    }

    public void ReceivePlayerRepsawnData(NWMPlayerRespawn respawn_data)
    {
        network_GM.SetToRespawn(respawn_data.player_id);
    }

    public void ReceiveUpdateTimer(NWMTimerUpdate timerData)
    {           
        if (!network_GM)
        {
            network_GM = GameObject.Find("NetworkGameManager").GetComponent<NetworkGameManager>();
        }
        network_GM.TimerManager.UpdateTimerUI(timerData.seconds);
    }   
    public void ReceiveGameFinishedInfo(NWMGameFinishedSendInfo isFinished)
    {
        if (isFinished.isGameFinished)
        {
           network_GM.FinishGame();
        }
    }
    
    public void SendInfoCartOffTracks()
    {
       NWMCartOffTracks offTracksData = new NWMCartOffTracks();
       offTracksData.cartBehindTracks = true;
       SendServerMessage(offTracksData, reliable);
    }
    
}