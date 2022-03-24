using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : CountingText
{
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

        if (SaveSytem.CheckIfFileExist())
        {
            PlayerData data = SaveSytem.LoadGame();
            highScore = data.highScore;
        }
    }

    public bool CheckIfNewHighScore()
    {
        if (value > highScore)
        {
            SaveSytem.SaveGame();
            highScore = value;
            return true;
        }

        return false;
    }
}
