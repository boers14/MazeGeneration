using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCounter : CountingText
{
    public static BulletCounter instance = null;

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
