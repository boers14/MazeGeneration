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

        Vector3 removedWallPos = Vector3.zero;
        int wallNumber = 0;
        int outerSide = 0;
        bool widthOuterWall = false;
        Vector2Int randomOuterWall = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

        if ((int)Random.Range(1, 11) % 2 == 0)
        {
            widthOuterWall = true;
            outerSide = ReturnOuterSide(outerSide, height - 1);
            wallNumber = ReturnWallNumber(randomOuterWall.x, outerSide, width - 1, height - 1);
        } else
        {
            outerSide = ReturnOuterSide(outerSide, width - 1);
            wallNumber = ReturnWallNumber(randomOuterWall.y, outerSide, height - 1, width - 1);
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                WallState node = walls[i, j];

                bool createWall = true;
                if (widthOuterWall)
                {
                    if (j == outerSide && i == wallNumber)
                    {
                        createWall = false;
                        removedWallPos = ReturnRemovedWallPos(outerSide, i, j, new Vector3(0.5f, 0, 0), new Vector3(-0.5f, 0, 0));
                    }
                } else
                {
                    if (i == outerSide && j == wallNumber)
                    {
                        createWall = false;
                        removedWallPos = ReturnRemovedWallPos(outerSide, i, j, new Vector3(0, 0, 0.5f), new Vector3(0, 0, 0));
                    }
                }

                if (!widthOuterWall || createWall)
                {
                    GenerateWall(node, WallState.Right, i, j, 0, i == width - 1, new Vector3(0.5f, 0, 0), j == 0);
                    GenerateWall(node, WallState.Left, i, j, 180, i == 0 || j < height - 1, new Vector3(-0.5f, 0, 0), j == height - 1);
                }

                if (widthOuterWall || createWall)
                {
                    GenerateWall(node, WallState.Down, i, j, 90, j == 0 || i < width - 1, new Vector3(0, 0, -0.5f), i == width - 1);
                    GenerateWall(node, WallState.Up, i, j, 270, j == height - 1, new Vector3(0, 0, 0.5f), i == 0);
                }
            }
        }

        Vector3 newPos = new Vector3(-0.5f, 0, -0.5f);
        Vector3 newScale = new Vector3(width / 10, 0, height / 10);
        for (int i = 0; i < groundAndRoof.Count; i++)
        {
            groundAndRoof[i].position = new Vector3(newPos.x, groundAndRoof[i].position.y, newPos.z);
            groundAndRoof[i].localScale = new Vector3(newScale.x, groundAndRoof[i].localScale.y, newScale.z);
        }

        Transform wallToRemove = activeWalls.Find(wall => wall.transform.position == removedWallPos);
        if (wallToRemove)
        {
            print("hi");
            wallToRemove.gameObject.SetActive(false);
        }
    }

    private int ReturnWallNumber(int wallValue, int outerSide, int maxValueWall, int maxValueOuterSide)
    {
        if (outerSide == 0 && wallValue == maxValueWall)
        {
            wallValue -= 1;
        } else if (outerSide == maxValueOuterSide && wallValue == 0)
        {
            wallValue += 1;
        }

        return wallValue;
    }

    private int ReturnOuterSide(int outerSide, int valueToChangeTo)
    {
        if ((int)Random.Range(1, 11) % 2 == 0)
        {
            outerSide = valueToChangeTo;
        }

        return outerSide;
    }

    private Vector3 ReturnRemovedWallPos(int outerSide, int x, int y, Vector3 positiveOffset, Vector3 negativeOffset)
    {
        Vector3 basePos = new Vector3(-width / 2 + x, 0, -height / 2 + y);
        if (outerSide == 0)
        {
            return basePos + positiveOffset;
        } else
        {
            return basePos + negativeOffset;
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
