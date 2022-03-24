using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListing : MonoBehaviour
{
    [SerializeField]
    private List<SetUIStats> uiList = new List<SetUIStats>();

    [SerializeField, Range(0, 1)]
    private float distanceBetweenUIObjects = 0.025f;

    [SerializeField]
    private bool moveDown = true;

    // Places all objects in list in order down or up
    // All objects need same position to correctly work
    // TODO: Use position of first object as starting pos for all objects
    private void Start()
    {
        float movementDistance = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta.y * distanceBetweenUIObjects;
        float totalYDistance = 0;
        for (int i = 0; i < uiList.Count; i++)
        {
            uiList[i].SetUIPosition(true, totalYDistance);
            if (moveDown)
            {
                totalYDistance += uiList[i].size.y / 2 + movementDistance;
            } else
            {
                totalYDistance -= uiList[i].size.y / 2 + movementDistance;
            }
        }
    }
}
