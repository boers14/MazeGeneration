using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoIncrease : PickupAbleObject
{
    [SerializeField]
    private float addedGrenades = 5, addedBullets = 10;

    // Increase max ammo/ add grenades when collided with player
    public override void GrantEffect()
    {
        PlayerGrenadePool.instance.AddGrenades(addedGrenades);
        FindObjectOfType<PlayerShooting>().AddBullets(addedBullets);
    }
}
