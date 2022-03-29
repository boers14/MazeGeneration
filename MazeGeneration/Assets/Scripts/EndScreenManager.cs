using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreenManager : MonoBehaviour
{
    // Is singleton
    // TODO: Make singleton class where all singletons can extend from
    public static EndScreenManager instance = null;

    [System.NonSerialized]
    public bool isInGame = false, isOnEndScreen = false;

    [SerializeField]
    private StartGameButton startGameButton = null;

    [SerializeField]
    private GameObject mainMenu = null, endScreen = null;

    [SerializeField]
    private TMP_Text scoreText = null, highScoreText = null, currentHighScoreText = null;

    [SerializeField]
    private List<CountingText> countingTexts = new List<CountingText>();

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

    // Perform different actions based on which menu the player is on
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isInGame)
            {
                // End current run
                //EndRun();
            } else if (isOnEndScreen)
            {
                // Go to main menu
                SwitchToMainMenu();
            } else
            {
                // Quit game on main menu or maze generation
                Application.Quit();
            }
        }

        // Delete current highscore
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveSytem.DeleteGame();
        }
    }

    // Turn off all current game effects and reset all game managers
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
        for (int i = 0; i < countingTexts.Count; i++)
        {
            countingTexts[i].ResetValue();
        }

        EnemyManager.instance.ReturnAllObjectsToPool();
        EnemyManager.instance.GetComponent<EnemyManager>().StopGeneratingEnemys();

        PickUpSpawner.instance.ReturnAllObjectsToPool();
        PickUpSpawner.instance.StopGeneratingPickups();

        // Set end screen values
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

    // Go to main menu screen
    private void SwitchToMainMenu()
    {
        isOnEndScreen = false;
        MazeRenderer.instance.ClearMaze();
        endScreen.SetActive(false);
        mainMenu.SetActive(true);
    }
}
