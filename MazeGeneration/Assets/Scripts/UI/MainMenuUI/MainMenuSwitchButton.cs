using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSwitchButton : SwitchActiveStateButton
{
    [SerializeField]
    private bool mainMenuButton = true;

    private new FlyCamera camera = null;

    // For switching between maze generation menu/ main menu
    public override void Awake()
    {
        base.Awake();
        camera = Camera.main.GetComponent<FlyCamera>();
        GetComponent<Button>().onClick.AddListener(SwitchMenuState);
    }

    // Clear maze, set sky camera function the what the menu requires
    private void SwitchMenuState()
    {
        camera.enabled = mainMenuButton;
        MazeRenderer.instance.ClearMaze();
    }
}
