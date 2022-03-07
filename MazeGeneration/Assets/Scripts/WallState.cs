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
    Down = 8
}
