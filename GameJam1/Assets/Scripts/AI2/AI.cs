using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    // CreateGrid section;
    GridManager grid;
    public GameObject m_curvedTrackPrefab;
    public GameObject m_straightTrackPrefab;
    public List<Node> path;
    private int pathIndex = 0;
    public float speed = 2.0f;
    private bool waitingForPath;
    public bool dumbAI = false;
    List<Node> nodeArray = new List<Node>();
    public List<Node> openSet;
    public List<Node> closedSet;
    public List<Node> neighbours;
    public Node start;
    public Node selected;
    float nodeSize = 1.0f;
    int width;
    int height;
    Vector3 startPos;
    public MobilePlayer playercontroller;
    public PlayerDirection direction;
    PlayerDirection pathDirection;
    bool pressedOnce = false;
    public bool GameStarted = false;
    float time = 0;
    float timer = 0.5f;
    bool TargetGold = false;

    public Node node;
    // Use this for initialization
    void Start()
    {

        startPos = transform.position;
        grid = FindObjectOfType<GridManager>();

      //  playercontroller.GetComponent<MobilePlayer>();
        waitingForPath = false;

        width = grid.width;
        height = grid.height;
        if (dumbAI)
        {
            GetNodes();
        }

    }
    private void Awake()
    {
      //  
    }

    public void GetNodes()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                nodeArray.Add(grid.GetNode(j, i));
            }
        }
    }
    //// Update is called once per frame
    void Update()
    {
        if (path != null && path.Count > 0)
        {
            
            Vector3 target = path[pathIndex].position;
            //target.y = groundOffset;
            /*  transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
              transform.LookAt(target);
              transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);*/
            if (!pressedOnce && time > timer)
            {
                if(pathIndex !=0)
                { 
                ButtonPressed(path[pathIndex-1], path[pathIndex]);
                }
                pressedOnce = true;
                time = 0;
            }
            else
            {
                time += Time.deltaTime;
            }
            //  if (roundVec(transform.position, 0.2f) == roundVec(target, 0.2f))
            // {
            if (path != null)
            {
                pathIndex++;
                pressedOnce = false;
                if (pathIndex >= path.Count)
                {
                    path = null;
                    pathIndex = 0;
                    waitingForPath = false;
                }
            }
        }
        else if (!waitingForPath && GameStarted)
        {
            direction = playercontroller.direction;
            pathDirection = direction;


            if (CheckGoldPos() && TargetGold == false)
            {
                TargetGold = true;
                path = CreatePath(transform.position, new Vector2((int)grid.GoldPos.x, (int)grid.GoldPos.y));
                waitingForPath = false;
            }
            else
            {
                findNewRandomPath();
           }

       
            waitingForPath = true;

        }
        else {
            waitingForPath = false;
        }
  
    }

    private void ButtonPressed(Node current, Node Next)
    {
        
        direction = playercontroller.direction;
        switch(direction)
        {
            case PlayerDirection.UP:
                {
                    if(current.arrayPosition.x-1 == Next.arrayPosition.x)
                    {
                        playercontroller.LeftButtonPressed();
                    }
                    else if(current.arrayPosition.x+1 == Next.arrayPosition.x)
                    {
                        playercontroller.RightButtonPressed();
                    }
                    else if(current.arrayPosition.y+1 == Next.arrayPosition.y)
                    {
                        playercontroller.StraightButtonPressed();
                    }
                    else
                    {
                        
                        ReGenPath();
                        if(TargetGold)
                        {
                            findNewRandomPath();
                            TargetGold = false;
                        }
                    }
                }
                break;
            case PlayerDirection.LEFT:
                {
                    if (current.arrayPosition.x+ 1 == Next.arrayPosition.x)
                    {
                        playercontroller.StraightButtonPressed();
                    }
                    else if (current.arrayPosition.y - 1 == Next.arrayPosition.y)
                    {
                        playercontroller.LeftButtonPressed();
                    }
                    else if (current.arrayPosition.y + 1 == Next.arrayPosition.y)
                    {
                        playercontroller.RightButtonPressed();
                    }
                    else
                    {
                        ReGenPath();
                        if (TargetGold)
                        {
                            findNewRandomPath();
                            TargetGold = false;
                        }
                    }
                }
                break;
            case PlayerDirection.RIGHT:
                {
                    if (current.arrayPosition.x - 1 == Next.arrayPosition.x)
                    {
                        playercontroller.StraightButtonPressed();
                    }
                    else if (current.arrayPosition.y + 1 == Next.arrayPosition.y)
                    {
                        playercontroller.LeftButtonPressed();
                    }
                    else if (current.arrayPosition.y - 1 == Next.arrayPosition.y)
                    {
                        playercontroller.RightButtonPressed();
                    }
                    else
                    {
                        ReGenPath();
                        if (TargetGold)
                        {
                            findNewRandomPath();
                            TargetGold = false;
                        }
                    }
                }
                break;
            case PlayerDirection.DOWN:
                {
                    if (current.arrayPosition.x - 1 == Next.arrayPosition.x)
                    {
                        playercontroller.LeftButtonPressed();
                    }
                    else if (current.arrayPosition.x + 1 == Next.arrayPosition.x)
                    {
                        playercontroller.RightButtonPressed();
                    }
                    else if (current.arrayPosition.y - 1 == Next.arrayPosition.y)
                    {
                        playercontroller.StraightButtonPressed();
                    }
                    else
                    {
                        ReGenPath();
                        if (TargetGold)
                        {
                            findNewRandomPath();
                            TargetGold = false;
                        }
                    }
                }
                break;

        }

    }

    private void ReGenPath()
    {
        path.Clear();
        openSet.Clear();
        closedSet.Clear();
        neighbours.Clear();
        path = null;
        pathIndex = 0;
        waitingForPath = false;
    }
    private bool CheckGoldPos()
    {

        if (Vector2.Distance(grid.GoldPos, new Vector2(transform.position.x, transform.position.z)) < 8.0f)
        {
            return true;
        }
        return false;

    }

    private Vector3 roundVec(Vector3 vector, float roundTo)
    {
        return new Vector3(
             Mathf.Round(vector.x / roundTo) * roundTo,
             Mathf.Round(vector.y / roundTo) * roundTo,
             Mathf.Round(vector.z / roundTo) * roundTo);
    }


    private void findNewRandomPath()
    {
        Node randomNode = getRandomWalkableNode();
        path = CreatePath(transform.position, randomNode.position);
        waitingForPath = false;
    }

    private void CheckPlayerDirection(Node current, Node Next)
    {
            switch (pathDirection)
            {
                case PlayerDirection.UP:
                    {
                        if (Next.arrayPosition.x == current.arrayPosition.x - 1)
                        {
                            pathDirection = PlayerDirection.LEFT;
                        }
                        else if (Next.arrayPosition.x == current.arrayPosition.x + 1)
                        {
                            pathDirection = PlayerDirection.RIGHT;
                        }
                        else if (Next.arrayPosition.y == current.arrayPosition.y + 1)
                        {
                            pathDirection = PlayerDirection.UP;
                        }
                    }
                    break;
                case PlayerDirection.DOWN:
                    {
                        if (Next.arrayPosition.x == current.arrayPosition.x - 1)
                        {
                            pathDirection = PlayerDirection.RIGHT;
                        }
                        else if (Next.arrayPosition.x == current.arrayPosition.x + 1)
                        {
                            pathDirection = PlayerDirection.LEFT;
                        }
                        else if (Next.arrayPosition.y == current.arrayPosition.y - 1)
                        {
                            pathDirection = PlayerDirection.DOWN;
                        }
                    }
                    break;
                case PlayerDirection.LEFT:
                    {
                        if (Next.arrayPosition.x == current.arrayPosition.x + 1)
                        {
                            pathDirection = PlayerDirection.LEFT;
                        }
                        else if (Next.arrayPosition.y == current.arrayPosition.y + 1)
                        {
                            pathDirection = PlayerDirection.UP;
                        }
                        else if (Next.arrayPosition.y == current.arrayPosition.y - 1)
                        {
                            pathDirection = PlayerDirection.DOWN;
                        }
                    }
                    break;
                case PlayerDirection.RIGHT:
                    {
                        if (Next.arrayPosition.x == current.arrayPosition.x - 1)
                        {
                            pathDirection = PlayerDirection.RIGHT;
                        }
                        else if (Next.arrayPosition.y == current.arrayPosition.y + 1)
                        {
                            pathDirection = PlayerDirection.UP;
                        }
                        else if (Next.arrayPosition.y == current.arrayPosition.y - 1)
                        {
                            pathDirection = PlayerDirection.DOWN;
                        }
                    }
                    break;
            }
    }
    public List<Node> CreatePath(Vector3 startPosition, Vector3 endPosition)
    {
        start = getNodeFromPosition(startPosition);
        Node end = getNodeFromPosition(endPosition);

        if (start == null || end == null)
        {
            Debug.Log("Could not find path from: " + startPosition + " to " + endPosition);
            return null;
        }

        openSet = new List<Node>();
        closedSet = new List<Node>();

        openSet.Add(start);

        while (openSet.Count > 0)
        {
//            Debug.Log(openSet.Count);
            Node currentNode = openSet[0];
            if(currentNode != start || currentNode.parent != null)
            {
                CheckPlayerDirection(currentNode.parent, currentNode);
            }

            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].calculateFCost() < currentNode.calculateFCost() //If it has a lower fCost
                || openSet[i].calculateFCost() == currentNode.calculateFCost() // Or if the fCost is the same but the hCost is lower
                && openSet[i].hCost < currentNode.hCost)
                {

                    currentNode = openSet[i];

                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == end)
            {

                return CheckPath(start, end);    //End reached trace path back
            }

            foreach (Node neighbour in getNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                //Calculate the new lowest costs for the neighbour nodes
                int costToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour);
                if (costToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = costToNeighbour;
                    neighbour.hCost = getDistance(neighbour, end);
                    neighbour.parent = currentNode;    //Set parent node

                    openSet.Add(neighbour);
                    CheckPlayerDirection(currentNode, neighbour);
                }
            }
        }
        Debug.Log("returned null");
        return null;

    }
    public Node getRandomWalkableNode()
    {
        selected = null;

        do
        {
            int index = Random.Range(0, nodeArray.Count - 1);
            selected = nodeArray[index];
        } while (!selected.walkable);

        return selected;
    }

    private Node getNodeFromPosition(Vector3 position)
    {
        int gridX = Mathf.RoundToInt(position.x / nodeSize);
        int gridY = Mathf.RoundToInt(position.z / nodeSize);

        int index = (int)gridX + (gridY * width);
        Node node = null;

        if (index < nodeArray.Count - 1)
        {
            node = nodeArray[index];
        }

        return node;
    }

    private List<Node> CheckPath(Node start, Node end)
    {
        List<Node> path = new List<Node>();

        Node currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private List<Node> getNeighbours(Node node)
    {
//        Debug.Log("Neighbours");
        neighbours = new List<Node>();
/*
        switch (pathDirection)
        {
            case PlayerDirection.UP:
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int x = -1; x <= 1; x++)
                        {
                            if (y == -1 || (Mathf.Abs(x) == Mathf.Abs(y)))
                            {
                                continue;
                            }
                            //make sure the pos is within grid
                            CheckNeighbours(x, y, node);
                        }
                    }
                }
                break;
            case PlayerDirection.DOWN:
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int x = -1; x <= 1; x++)
                        {
                            if (y == +1 || (Mathf.Abs(x) == Mathf.Abs(y)))
                            {
                                continue;
                            }
                            //make sure the pos is within grid
                            CheckNeighbours(x, y, node);
                        }
                    }
                }
                break;
            case PlayerDirection.LEFT:
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int x = -1; x <= 1; x++)
                        {
                            if (x == -1 || (Mathf.Abs(x) == Mathf.Abs(y)))
                            {
                                continue;
                            }
                            //make sure the pos is within grid
                            CheckNeighbours(x, y, node);
                        }
                    }
                }
                break;
            case PlayerDirection.RIGHT:
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int x = -1; x <= 1; x++)
                        {
                            if (x == 1 || (Mathf.Abs(x) == Mathf.Abs(y)))
                            {
                                continue;
                            }
                            //make sure the pos is within grid
                            CheckNeighbours(x, y, node);
                        }
                    }
                }
                break;
        }
        
        */

           for (int y = -1; y <= 1; y++)
            {
            for (int x = -1; x <= 1; x++)
            {
                if ((Mathf.Abs(x) == Mathf.Abs(y)))
                {
                    continue;
                }
                CheckNeighbours(x, y, node);
            }
         
        }
        return neighbours;
    }

    void CheckNeighbours(int x, int y, Node node)
    {
        int checkX = (int)node.arrayPosition.x + x;
        int checkY = (int)node.arrayPosition.y + y;

        if (checkY >= 0 && checkY < width && checkX >= 0 && checkX < height)
        {
//            Debug.Log(checkX + checkY * width);
            neighbours.Add(nodeArray[checkX + checkY * width]);
        }
    }

    private int getDistance(Node a, Node b)
    {
        int distX = (int)Mathf.Abs(a.arrayPosition.y - b.arrayPosition.y);
        int distY = (int)Mathf.Abs(a.arrayPosition.x - b.arrayPosition.x);

        if (distY > distX)
        {
//            Debug.Log(distX + distY + "distance");
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distY + 10 * (distX - distY);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Gold")
        {
           // Destroy(collision.gameObject);
           // grid.SpawnGold();
        }
        else
        {
          //  path.Clear();
           // openSet.Clear();
            //closedSet.Clear();
            //neighbours.Clear();
            path = null;
            pathIndex = 0;
           // transform.position = startPos;
            waitingForPath = false;
        }
    }
}

