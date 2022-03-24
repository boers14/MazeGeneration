using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreenManager : MonoBehaviour
{
    public static EndScreenManager instance = null;

    [System.NonSerialized]
    public bool isInGame = false, isOnEndScreen = false;

    [SerializeField]
    private StartGameButton startGameButton = null;

    [SerializeField]
    private GameObject mainMenu = null, endScreen = null;

    [SerializeField]
    private TMP_Text scoreText = null, highScoreText = null, currentHighScoreText = null;

    private void Start()
    {
        if (!instance)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isInGame)
            {
                EndRun();
            } else if (isOnEndScreen)
            {
                SwitchToMainMenu();
            } else
            {
                Application.Quit();
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveSytem.DeleteGame();
        }
    }

    public void EndRun()
    {
        isInGame = false;
        isOnEndScreen = true;

        Cursor.lockState = CursorLockMode.None;
        PlayerGrenadePool.instance.ReturnAllObjectsToPool();
        for (int i = 0; i < PlayerGrenadePool.instance.objectPool.Count; i++)
        {
            Destroy(PlayerGrenadePool.instance.objectPool[i].gameObject);
        }

        startGameButton.DestroyCurrentRun();
        PlayerUIHandler.instance.completeInGameUI.SetActive(false);
        EnemyCounter.instance.ResetValue();
        BulletCounter.instance.ResetValue();
        GrenadeCounter.instance.ResetValue();

        EnemyManager.instance.ReturnAllObjectsToPool();
        EnemyManager.instance.StopGeneratingEnemys();

        PickUpSpawner.instance.ReturnAllObjectsToPool();
        PickUpSpawner.instance.StopGeneratingPickups();

        endScreen.SetActive(true);
        if (ScoreManager.instance.CheckIfNewHighScore())
        {
            highScoreText.enabled = true;
        } else
        {
            highScoreText.enabled = false;
        }

        currentHighScoreText.text = "Current highscore: " + ScoreManager.instance.highScore;
        scoreText.text = "Final score: " + ScoreManager.instance.value;
    }

    private void SwitchToMainMenu()
    {
        isOnEndScreen = false;
        MazeRenderer.instance.ClearMaze();
        endScreen.SetActive(false);
        mainMenu.SetActive(true);
    }
}
