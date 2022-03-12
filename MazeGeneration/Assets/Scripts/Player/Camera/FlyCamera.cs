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

    public void SetZoom(int width, int lenght)
    {
        float widthZoom = ReturnZoom(canvasSize.x, width);
        float lenghtZoom = ReturnZoom(canvasSize.y, lenght);

        float neededZoom = widthZoom > lenghtZoom ? widthZoom : lenghtZoom;
        transform.position = new Vector3(0, neededZoom, 0);
        transform.eulerAngles = new Vector3(90, 0, 0);
    }

    private float ReturnZoom(float canvasSize, float direction)
    {
        return canvasSize * (direction / canvasSize) * 0.8f;
    }
}
