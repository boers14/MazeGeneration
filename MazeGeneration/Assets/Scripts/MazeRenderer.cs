using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField]
    private int width = 50, height = 50;

    [SerializeField]
    private Transform wallPrefab = null, planePrefab = null;

    private List<Transform> wallPool = new List<Transform>(), activeWalls = new List<Transform>(), 
        groundAndRoof = new List<Transform>();

    public static MazeRenderer instance = null;

    private Vector3 rightOffset = new Vector3(0.5f, 0, 0), leftOffset = new Vector3(-0.5f, 0, 0), 
        downOffset = new Vector3(0, 0, -0.5f), upOffset = new Vector3(0, 0, 0.5f);

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

        CreatePlane(new Vector3(0, -wallPrefab.localScale.y / 2, 0), Vector3.zero);
        CreatePlane(new Vector3(0, wallPrefab.localScale.y / 2, 0), new Vector3(180, 0, 0));

        AddWallsToPool(5000);
        GenerateMaze(MazeGenerator.GenerateMaze(width, height, MazeGenerator.MazeType.RecursiveBackTracking));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMaze(MazeGenerator.GenerateMaze(width, height, MazeGenerator.MazeType.RecursiveBackTracking));
        }
    }

    public void GenerateMaze(WallState[,] walls)
    {
        for (int i = activeWalls.Count - 1; i >= 0; i--)
        {
            activeWalls[i].gameObject.SetActive(false);
            wallPool.Add(activeWalls[i]);
        }
        activeWalls.Clear();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                WallState node = walls[i, j];

                switch (node.direction)
                {
                    case WallState.Direction.Right:
                        CreateWall(i, j, rightOffset, 0, false);
                        break;
                    case WallState.Direction.Left:
                        CreateWall(i, j, leftOffset, 180, false);
                        break;
                    case WallState.Direction.Down:
                        CreateWall(i, j, downOffset, 90, j == 0);
                        break;
                    case WallState.Direction.Up:
                        CreateWall(i, j, upOffset, 270, j == height - 1);
                        break;
                }
            }
        }

        for (int i = -1; i < width; i++)
        {
            CreateWall(i, -1, rightOffset, 0, false);
            CreateWall(i, height, rightOffset, 0, false);
        }

        for (int i = -1; i < height; i++)
        {
            CreateWall(-1, i, upOffset, 90, false);
            CreateWall(width, i + 1, downOffset, 90, false);
        }

        Vector3 newPos = new Vector3(-0.5f, 0, -0.5f);
        Vector3 newScale = new Vector3(ReturnScaleOfPlane(width), 0, ReturnScaleOfPlane(height));
        for (int i = 0; i < groundAndRoof.Count; i++)
        {
            groundAndRoof[i].position = new Vector3(newPos.x, groundAndRoof[i].position.y, newPos.z);
            groundAndRoof[i].localScale = new Vector3(newScale.x, groundAndRoof[i].localScale.y, newScale.z);
        }
    }

    private float ReturnScaleOfPlane(int value)
    {
        return (value / 10) + 0.1f;
    }

    private void CreateWall(float x, float y, Vector3 offset, float yRot , bool dontCreate)
    {
        if (dontCreate) { return; }

        if (wallPool.Count == 0)
        {
            AddWallsToPool(5);
        }

        Transform wall = wallPool[0];
        wallPool.RemoveAt(0);

        wall.gameObject.SetActive(true);
        wall.position = new Vector3(-width / 2 + x, 0, -height / 2 + y) + offset;
        wall.eulerAngles = new Vector3(0, yRot, 0);
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
        groundAndRoof.Add(newPlane);
    }
}
