using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : ObjectPool
{
    // Is singleton
    public static EnemyManager instance = null;

    [SerializeField]
    private float enemysCreatedPerWave = 10, enemysAddedPerWave = 2, waveTimer = 60, baseEnemyAttackPower = 10,
        enemyAttackIncreasePerWave = 2, baseEnemyHealth = 50, enemyHealthIncreasePerWave = 10, minRangeFromPlayer = 5, 
        maxRangeFromPlayer = 25, baseAddedScore = 100, scoreAddedPerWave = 50, runSpeedAddedPerWave = 0.035f;

    private float currentWave = 0, baseRunSpeed = 0;

    private Transform player = null;

    private bool firstRun = true;

    private AudioSource newWaveSound = null;

    public override void Start()
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

        baseRunSpeed = objectForPool.GetComponent<Enemy>().runSpeed;
    }

    // Find the player and start generating enemys
    public void FindPlayerObject()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        newWaveSound = player.GetComponent<AudioSource>();
        if (firstRun)
        {
            firstRun = false;
            base.Start();
        }

        // Start generating pick-ups
        StartCoroutine(PickUpSpawner.instance.SpawnPickUps());
        StartCoroutine(CreateNewWaveOfEnemys());
    }

    // Calculate new stats for enemys and create a new position for the enemy
    private IEnumerator CreateNewWaveOfEnemys()
    {
        newWaveSound.Play();
        float amountOfEnemys = enemysCreatedPerWave + (enemysAddedPerWave * currentWave);
        float attackPowerEnemys = baseEnemyAttackPower + (enemyAttackIncreasePerWave * currentWave);
        float healthEnemys = baseEnemyHealth + (enemyHealthIncreasePerWave * currentWave);
        float scoreEnemys = baseAddedScore + (scoreAddedPerWave * currentWave);
        float speedEnemys = baseRunSpeed + (runSpeedAddedPerWave * currentWave);

        for (int i = 0; i < amountOfEnemys; i++)
        {
            if (objectPool.Count == 0) { AddObjectsToPool(5); }

            Enemy newEnemy = RetrieveObjectFromPool().GetComponent<Enemy>();

            newEnemy.health = healthEnemys;
            newEnemy.attackPower = attackPowerEnemys;
            newEnemy.transform.position = CreatePositionForEnemy();
            newEnemy.isDead = false;
            newEnemy.addedScore = scoreEnemys;
            newEnemy.runSpeed = speedEnemys;
            newEnemy.player = player;
            // Search for player
            newEnemy.RelocatePlayer();
        }

        // Update enemy counter
        EnemyCounter.instance.UpdateValue((int)amountOfEnemys);

        currentWave++;
        yield return new WaitForSeconds(waveTimer);

        StartCoroutine(CreateNewWaveOfEnemys());
    }

    // Make sure the position for the enemy is within bounds and outside of player range
    public Vector3 CreatePositionForEnemy()
    {
        PathfindingGrid grid = PathfindingGrid.instance;
        Vector3 enemyPos = ReturnRandomPos();
        while (Vector3.Distance(enemyPos, player.position) < minRangeFromPlayer || enemyPos.x < grid.startX || enemyPos.x > grid.endX
            || enemyPos.z < grid.startZ || enemyPos.z > grid.endZ)
        {
            enemyPos = ReturnRandomPos();
        }

        return enemyPos;
    }

    // Create a random position for the enemy
    private Vector3 ReturnRandomPos()
    {
        return new Vector3(ReturnRandomPosPart(player.position.x), -1, ReturnRandomPosPart(player.position.z));
    }

    // Create random float for pos based on player pos
    private float ReturnRandomPosPart(float playerPos)
    {
        return Random.Range(playerPos - maxRangeFromPlayer, playerPos + maxRangeFromPlayer);
    }

    // Stop creating enemys
    public void StopGeneratingEnemys()
    {
        currentWave = 0;
        StopAllCoroutines();
    }
}
