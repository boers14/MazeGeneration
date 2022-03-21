using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Pathfinding
{
    private Transform player = null;

    [SerializeField]
    private float moveSpeed = 1f;

    private Vector3 nextPosition = Vector3.zero;

    public override void Start()
    {
        base.Start();
        player = FindObjectOfType<PlayerMovement>().transform;
    }

    private void FixedUpdate()
    {
        if (!found)
        {
            FindPath(transform.position, player.position);
        }
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
            nextPosition = pathToPosition[0];
            iTween.MoveTo(gameObject, iTween.Hash("position", nextPosition, "time", moveSpeed, "easetype",
                iTween.EaseType.linear, "oncomplete", "GetNextPositionNode", "oncompletetarget", gameObject));
            pathToPosition.RemoveAt(0);
        } else
        {
            if (Vector3.Distance(transform.position, player.position) > 1)
            {
                found = false;
            }
        }
    }
}
