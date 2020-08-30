using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 8;
    public int height = 8;

    [SerializeField] private int m_obstacleChance;
    [SerializeField] private int _obstaclesAmount;
   
    Node[,] grid;

    public List<GameObject> m_tilePrefabs;
    public List<GameObject> _obstaclePrefabs;


    public GameObject m_goldPrefab;
    public GameObject m_powerupPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        int players = FindObjectOfType<MobileGameManager>().players;
        height = 8;
        width = 8;
        grid = new Node[width, height];
        _obstaclesAmount = (height * width * m_obstacleChance) / 100; // will loose decimal places
        InitialiseGrid();
    }

    void InitialiseGrid()
    {
        List<Vector2Int> obstaclesCoordinates = new List<Vector2Int>();
        for (int i = 0; i < _obstaclesAmount; i++)
        {
            Vector2Int newCoordinate = new Vector2Int(Random.Range(1, width - 1), Random.Range(2, height - 2));
            while (obstaclesCoordinates.Contains(newCoordinate))
            {
                newCoordinate = new Vector2Int(Random.Range(1, width - 1), Random.Range(2, height - 2));
            }

            obstaclesCoordinates.Add(newCoordinate);
            Debug.Log(obstaclesCoordinates[i]);
        }


        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                grid[x, y] = new Node(x, y);

                if (obstaclesCoordinates.Contains(new Vector2Int(x, y)))
                {
                    SpawnObstacle(new Vector2(x, y));
                    grid[x, y].obstacle = true;
                }
                else
                {
                    SpawnTile(new Vector2(x, y));
                }
            }
        }

        SpawnGold();
        SpawnPowerUp();
    }

    public Node GetNode(int x, int y)
    {
        return grid[x, y];
    }

    public void SetPlayerOwnership(Node n, int player)
    {
        grid[(int) n.arrayPosition.x, (int) n.arrayPosition.y].player = player;
    }

    public void ResetTiles(int playerNumber)
    {
        foreach (Node n in grid)
        {
            if (n.player == playerNumber)
            {
                n.player = 0;
            }
        }

        //foreach (Node n in nodes)
        //{
        //    grid[(int)n.arrayPosition.x, (int)n.arrayPosition.y].player = 0;
        //}
    }

    public bool GetNextNode(Node currentNode, ref Node nextNode, PlayerDirection dir)
    {
        switch (dir)
        {
            case PlayerDirection.UP:
                if (currentNode.arrayPosition.y + 1 < height)
                {
                    nextNode = grid[(int) currentNode.arrayPosition.x, (int) currentNode.arrayPosition.y + 1];
                    if (nextNode.obstacle)
                        return false;
                    return true;
                }

                break;

            case PlayerDirection.DOWN:
                if (currentNode.arrayPosition.y - 1 >= 0)
                {
                    nextNode = grid[(int) currentNode.arrayPosition.x, (int) currentNode.arrayPosition.y - 1];
                    if (nextNode.obstacle)
                        return false;
                    return true;
                }

                break;

            case PlayerDirection.LEFT:
                if (currentNode.arrayPosition.x - 1 >= 0)
                {
                    nextNode = grid[(int) currentNode.arrayPosition.x - 1, (int) currentNode.arrayPosition.y];
                    if (nextNode.obstacle)
                        return false;
                    return true;
                }

                break;

            case PlayerDirection.RIGHT:
                if (currentNode.arrayPosition.x + 1 < width)
                {
                    nextNode = grid[(int) currentNode.arrayPosition.x + 1, (int) currentNode.arrayPosition.y];
                    if (nextNode.obstacle)
                        return false;
                    return true;
                }

                break;
        }

        return false;
    }

    public void SpawnGold()
    {
        if (GameObject.FindGameObjectsWithTag("Gold").Length > 2)
            return;

        int x = 0;
        int y = 0;

        const int safetyCount = 1000;
        int i = 0;

        bool result;

        do
        {
            result = true;

            if (++i > safetyCount)
            {
                Invoke("SpawnGold", 5.0f);
                return;
            }

            x = Random.Range(0, width);
            y = Random.Range(0, height);

            if (grid[x, y].obstacle)
                result = false;

            if (GetNode(x, y).player != 0)
                result = false;

            foreach (GameObject s in GameObject.FindGameObjectsWithTag("Stash"))
                if (new Vector3(x, 0, y) == s.transform.position)
                    result = false;
        } while (!result);

        Instantiate(m_goldPrefab, new Vector3(x, 0, y), Quaternion.identity);
    }

    public void SpawnPowerUp()
    {
        if (GameObject.FindGameObjectsWithTag("PowerUp").Length > 2)
            return;

        int x = 0;
        int y = 0;

        const int safetyCount = 1000;
        int i = 0;

        bool result;

        do
        {
            result = true;

            if (++i > safetyCount)
            {
                Invoke("SpawnPowerUp", 5.0f);
                return;
            }

            x = Random.Range(0, width);
            y = Random.Range(0, height);

            if (grid[x, y].obstacle)
                result = false;

            if (GetNode(x, y).player != 0)
                result = false;

        } while (!result);

        Instantiate(m_powerupPrefab, new Vector3(x, 0, y), Quaternion.identity);
    }

    void SpawnTile(Vector2 pos)
    {
        Instantiate(m_tilePrefabs[Random.Range(0, m_tilePrefabs.Count)], new Vector3(pos.x, -0.03f, pos.y), Quaternion.identity);
    }

    private void SpawnObstacle(Vector2 pos)
    {
        Instantiate(_obstaclePrefabs[Random.Range(0, _obstaclePrefabs.Count)], new Vector3(pos.x, -0.03f, pos.y), Quaternion.identity);
    }

    void OnDrawGizmos()
    {
        if (grid == null) return;
        foreach (Node n in grid)
        {
            Gizmos.color = Color.red;
            if (n.obstacle)
            {
                Gizmos.color = Color.yellow;
            }

            Gizmos.DrawWireCube(n.position, Vector3.one * 0.7f);
        }
    }
}

public enum PlayerDirection
{
    UP,
    RIGHT,
    DOWN,
    LEFT
}