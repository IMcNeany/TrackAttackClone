using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum connection_status
{
    none,
    connected,
    disconnected
}

public class PlayerInfo
{
    public string name;
    public connection_status status;
    public bool ready_state;
    public float disconnection_timer;
}

public class LobbyScript : MonoBehaviour
{
    public bool lobby_created = false;
    public float ready_countdown = 5.0f;
    public List<Text> player_name_text;
    public float timeout_time = 5.0f; //if a player disconnects, how long till another player can take their spot
    public bool force_ready = false; //for debug/testing
    public Text countdown_text;
    public Server server;

    private List<Text> status_texts = new List<Text>();
    private List<Text> ready_texts = new List<Text>();
    private List<PlayerInfo> player_infos = new List<PlayerInfo>();
    private const int max_players = 4;
    private float current_countdown = 5.0f;

    public List<PlayerInfo> PlayerInfos => player_infos;

    void Update()
    {
        if(lobby_created)
        {
            if(!server)
            {
                server = GameObject.Find("ServerNetworkManager").GetComponent<Server>();
            }
            for (int i = 0; i < max_players; i++)
            {
                UpdateFromPlayerInfo(i + 1);
                DisconnectTimeOut(i + 1);
            }
            UpdateCountDown();
        }
    }

    private void UpdateCountDown()
    {
        bool counting_down = true;
        countdown_text.text = current_countdown.ToString("F0");
        for(int i = 0; i < max_players; i++)
        {
            if(PlayerInfos[i].ready_state == false && force_ready != true)
            {
                counting_down = false;
            }
        }
        if(counting_down == true)
        {
            current_countdown -= 1 * Time.deltaTime;
        }
        else
        {
            current_countdown = ready_countdown;
        }

        if(current_countdown <= 0.0f)
        {
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
            //tell clients to load scene and how many players are in
        }
    }

    public void ForceStart()
    {
        force_ready = true;
    }

    //once lobby is created players can join
    public void CreateLobby()
    {
        for(int i = 0; i < max_players; i++)
        {
            //assign lists from scene
            Text con_text = player_name_text[i].transform.GetChild(0).GetComponent<Text>();
            status_texts.Add(con_text);
            Text ready_text = player_name_text[i].transform.GetChild(1).GetComponent<Text>();
            ready_texts.Add(ready_text);
            

            //asign player start info
            PlayerInfo new_player = new PlayerInfo();
            new_player.status = connection_status.none;
            new_player.disconnection_timer = 0.0f;
            new_player.ready_state = false;
            new_player.name = "Player " + (i + 1);
            PlayerInfos.Add(new_player);

            UpdateFromPlayerInfo(i + 1);
        }
        lobby_created = true;
    }

    public void DisconnectTimeOut(int ID)
    {
        ID--;
        if (PlayerInfos[ID].status == connection_status.disconnected)
        {
            PlayerInfos[ID].ready_state = false;

            PlayerInfos[ID].disconnection_timer += 1 * Time.deltaTime;
            if(PlayerInfos[ID].disconnection_timer > timeout_time)
            {
                PlayerInfos[ID].status = connection_status.none;
                PlayerInfos[ID].disconnection_timer = 0.0f;
                PlayerInfos[ID].name = "Player " + (ID + 1);
            }
        }
        else
        {
            PlayerInfos[ID].disconnection_timer = 0.0f;
        }
        
    }

    public void UpdateFromPlayerInfo(int ID)
    {
        ID--;
        switch (PlayerInfos[ID].status)
        {
            case connection_status.none:
                status_texts[ID].text = "Not connected";
                status_texts[ID].color = Color.grey;
                ready_texts[ID].gameObject.SetActive(false);
                break;
            case connection_status.connected:
                status_texts[ID].text = "Connected";
                status_texts[ID].color = Color.green;
                ready_texts[ID].gameObject.SetActive(true);
                break;
            case connection_status.disconnected:
                status_texts[ID].text = "Disconnected";
                status_texts[ID].color = Color.magenta;
                ready_texts[ID].gameObject.SetActive(false);
                break;
        }
        switch (PlayerInfos[ID].ready_state)
        {
            case true:
                ready_texts[ID].text = "Ready!";
                ready_texts[ID].color = Color.green;
                break;
            case false:
                ready_texts[ID].text = "Not Ready!";
                ready_texts[ID].color = Color.magenta;
                break;
        }

        player_name_text[ID].text = PlayerInfos[ID].name;

    }

    public void SetPlayerConnection(int ID, connection_status status)
    {
        ID--;
        PlayerInfos[ID].status = status;
    }

    public void SetPlayerReadyState(int ID, bool state)
    {
        ID--;
        PlayerInfos[ID].ready_state = state;
    }

    public void SetPlayerName(int ID, string name)
    {
        ID--;
        PlayerInfos[ID].name = name;
    }
}
