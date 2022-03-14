using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MazeGenerationInputField : MonoBehaviour
{
    [SerializeField]
    private int minimumValue = 10, maximumValue = 250;

    private TMP_InputField inputField = null;

    private void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onEndEdit.AddListener(CheckIfValidNumber);
    }

    private void CheckIfValidNumber(string text)
    {
        int value = int.Parse(text);
        CheckValue(value < minimumValue, minimumValue);
        CheckValue(value > maximumValue, maximumValue);
    }

    private void CheckValue(bool invalidValue, int newValue)
    {
        if (invalidValue)
        {
            inputField.text = newValue.ToString();
        }
    }
}
