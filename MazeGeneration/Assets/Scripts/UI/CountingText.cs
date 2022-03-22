using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountingText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text = null;

    [SerializeField]
    private float valueCounterSpeed = 500;

    [SerializeField]
    private string baseText = "";

    public int value { get; private set; } = 0;

    private int currentShowingValue = 0, startTweenValue = 0;

    public void UpdateValue(int addedValue)
    {
        iTween.Stop(gameObject);
        startTweenValue = currentShowingValue;
        value += addedValue;
        float time = (float)(value - startTweenValue) / valueCounterSpeed;
        if (time < 0)
        {
            time *= -1;
        }

        iTween.ValueTo(gameObject, iTween.Hash("from", startTweenValue, "to", value, "time", time, "onupdate", "UpdateValueText"));
    }

    private void UpdateValueText(int val)
    {
        currentShowingValue = val;
        text.text = baseText + currentShowingValue;
    }

    public void ResetValue()
    {
        value = 0;
        text.text = baseText + value;
    }

    public virtual void EnableValueText(bool enabled)
    {
        text.enabled = enabled;
        text.enableAutoSizing = true;
    }
}
