using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCounter : CountingText
{
    // Is singleton
    public static GrenadeCounter instance = null;

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
