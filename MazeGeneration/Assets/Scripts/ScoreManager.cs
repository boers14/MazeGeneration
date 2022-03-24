using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : CountingText
{
    // Is singleton
    public static ScoreManager instance = null;

    public int highScore { private set; get; } = 0;

    private void Start()
    {
        if(!instance)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }

        // Grab highscore from saved data if its there
        if (SaveSytem.CheckIfFileExist())
        {
            PlayerData data = SaveSytem.LoadGame();
            highScore = data.highScore;
        }
    }

    // Check and set new highscore for game 
    public bool CheckIfNewHighScore()
    {
        if (value > highScore)
        {
            // Only saves game if there is a new highscore
            SaveSytem.SaveGame();
            highScore = value;
            return true;
        }

        return false;
    }
}
