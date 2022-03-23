using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoIncrease : PickupAbleObject
{
    [SerializeField]
    private float addedGrenades = 5, addedBullets = 10;

    private PlayerShooting player = null;

    public override void Instantiate()
    {
        base.Instantiate();
        player = FindObjectOfType<PlayerShooting>();
    }

    public override void GrantEffect()
    {
        PlayerGrenadePool.instance.AddGrenades(addedGrenades);
        player.AddBullets(addedBullets);
    }
}
