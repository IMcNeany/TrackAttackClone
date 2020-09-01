public static class NetworkOperation
{
    public const int none = 0;
    public const int set_ready = 1;
    public const int cart_data = 2;
    public const int grid_single = 3;
    public const int user_counter = 4;
    public const int scene_load = 5;
    public const int assign_num = 6;
    public const int grid_all = 7;    
    public const int gold_pos = 8;
    public const int player_score = 9;
    public const int player_info = 10;
    public const int game_start = 11;
    public const int player_respawn = 12;
    public const int timer_seconds = 13;
    public const int game_finished = 14;
    public const int cart_off_tracks = 15;


}

public enum TileType
{
    none,
    straight,
    left,
    right
}

[System.Serializable]
public abstract class NetworkMessage 
{
    public byte operation { set; get; }

    public void Message()
    {
        operation = NetworkOperation.none;
    }
}
[System.Serializable]
public class NWMPlayerReady : NetworkMessage
{
    public NWMPlayerReady()
    {
        operation = NetworkOperation.set_ready;
    }

    public bool ready_state { set; get; }
    public string player_name { set; get; }
}
[System.Serializable]
public class NWMCartData : NetworkMessage
{
    public NWMCartData()
    {
        operation = NetworkOperation.cart_data;
    }
    public int player_ID { set; get; }
    public float[] POSX { set; get; }
    public float[] POSY { set; get; }
    public float[] POSZ { set; get; }
    public float[] ROTX { set; get; }
    public float[] ROTY { set; get; }
    public float[] ROTZ { set; get; }

    public void SetArrays()
    {
        POSX = new float[4];
        POSY = new float[4];
        POSZ = new float[4];
        ROTX = new float[4];
        ROTY = new float[4];
        ROTZ = new float[4];
    }
}
[System.Serializable]
public class NWMGridData : NetworkMessage
{
    public NWMGridData()
    {
        operation = NetworkOperation.grid_single;
    }
    //where on the grid
    public int grid_x { set; get; }
    public int grid_y { set; get; }
    //type of tile(straight/curve/none/gold etc)
    public int tile_type { set; get; }
    //who it belongs to e.g player 1/player2/noone
    public int player_ID { set; get; }
    public float tile_rotation { set; get; }
}
[System.Serializable]
public class NWMUserCount : NetworkMessage
{
    public NWMUserCount()
    {
        operation = NetworkOperation.user_counter;
    }
    public int count { get; set; }
}

[System.Serializable]
public class NWMSceneLoad : NetworkMessage
{
    public NWMSceneLoad()
    {
        operation = NetworkOperation.scene_load;
    }

    public string scene_name { get; set; }
}
[System.Serializable]
public class NWMAssignPlayerNum : NetworkMessage
{
    public NWMAssignPlayerNum()
    {
        operation = NetworkOperation.assign_num;
    }

    public int num { get; set; }
}

[System.Serializable]
public class NWMGoldUpdate : NetworkMessage
{
    public NWMGoldUpdate()
    {
        operation = NetworkOperation.gold_pos;
    }

    public int posX { get; set; }
    public int posZ { get; set; }
}

[System.Serializable]
public class NWMScoreUpdate : NetworkMessage
{
    public NWMScoreUpdate()
    {
        operation = NetworkOperation.player_score;
    }

    public int playerId { get; set; }
    public int playerIdScore { get; set; }
}
[System.Serializable]
public class NWMPlayerIDUpdate : NetworkMessage
{
    public NWMPlayerIDUpdate()
    {
        operation = NetworkOperation.player_info;
    }

  //  public bool addPlayerToList { get; set; }
    public int playerId { get; set; }
    public string playerName { get; set; }
}

[System.Serializable]
public class NWMGameStart : NetworkMessage
{
    public NWMGameStart()
    {
        operation = NetworkOperation.game_start;
    }
    public bool started { get; set; }
}

[System.Serializable]
public class NWMPlayerRespawn : NetworkMessage
{
    public NWMPlayerRespawn()
    {
        operation = NetworkOperation.player_respawn;
    }
    public int player_id { get; set; }
}

[System.Serializable]
public class NWMTimerUpdate : NetworkMessage
{
    public NWMTimerUpdate()
    {
        operation = NetworkOperation.timer_seconds;
    }
    public int seconds { get; set; }
}
[System.Serializable]
public class NWMGameFinishedSendInfo : NetworkMessage
{
    public NWMGameFinishedSendInfo()
    {
        operation = NetworkOperation.game_finished;
    }
    public bool isGameFinished { get; set; }
}

[System.Serializable]
public class NWMCartOffTracks : NetworkMessage
{
    public NWMCartOffTracks()
    {
        operation = NetworkOperation.cart_off_tracks;
    }
    public bool cartBehindTracks { get; set; }
}