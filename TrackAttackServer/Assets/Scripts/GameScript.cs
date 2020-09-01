using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GridNode holds data on a tile on the grid, e.g what tile, who owns it etc and this gets passed to each client
public class GridNode
{
    public int x;
    public int y;
    public int player_id;
    public int tile_type;
    public float tile_y_rotation; //testing this, might need additional tile types if the rotation needs to stay the same
}

public class GameScript : MonoBehaviour
{
    public Server server;
    private int player_count;
    public int grid_y = 10;
    public int grid_x = 10;
    public List<GameObject> player_carts;
    public List<GameObject> grid_base_objects;
    public List<GameObject> tiles;
    private List<bool> player_ready_states;
    private float ready_timeout = 15.0f;
    private float current_timeout = 0.0f;
    private bool clients_ready = false;
    private float start_countdown = 5.0f;
    private bool game_started = false;
    public List<List<GridNode>> grid_list;
    public bool force_start = false;
    private bool ar_ready = false;
    public GoldManager gold_manager;

    public bool set_client_user_info = false; //a check so we can send each clients the user ids of other players
    private PlayerManager _playerManager;
    public void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        CreateGame();
    }
    //set up the game scene, ready for players to load into
    public void CreateGame()
    {
        if (!server)
        {
            server = GameObject.Find("ServerNetworkManager").GetComponent<Server>();
        }

        player_count = server.GetPlayerCount();

        SetupGrid();
        SetupPlayers();
        _playerManager.SendPlayersListToClients();
        server.SendSceneToLoad("MultiplayerGame");

    }
    public void SetPlayerAmount(int num)
    {
        player_count = num;
    }

    public void ForceStart()
    {
        force_start = true;
    }

    private void Update()
    {
        if(!server)
        {
            server = GameObject.Find("ServerNetworkManager").GetComponent<Server>();
        }
        if(clients_ready == false && game_started == false) //players loading in
        {
            server.SendAllClientsUserCount();
            //send grid size to clients here
            CheckClientsReady();
        }
        else if(clients_ready == true && game_started == false) //countdown till game begins
        {
            UpdateCountdown();
            if(force_start)
            {
                game_started = true;

            }
        }
        else if(clients_ready == true && game_started == true) //game has started
        {
            UpdateGame();

        }
    }

    public void CheckClientsReady()
    {
        current_timeout += 1 * Time.deltaTime;
        if(current_timeout > ready_timeout)
        {
            //clients took longer than 15 seconds to load the game, probably timed out or something
            Debug.Log("Clients timedout?");
        }
        bool all_ready = true;
        for(int i = 0; i < player_count; i++)
        {
            if (player_ready_states[i] == false && force_start == false)
            {
                all_ready = false;
            }
        }
        if(all_ready == true)
        {
            clients_ready = true;
        }
    }

    public void UpdateCountdown()
    {
        if (!ar_ready)
        {
            gold_manager.SendGoldPositionUpdate();
            ar_ready = true;
        }
        start_countdown -= 1 * Time.deltaTime;
        //update clients with the current countdown
        if(start_countdown <= 0.0f)
        {
            game_started = true;
            NWMGameStart start = new NWMGameStart();
            start.started = true;
            server.SendGameStarted(start);
            server.GetComponent<InGameTimer>().StartTimer();

        }
    }

    public void ClientReady(int client_id, bool state)
    {
        client_id--;
        player_ready_states[client_id] = state;
    }

    public void UpdateGame()
    {
        SendUpdatedCartData();
       
    }

    private void SendUpdatedCartData()
    {
        NWMCartData cart_data = new NWMCartData();
        cart_data.SetArrays();
        for (int i = 0; i < player_carts.Count; i++)
        {
            cart_data.POSX[i] = player_carts[i].transform.position.x;
            cart_data.POSY[i] = player_carts[i].transform.position.y;
            cart_data.POSZ[i] = player_carts[i].transform.position.z;
            cart_data.ROTX[i] = player_carts[i].transform.rotation.x;
            cart_data.ROTY[i] = player_carts[i].transform.rotation.y;
            cart_data.ROTZ[i] = player_carts[i].transform.rotation.z;
        }
        server.UpdateClientCarts(cart_data);
    }

    public void SetPlayerCartData(NWMCartData cart_data)
    {
        int id = cart_data.player_ID - 1;
        for(int i = 0; i < player_carts.Count; i++)
        {
            //only want to get updates from cart_data of the player who sent it
            if(i != id)
            {
                continue;
            }
            Vector3 new_position = new Vector3(cart_data.POSX[i], cart_data.POSY[i], cart_data.POSZ[i]);
            Vector3 new_rotation = new Vector3(cart_data.ROTX[i], cart_data.ROTY[i], cart_data.ROTZ[i]);
            Vector3 current_position = player_carts[i].transform.position;
            Vector3 current_rotation = player_carts[i].transform.rotation.eulerAngles;

            player_carts[i].transform.position = new_position;
            player_carts[i].transform.rotation = Quaternion.Euler(new_rotation);

        }
    }

    //set new info of the grid from a client
    public void SetGridTile(NWMGridData grid_message)
    {
        for(int y = 0; y < grid_y; y++)
        {
            for(int x = 0; x < grid_y; x++)
            {
                if(grid_message.grid_x == x && grid_message.grid_y == y)
                {
                    grid_list[y][x].tile_type = grid_message.tile_type;
                    grid_list[y][x].player_id = grid_message.player_ID;
                    grid_list[y][x].tile_y_rotation = grid_message.tile_rotation;
                    UpdateVisualGrid(grid_message.grid_x, grid_message.grid_y, grid_message.tile_type, grid_message.player_ID, grid_message.tile_rotation);
                    server.UpdateClientGrids(grid_message);
                }
            }
        }
    }

    public void SetupGrid()
    {
        //create the grid list - gameobjects are created using grid nodes as reference
        grid_list = new List<List<GridNode>>();
        for (int y = 0; y < grid_y; y++)
        {
            List<GridNode> row_list = new List<GridNode>();
            for (int x = 0; x < grid_y; x++)
            {
                GridNode node = new GridNode();
                node.x = x;
                node.y = y;
                node.player_id = -1;
                node.tile_type = 0;
                node.tile_y_rotation = 0.0f;
                row_list.Add(node);
            }
            grid_list.Add(row_list);
        }

        //set the grid base tiles 
        for (int y = 0; y < grid_y; y++)
        {
            for(int x = 0; x < grid_y; x++)
            {
                Vector3 position = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + y);
                GameObject base_object = Instantiate(Resources.Load("Prefabs/Base"), position, transform.rotation) as GameObject;
                base_object.transform.parent = transform;
                base_object.name = "Base Tile " + y + " " + x; 
                grid_base_objects.Add(base_object);
            }
        }
        //set the starting tiles

    }

    private void SetupPlayers()
    {
        player_ready_states = new List<bool>(player_count);
        for (int i = 0; i < player_count; i++)
        {
            bool ready_state = false;
            player_ready_states.Add(ready_state);
        }
        float cart_height_adjustment = 0.85f;
        player_carts = new List<GameObject>();
        for (int i = 0; i < player_count; i++)
        {
            Vector3 cart_position = Vector3.zero;
            if (i == 0)
            {
                cart_position = new Vector3(transform.position.x + grid_list[0][0].x, transform.position.y + cart_height_adjustment, transform.position.z + grid_list[0][0].y);
            }
            if (i == 1)
            {
                cart_position = new Vector3(transform.position.x + grid_list[grid_y - 1][0].x, transform.position.y + cart_height_adjustment, transform.position.z + grid_list[grid_y - 1][0].y);
            }
            if (i == 2)
            {
                cart_position = new Vector3(transform.position.x + grid_list[0][grid_y - 1].x, transform.position.y + cart_height_adjustment, transform.position.z + grid_list[0][grid_y - 1].y);
            }
            if (i == 3)
            {
                cart_position = new Vector3(transform.position.x + grid_list[grid_y - 1][grid_y - 1].x, transform.position.y + cart_height_adjustment, transform.position.z + grid_list[grid_y - 1][grid_y - 1].y);
            }
            GameObject player = Instantiate(Resources.Load("Prefabs/PlayerCart"), cart_position, transform.rotation) as GameObject;
            player.name = "Player Cart " + (i + 1);
            player.GetComponent<PlayerID>().playerID = i+1; // add id to the cart so it can be checked who is the owner of the cart eg. on collison
            player_carts.Add(player);
        }
    }

    //using gridnode data, update gameobjects of tile on the server
    public void UpdateVisualGrid(int x, int y, int type, int id, float rot)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            //player exited the area, make em respawn
        }
    }
}
