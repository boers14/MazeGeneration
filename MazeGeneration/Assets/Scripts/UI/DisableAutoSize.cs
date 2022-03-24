using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisableAutoSize : MonoBehaviour
{
    [SerializeField]
    private bool stopAutoSize = true;

    public virtual void Awake()
    {
        StartCoroutine(StopAutoSize());
    }

    // Stops auto resize of text of after setting correct font size
    private IEnumerator StopAutoSize()
    {
        yield return new WaitForEndOfFrame();
        TMP_Text text = GetComponent<TMP_Text>();
        if (text && stopAutoSize)
        {
            float bestFontSize = 0;
            text.ForceMeshUpdate();
            bestFontSize = text.fontSize;
            text.enableAutoSizing = false;
            text.fontSize = bestFontSize;
        }
    }
}
