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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" && enemy.canDealDamage)
        {
            print("dealt damage");
            collision.transform.GetComponent<PlayerMovement>().health -= enemy.attackPower;
        }
    }
}
