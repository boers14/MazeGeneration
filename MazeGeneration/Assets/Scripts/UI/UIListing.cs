using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListing : MonoBehaviour
{
    [SerializeField]
    private List<SetUIStats> uiList = new List<SetUIStats>();

    [SerializeField, Range(0, 1)]
    private float distanceBetweenUIObjects = 0.025f;

    private void Start()
    {
        float movementDistance = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta.y * distanceBetweenUIObjects;
        float totalYDistance = 0;
        for (int i = 0; i < uiList.Count; i++)
        {
            uiList[i].SetUIPosition(true, totalYDistance);
            totalYDistance += uiList[i].size.y / 2 + movementDistance;
        }
    }
}
