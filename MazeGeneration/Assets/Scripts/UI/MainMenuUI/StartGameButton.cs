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

    [SerializeField]
    private SetPostProcessingSize postProcessingPrefab = null;

    [SerializeField]
    private EnvironmentParticles environmentParticlesPrefab = null;

    private GameObject currentPlayer = null;

    private SetPostProcessingSize currentPostProcessing = null;

    private EnvironmentParticles currentEnvironmentParticles = null;

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
        MazeRenderer.instance.RemoveDoubleWalls();
        PathfindingGrid.instance.StartCreateGrid(widthGameMaze, heightGameMaze);
        ScoreManager.instance.EnableValueText(true);
        EnemyCounter.instance.EnableValueText(true);
        if (!currentPlayer)
        {
            currentPostProcessing = Instantiate(postProcessingPrefab);
            currentPostProcessing.SetPostProcessingBoxSize(widthGameMaze, heightGameMaze);

            currentEnvironmentParticles = Instantiate(environmentParticlesPrefab);

            currentPlayer = Instantiate(playerPrefab);
            currentPlayer.transform.position = new Vector3(MazeRenderer.instance.centerMazePos.x, 
                currentPlayer.transform.localScale.y, MazeRenderer.instance.centerMazePos.z);
            camera.SetCameraPosToPlayer();
        }
    }
}
