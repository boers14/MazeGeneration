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

    // Check if the value of the text is within the range
    private void CheckIfValidNumber(string text)
    {
        int value = int.Parse(text);
        CheckValue(value < minimumValue, minimumValue);
        CheckValue(value > maximumValue, maximumValue);
    }

    // Edit the value if it is not range
    private void CheckValue(bool invalidValue, int newValue)
    {
        if (invalidValue)
        {
            inputField.text = newValue.ToString();
        }
    }
}
