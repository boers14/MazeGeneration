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
                GenerateWall(node, WallState.Right, i, j, 0, i == width - 1, new Vector3(0.5f, 0, 0), j == 0);
                GenerateWall(node, WallState.Down, i, j, 90, j == 0 || i < width - 1, new Vector3(0, 0, -0.5f), i == width - 1);
                GenerateWall(node, WallState.Left, i, j, 180, i == 0 || j < height - 1, new Vector3(-0.5f, 0, 0), j == height - 1);
                GenerateWall(node, WallState.Up, i, j, 270, j == height - 1, new Vector3(0, 0, 0.5f), i == 0);
            }
        }

        Vector3 newPos = new Vector3(-0.5f, 0, -0.5f);
        Vector3 newScale = new Vector3(width / 10, 0, height / 10);
        for (int i = 0; i < groundAndRoof.Count; i++)
        {
            groundAndRoof[i].position = new Vector3(newPos.x, groundAndRoof[i].position.y, newPos.z);
            groundAndRoof[i].localScale = new Vector3(newScale.x, groundAndRoof[i].localScale.y, newScale.z);
        }
    }

    private void GenerateWall(WallState node, WallState wallState, float x, float y, float yRot, bool ignoreValue, Vector3 offset,
        bool alwaysCreateWall)
    {
        if (!ignoreValue)
        {
            if (node.HasFlag(wallState))
            {
                CreateWall(x, y, offset, yRot);
            }
            else if (alwaysCreateWall)
            {
                CreateWall(x, y, offset, yRot);
            }
        }
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
