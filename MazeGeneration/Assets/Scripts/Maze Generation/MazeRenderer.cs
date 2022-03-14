using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField]
    private Transform wallPrefab = null, planePrefab = null;

    [SerializeField]
    private float mazeWallSize = 2;

    private List<Transform> wallPool = new List<Transform>(), activeWalls = new List<Transform>(), 
        groundAndRoof = new List<Transform>();

    public static MazeRenderer instance = null;

    private Vector3 rightOffset = new Vector3(0.5f, 0, 0), leftOffset = new Vector3(-0.5f, 0, 0), 
        downOffset = new Vector3(0, 0, -0.5f), upOffset = new Vector3(0, 0, 0.5f);

    [System.NonSerialized]
    public Vector3 centerMazePos = Vector3.zero;

    private int width = 50, height = 30;

    private void Start()
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

        rightOffset *= mazeWallSize;
        leftOffset *= mazeWallSize;
        downOffset *= mazeWallSize;
        upOffset *= mazeWallSize;

        CreatePlane(new Vector3(0, -wallPrefab.localScale.y / 2, 0), Vector3.zero);
        CreatePlane(new Vector3(0, wallPrefab.localScale.y / 2, 0), new Vector3(180, 0, 0));

        AddWallsToPool(5000);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void StartGenerateMaze(int width, int height)
    {
        this.width = width;
        this.height = height;
        GenerateMaze(MazeGenerator.GenerateMaze(width, height, MazeGenerator.MazeType.RecursiveBackTracking));
    }

    private void GenerateMaze(WallState[,] walls)
    {
        ClearMaze();

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

        centerMazePos = new Vector3(ReturnPosOfPlane(width), 0, ReturnPosOfPlane(height));
        Vector3 newScale = new Vector3(ReturnScaleOfPlane(width), 0, ReturnScaleOfPlane(height));
        for (int i = 0; i < groundAndRoof.Count; i++)
        {
            groundAndRoof[i].position = new Vector3(centerMazePos.x, groundAndRoof[i].position.y, centerMazePos.z);
            groundAndRoof[i].localScale = new Vector3(newScale.x, groundAndRoof[i].localScale.y, newScale.z);
            groundAndRoof[i].gameObject.SetActive(true);
        }
    }

    private float ReturnPosOfPlane(int value)
    {
        return -value / 2 + value / 2 * mazeWallSize - 0.5f * mazeWallSize;
    }

    private float ReturnScaleOfPlane(int value)
    {
        return (value * mazeWallSize / 10) + (mazeWallSize / 10);
    }

    private void CreateWall(float x, float y, Vector3 offset, float yRot)
    {
        if (wallPool.Count == 0)
        {
            AddWallsToPool(5);
        }

        Transform wall = wallPool[0];
        wallPool.RemoveAt(0);

        wall.gameObject.SetActive(true);
        wall.position = new Vector3(-width / 2 + x * mazeWallSize, 0, -height / 2 + y * mazeWallSize) + offset;
        wall.eulerAngles = new Vector3(0, yRot, 0);
        wall.localScale = new Vector3(wall.localScale.x * mazeWallSize, wall.localScale.y, wall.localScale.z);
        activeWalls.Add(wall);
    }

    private void AddWallsToPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform newWall = Instantiate(wallPrefab, transform);
            newWall.gameObject.SetActive(false);
            wallPool.Add(newWall);
        }
    }

    private void CreatePlane (Vector3 pos, Vector3 rot)
    {
        Transform newPlane = Instantiate(planePrefab);
        newPlane.position = pos;
        newPlane.eulerAngles = rot;
        newPlane.gameObject.SetActive(false);
        groundAndRoof.Add(newPlane);
    }

    public void ClearMaze()
    {
        for (int i = activeWalls.Count - 1; i >= 0; i--)
        {
            activeWalls[i].gameObject.SetActive(false);
            wallPool.Add(activeWalls[i]);
        }
        activeWalls.Clear();

        for (int i = 0; i < groundAndRoof.Count; i++)
        {
            groundAndRoof[i].gameObject.SetActive(false);
        }
    }
}
