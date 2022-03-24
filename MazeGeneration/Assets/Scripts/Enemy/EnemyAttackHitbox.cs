using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private Enemy enemy = null;

    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    // Enemy deals damage when colliding with player
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" && enemy.canDealDamage)
        {
            collision.transform.GetComponent<PlayerHealth>().ChangeHealth(-enemy.attackPower);
            // Can only deal damage once per attack
            enemy.canDealDamage = false;
        }
    }
}
