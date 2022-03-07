using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MazeGenerator
{
    public static WallState[,] GenerateMaze(int width, int height)
    {
        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.Down | WallState.Left | WallState.Right | WallState.Up;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = initial;
            }
        }

        return maze;
    } 
}
