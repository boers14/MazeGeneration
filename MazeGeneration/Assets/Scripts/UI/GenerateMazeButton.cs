using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerateMazeButton : SetUIStats
{
    private new FlyCamera camera = null;

    [SerializeField]
    private TMP_InputField widthInput = null, lenghtInput = null;

    public override void Awake()
    {
        base.Awake();
        GetComponent<Button>().onClick.AddListener(GenerateMaze);
        camera = Camera.main.GetComponent<FlyCamera>();
    }

    private void GenerateMaze()
    {
        int width = int.Parse(widthInput.text);
        int lenght = int.Parse(lenghtInput.text);

        MazeRenderer.instance.StartGenerateMaze(width, lenght);
        camera.SetZoom(width, lenght);
    }
}
