using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisableAutoSize : MonoBehaviour
{
    public virtual void Start()
    {
        StartCoroutine(StopAutoSize());
    }

    private IEnumerator StopAutoSize()
    {
        yield return new WaitForEndOfFrame();
        TMP_Text text = GetComponent<TMP_Text>();
        if (text)
        {
            float bestFontSize = 0;
            text.ForceMeshUpdate();
            bestFontSize = text.fontSize;
            text.enableAutoSizing = false;
            text.fontSize = bestFontSize;
        }
    }
}
