using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private PathfindingGrid grid = null;

    [System.NonSerialized]
    public List<Vector3> pathToPosition = new List<Vector3>();

    public List<PathfindingNode> finalPath = new List<PathfindingNode>();

    public virtual void Awake()
    {
        grid = PathfindingGrid.instance;
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos, PathfindingNode startNode)
    {
        if (startNode == null)
        {
            startNode = grid.NodeFromWorldPos(startPos);
        }

        PathfindingNode targetNode = grid.NodeFromWorldPos(targetPos);

        List<PathfindingNode> openList = new List<PathfindingNode>();
        HashSet<PathfindingNode> closedList = new HashSet<PathfindingNode>();
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathfindingNode currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (Vector3.Distance(openList[i].pos, targetNode.pos) < Vector3.Distance(currentNode.pos, targetNode.pos))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == targetNode)
            {
                GetFinalPath(startNode, targetNode);
                return;
            }

            foreach (PathfindingNode node in grid.GetNeighborNodes(currentNode))
            {
                if (node.IsWall() && node != targetNode || closedList.Contains(node) || openList.Contains(node))
                {
                    continue;
                }

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

    private int GetManhattenDist(PathfindingNode currentNode, PathfindingNode neighborNode)
    {
        int x = Mathf.Abs(currentNode.GetGridX() - neighborNode.GetGridX());
        int y = Mathf.Abs(currentNode.GetGridY() - neighborNode.GetGridY());

        return x + y;
    }
}
