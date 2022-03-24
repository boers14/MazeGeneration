using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    private Vector2 canvasSize = Vector2.zero;

    [SerializeField]
    private float moveSpeed = 0.1f;

    private void Start()
    {
        canvasSize = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta;
    }

    // Move cam based on input
    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(-transform.forward * moveSpeed);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(transform.forward * moveSpeed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-transform.right * moveSpeed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(transform.right * moveSpeed);
        }

        if (Input.mouseScrollDelta != Vector2.zero)
        {
            transform.Translate(transform.up * Input.mouseScrollDelta.y);
        }
    }

    // Set camera pos above maze so the entire maze in visible
    public void SetZoom(int width, int lenght)
    {
        if(canvasSize == Vector2.zero) { Start(); }

        float widthZoom = ReturnZoom(canvasSize.x, width);
        float lenghtZoom = ReturnZoom(canvasSize.y, lenght);

        float neededZoom = widthZoom > lenghtZoom ? widthZoom : lenghtZoom;
        transform.position = new Vector3(MazeRenderer.instance.centerMazePos.x, neededZoom, MazeRenderer.instance.centerMazePos.z);
        transform.eulerAngles = new Vector3(90, 0, 0);
    }

    // Calculate position from maze based on the maze wall size/ canvas size/ maze size
    private float ReturnZoom(float canvasSize, float direction)
    {
        return canvasSize * (direction / canvasSize) * MazeRenderer.instance.mazeWallSize;
    }
}
