using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkGameManager : MonoBehaviour
{
    public List<GameObject> player_carts;
    public List<NetworkedPlayer> networked_players;
    public ClientPlayer client_player;
    public ClientGrid client_grid;
    public ClientNetwork client_network;
    public int grid_size = 10;
    //public List<AIPlayer> ai_player;
    public ClientGoldManager clientGoldManager;
    public float respawn_timer = 0.0f;
    public bool should_respawn = false;
    private float respawn_time = 5.0f;
    private float current_respawn = 0.0f;
    public bool game_started = false;
    public bool loading = true;
    private TimerManager _timerManager;
    public GameObject loadingScreen;
    public TextMeshProUGUI respawnTimerText;
    public List<Button> playerControls;
    public TimerManager TimerManager => _timerManager;
    private FollowCanvas _followCanvas;
    private bool canSendData;
    public bool AR = false;
    public bool game_finished = false;
    public SoundsPlayer soundsPlayer;
    private void Awake()
    {
        canSendData = true;
        soundsPlayer = GetComponent<SoundsPlayer>();
        _followCanvas = FindObjectOfType<FollowCanvas>();
        _timerManager = GetComponent<TimerManager>();
    }



    void Update()
    {
        //update server with clients cart data
        if(client_player && game_started && canSendData)
        {
            client_network.SendCartData(client_player.ID, client_player.cart_pos, client_player.cart_rot);
            client_grid.game_start = true;
            if (loading)
            {
                loadingScreen.SetActive(true);
            }
            UpdateRespawns();          
          
        }
    }

    public void ChangeControlsState(bool state)
    {
        foreach (var button in playerControls)
        {
            button.interactable = state;
        }
    }

    public void SetGameStarted(bool value)
    {
        game_started = value;
       
    }

    public void SetClientGrid(ClientGrid grid)
    {
        client_grid = grid;
    }

    public void SetupScene()
    {
        clientGoldManager = FindObjectOfType<ClientGoldManager>();
        //if start is called the scene is probably loaded?
        client_network = GameObject.Find("ClientNetworkManager").GetComponent<ClientNetwork>();
        client_network.PlayerReady(); // player ready will be false here, as we need to load in
        clientGoldManager.AssignPlayerManager(client_network.playerManager);
        clientGoldManager.LeaderboardSetup();

        if (client_network == null)
        {
            Debug.Log("No ClientNetwork Object");
        }

        client_grid.SetupClientGrid(grid_size, this);
        SetupPlayerCarts(client_network.connected_user_count);
        client_network.in_game = true;
        client_network.in_lobby = false;

      
        client_network.PlayerReady(); //player ready gets set to true again when we're ready
       

    }

    public void SetupPlayerCarts(int player_count)
    {
        float cart_height_adjustment = 0.85f;
        Vector3 grid_position = client_grid.gameObject.transform.position;
        for (int i = 0; i < player_count; i++)
        {
            Vector3 cart_position = Vector3.zero;
            if (i == 0)
            {
                cart_position = new Vector3(grid_position.x + client_grid.grid_list[0][0].x, grid_position.y + cart_height_adjustment, grid_position.z + client_grid.grid_list[0][0].y);
            }
            if (i == 1)
            {
                cart_position = new Vector3(grid_position.x + client_grid.grid_list[grid_size - 1][0].x, grid_position.y + cart_height_adjustment, grid_position.z + client_grid.grid_list[grid_size - 1][0].y);
            }
            if (i == 2)
            {
                cart_position = new Vector3(grid_position.x + client_grid.grid_list[0][grid_size - 1].x, grid_position.y + cart_height_adjustment, grid_position.z + client_grid.grid_list[0][grid_size - 1].y);
            }
            if (i == 3)
            {
                cart_position = new Vector3(grid_position.x + client_grid.grid_list[grid_size - 1][grid_size - 1].x, grid_position.y + cart_height_adjustment, grid_position.z + client_grid.grid_list[grid_size - 1][grid_size - 1].y);
            }
            GameObject player = Instantiate(Resources.Load("Prefabs/PlayerCart"), cart_position, client_grid.gameObject.transform.rotation) as GameObject;
            player.name = "Player Cart " + (i + 1);
            player.transform.parent = client_grid.gameObject.transform;
            player_carts.Add(player);
        }
    }

    public void UpdateRespawns()
    {
        if(should_respawn)
        {
            
            respawnTimerText.gameObject.SetActive(true);

            current_respawn -= 1 * Time.deltaTime;
            int timerInt = Convert.ToInt32(current_respawn);
            respawnTimerText.text = " Respawning in " + timerInt + "...";

            if(current_respawn <= 0.0f)
            {
                SetCartStartPosition(client_player.ID);
                current_respawn = respawn_time;
                should_respawn = false;
                ChangeControlsState(true);
                respawnTimerText.gameObject.SetActive(false);

            }
        }
    }

    public void SetToRespawn(int id)
    {
        if(id == client_player.ID)
        {
            ChangeControlsState(false);
            soundsPlayer.PlayCrashSound();
            Handheld.Vibrate();
            should_respawn = true;
            current_respawn = respawn_time;
        }
    }
    public void SetCartStartPosition(int id)
    {
        id -= 1;
        float cart_height_adjustment = 0.85f;
        Vector3 grid_position = client_grid.gameObject.transform.position;
        Vector3 cart_position = Vector3.zero;
        switch(id)
        {
            case 0:
                cart_position = new Vector3(grid_position.x + client_grid.grid_list[0][0].x, grid_position.y + cart_height_adjustment, grid_position.z + client_grid.grid_list[0][0].y);
                break;

            case 1:
                cart_position = new Vector3(grid_position.x + client_grid.grid_list[grid_size - 1][0].x, grid_position.y + cart_height_adjustment, grid_position.z + client_grid.grid_list[grid_size - 1][0].y);
                break;
            case 2:
                cart_position = new Vector3(grid_position.x + client_grid.grid_list[0][grid_size - 1].x, grid_position.y + cart_height_adjustment, grid_position.z + client_grid.grid_list[0][grid_size - 1].y);
                break;
            case 3:
                cart_position = new Vector3(grid_position.x + client_grid.grid_list[grid_size - 1][grid_size - 1].x, grid_position.y + cart_height_adjustment, grid_position.z + client_grid.grid_list[grid_size - 1][grid_size - 1].y);
                break;
        }
        player_carts[id].transform.position = cart_position;
        id += 1;
        client_grid.AssignStartSelection(id);

    }

    public void AssignClientPlayer(int player_num)
    {
        //id - 1 because id = 1-4 but list starts at 0
        int clients_ID = player_num - 1;
        for(int i = 0; i < player_carts.Count; i++)
        {
            if(i == clients_ID)
            {
                player_carts[i].AddComponent<ClientPlayer>();
                client_player = player_carts[i].GetComponent<ClientPlayer>();
                client_player.ID = player_num;
                _followCanvas.CartToFollow = player_carts[i].transform;
                client_grid.AssignStartSelection(player_num);
            }
        }
    }

    public void AssignNetworkPlayer(int count)
    {
        for(int i = 0; i < player_carts.Count; i++)
        {
            if(i != client_player.ID - 1)
            {
                player_carts[i].AddComponent<NetworkedPlayer>();
                player_carts[i].GetComponent<NetworkedPlayer>().SetPlayerID(i + 1);
                Destroy(player_carts[i].GetComponent<CartMovement>());
                networked_players.Add(player_carts[i].GetComponent<NetworkedPlayer>());
            }
        }
    }

    public void ReceiveGridUpdateFromServer(NWMGridData grid_data)
    {
        GridNode node_update = new GridNode();
        node_update.player_id = grid_data.player_ID;
        node_update.tile_type = grid_data.tile_type;
        node_update.tile_y_rotation = grid_data.tile_rotation;
        node_update.x = grid_data.grid_x;
        node_update.y = grid_data.grid_y;
        client_grid.UpdateClientGrid(node_update);
    }

    public void ReadyPlayer()
    {
        client_network.PlayerReady();
    }

    public void SendGridUpdateToServer(GridNode node)
    {
        NWMGridData grid_data = new NWMGridData();
        grid_data.grid_x = node.x;
        grid_data.grid_y = node.y;
        grid_data.player_ID = node.player_id;
        grid_data.tile_rotation = node.tile_y_rotation;
        grid_data.tile_type = node.tile_type;
        client_network.SendGridData(grid_data);
    }

    public void SetRespawn()
    {

    }

    public void FinishGame()
    {
        game_finished = true;
        clientGoldManager.DisplayFinalLeaderboard();
        client_player.canMove = false;
        canSendData = false;
        client_network.StopClientServer();
        Destroy(client_network.gameObject);
    }
}
