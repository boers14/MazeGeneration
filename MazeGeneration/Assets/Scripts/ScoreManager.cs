using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : CountingText
{
    public static ScoreManager instance = null;

    [SerializeField]
    private GameObject scoreUI = null;

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

    public override void EnableValueText(bool enabled)
    {
        base.EnableValueText(enabled);
        scoreUI.SetActive(enabled);
    }
}
