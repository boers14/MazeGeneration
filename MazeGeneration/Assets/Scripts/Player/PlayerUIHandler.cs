using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    // Set the values of given objects so they can be easily acceseble for newly created objects
    public Image greenHealth = null, animatedHealth = null, staminaFill = null;

    public GameObject completeInGameUI = null, completeStaminaUI = null;

    // Is singleton
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
