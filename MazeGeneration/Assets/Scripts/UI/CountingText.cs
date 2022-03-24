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

    // Update the value and start tween for the text number value
    public void UpdateValue(int addedValue)
    {
        iTween.Stop(gameObject);
        startTweenValue = currentShowingValue;
        value += addedValue;
        // Make sure time is positive
        float time = (float)(value - startTweenValue) / valueCounterSpeed;
        if (time < 0)
        {
            time *= -1;
        }

        iTween.ValueTo(gameObject, iTween.Hash("from", startTweenValue, "to", value, "time", time, "onupdate", "UpdateValueText"));
    }

    // Update current show value and set text equal to show that
    private void UpdateValueText(int val)
    {
        currentShowingValue = val;
        text.text = baseText + currentShowingValue;
    }

    // Reset value and text to 0
    public void ResetValue()
    {
        value = 0;
        text.text = baseText + value;
    }
}
