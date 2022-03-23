using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    public Image greenHealth = null, animatedHealth = null, staminaFill = null;

    public GameObject completeInGameUI = null, completeStaminaUI = null;

    public static PlayerUIHandler instance = null;

    private void Start()
    {
        if (!instance)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }
}
