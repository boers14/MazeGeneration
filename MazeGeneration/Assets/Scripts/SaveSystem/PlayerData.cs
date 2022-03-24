using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int highScore = 0;

    public PlayerData(int highScore)
    {
        this.highScore = highScore;
    }
}
