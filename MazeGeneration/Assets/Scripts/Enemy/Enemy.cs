using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Pathfinding
{
    [System.NonSerialized]
    public Transform player = null;

    [SerializeField]
    private float walkSpeed = 1f, relocatePlayerTimer = 3f, runRange = 4f, attackRange = 1f;

    public float runSpeed = 3f;

    [SerializeField]
    private Transform wallPrefab = null;

    private new Animation animation = null;

    private Vector3 nextPosition = Vector3.zero;

    private float yPos = 0;

    private int currentNodeCount = 0;

    private PathfindingNode lastUsedNode = null;

    [System.NonSerialized]
    public float health = 0, attackPower = 0, addedScore = 0;

    [System.NonSerialized]
    public bool isDead = true, canDealDamage = false;

    private bool isTweening = false, isInAttackAnim = false, returnToWalkFromAttack = false;

    private AudioSource gruntNoice = null;

    // Set vars
    public override void Awake()
    {
        base.Awake();
        gruntNoice = GetComponent<AudioSource>();
        animation = GetComponent<Animation>();
        yPos = -wallPrefab.localScale.y / 2 - 0.015f;
    }

    // Can only update if not attacking and is alive
    private void FixedUpdate()
    {
        if (isDead || isInAttackAnim) { return; }

        // Check if player is in range
        float playerDist = Vector3.Distance(transform.position, player.position);
        if (playerDist <= runRange)
        {
            // Check if there is no wall between player and enemy
            if (Physics.Raycast(transform.position - transform.forward * 0.1f + new Vector3(0, -yPos / 2, 0), 
                (player.position - transform.position).normalized, out RaycastHit hit, playerDist, wallMask))
            {
                if (!isTweening)
                {
                    RelocatePlayer();
                }
                return;
            }

            // If coming from walk or attack animation, set vars for running
            if (isTweening || returnToWalkFromAttack)
            {
                if (!gruntNoice.isPlaying)
                {
                    gruntNoice.Play();
                }

                returnToWalkFromAttack = false;
                StopAllCoroutines();
                lastUsedNode = null;
                isTweening = false;
                iTween.Stop(gameObject);
                animation.Play("Run");
            }

            // update positions
            nextPosition = new Vector3(player.position.x, yPos, player.position.z);
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, runSpeed);

            // If player in range, attack
            if (playerDist < attackRange && !canDealDamage)
            {
                StartCoroutine(DealDamage());
            }
        } else if (!isTweening)
        {
            RelocatePlayer();
        }

        transform.LookAt(nextPosition);
    }

    // Get next position after having the final path
    public override void GetFinalPath(PathfindingNode startNode, PathfindingNode endNode)
    {
        base.GetFinalPath(startNode, endNode);
        GetNextPositionNode();
    }

    // Walk to next position node with tweens
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

            // Relocate player if not near player
        } else if (Vector3.Distance(transform.position, player.position) > runRange)
        {
            RelocatePlayer();
        }
    }

    // Locate the player every so often
    private IEnumerator LocatePlayer()
    {
        FindPath(transform.position, player.position, lastUsedNode);
        yield return new WaitForSeconds(relocatePlayerTimer);

        RelocatePlayer();
    }

    // Refind the player on the grid
    public void RelocatePlayer()
    {
        gruntNoice.Stop();
        returnToWalkFromAttack = false;
        animation.Play("Walk");
        currentNodeCount = 0;
        pathToPosition.Clear();
        StartCoroutine(LocatePlayer());
    }

    // Take damage, die if lower the 0 health
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !isDead)
        {
            StopAllCoroutines();
            StartCoroutine(Die());
        }
    }

    // Death animations and reset bools
    private IEnumerator Die()
    {
        isDead = true;
        isTweening = false;
        isInAttackAnim = false;
        returnToWalkFromAttack = false;
        canDealDamage = false;
        lastUsedNode = null;
        iTween.Stop(gameObject);

        animation.Play("Death");
        ScoreManager.instance.UpdateValue((int)addedScore);
        yield return new WaitForSeconds(animation.GetClip("Death").length);

        EnemyManager.instance.ReturnObjectToPool(transform);
        EnemyCounter.instance.UpdateValue(-1);
    }

    // Perform attack animation
    private IEnumerator DealDamage()
    {
        returnToWalkFromAttack = true;
        isInAttackAnim = true;
        int attackAnimation = Random.Range(1, 3);
        string attackAnim = "Attack" + attackAnimation;
        animation.Play(attackAnim);

        canDealDamage = true;
        yield return new WaitForSeconds(animation.GetClip(attackAnim).length);
        canDealDamage = false;
        isInAttackAnim = false;
    }
}
