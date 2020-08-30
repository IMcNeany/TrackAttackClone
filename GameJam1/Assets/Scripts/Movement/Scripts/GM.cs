using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GM : MonoBehaviour
{
    public Material m_transitionMaterial;
    public GameObject playerPrefab;
    GridManager grid;
    public int players = 2;
    public AudioClip[] menuMusic;
    public AudioClip[] gameMusic;
    AudioSource audioPlayer;
    public int m_goldToWin = 5;
    public List<GameObject> m_playerList = new List<GameObject>();
    public List<Material> m_altMaterials = new List<Material>();
    public List<direction> m_playerDirections = new List<direction>();

    private bool m_gameStarting = false;

    // Start is called before the first frame update
    void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
        DontDestroyOnLoad(this.gameObject);
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            audioPlayer.clip = menuMusic[Random.Range(0, menuMusic.Length - 1)];
            audioPlayer.Play();
        }
        
    }

    void Start()
    {
        //grid = GetComponent<GridManager>();
        //InstantiatePlayers();
        
    }

    // Update is called once per frame
    void Update()
    {
      if (Input.GetKeyDown(KeyCode.K))
        {
            StartGame(4);
        }
      if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                StartGame(0);
            }
        }
    }

    void InstantiatePlayers()
    {
        foreach(GM gm in FindObjectsOfType<GM>())
        {
            m_playerDirections = gm.m_playerDirections;
        }

        foreach(direction d in m_playerDirections)
        {
            d.gameObject.SetActive(false);
        }

        GameObject player1 = Instantiate(playerPrefab);
        m_playerList.Add(player1);
        player1.GetComponent<Player>().Initialise(1, 1, grid.height - 2, PlayerDirection.RIGHT);
        player1.GetComponent<Player>().m_altMaterial = m_altMaterials[0];
        m_playerDirections[0].gameObject.SetActive(true);
        player1.GetComponent<PieceList>().m_UIDirections = m_playerDirections[0];
        player1.GetComponent<Player>().score = m_playerDirections[0].transform.GetChild(2).GetComponent<Text>();

        GameObject player2 = Instantiate(playerPrefab);
        m_playerList.Add(player2);
        player2.GetComponent<Player>().Initialise(2, grid.width - 2, grid.height - 2, PlayerDirection.LEFT);
        player2.GetComponent<Player>().m_altMaterial = m_altMaterials[1];
        m_playerDirections[1].gameObject.SetActive(true);
        player2.GetComponent<PieceList>().m_UIDirections = m_playerDirections[1];
        player2.GetComponent<Player>().score = m_playerDirections[1].transform.GetChild(2).GetComponent<Text>();

        if (players < 3) return;
        GameObject player3 = Instantiate(playerPrefab);
        m_playerList.Add(player3);
        player3.GetComponent<Player>().Initialise(3, 1, 1, PlayerDirection.RIGHT);
        player3.GetComponent<Player>().m_altMaterial = m_altMaterials[2];
        m_playerDirections[2].gameObject.SetActive(true);
        player3.GetComponent<PieceList>().m_UIDirections = m_playerDirections[2];
        player3.GetComponent<Player>().score = m_playerDirections[2].transform.GetChild(2).GetComponent<Text>();

        if (players < 4) return;
        GameObject player4 = Instantiate(playerPrefab);
        m_playerList.Add(player4);
        player4.GetComponent<Player>().Initialise(4, grid.width - 2, 1, PlayerDirection.LEFT);
        player4.GetComponent<Player>().m_altMaterial = m_altMaterials[3];
        m_playerDirections[3].gameObject.SetActive(true);
        player4.GetComponent<PieceList>().m_UIDirections = m_playerDirections[3];
        player4.GetComponent<Player>().score = m_playerDirections[3].transform.GetChild(2).GetComponent<Text>();
    }

    public void StartGame(int _players)
    {
        Debug.Log(Input.GetJoystickNames().Length);
        players = Input.GetJoystickNames().Length;
        SceneManager.sceneLoaded += InitialiseGame;
        audioPlayer.Stop();

        TransitionScene t = Camera.main.gameObject.AddComponent<TransitionScene>();
        t.m_nextScene = 1;
        t.m_transitionMaterial = m_transitionMaterial;
    }

    void InitialiseGame(Scene scene, LoadSceneMode mode)
    {
        grid = FindObjectOfType<GridManager>();
        InstantiatePlayers();
        SetCameraSettings();
        audioPlayer.clip = gameMusic[Random.Range(0, gameMusic.Length - 1)];
        audioPlayer.Play();
        SceneManager.sceneLoaded -= InitialiseGame;
    }

    void SetCameraSettings()
    {
        Camera cam = Camera.main;
        switch (2)
        {
            case 2:
                cam.gameObject.transform.position = new Vector3(3.5f, 8, 0.2f);
                cam.gameObject.transform.rotation = Quaternion.Euler(70, 0, 0);
                break;
            case 3:
                cam.gameObject.transform.position = new Vector3(5.5f, 12, 5.5f);
                break;
            case 4:
                cam.gameObject.transform.position = new Vector3(7.5f, 15, 7.5f);
                break;
        }
    }

    public void CheckEndGame(int goldCount, int playerID)
    {
        if(goldCount > m_goldToWin)
        {
            Debug.Log("It's endgame now boys");

            FindObjectOfType<Victory>().Win(playerID);
            audioPlayer.Stop();
        }
    }
}
