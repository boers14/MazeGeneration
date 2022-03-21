using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Pathfinding
{
    private Transform player = null;

    [SerializeField]
    private float walkSpeed = 1f, runSpeed = 3f, relocatePlayerTimer = 3f, runRange = 4f, attackRange = 1f;

    [SerializeField]
    private Transform wallPrefab = null;

    private new Animation animation = null;

    private Vector3 nextPosition = Vector3.zero;

    private float yPos = 0;

    private int currentNodeCount = 0;

    private PathfindingNode lastUsedNode = null;

    [System.NonSerialized]
    public float health = 0, attackPower = 0;

    [System.NonSerialized]
    public bool isDead = true, canDealDamage = false;

    private bool isTweening = false, isInAttackAnim = false;

    private string attackAnim = "";

    public override void Awake()
    {
        base.Awake();
        animation = GetComponent<Animation>();
        player = FindObjectOfType<PlayerMovement>().transform;
        yPos = -wallPrefab.localScale.y / 2 - 0.015f;
    }

    private void FixedUpdate()
    {
        if (isDead || canDealDamage) { return; }

        if (Vector3.Distance(transform.position, player.position) <= runRange)
        {
            if (isTweening || isInAttackAnim)
            {
                StopAllCoroutines();
                lastUsedNode = null;
                isInAttackAnim = false;
                isTweening = false;
                iTween.Stop(gameObject);
                animation.Play("Run");
            }

            nextPosition = new Vector3(player.position.x, yPos, player.position.z);
            transform.position = Vector3.Lerp(transform.position, nextPosition, runSpeed);

            if (Vector3.Distance(transform.position, player.position) < attackRange && !canDealDamage)
            {
                StartCoroutine(DealDamage());
            }
        } else if (!isTweening)
        {
            RelocatePlayer();
        }

        transform.LookAt(nextPosition);
    }

    public override void GetFinalPath(PathfindingNode startNode, PathfindingNode endNode)
    {
        base.GetFinalPath(startNode, endNode);
        GetNextPositionNode();
    }

    private void GetNextPositionNode()
    {
        if (pathToPosition.Count > 0)
        {
            isTweening = true;
            currentNodeCount++;
            lastUsedNode = finalPath[finalPath.Count - currentNodeCount];

            nextPosition = new Vector3(pathToPosition[0].x, yPos, pathToPosition[0].z);
            iTween.MoveTo(gameObject, iTween.Hash("position", nextPosition, "time", walkSpeed, "easetype",
                iTween.EaseType.linear, "oncomplete", "GetNextPositionNode", "oncompletetarget", gameObject));
            pathToPosition.RemoveAt(0);
        } else if (Vector3.Distance(transform.position, player.position) > runRange)
        {
            RelocatePlayer();
        }
    }

    private IEnumerator LocatePlayer()
    {
        FindPath(transform.position, player.position, lastUsedNode);
        yield return new WaitForSeconds(relocatePlayerTimer);

        RelocatePlayer();
    }

    public void RelocatePlayer()
    {
        isInAttackAnim = false;
        animation.Play("Walk");
        currentNodeCount = 0;
        pathToPosition.Clear();
        StartCoroutine(LocatePlayer());
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        isDead = true;
        animation.Play("Death");
        yield return new WaitForSeconds(animation.GetClip("Death").length);

        EnemyManager.instance.activeObjects.Remove(transform);
        EnemyManager.instance.objectPool.Add(transform);
        gameObject.SetActive(false);
    }

    private IEnumerator DealDamage()
    {
        isInAttackAnim = true;
        int attackAnimation = Random.Range(1, 3);
        attackAnim = "Attack" + attackAnimation;
        animation.Play(attackAnim);

        canDealDamage = true;
        yield return new WaitForSeconds(animation.GetClip(attackAnim).length);
        canDealDamage = false;
    }
}
