using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallState
{
    public enum Direction {
        None,
        Left,
        Right,
        Up,
        Down,
    }

    public bool visited = false;
    public Direction direction = Direction.None;
}
