using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private PathfindingGrid grid = null;

    [System.NonSerialized]
    public List<Vector3> pathToPosition = new List<Vector3>();

    public List<PathfindingNode> finalPath = new List<PathfindingNode>();

    public LayerMask wallMask = 0;

    public virtual void Awake()
    {
        grid = PathfindingGrid.instance;
    }

    // Find a path from the startpos to the targetpos with a*pathfinding
    public void FindPath(Vector3 startPos, Vector3 targetPos, PathfindingNode startNode)
    {
        // Get a startnode if none is given
        if (startNode == null)
        {
            startNode = grid.NodeFromWorldPos(startPos);
        }

        // Find the endnode
        PathfindingNode targetNode = grid.NodeFromWorldPos(targetPos);

        // If the endwall is a node get one of its neighbours as the end pos instead
        List<PathfindingNode> possibleTargetNodes = new List<PathfindingNode>();
        while (targetNode.IsWall())
        {
            possibleTargetNodes.Add(targetNode);
            List<PathfindingNode> neighborNodes = grid.GetNeighborNodes(targetNode);
            PathfindingNode closestNode = null;
            float closestDist = 100;

            foreach (PathfindingNode node in neighborNodes)
            {
                float dist = Vector3.Distance(targetPos, node.pos);
                if (!possibleTargetNodes.Contains(node) && dist < closestDist && 
                    !Physics.Raycast(node.pos, (targetPos - node.pos).normalized, out RaycastHit hit, dist, wallMask))
                {
                    closestNode = node;
                    closestDist = dist;
                }
            }

            targetNode = closestNode;
        }

        // Add startnode a start searching
        List<PathfindingNode> openList = new List<PathfindingNode>();
        HashSet<PathfindingNode> closedList = new HashSet<PathfindingNode>();
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathfindingNode currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                // Get the closestnode to the end position
                if (Vector3.Distance(openList[i].pos, targetNode.pos) < Vector3.Distance(currentNode.pos, targetNode.pos))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // If at the end, get the final path
            if (currentNode == targetNode)
            {
                GetFinalPath(startNode, targetNode);
                break;
            }

            // Get neighbour nodes
            foreach (PathfindingNode node in grid.GetNeighborNodes(currentNode))
            {
                // Make sure neighbour node is now wall or in one of the two lists
                if (node.IsWall() || closedList.Contains(node) || openList.Contains(node))
                {
                    continue;
                }

                // Find the most efficient route
                int moveCost = currentNode.GetGCost() + GetManhattenDist(currentNode, node);

                if (moveCost < node.GetFCost() || !openList.Contains(node))
                {
                    node.SetGCost(moveCost);
                    node.SetHcost(GetManhattenDist(node, targetNode));
                    node.SetParent(currentNode);
                    openList.Add(node);
                }
            }
        }
    }

    // Get final path based on the parents of all the nodes, fill path to position
    public virtual void GetFinalPath(PathfindingNode startNode, PathfindingNode endNode)
    {
        finalPath.Clear();
        PathfindingNode currentNode = endNode;

        while (currentNode != startNode)
        {
            finalPath.Add(currentNode);
            currentNode = currentNode.GetParent();
        }

        for (int i = finalPath.Count - 1; i >= 0; i--)
        {
            pathToPosition.Add(finalPath[i].pos);
        }
    }

    // Calculate distance based on manhatten dist
    private int GetManhattenDist(PathfindingNode currentNode, PathfindingNode neighborNode)
    {
        int x = Mathf.Abs(currentNode.GetGridX() - neighborNode.GetGridX());
        int y = Mathf.Abs(currentNode.GetGridY() - neighborNode.GetGridY());

        return x + y;
    }
}
