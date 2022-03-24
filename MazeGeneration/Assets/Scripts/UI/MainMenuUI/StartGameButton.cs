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

    // Spawn all required objects for the game, generate a maze, spawn grid for enemys to use, reset score manager
    private void StartGame()
    {
        EndScreenManager.instance.isInGame = true;
        ScoreManager.instance.ResetValue();
        MazeRenderer.instance.StartGenerateMaze(widthGameMaze, heightGameMaze);
        MazeRenderer.instance.RemoveDoubleWalls();
        PathfindingGrid.instance.StartCreateGrid(widthGameMaze, heightGameMaze);
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

    // Destroys created objects for start run and sets camera back to defualt stats
    public void DestroyCurrentRun()
    {
        camera.transform.SetParent(null);
        RenderSettings.fog = false;
        camera.enabled = false;
        camera.GetComponent<FlyCamera>().SetZoom(widthGameMaze, heightGameMaze);

        Destroy(currentPostProcessing.gameObject);
        Destroy(currentEnvironmentParticles.gameObject);
        Destroy(currentPlayer);
    }
}
