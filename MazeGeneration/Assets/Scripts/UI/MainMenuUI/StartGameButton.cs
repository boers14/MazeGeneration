using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameButton : SwitchActiveStateButton
{
    [SerializeField]
    private int widthGameMaze = 50, heightGameMaze = 30;

    [SerializeField]
    private GameObject playerPrefab = null;

    private GameObject currentPlayer = null;

    private new PlayerCamera camera = null;

    public override void Awake()
    {
        base.Awake();
        GetComponent<Button>().onClick.AddListener(StartGame);
        camera = Camera.main.GetComponent<PlayerCamera>();
    }

    private void StartGame()
    {
        MazeRenderer.instance.StartGenerateMaze(widthGameMaze, heightGameMaze);
        if (!currentPlayer)
        {
            currentPlayer = Instantiate(playerPrefab);
            currentPlayer.transform.position = new Vector3(MazeRenderer.instance.centerMazePos.x, 
                currentPlayer.transform.localScale.y, MazeRenderer.instance.centerMazePos.z);
            camera.SetCameraPosToPlayer();
        }
    }
}
