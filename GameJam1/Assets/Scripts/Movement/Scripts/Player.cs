using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    bool m_colChecked = false;
    bool wallHit = false;
    int pathIndex = 1;

    public float speed = 3f;
    float minSpeed = 1f;
    float maxSpeed = 5f;
    float soundTimer = 2;


    public int m_goldCount = 0;

    public GameObject m_stashPrefab;
    public GameObject m_curvedTrackPrefab;
    public GameObject m_straightTrackPrefab;

    public Text score;

    public Material m_altMaterial;

    List<Node> path = new List<Node>();
    public GridManager grid;
    public GameObject stash;
    Node startingNode;
    Node currentNode;
    Node nextNode;
    PlayerDirection direction;
    PlayerDirection startDirection;
    int playerNumber = 0;

    PieceList pieceManager;
    PlayerControls controls;

    List<GameObject> goldBars = new List<GameObject>();
    List<GameObject> m_laidTrack = new List<GameObject>();
    public AudioClip trackSound;
    public AudioClip placeSound;

    GameObject nextNodeCube;


    AudioSource a;
    void Start()
    {
        nextNodeCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nextNodeCube.transform.localScale = new Vector3(0.6f, 0.2f, 0.6f);
        nextNodeCube.GetComponent<MeshRenderer>().material = m_altMaterial;
        a = GetComponent<AudioSource>();
        //a.clip = trackSound;
        //a.Play();
        foreach (Transform t in transform)
        {
            if (t.gameObject.name == "Gold")
            {
                goldBars.Add(t.gameObject);
                t.gameObject.SetActive(false);
            }
        }
    }

    public void Initialise(int _playerNumber, int startX, int startY, PlayerDirection dir)
    {
        startDirection = dir;
        direction = dir;
        controls = new PlayerInput(_playerNumber);
        playerNumber = _playerNumber;
        grid = FindObjectOfType<GridManager>();
        startingNode = grid.GetNode(startX, startY);
        currentNode = startingNode;
        transform.position = currentNode.position;
        path.Add(currentNode);
      //  pieceManager = GetComponent<PieceList>();

        stash = Instantiate(m_stashPrefab, transform.position, Quaternion.identity);
       // stash.GetComponent<Stash>().manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<MobileGameManager>();
        stash.GetComponent<Stash>().id = playerNumber;

        if (startingNode.obstacle)
        {
            startingNode.obstacle = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_colChecked = false;
        GetInput();
        Move();
        HandleSpeed();
        PlaySound();
        NextNodeCube();
    }

    void NextNodeCube()
    {
        nextNodeCube.transform.position = nextNode.position;
    }

    void HandleSpeed()
    {
        if (Input.GetKeyDown(controls.actionButton))
        {
            speed += 0.3f;
        }
        speed -= 1 * Time.deltaTime;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
    }

    void GetInput()
    {
        if (wallHit) return;

        //crash
        if (!grid.GetNextNode(currentNode, ref nextNode, direction))
        {
            wallHit = true;
            return;
        }

        if (nextNode.player == playerNumber)
        {
            wallHit = true;
        }


        if (Input.GetKeyDown(KeyCode.W) && direction != PlayerDirection.DOWN)
        {
            if (!CanPlaceTile(PlayerDirection.UP)) return;

            grid.SetPlayerOwnership(nextNode, playerNumber);
            //path.Add(nextNode);
            currentNode = nextNode;
            direction = PlayerDirection.UP;
        }

        else if (controls.GetUpButton() && direction != PlayerDirection.DOWN)
        {
            if (!CanPlaceTile(PlayerDirection.UP)) return;

            grid.SetPlayerOwnership(nextNode, playerNumber);
            //path.Add(nextNode);
            currentNode = nextNode;
            direction = PlayerDirection.UP;
        }

        else if (Input.GetKeyDown(KeyCode.A) && direction != PlayerDirection.RIGHT)
        {
            if (!CanPlaceTile(PlayerDirection.LEFT)) return;

            grid.SetPlayerOwnership(nextNode, playerNumber);
            //path.Add(nextNode);
            currentNode = nextNode;
            direction = PlayerDirection.LEFT;
        }

        else if (controls.GetLeftButton() && direction != PlayerDirection.RIGHT)
        {
            if (!CanPlaceTile(PlayerDirection.LEFT)) return;

            grid.SetPlayerOwnership(nextNode, playerNumber);
            //path.Add(nextNode);
            currentNode = nextNode;
            direction = PlayerDirection.LEFT;
        }

        else if (Input.GetKeyDown(KeyCode.S) && direction != PlayerDirection.UP)
        {
            if (!CanPlaceTile(PlayerDirection.DOWN)) return;

            grid.SetPlayerOwnership(nextNode, playerNumber);
            //path.Add(nextNode);
            currentNode = nextNode;
            direction = PlayerDirection.DOWN;
        }

        else if (controls.GetDownButton() && direction != PlayerDirection.UP)
        {
            if (!CanPlaceTile(PlayerDirection.DOWN)) return;

            grid.SetPlayerOwnership(nextNode, playerNumber);
            //path.Add(nextNode);
            currentNode = nextNode;
            direction = PlayerDirection.DOWN;
        }

        else if (Input.GetKeyDown(KeyCode.D) && direction != PlayerDirection.LEFT)
        {
            if (!CanPlaceTile(PlayerDirection.RIGHT)) return;

            grid.SetPlayerOwnership(nextNode, playerNumber);
            //path.Add(nextNode);
            currentNode = nextNode;
            direction = PlayerDirection.RIGHT;
        }

        else if (controls.GetRightButton() && direction != PlayerDirection.LEFT)
        {
            if (!CanPlaceTile(PlayerDirection.RIGHT)) return;

            grid.SetPlayerOwnership(nextNode, playerNumber);
            //path.Add(nextNode);
            currentNode = nextNode;
            direction = PlayerDirection.RIGHT;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    //Need Imprvement
    bool CanPlaceTile(PlayerDirection attemptedDirection)
    {
        //bool result = false;

        //if (attemptedDirection == PlayerDirection.RIGHT || 
        //    attemptedDirection == PlayerDirection.LEFT)
        //{
        //    if (direction == PlayerDirection.RIGHT ||
        //        direction == PlayerDirection.LEFT)
        //    {
        //      //  result = pieceManager.GetTrack(MixedBagRandom.Types.STRAIGHT);
        //    }
        //    else
        //    {
        //      //  result = pieceManager.GetTrack(MixedBagRandom.Types.CURVED);
        //    }
        //}
        //else
        //{
        //    if (direction == PlayerDirection.RIGHT ||
        //        direction == PlayerDirection.LEFT)
        //    {
        //        result =  pieceManager.GetTrack(MixedBagRandom.Types.CURVED);
        //    }
        //    else
        //    {
        //        result = pieceManager.GetTrack(MixedBagRandom.Types.STRAIGHT);
        //    }
        //}

        //if(result)
        //{
        if (attemptedDirection == direction)
            {
                if (direction == PlayerDirection.UP || direction == PlayerDirection.DOWN)
                {
                    // Instantiate straight track rotated 0 degrees
                    m_laidTrack.Add(Instantiate(m_straightTrackPrefab, nextNode.position, Quaternion.Euler(0,0,0)));
                }
                else
                {
                    // Instantiate straight track rotated 90 degrees
                    m_laidTrack.Add(Instantiate(m_straightTrackPrefab, nextNode.position, Quaternion.Euler(0, 90, 0)));
                }
                if (direction == PlayerDirection.RIGHT || direction == PlayerDirection.UP)
                    m_laidTrack[m_laidTrack.Count - 1].GetComponent<NodeList>().nodes.Reverse();
            }
            else
            {
                if (direction == PlayerDirection.UP     && attemptedDirection == PlayerDirection.RIGHT)
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 0, 0)));
                if (direction == PlayerDirection.LEFT   && attemptedDirection == PlayerDirection.DOWN)
                {
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 0, 0)));
                    m_laidTrack[m_laidTrack.Count - 1].GetComponent<NodeList>().nodes.Reverse();
                }

                if (direction == PlayerDirection.UP      && attemptedDirection == PlayerDirection.LEFT)
                {
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 90, 0)));
                    m_laidTrack[m_laidTrack.Count - 1].GetComponent<NodeList>().nodes.Reverse();
                }
                if (direction == PlayerDirection.RIGHT  && attemptedDirection == PlayerDirection.DOWN)
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 90, 0)));
                
                if (direction == PlayerDirection.RIGHT  && attemptedDirection == PlayerDirection.UP)
                {
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 180, 0)));
                    m_laidTrack[m_laidTrack.Count - 1].GetComponent<NodeList>().nodes.Reverse();
                }
                if (direction == PlayerDirection.DOWN   && attemptedDirection == PlayerDirection.LEFT)
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 180, 0)));

                if (direction == PlayerDirection.LEFT   && attemptedDirection == PlayerDirection.UP)
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 270, 0)));
                if (direction == PlayerDirection.DOWN   && attemptedDirection == PlayerDirection.RIGHT)
                {
                    m_laidTrack.Add(Instantiate(m_curvedTrackPrefab, nextNode.position, Quaternion.Euler(0, 270, 0)));
                    m_laidTrack[m_laidTrack.Count - 1].GetComponent<NodeList>().nodes.Reverse();
                }                
            }
            foreach (Transform t in m_laidTrack[m_laidTrack.Count - 1].GetComponent<NodeList>().nodes)
            {
                path.Add(new Node(t.position.x, t.position.z));
            }
            AudioSource.PlayClipAtPoint(placeSound, Vector3.zero, 5f);


            m_laidTrack[m_laidTrack.Count - 1].GetComponent<ChangeAltMaterial>().SetMaterials(m_altMaterial);
        //  }

        // return result;
        return true;
    }

    void Move()
    {
        if (path.Count < 2) return;
        transform.LookAt(path[pathIndex].position);
        transform.position = Vector3.MoveTowards(transform.position, path[pathIndex].position, speed * Time.deltaTime);
        if (transform.position == path[pathIndex].position) pathIndex++;
        pathIndex = Mathf.Clamp(pathIndex, 0, path.Count - 1);
        if (pathIndex == path.Count - 1 && wallHit && transform.position == path[pathIndex].position) Reset(); 
    }

    void PlaySound()
    {
        if (GetComponent<Rigidbody>().IsSleeping()) return;
        soundTimer -= speed * Time.deltaTime;
        //a.pitch = speed;
        if (soundTimer <= 0)
        {
            AudioSource.PlayClipAtPoint(trackSound, Vector3.zero, 5f);
            soundTimer = .6f;
        }
    }

    void OnDrawGizmos()
    {
        if (nextNode == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(nextNode.position, Vector3.one * .7f);

        foreach (Node n in path)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(n.position, Vector3.one * .7f);
        }
    }

    public void Reset()
    {
        for(int i = 0; i < m_laidTrack.Count; ++i)
            Destroy(m_laidTrack[m_laidTrack.Count - 1 - i], i / 20.0f );

        if (m_goldCount > 0)
            Instantiate(grid.m_goldPrefab, new Vector3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z), Quaternion.identity);

        m_laidTrack.Clear();

        grid.ResetTiles(playerNumber);
        direction = startDirection;
        transform.position = startingNode.position;
        currentNode = startingNode;
        path.Clear();
        path.Add(currentNode);
        pathIndex = 1;
        m_goldCount = 0;
        wallHit = false;
    }

    void SetActiveGoldBars()
    {
        for (int i = 0; i < 3; i++)
        {
            goldBars[i].SetActive(i < m_goldCount);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Gold")
        {
            Destroy(collision.gameObject);

            if (m_goldCount < 3)
                ++m_goldCount;

            SetActiveGoldBars();

            grid.SpawnGold();
        }
        else if (collision.gameObject.tag == "Stash")
        {
            if (collision.gameObject == stash)
            {
                // Increase stash total
              //  score.text = "" + collision.gameObject.GetComponent<Stash>().UpdateStash(m_goldCount);

                m_goldCount = 0;
                SetActiveGoldBars();
            }
            Reset();
        }
        else if (collision.gameObject.tag == "Player")
        {
            if (!m_colChecked)
            {
                m_colChecked = true;
                collision.gameObject.GetComponent<Player>().m_colChecked = true;

                if (m_goldCount < collision.gameObject.GetComponent<Player>().m_goldCount)
                {
                    m_goldCount += collision.gameObject.GetComponent<Player>().m_goldCount;

                    m_goldCount = Mathf.Clamp(m_goldCount, 0, 3);

                    collision.gameObject.GetComponent<Player>().m_goldCount = 0;

                    collision.gameObject.GetComponent<Player>().Reset();
                }
                else if (m_goldCount > collision.gameObject.GetComponent<Player>().m_goldCount)
                {
                    collision.gameObject.GetComponent<Player>().m_goldCount += m_goldCount;

                    collision.gameObject.GetComponent<Player>().m_goldCount = Mathf.Clamp(collision.gameObject.GetComponent<Player>().m_goldCount, 0, 3);

                    m_goldCount = 0;

                    Reset();
                }
                else if (m_goldCount == collision.gameObject.GetComponent<Player>().m_goldCount)
                {
                    collision.gameObject.GetComponent<Player>().Reset();

                    Reset();
                }
            }
            SetActiveGoldBars();
        }
    }


}
