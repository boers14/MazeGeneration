using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameButton : SwitchActiveStateButton
{
    [SerializeField]
    private int widthGameMaze = 50, heightGameMaze = 30;

    public override void Awake()
    {
        base.Awake();
        GetComponent<Button>().onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        MazeRenderer.instance.StartGenerateMaze(widthGameMaze, heightGameMaze);
    }
}
