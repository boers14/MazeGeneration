using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : ObjectPool
{
    [SerializeField]
    private Transform planePrefab = null;

    public float mazeWallSize = 2;

    private List<Transform> groundAndRoof = new List<Transform>();

    // Is singleton
    public static MazeRenderer instance = null;

    private Vector3 rightOffset = new Vector3(0.5f, 0, 0), leftOffset = new Vector3(-0.5f, 0, 0), 
        downOffset = new Vector3(0, 0, -0.5f), upOffset = new Vector3(0, 0, 0.5f);

    [System.NonSerialized]
    public Vector3 centerMazePos = Vector3.zero;

    private int width = 50, height = 30;

    // Add objects to pool and calculate the offsets
    public override void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        base.Start();

        rightOffset *= mazeWallSize;
        leftOffset *= mazeWallSize;
        downOffset *= mazeWallSize;
        upOffset *= mazeWallSize;

        CreatePlane(new Vector3(0, -objectForPool.localScale.y / 2, 0), Vector3.zero);
        CreatePlane(new Vector3(0, objectForPool.localScale.y / 2, 0), new Vector3(180, 0, 0));

        AddObjectsToPool(5000);
    }

    // Set width/height. Generate maze
    public void StartGenerateMaze(int width, int height)
    {
        this.width = width;
        this.height = height;
        GenerateMaze(MazeGenerator.GenerateMaze(width, height));
    }
    
    // Remove all walls from maze, create new maze based on given maze
    private void GenerateMaze(WallState[,] walls)
    {
        ClearMaze();

        // Create a wall based on the direction given in the node
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                WallState node = walls[i, j];

                switch (node.direction)
                {
                    case WallState.Direction.Right:
                        CreateWall(i, j, rightOffset, 0);
                        break;
                    case WallState.Direction.Left:
                        CreateWall(i, j, leftOffset, 180);
                        break;
                    case WallState.Direction.Down:
                        CreateWall(i, j, downOffset, 90);
                        break;
                    case WallState.Direction.Up:
                        CreateWall(i, j, upOffset, 270);
                        break;
                }
            }
        }

        // Create outer walls
        for (int i = -1; i < width; i++)
        {
            CreateWall(i, -1, rightOffset, 0);
            CreateWall(i, height, rightOffset, 0);
        }

        for (int i = -1; i < height; i++)
        {
            CreateWall(-1, i, upOffset, 90);
            CreateWall(width, i + 1, downOffset, 90);
        }

        // Set roof and ground pos/ scale
        centerMazePos = new Vector3(ReturnPosOfPlane(width), 0, ReturnPosOfPlane(height));
        Vector3 newScale = new Vector3(ReturnScaleOfPlane(width), 0, ReturnScaleOfPlane(height));
        for (int i = 0; i < groundAndRoof.Count; i++)
        {
            groundAndRoof[i].position = new Vector3(centerMazePos.x, groundAndRoof[i].position.y, centerMazePos.z);
            groundAndRoof[i].localScale = new Vector3(newScale.x, groundAndRoof[i].localScale.y, newScale.z);
            groundAndRoof[i].gameObject.SetActive(true);
        }
    }

    // Return pos of plane based on maze size/ wall size. Add 0.5 for uneven maze size
    private float ReturnPosOfPlane(float value)
    {
        if (value % 2 == 0)
        {
            return (-value / 2f) + (value / 2f * mazeWallSize) - (0.5f * mazeWallSize);
        } else
        {
            return (-value / 2f) + (value / 2f * mazeWallSize) - (0.5f * mazeWallSize) + 0.5f;
        }
    }

    // Return scale of plane based on wall size/ maze size
    private float ReturnScaleOfPlane(float value)
    {
        return (value * mazeWallSize / 10f) + (mazeWallSize / 10f);
    }

    // Create a wall on given x/ y with extra offset calculated in, make wall x scale bigger based on wall size
    private void CreateWall(float x, float y, Vector3 offset, float yRot)
    {
        Transform wall = RetrieveObjectFromPool();
        wall.position = new Vector3(-width / 2 + x * mazeWallSize, 0, -height / 2 + y * mazeWallSize) + offset;
        wall.eulerAngles = new Vector3(0, yRot, 0);
        wall.localScale = new Vector3(mazeWallSize, wall.localScale.y, wall.localScale.z);
    }

    // Create a new plane on given pos with given rot
    private void CreatePlane (Vector3 pos, Vector3 rot)
    {
        Transform newPlane = Instantiate(planePrefab);
        newPlane.position = pos;
        newPlane.eulerAngles = rot;
        newPlane.gameObject.SetActive(false);
        groundAndRoof.Add(newPlane);
    }

    // Remove walls that were doubly placed
    public void RemoveDoubleWalls()
    {
        for (int i = activeObjects.Count - 1; i >= 0 ; i--)
        {
            List<Transform> objectsOnSamePos = activeObjects.FindAll(wall => wall.position == activeObjects[i].position);
            if (objectsOnSamePos.Count > 1)
            {
                activeObjects[i].gameObject.SetActive(false);
                objectPool.Add(activeObjects[i]);
                activeObjects.Remove(activeObjects[i]);
            }
        }
    }

    // Clear maze by returning walls to pool and turning off the ground and roof
    public void ClearMaze()
    {
        ReturnAllObjectsToPool();
        for (int i = 0; i < groundAndRoof.Count; i++)
        {
            groundAndRoof[i].gameObject.SetActive(false);
        }
    }
}
