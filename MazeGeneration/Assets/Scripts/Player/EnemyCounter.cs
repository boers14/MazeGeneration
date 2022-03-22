using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyCounter : CountingText
{
    public static EnemyCounter instance = null;

    private void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
