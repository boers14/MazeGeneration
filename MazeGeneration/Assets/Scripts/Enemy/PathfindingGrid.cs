using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid : MonoBehaviour
{
    [SerializeField]
    private LayerMask wallMask = 0;

    private int xSize = 0, ySize = 0;

    [System.NonSerialized]
    public float startX = 0, endX = 0, startZ = 0, endZ = 0;

    private float addedX = 0, addedZ = 0;

    [SerializeField]
    private int amountOfNodesComparedToMazeSize = 3;

    private PathfindingNode[,] grid = null;

    public static PathfindingGrid instance = null;

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

        for (int i= 0; i < wallPool.Count; i++)
        {
            startX = CheckIfShouldChangeValue(wallPool[i].position.x < startX, startX, wallPool[i].position.x);
            endX = CheckIfShouldChangeValue(wallPool[i].position.x > endX, endX, wallPool[i].position.x);
            startZ = CheckIfShouldChangeValue(wallPool[i].position.z < startZ, startZ, wallPool[i].position.z);
            endZ = CheckIfShouldChangeValue(wallPool[i].position.z > endZ, endZ, wallPool[i].position.z);
        }

        float xPos = startX;
        float zPos = startZ;
        addedX = (endX - startX) / xSize;
        addedZ = (endZ - startZ) / ySize;

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

        EnemyManager.instance.FindPlayerObject();
    }

    private float CheckIfShouldChangeValue(bool shouldChange, float oldValue, float newValue)
    {
        if (shouldChange)
        {
            return newValue;
        }

        return oldValue;
    }

    public PathfindingNode NodeFromWorldPos(Vector3 worldPos)
    {
        int x = (int)((worldPos.x - startX) / addedX);
        int y = (int)((worldPos.z - startZ) / addedZ);

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
