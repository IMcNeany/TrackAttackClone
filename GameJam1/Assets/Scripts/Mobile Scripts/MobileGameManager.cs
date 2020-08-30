using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class MobileGameManager : MonoBehaviour
{
    public Material m_transitionMaterial;
    public GameObject playerPrefab;
    public GameObject otherPlayerPrefab;
    public MobilePlayer mobilePlayer;
    public GridManager grid;
    public int players = 2;
    public AudioClip[] menuMusic;
    public AudioClip[] gameMusic;
    AudioSource audioPlayer;
  //  public int m_goldToWin = 5;
    public List<GameObject> m_playerList = new List<GameObject>();
    public List<Material> m_altMaterials = new List<Material>();
    public List<direction> m_playerDirections = new List<direction>();
    public float timeStart = 60;
    public float restartTime = 2f;
    public TMPro.TextMeshProUGUI countdown;
    public GameObject resultsUI;
    public TMPro.TextMeshProUGUI victor;

    private bool m_gameStarting = false;
    private bool gameHasEnded = false;

    // Start is called before the first frame update
    void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
     
    }

    void Start()
    {
       // grid = GetComponent<GridManager>();
        //This needs to pass in No of players got from serverside and player No
        InstantiatePlayers(1,1);
        countdown.text = timeStart.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        timeStart -= Time.deltaTime;
        countdown.text = Mathf.Round(timeStart).ToString();
        if (timeStart <= 0)
        {
            EndGame(mobilePlayer.m_goldCount,0,0,0);
        }
    }

    void InstantiatePlayers(int noPlayers, int playerNo)
    {
        foreach (MobileGameManager gm in FindObjectsOfType<MobileGameManager>())
        {
            m_playerDirections = gm.m_playerDirections;
        }

        foreach (direction d in m_playerDirections)
        {
            d.gameObject.SetActive(false);
        }

        // GameObject player1 = Instantiate(playerPrefab);
        //m_playerDirections[0].gameObject.SetActive(true);
        switch(noPlayers)
        {
            case 1:
                {
                    GameObject player1 = playerPrefab;
                    m_playerList.Add(player1);
                    mobilePlayer.AssignPlayer(player1);
                    mobilePlayer.Initialise(playerNo, 1, grid.height - 2, PlayerDirection.RIGHT);

                   
                }
                break;
            case 2:
                {
                    if (playerNo == 1)
                    {
                        GameObject player2 = Instantiate(otherPlayerPrefab, new Vector3(grid.width - 2, 0, grid.height - 2), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player2);

                        GameObject player1 = playerPrefab;
                        m_playerList.Add(player1);
                        mobilePlayer.AssignPlayer(player1);
                        mobilePlayer.Initialise(1, 0, grid.height - 4, PlayerDirection.RIGHT);

                        
                        
                    }
                    else if (playerNo == 2)
                    {
                        GameObject player1 = Instantiate(otherPlayerPrefab, new Vector3(1, 0, grid.height - 4), new Quaternion(0, 0, 0, 0)); 
                        m_playerList.Add(player1);
                        

                        GameObject player2 = playerPrefab;
                        m_playerList.Add(player2);
                        mobilePlayer.AssignPlayer(player2);
                        mobilePlayer.Initialise(2, grid.width - 2, grid.height - 2, PlayerDirection.LEFT);


                    }
                }
                break;
            case 3:
                {

                    if (playerNo == 1)
                    {
                        GameObject player2 = Instantiate(otherPlayerPrefab, new Vector3(grid.width - 2, 0, grid.height - 2), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player2);

                        GameObject player3 = Instantiate(otherPlayerPrefab, new Vector3(1, 0, 1), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player3);

                        GameObject player1 = playerPrefab;
                        m_playerList.Add(player1);
                        mobilePlayer.AssignPlayer(player1);
                        mobilePlayer.Initialise(1, 0, grid.height - 4, PlayerDirection.RIGHT);
                    }


                    else if (playerNo == 2)
                    {
                        GameObject player1 = Instantiate(otherPlayerPrefab, new Vector3(1, 0, grid.height - 4), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player1);

                        GameObject player3 = Instantiate(otherPlayerPrefab, new Vector3(1, 0, 1), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player3);

                        GameObject player2 = playerPrefab;
                        m_playerList.Add(player2);
                        mobilePlayer.AssignPlayer(player2);
                        mobilePlayer.Initialise(2, grid.width - 2, grid.height - 2, PlayerDirection.LEFT);
                    }
                    else if (playerNo == 3)
                    {
                        GameObject player1 = Instantiate(otherPlayerPrefab, new Vector3(1, 0, grid.height - 4), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player1);

                        GameObject player2 = Instantiate(otherPlayerPrefab, new Vector3(grid.width - 2, 0, grid.height - 2), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player2);

                        GameObject player3 = playerPrefab;
                        m_playerList.Add(player3);
                        mobilePlayer.AssignPlayer(player3);
                        mobilePlayer.Initialise(3, 1, 1, PlayerDirection.RIGHT);
                    }

                    }
                break;
            case 4:
                {
                    if (playerNo == 1)
                    {
                        GameObject player2 = Instantiate(otherPlayerPrefab, new Vector3(grid.width - 2, 0, grid.height - 2), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player2);

                        GameObject player3 = Instantiate(otherPlayerPrefab, new Vector3(1, 0, 1), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player3);

                        GameObject player4 = Instantiate(otherPlayerPrefab, new Vector3(grid.width - 2, 0, 1), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player4);

                        GameObject player1 = playerPrefab;
                        m_playerList.Add(player1);
                        mobilePlayer.AssignPlayer(player1);
                        mobilePlayer.Initialise(1, 0, grid.height - 4, PlayerDirection.RIGHT);
                    }


                    else if (playerNo == 2)
                    {
                        GameObject player1 = Instantiate(otherPlayerPrefab, new Vector3(1, 0, grid.height - 4), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player1);

                        GameObject player3 = Instantiate(otherPlayerPrefab, new Vector3(1, 0, 1), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player3);

                        GameObject player4 = Instantiate(otherPlayerPrefab, new Vector3(grid.width - 2, 0, 1), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player4);

                        GameObject player2 = playerPrefab;
                        m_playerList.Add(player2);
                        mobilePlayer.AssignPlayer(player2);
                        mobilePlayer.Initialise(2, grid.width - 2, grid.height - 2, PlayerDirection.LEFT);
                    }
                    else if (playerNo == 3)
                    {
                        GameObject player1 = Instantiate(otherPlayerPrefab, new Vector3(1, 0, grid.height - 4), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player1);

                        GameObject player2 = Instantiate(otherPlayerPrefab, new Vector3(grid.width - 2, 0, grid.height - 2), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player2);

                        GameObject player4 = Instantiate(otherPlayerPrefab, new Vector3(grid.width - 2, 0, 1), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player4);

                        GameObject player3 = playerPrefab;
                        m_playerList.Add(player3);
                        mobilePlayer.AssignPlayer(player3);
                        mobilePlayer.Initialise(3, 1, 1, PlayerDirection.RIGHT);
                    }

                    else if (playerNo == 4)
                    {
                        GameObject player1 = Instantiate(otherPlayerPrefab, new Vector3(1, 0, grid.height - 4), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player1);

                        GameObject player2 = Instantiate(otherPlayerPrefab, new Vector3(grid.width - 2, 0, grid.height - 2), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player2);

                        GameObject player3 = Instantiate(otherPlayerPrefab, new Vector3(1, 0, 1), new Quaternion(0, 0, 0, 0));
                        m_playerList.Add(player3);

                        GameObject player4 = playerPrefab;
                        m_playerList.Add(player4);
                        mobilePlayer.AssignPlayer(player4);
                        mobilePlayer.Initialise(4, grid.width - 2, 1, PlayerDirection.LEFT);
                    }


                }
                break;
        }
        
 
    }



    public void EndGame(int player1Gold, int player2Gold, int player3Gold, int player4Gold)
    {

        if (!gameHasEnded)
        {
            gameHasEnded = true;
            resultsUI.SetActive(true);

            int winner = Mathf.Max(Mathf.Max(Mathf.Max(player1Gold, player2Gold), player3Gold), player4Gold);

            Debug.Log("the winner with [ " + winner + " ] gold is! ");

            if (winner == player1Gold)
            {
                if (winner != player2Gold && winner != player3Gold && winner != player4Gold)
                {
                    victor.text = "Player 1 Wins";
                }
                else
                {
                    victor.text = "Draw";
                }
            }
            else if (winner == player2Gold)
            {
                if (winner != player1Gold && winner != player3Gold && winner != player4Gold)
                {
                    victor.text = "Player 2 Wins";
                }
                else
                {
                    victor.text = "Draw";
                }
            }
            else if (winner == player3Gold)
            {
                if (winner != player1Gold && winner != player2Gold && winner != player4Gold)
                {
                    victor.text = "Player 3 Wins";
                }
                else
                {
                    victor.text = "Draw";
                }
            }
            else if (winner == player4Gold)
            {
                if (winner != player1Gold && winner != player2Gold && winner != player3Gold)
                {
                    victor.text = "Player 4 Wins";
                }
                else
                {
                    victor.text = "Draw";
                }
            }

            Invoke("Restart", restartTime);
        }

    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}

