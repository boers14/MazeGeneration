using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerateMazeButton : SetUIStats
{
    [SerializeField]
    private TMP_InputField widthInput = null, lenghtInput = null;

    public override void Start()
    {
        base.Start();
        GetComponent<Button>().onClick.AddListener(GenerateMaze);
    }

    private void GenerateMaze()
    {
        MazeRenderer.instance.StartGenerateMaze(int.Parse(widthInput.text), int.Parse(lenghtInput.text));
    }
}
