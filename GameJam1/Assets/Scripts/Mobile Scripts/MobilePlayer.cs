using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobilePlayer : MonoBehaviour
{
    
    private const int trailLength = 4;

    List<GameObject> m_laidTrack = new List<GameObject>();
    public GameObject player;
    public int m_goldCount = 0;
    public float speed = 3f;
    public bool AI;

    bool MovePlayer = false;
    bool m_colChecked = false;
    bool wallHit = false;
    int pathIndex = 0;
    int trailLengthCount = 0;

    public GameObject m_stashPrefab;
    public GameObject m_curvedTrackPrefab;
    public GameObject m_straightTrackPrefab;

    List<GameObject> goldBars = new List<GameObject>();
    private List<Node> path = new List<Node>();
    public GridManager grid;
    public GameObject stash;
    public TMPro.TextMeshProUGUI goldtext;

    public Node startingNode;
    public Node currentNode;
    Node nextNode;

   public PlayerDirection direction;
    PlayerDirection startDirection;

    PlayerControls controls;
    public int playerNumber;

    GameObject nextNodeCube;

    private int goldscore;
    private float timer;
    public TMPro.TextMeshProUGUI highScore;
    public MobileGameManager mobileGameManager;

    // Start is called before the first frame update
    void Start()
    {
       
     //   highScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
 if(GetComponent<AI>())
  {
            AI = true;
            GetComponent<AI>().enabled = true;
            currentNode = grid.GetNode((int)transform.position.x, (int)transform.position.z);

        }
         currentNode = grid.GetNode((int)transform.position.x, (int)transform.position.z);

         startingNode = currentNode;        //creates the square that shows where the track will be placed next
        player = this.gameObject;
        //  nextNodeCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //  nextNodeCube.transform.localScale = new Vector3(0.6f, 0.2f, 0.6f);


        // stash = Instantiate(m_stashPrefab, player.transform.position, Quaternion.identity);
        // stash.GetComponent<Stash>().manager = GameObject.FindGameObjectWithTag("Player").GetComponent<MobilePlayer>();
        // stash.GetComponent<Stash>().id = playerNumber;

        foreach (Transform t in transform)
        {
            if (t.gameObject.name == "Gold")
            { 
                goldBars.Add(t.gameObject);
                t.gameObject.SetActive(false);
            }
        }

        UpdateGoldCountText();
    }

    public void AssignPlayer(GameObject playerObj)
    {
        player = playerObj;
    }

   
  /*  public void Initialise(int _playerNumber, int startX, int startY, PlayerDirection dir)
    {

        //direction player is facing
        startDirection = dir;
        direction = dir;

       // controls = new PlayerInput(_playerNumber);
        playerNumber = _playerNumber;

        //get starting node
        grid = FindObjectOfType<GridManager>();
        startingNode = grid.GetNode(startX, startY);
        currentNode = startingNode;
        //Places Player
        player.transform.position = currentNode.position;
        path.Add(currentNode);

        //sets rotation of cart on spawn
        switch (direction)
        {
            case PlayerDirection.UP:
                {
                    player.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                break;
            case PlayerDirection.DOWN:
                {
                    player.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                break;
            case PlayerDirection.LEFT:
                {
                    player.transform.rotation = Quaternion.Euler(0, 90, 0);
                }
                break;
            case PlayerDirection.RIGHT:
                {
                    player.transform.rotation = Quaternion.Euler(0, 270, 0);
                }
                break;
        }

        //assign stash


        if (startingNode.obstacle)
        {
            startingNode.obstacle = false;
        }
       // NextNodeHighlight();
    }

       
    }*/


    // Update is called once per frame
    void Update()
    {
        if (MovePlayer) {
            Move();
        }

        CheckCrash();
       
    }

    void Move()
    {

        if (path.Count < 2)
        {
            return;
        }
        player.transform.LookAt(path[pathIndex].position);
        player.transform.position = Vector3.MoveTowards(player.transform.position, path[pathIndex].position, speed * Time.deltaTime);
        
        if (player.transform.position == path[pathIndex].position)
        {
            pathIndex++;

            pathIndex = Mathf.Clamp(pathIndex, 0, path.Count - 1);
            player.transform.LookAt(path[pathIndex].position);
            player.transform.position = Vector3.MoveTowards(player.transform.position, path[pathIndex].position, speed * Time.deltaTime);

        }
    }

    void CheckCrash()
    {
        if(wallHit)
        {
            Reset();
        }
    }

    void NextNodeHighlight()
    {
       // nextNodeCube.transform.position = nextNode.position;
        if (!AI)
        {
            nextNodeCube.transform.position = nextNode.position;
        }
    }
    public void GameStart()
    {
        if (AI)
        {
            GetComponent<AI>().GameStarted = true;
        }
    }

    public void LeftButtonPressed()
    {

        MovePlayer = true;
        //check the trail length 
        trailLengthCount++;
        if (trailLengthCount > trailLength)
        {
            TrailCut();
        }

        //crash
        if (!grid.GetNextNode(currentNode, ref nextNode, direction))
        {
            wallHit = true;
            return;
        }
       // NextNodeHighlight();
        switch (direction)
        {
            case PlayerDirection.UP:
                {
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 90, 0)));
                }
                break;
            case PlayerDirection.DOWN:
                {
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 270, 0)));
                }
                break;
            case PlayerDirection.LEFT:
                {
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 0, 0)));
                }
                break;
            case PlayerDirection.RIGHT:
                {
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 180, 0)));
                }
                break;
        }

        TurnLeft();

        grid.SetPlayerOwnership(nextNode, playerNumber);
        path.Add(nextNode);
        currentNode = nextNode;

       // NextNodeHighlight();
    }

    public void StraightButtonPressed()
    {

        MovePlayer = true;

        ////check the trail length 
        trailLengthCount++;
        if (trailLengthCount > trailLength)
        {
            TrailCut();
        }
       
        //  if (wallHit) return;
        if (!grid.GetNextNode(currentNode, ref nextNode, direction))
        {
            wallHit = true;
            return;
        }
       // NextNodeHighlight();
        //use player direction
        if (direction == PlayerDirection.UP || direction == PlayerDirection.DOWN)
        {
            m_laidTrack.Add(Instantiate(m_straightTrackPrefab, nextNode.position, Quaternion.Euler(0, 0, 0)));
        }
        else
        {
            // Instantiate straight track rotated 90 degrees
            m_laidTrack.Add(Instantiate(m_straightTrackPrefab, nextNode.position, Quaternion.Euler(0, 90, 0)));
        }

        grid.SetPlayerOwnership(nextNode, playerNumber);
        path.Add(nextNode);
        currentNode = nextNode;

       // NextNodeHighlight();
    }

    public void RightButtonPressed()
    {

        MovePlayer = true;

        //check the trail length 
        trailLengthCount++;
        if (trailLengthCount > trailLength)
        {
            TrailCut();
        }

        // if (wallHit) return;
        //crash
        if (!grid.GetNextNode(currentNode, ref nextNode, direction))
        {
            wallHit = true;
            return;
        }

      //  NextNodeHighlight();
        switch (direction)
        {
            case PlayerDirection.UP:
                {
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 0, 0)));
                }
                break;
            case PlayerDirection.DOWN:
                {
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 180, 0)));
                }
                break;
            case PlayerDirection.LEFT:
                {
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 270, 0)));
                }
                break;
            case PlayerDirection.RIGHT:
                {
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 90, 0)));
                }
                break;
        }

        TurnRight();

        grid.SetPlayerOwnership(nextNode, playerNumber);
        path.Add(nextNode);
        currentNode = nextNode;

      //  NextNodeHighlight();
        //player dir +1
    }


    void TurnLeft()
    {

        direction--;

        if(direction < (PlayerDirection)0)
        {
            direction = (PlayerDirection)3;
        }
    }

    void TurnRight()
    {

        direction++;

        if (direction > (PlayerDirection)3)
        {
            direction = (PlayerDirection)0;
        }
    }

    void SetActiveGoldBars()
    {
        for (int i = 0; i < 3; i++)
        {
            goldBars[i].SetActive(i < m_goldCount);
        }
    }

    void UpdateGoldCountText()
    {
        goldtext.text = "" + m_goldCount;
        goldscore = int.Parse(goldtext.text);
     /*   if (goldscore > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", goldscore);
            highScore.text = goldtext.text;
        }*/
    }

    public void Reset()
    {
        //destroy track
        if (!gameObject.GetComponent<AI>())
        {
            Handheld.Vibrate();
        }
        for (int i = 0; i < m_laidTrack.Count; ++i)
            Destroy(m_laidTrack[m_laidTrack.Count - 1 - i], i / 20.0f);

        //reset trail length
        trailLengthCount = 0;

        //drop gold
        /*while (m_goldCount > 0)
        {
            Instantiate(grid.m_goldPrefab, new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z), Quaternion.identity);
            m_goldCount--;
            UpdateGoldCountText();
        }*/
        //clear list
        m_laidTrack.Clear();

        //reset position
        grid.ResetTiles(playerNumber);
        direction = startDirection;
        gameObject.transform.position = startingNode.position;
        currentNode = startingNode;
        path.Clear();
        path.Add(currentNode);
        pathIndex = 1;
        //m_goldCount = 0;
        wallHit = false;
        UpdateGoldCountText();
    }

    public void TrailCut()
    {
        Destroy(m_laidTrack[m_laidTrack.Count - trailLength], trailLength / 20.0f);
        //you need to set player ownsership of the grid 
    }

    //this needs to be on like aplyer script on the cart
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Gold")
        {
            Destroy(collision.gameObject);

           // if (m_goldCount < 3)
                ++m_goldCount;
            UpdateGoldCountText();
            SetActiveGoldBars();
          /*  timer = mobileGameManager.timeStart;
            timer = timer + 5.0f;
            mobileGameManager.timeStart = timer;*/

            currentNode.GoldPickup = false;

            grid.SpawnGold();
        }
        else if (collision.gameObject.tag == "Stash")
        {
            if (startingNode != null)
            {
                if (collision.gameObject == stash)
                {
                    // Increase stash total
                    //  score.text = "" + collision.gameObject.GetComponent<Stash>().UpdateStash(m_goldCount);

                   // m_goldCount = 0;
                    SetActiveGoldBars();
                }
                Reset();
            }
        }
        else if (collision.gameObject.tag == "Player")
        {
            if (!m_colChecked)
            {
                m_colChecked = true;
                collision.gameObject.GetComponent<MobilePlayer>().m_colChecked = true;

                if (m_goldCount < collision.gameObject.GetComponent<MobilePlayer>().m_goldCount)
                {
                    m_goldCount += collision.gameObject.GetComponent<MobilePlayer>().m_goldCount;

                    m_goldCount = Mathf.Clamp(m_goldCount, 0, 3);

                   // collision.gameObject.GetComponent<MobilePlayer>().m_goldCount = 0;

                    collision.gameObject.GetComponent<MobilePlayer>().Reset();
                }
                else if (m_goldCount > collision.gameObject.GetComponent<MobilePlayer>().m_goldCount)
                {
                    collision.gameObject.GetComponent<MobilePlayer>().m_goldCount += m_goldCount;

                    collision.gameObject.GetComponent<MobilePlayer>().m_goldCount = Mathf.Clamp(collision.gameObject.GetComponent<MobilePlayer>().m_goldCount, 0, 3);

                   // m_goldCount = 0;

                    Reset();
                }
                else if (m_goldCount == collision.gameObject.GetComponent<MobilePlayer>().m_goldCount)
                {
                    collision.gameObject.GetComponent<MobilePlayer>().Reset();

                    Reset();
                }
            }
            SetActiveGoldBars();
        }
    }
}
