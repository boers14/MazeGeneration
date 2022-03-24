using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetUIStats : DisableAutoSize
{
    // size based on canvas 0.1 = 10% of canvas, 0.5 = 50% of canvas
    [SerializeField, Range(0, 1)]
    private float xSize = 0.5f, ySize = 0.5f;

    // Placement of object based on size delta of object
    [SerializeField, Range(-1, 1)]
    private float yAnchor = 0, xAnchor = 0;

    // Position on screen 0.5 = positive outer of canvas, -0.5 is negative outer of canvas
    [SerializeField, Range(-0.5f, 0.5f)]
    private float xPos = 0.5f, yPos = 0.5f;

    [SerializeField]
    private bool iterateThroughtChilds = false, isInList = false;

    private Canvas canvas = null;

    private RectTransform rectTransform = null, canvasRectTransform = null;

    [System.NonSerialized]
    public Vector2 size = Vector2.zero;

    // Only set object pos if its not in a UIList
    public override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();

        if (!isInList)
        {
            SetUIPosition();
        }
    }

    // Calculate the position of UI based on canvas size and given values
    public void SetUIPosition(bool moveVertically = false, float totalYMovement = 0)
    {
        Vector2 originalSize = rectTransform.sizeDelta;

        size = Vector2.zero;
        size.y = canvasRectTransform.sizeDelta.y * ySize;
        size.x = ReturnXSize(originalSize, size.y, canvasRectTransform.sizeDelta.x, xSize);
        rectTransform.sizeDelta = size;

        // Calculate position/ size of childeren
        if (iterateThroughtChilds)
        {
            LoopTroughtChilds(transform, size, originalSize);
        }

        Vector3 pos = Vector3.zero;

        // If is in list move extra down based on given value
        if (moveVertically)
        {
            pos.y = (canvasRectTransform.sizeDelta.y * yPos) + (size.y / 2 * yAnchor) - totalYMovement;
        }
        else
        {
            pos.y = (canvasRectTransform.sizeDelta.y * yPos) + (size.y / 2 * yAnchor);
        }

        pos.x = (canvasRectTransform.sizeDelta.x * xPos) + (size.x / 2 * xAnchor);

        rectTransform.localPosition = pos;
    }

    // Get all childs and childs of childs etc.
    private void LoopTroughtChilds(Transform uiObject, Vector2 size, Vector2 originalSize)
    {
        for (int i = 0; i < uiObject.childCount; i++)
        {
            SetComparitiveSize(uiObject.GetChild(i), size, originalSize);
            for (int j = 0; j < uiObject.GetChild(i).childCount; j++)
            {
                LoopTroughtChilds(uiObject.GetChild(i), size, originalSize);
            }
        }
    }

    // Set new size and position of child based on the percentual growth of the original object
    private void SetComparitiveSize(Transform uiObject, Vector2 size, Vector2 originalSize)
    {
        Vector2 decimalGrowth = Vector2.zero;
        decimalGrowth.x = size.x / originalSize.x;
        decimalGrowth.y = size.y / originalSize.y;
        RectTransform uiTransform = uiObject.GetComponent<RectTransform>();

        Vector2 newSize = Vector2.zero;
        newSize.y = uiTransform.sizeDelta.y * decimalGrowth.y;
        newSize.x = ReturnXSize(uiTransform.sizeDelta, newSize.y, uiTransform.sizeDelta.x, decimalGrowth.x);
        uiTransform.sizeDelta = newSize;

        Vector3 newPos = Vector3.zero;
        newPos.x = uiTransform.localPosition.x * decimalGrowth.x;
        newPos.y = uiTransform.localPosition.y * decimalGrowth.y;
        uiTransform.localPosition = newPos;
    }

    // Keep squared objects in square form
    private float ReturnXSize(Vector2 sizeDelta, float ySize, float xStart, float xGrowth)
    {
        if (sizeDelta.y == sizeDelta.x)
        {
            return ySize;
        }
        else
        {
            return xStart * xGrowth;
        }
    }
}
