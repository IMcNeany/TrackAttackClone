using System;
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

public enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public class ClientGrid : MonoBehaviour
{
    public int player_id;
    public int grid_y = 10;
    public int grid_x = 10;
    public int selection_x = 0;
    public int selection_y = 0;
    public NetworkGameManager network_manager;
    public Direction current_direction;
    public GameObject selection_object;
    public List<List<GridNode>> grid_list;
    public List<List<GameObject>> grid_base_objects;
    public List<List<GameObject>> visual_tracks;
    public List<Vector2> trackNodePos;
    public GameObject track_holder;
    public bool game_start = false;
    public bool in_editor = false;

    private int grid_size;
    float timer = 5.0f;
    float time = 0;
    bool alreadyInvoking = false;

    private void Awake()
    {
        if(in_editor)
        {
            //SetupClientGrid(10, null);
        }
    }

    public void SetupClientGrid(int size, NetworkGameManager manager)
    {
        if(!in_editor)
        {
            network_manager = manager;
        }
        grid_size = size;
        grid_base_objects = new List<List<GameObject>>();
        trackNodePos = new List<Vector2>();
        grid_list = new List<List<GridNode>>();
        visual_tracks = new List<List<GameObject>>();
        for (int y = 0; y < size; y++)
        {
            List<GridNode> row_list = new List<GridNode>();
            for (int x = 0; x < size; x++)
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
        for (int y = 0; y < size; y++)
        {
            List<GameObject> base_rows = new List<GameObject>();
            List<GameObject> tile_rows = new List<GameObject>();

            for (int x = 0; x < size; x++)
            {
                GameObject base_object = Instantiate(Resources.Load("Prefabs/Base")) as GameObject;
                base_object.transform.parent = transform;
                Vector3 position = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + y);
                base_object.transform.position = position;
                base_object.name = "Base Tile " + y + " " + x;
                base_rows.Add(base_object);
                tile_rows.Add(null);
            }
            grid_base_objects.Add(base_rows);
            visual_tracks.Add(tile_rows);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            UpdatePlayerSelection((int)TileType.straight);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            UpdatePlayerSelection((int)TileType.left);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            UpdatePlayerSelection((int)TileType.right);
        }
        if (game_start)
        {
            if (CheckPlayerIdle())
            {
                if (trackNodePos.Count > 1)
                {
                    if (!alreadyInvoking)
                    {
                        Invoke("RemoveIdleTrack", 1.0f);
                        alreadyInvoking = true;
                    }

                }
                else
                {
                    time = 0;
                }
            }
        }

    }

    public void PlayerInput(int value)
    {
        UpdatePlayerSelection(value);
    }

    public void AssignStartSelection(int id)
    {
        if(!selection_object)
        {
            selection_object = Instantiate(Resources.Load("Prefabs/Selection")) as GameObject;
        }
        player_id = id;
        switch(id)
        {
            case 1:
                selection_y = 1;
                selection_x = 0;
                current_direction = Direction.UP;
                break;
            case 2:
                selection_y = grid_size - 2;
                selection_x = 0;
                current_direction = Direction.DOWN;
                break;
            case 3:
                selection_y = 1;
                selection_x = grid_size - 1;
                current_direction = Direction.UP;
                break;
            case 4:
                selection_y = grid_size - 2;
                selection_x = grid_size - 1;
                current_direction = Direction.DOWN;
                break;
        }
        selection_object.transform.position = new Vector3(transform.position.x + selection_x, transform.position.y, transform.position.z + selection_y);

    }

    public void UpdateClientGrid(GridNode new_node)
    {
        int x = new_node.x;
        int y = new_node.y;

        if(grid_list[y][x].tile_type != new_node.tile_type || grid_list[y][x].player_id != new_node.player_id)
        {
            ChangeVisualTile(y, x, new_node.tile_type, new_node.player_id, new_node.tile_y_rotation);
        }
        grid_list[y][x] = new_node;

    }

    private void ChangeArraySelection(int y, int x)
    {
        selection_y = y;
        selection_x = x;

        if(selection_y >= grid_size)
        {
            selection_y = grid_size - 1;
        }
        if(selection_y < 0)
        {
            selection_y = 0;
        }

        if (selection_x >= grid_size)
        {
            selection_x = grid_size - 1;
        }
        if (selection_x < 0)
        {
            selection_x = 0;
        }

        selection_object.transform.position = new Vector3(transform.position.x + selection_x, transform.position.y, transform.position.z + selection_y);
    }

    private void SetSelectionForTrack(int value)
    {
        TileType type = (TileType)value;

        if(type == TileType.straight) //straight
        {
            switch (current_direction)
            {
                case Direction.UP:
                    ChangeArraySelection(selection_y + 1, selection_x);
                    break;
                case Direction.DOWN:
                    ChangeArraySelection(selection_y - 1, selection_x);
                    break;
                case Direction.LEFT:
                    ChangeArraySelection(selection_y, selection_x - 1);
                    break;
                case Direction.RIGHT:
                    ChangeArraySelection(selection_y, selection_x + 1);
                    break;
            }
        }
        else if(type == TileType.left) //left
        {
            switch (current_direction)
            {
                case Direction.UP:
                    ChangeArraySelection(selection_y, selection_x - 1);
                    current_direction = Direction.LEFT;
                    break;
                case Direction.DOWN:
                    ChangeArraySelection(selection_y, selection_x + 1);
                    current_direction = Direction.RIGHT;
                    break;
                case Direction.LEFT:
                    ChangeArraySelection(selection_y - 1, selection_x);
                    current_direction = Direction.DOWN;
                    break;
                case Direction.RIGHT:
                    ChangeArraySelection(selection_y + 1, selection_x);
                    current_direction = Direction.UP;
                    break;
            }
        }
        else if(type == TileType.right) //right
        {
            switch (current_direction)
            {
                case Direction.UP:
                    ChangeArraySelection(selection_y, selection_x + 1);
                    current_direction = Direction.RIGHT;
                    break;
                case Direction.DOWN:
                    ChangeArraySelection(selection_y, selection_x - 1);
                    current_direction = Direction.LEFT;
                    break;
                case Direction.LEFT:
                    ChangeArraySelection(selection_y + 1, selection_x);
                    current_direction = Direction.UP;
                    break;
                case Direction.RIGHT:
                    ChangeArraySelection(selection_y - 1, selection_x);
                    current_direction = Direction.DOWN;
                    break;
            }
        }
    }

    public void UpdatePlayerSelection(int value)
    {
        if(!game_start)
        {
            return;
        }
        TileType type = (TileType)value;

        Vector3 track_position = selection_object.transform.position;
        Direction old_direction = current_direction;
         if(grid_list[selection_y][selection_x].tile_type != (int)TileType.none)
        {
            return;
        }
        GameObject track;
        if(type == TileType.straight)
        {
            track = Instantiate(Resources.Load("Prefabs/Straight")) as GameObject;
            track.transform.parent = track_holder.transform;
            FindObjectOfType<CartMovement>().AddTrack(track);
            visual_tracks[selection_y][selection_x] = track;
            trackNodePos.Add(new Vector2(selection_x, selection_y));


        }
        else if(type == TileType.left)
        {
            track = Instantiate(Resources.Load("Prefabs/Left")) as GameObject;
            track.transform.parent = track_holder.transform;
            FindObjectOfType<CartMovement>().AddTrack(track);
            visual_tracks[selection_y][selection_x] = track;
            trackNodePos.Add(new Vector2(selection_x, selection_y));
        }
        else if (type == TileType.right)
        {
            track = Instantiate(Resources.Load("Prefabs/Right")) as GameObject;
            track.transform.parent = track_holder.transform;
            FindObjectOfType<CartMovement>().AddTrack(track);
            visual_tracks[selection_y][selection_x] = track;
            trackNodePos.Add(new Vector2(selection_x, selection_y));

        }
        else
        {
            return;
        }

        track.transform.position = track_position;

        Quaternion dir = Quaternion.Euler(Vector3.zero);
        int rot_value = 0;
        switch (old_direction)
        {
            case Direction.UP:
                rot_value = 0;
                break;
            case Direction.DOWN:
                rot_value = 180;
                break;
            case Direction.LEFT:
                rot_value = 270;
                break;
            case Direction.RIGHT:
                rot_value = 90;
                break;
        }
        dir = Quaternion.Euler(new Vector3(0, rot_value, 0));
        track.transform.rotation = dir;
        SetGridNode(selection_y, selection_x, value, rot_value);
        int old_selection_x = selection_x;
        int old_selection_y = selection_y;
        SetSelectionForTrack(value);
        network_manager.SendGridUpdateToServer(grid_list[old_selection_y][old_selection_x]);

        if (trackNodePos.Count > 5)
        {
            RemoveoldTrack(0);
        }
        
    }

    private void RemoveoldTrack(int trackPosNo)
    {
        ClearGridNode((int)trackNodePos[trackPosNo].y,(int)trackNodePos[trackPosNo].x);
        trackNodePos.RemoveAt(trackPosNo);
    }

    private void SetGridNode(int index_y, int index_x, int tile_type, float y_rot)
    {
        grid_list[index_y][index_x].tile_type = tile_type;
        grid_list[index_y][index_x].player_id = player_id;
        grid_list[index_y][index_x].tile_y_rotation = y_rot;
    }

    private void ClearGridNode(int index_y, int index_x)
    {
        Destroy(visual_tracks[index_y][index_x]);
        grid_list[index_y][index_x].player_id = -1;
        grid_list[index_y][index_x].tile_type = 0;
        grid_list[index_y][index_x].tile_y_rotation = 0;
        network_manager.SendGridUpdateToServer(grid_list[index_y][index_x]);
    }

    public void ChangeVisualTile(int y, int x, int tile_type, int player_id, float rotation)
    {
        if(visual_tracks[y][x] != null)
        {
            Destroy(visual_tracks[y][x]);
        }
        TileType new_type = (TileType)tile_type;
        if(new_type == TileType.none)
        {
            return;
        }

        GameObject obj;
        
        switch (new_type)
        {
            case TileType.straight:
                obj = Instantiate(Resources.Load("Prefabs/Straight")) as GameObject;
                obj.transform.parent = track_holder.transform;
                obj.transform.position = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + y);
                obj.transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
                visual_tracks[y][x] = obj;
                break;
            case TileType.left:
                obj = Instantiate(Resources.Load("Prefabs/Left")) as GameObject;
                obj.transform.parent = track_holder.transform;
                obj.transform.position = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + y);
                obj.transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
                visual_tracks[y][x] = obj;
                break;
            case TileType.right:
                obj = Instantiate(Resources.Load("Prefabs/Right")) as GameObject;
                obj.transform.parent = track_holder.transform;
                obj.transform.position = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + y);
                obj.transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
                visual_tracks[y][x] = obj;
                break;
        }

    }

    private bool CheckPlayerIdle()
    {
        if (in_editor)
        {
            return false;
        }
        if (FindObjectOfType<CartMovement>().GetComponent<Rigidbody>().velocity == Vector3.zero)
        {
            time += Time.deltaTime;

            if (time >= timer)
            {
                return true;
            }
        }
        else
        {
            time = 0;
        }
        return false;
    }

    private void RemoveIdleTrack()
    {
       
       RemoveoldTrack(0);
        alreadyInvoking = false;
    }
}
