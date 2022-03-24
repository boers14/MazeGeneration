using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerIncrease : PickupAbleObject
{
    [SerializeField]
    private float extraGrenadeDamage = 2, extraBulletDamage = 3.5f;

    // Increase damage when collide with player
    public override void GrantEffect()
    {
        PlayerGrenadePool.instance.IncreaseGrenadePower(extraGrenadeDamage);
        FindObjectOfType<PlayerShooting>().IncreasePower(extraBulletDamage);
    }
}
