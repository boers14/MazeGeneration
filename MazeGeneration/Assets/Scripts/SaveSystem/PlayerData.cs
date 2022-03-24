using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int highScore = 0;

    // Save the highscore
    public PlayerData(int highScore)
    {
        this.highScore = highScore;
    }
}
