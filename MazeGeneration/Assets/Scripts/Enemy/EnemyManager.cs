using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : ObjectPool
{
    public static EnemyManager instance = null;

    [SerializeField]
    private float enemysCreatedPerWave = 10, enemysAddedPerWave = 2, waveTimer = 60, baseEnemyAttackPower = 10,
        enemyAttackIncreasePerWave = 2, baseEnemyHealth = 50, enemyHealthIncreasePerWave = 10;

    private float currentWave = 0;

    private Transform player = null;

    public override void Start()
    {
        if (!instance)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void FindPlayerObject()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        base.Start();
        StartCoroutine(CreateNewWaveOfEnemys());
    }

    private IEnumerator CreateNewWaveOfEnemys()
    {
        float amountOfEnemys = enemysCreatedPerWave + (enemysAddedPerWave * currentWave);
        float attackPowerEnemys = baseEnemyAttackPower + (enemyAttackIncreasePerWave * currentWave);
        float healthEnemys = baseEnemyHealth + (enemyHealthIncreasePerWave * currentWave);

        for (int i = 0; i < amountOfEnemys; i++)
        {
            if (objectPool.Count == 0) { AddObjectsToPool(5); }

            Enemy newEnemy = objectPool[0].GetComponent<Enemy>();
            newEnemy.gameObject.SetActive(true);
            newEnemy.health = healthEnemys;
            newEnemy.attackPower = attackPowerEnemys;
            newEnemy.transform.position = CreatePositionForEnemy();
            newEnemy.isDead = false;
            newEnemy.RelocatePlayer();

            activeObjects.Add(newEnemy.transform);
            objectPool.Remove(newEnemy.transform);
        }

        currentWave++;
        yield return new WaitForSeconds(waveTimer);

        StartCoroutine(CreateNewWaveOfEnemys());
    }

    private Vector3 CreatePositionForEnemy()
    {
        PathfindingGrid grid = PathfindingGrid.instance;
        Vector3 enemyPos = new Vector3(Random.Range(grid.startX, grid.endX), -1, Random.Range(grid.startZ, grid.endZ));
        while (Vector3.Distance(enemyPos, player.position) < 5)
        {
            enemyPos = new Vector3(Random.Range(grid.startX, grid.endX), -1, Random.Range(grid.startZ, grid.endZ));
        }

        return enemyPos;
    }
}
