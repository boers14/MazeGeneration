using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegain : PickupAbleObject
{
    [SerializeField]
    private float HealthRestore = 50, permanentHealthIncrease = 10;

    // Restore/ increase health when grabbed by player
    public override void GrantEffect()
    {
        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        player.maxHealth += permanentHealthIncrease;
        player.ChangeHealth(HealthRestore + PickUpSpawner.instance.amountOfHealthIncreased / 2);
        PickUpSpawner.instance.amountOfHealthIncreased += permanentHealthIncrease;
    }
}
