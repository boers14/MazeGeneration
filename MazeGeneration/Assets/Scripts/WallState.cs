using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum WallState
{
    Left = 1,
    Right = 2,
    Up = 4,
    Down = 8,

    Visited= 128
}

public struct Position
{
    public int X;
    public int Y;
}

public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}
