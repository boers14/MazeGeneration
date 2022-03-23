using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : CountingText
{
    public static ScoreManager instance = null;

    private void Start()
    {
        if(!instance)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }
    }
}
