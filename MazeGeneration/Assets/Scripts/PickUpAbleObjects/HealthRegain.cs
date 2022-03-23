using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegain : PickupAbleObject
{
    [SerializeField]
    private float HealthRestore = 50, permanentHealthIncrease = 10;

    private PlayerHealth player = null;

    public override void Instantiate()
    {
        base.Instantiate();
        player = FindObjectOfType<PlayerHealth>();
    }

    public override void GrantEffect()
    {
        player.maxHealth += permanentHealthIncrease;
        player.ChangeHealth(HealthRestore + PickUpSpawner.instance.amountOfHealthIncreased / 2);
        PickUpSpawner.instance.amountOfHealthIncreased += permanentHealthIncrease;
    }
}
