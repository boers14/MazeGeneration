using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPostProcessingSize : MonoBehaviour
{
    private PostProcessVolume postProcessing = null;

    private void Awake()
    {
        postProcessing = GetComponent<PostProcessVolume>();
    }

    // Set size and position of the post processing effect
    public void SetPostProcessingBoxSize(int width, int height)
    {
        transform.position = MazeRenderer.instance.centerMazePos;
        Vector3 boxSize = new Vector3(GetBoxSizeDirection(width), 5, GetBoxSizeDirection(height));

        postProcessing.OuterBoxSize = boxSize;
        postProcessing.InnerBoxSize = boxSize;

        GetComponent<BoxCollider>().size = boxSize;
    }

    // Calculate size of post processing based on size of the maze
    private float GetBoxSizeDirection(float value)
    {
        return value * MazeRenderer.instance.mazeWallSize + 3f;
    }
}
