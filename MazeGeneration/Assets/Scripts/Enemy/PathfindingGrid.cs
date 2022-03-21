using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid : MonoBehaviour
{
    [SerializeField]
    private LayerMask wallMask = 0;

    private int xSize = 0, ySize = 0;

    private float addedX = 0, startX = 0, addedZ = 0, startZ = 0;

    [SerializeField]
    private int amountOfNodesComparedToMazeSize = 3;

    private PathfindingNode[,] grid = null;

    public static PathfindingGrid instance = null;

    [SerializeField]
    private Enemy enemy = null;

    [SerializeField]
    private GameObject spherePrefab = null;

    private void Start()
    {
        if (!instance)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Enemy newEnemy = Instantiate(enemy);
            newEnemy.transform.position = new Vector3(Random.Range(-20, 120), 0.5f, Random.Range(-20, 120));
        }
    }

    public void StartCreateGrid(int width, int height)
    {
        xSize = width * amountOfNodesComparedToMazeSize;
        ySize = height * amountOfNodesComparedToMazeSize;
        transform.position = MazeRenderer.instance.centerMazePos;
        StartCoroutine(CreateGrid());
    }

    private IEnumerator CreateGrid()
    {
        yield return new WaitForSeconds(0.5f);

        List<Transform> wallPool = MazeRenderer.instance.activeObjects;

        float biggestX = 0, biggestZ = 0;
        for (int i= 0; i < wallPool.Count; i++)
        {
            startX = CheckIfShouldChangeValue(wallPool[i].position.x < startX, startX, wallPool[i].position.x);
            biggestX = CheckIfShouldChangeValue(wallPool[i].position.x > biggestX, biggestX, wallPool[i].position.x);
            startZ = CheckIfShouldChangeValue(wallPool[i].position.z < startZ, startZ, wallPool[i].position.z);
            biggestZ = CheckIfShouldChangeValue(wallPool[i].position.z > biggestZ, biggestZ, wallPool[i].position.z);
        }

        float xPos = startX;
        float zPos = startZ;
        addedX = (biggestX - startX) / xSize;
        addedZ = (biggestZ - startZ) / ySize;

        grid = new PathfindingNode[xSize + 1, ySize + 1];
        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                Vector3 worldPoint = new Vector3(xPos, 0.5f, zPos);
                bool wall = false;

                if (Physics.CheckSphere(worldPoint, addedX / 2, wallMask))
                {
                    wall = true;
                }

                grid[x, y] = new PathfindingNode(wall, worldPoint, x, y);

                xPos += addedX;
            }

            zPos += addedZ;
            xPos = startX;
        }
    }

    private float CheckIfShouldChangeValue(bool shouldChange, float oldValue, float newValue)
    {
        if (shouldChange)
        {
            return newValue;
        }

        return oldValue;
    }

    public PathfindingNode NodeFromWorldPos(Vector3 worldPos, PathfindingNode prevNote = null)
    {
        int x = (int)((worldPos.x - startX) / addedX);
        int y = (int)((worldPos.z - startZ) / addedZ);

        //if (prevNote != null)
        //{
        //    if (prevNote.GetGridX() - x > 5)
        //    {
        //        x = prevNote.GetGridX() - 5;
        //    } else if (prevNote.GetGridX() - x < -5)
        //    {
        //        x = prevNote.GetGridX() + 5;
        //    }

        //    if (prevNote.GetGridY() - y > 5)
        //    {
        //        y = prevNote.GetGridY() - 5;
        //    }
        //    else if (prevNote.GetGridY() - y < -5)
        //    {
        //        y = prevNote.GetGridY() + 5;
        //    }
        //}

        return grid[x, y];
    }

    public List<PathfindingNode> GetNeighborNodes(PathfindingNode node)
    {
        List<PathfindingNode> neighboringNodes = new List<PathfindingNode>();

        AddNeigbourNodeToList(neighboringNodes, node.GetGridX() + 1, node.GetGridY());
        AddNeigbourNodeToList(neighboringNodes, node.GetGridX() - 1, node.GetGridY());
        AddNeigbourNodeToList(neighboringNodes, node.GetGridX(), node.GetGridY() + 1);
        AddNeigbourNodeToList(neighboringNodes, node.GetGridX(), node.GetGridY() - 1);

        return neighboringNodes;
    }

    private void AddNeigbourNodeToList(List<PathfindingNode> neighboringNodes, int xCheck, int yCheck)
    {
        if (xCheck >= 0 && xCheck < xSize)
        {
            if (yCheck >= 0 && yCheck < ySize)
            {
                neighboringNodes.Add(grid[xCheck, yCheck]);
            }
        }
    }
}
