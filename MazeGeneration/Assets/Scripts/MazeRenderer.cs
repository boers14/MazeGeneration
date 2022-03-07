using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [SerializeField]
    private int width = 50, height = 50;

    [SerializeField]
    private Transform wallPrefab = null, planePrefab = null;

    private float baseOffset = 1;

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

        AddWallsToPool(500);
        baseOffset = wallPrefab.localScale.x / 2;
        GenerateMaze(MazeGenerator.GenerateMaze(width, height));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMaze(MazeGenerator.GenerateMaze(width, height));
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
                GenerateWall(node, WallState.Right, i, j, new Vector2(baseOffset, 0), 0, i == 0);
                GenerateWall(node, WallState.Down, i, j, new Vector2(0, -baseOffset), 90, j == height - 1);
                GenerateWall(node, WallState.Left, i, j, new Vector2(-baseOffset, 0), 180, i == width - 1);
                GenerateWall(node, WallState.Up, i, j, new Vector2(0, baseOffset), 270, j == 0);
            }
        }

        Vector3 newPos = new Vector3((width - 1f) / 2, 0, (height - 1f) / 2);
        Vector3 newScale = new Vector3(width / 10, 0, height / 10);
        for (int i = 0; i < groundAndRoof.Count; i++)
        {
            groundAndRoof[i].position = new Vector3(newPos.x, groundAndRoof[i].position.y, newPos.z);
            groundAndRoof[i].localScale = new Vector3(newScale.x, groundAndRoof[i].localScale.y, newScale.z);
        }
    }

    private void GenerateWall(WallState node, WallState wallState, float x, float y, Vector2 offset, float yRot, bool ignoreValue)
    {
        if (node.HasFlag(wallState) && !ignoreValue)
        {
            if (wallPool.Count == 0)
            {
                AddWallsToPool(5);
            }

            Transform wall = wallPool[0];
            wallPool.RemoveAt(0);

            wall.gameObject.SetActive(true);
            wall.position = new Vector3(x - offset.x, 0, y - offset.y);
            wall.eulerAngles = new Vector3(0, yRot, 0);
            activeWalls.Add(wall);
        }
    }

    private void AddWallsToPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform newWall = Instantiate(wallPrefab);
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
