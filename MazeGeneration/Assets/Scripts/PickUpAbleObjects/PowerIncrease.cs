using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerIncrease : PickupAbleObject
{
    [SerializeField]
    private float extraGrenadeDamage = 2, extraBulletDamage = 3.5f;

    private PlayerShooting player = null;

    public override void Instantiate()
    {
        base.Instantiate();
        player = FindObjectOfType<PlayerShooting>();
    }

    public override void GrantEffect()
    {
        PlayerGrenadePool.instance.IncreaseGrenadePower(extraGrenadeDamage);
        player.IncreasePower(extraBulletDamage);
    }
}
