using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MazeGenerator
{
    public enum MazeType
    {
        RecursiveBackTracking
    }

    public static WallState[,] GenerateMaze(int width, int height, MazeType mazeType)
    {
        WallState[,] maze = new WallState[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = new WallState();
            }
        }

        switch (mazeType)
        {
            case MazeType.RecursiveBackTracking:
                maze = ApplyRecursiveBackTracking(maze, width, height);
                break;
        }

        return maze;
    }

    private static WallState[,] ApplyRecursiveBackTracking(WallState[,] maze, int width, int height)
    {
        Stack<Position> positionStack = new Stack<Position>();
        Position position = new Position(Random.Range(0, width), Random.Range(0, height));

        maze[position.x, position.y].visited = true;
        positionStack.Push(position);

        while (positionStack.Count > 0)
        {
            Position current = positionStack.Pop();
            List<Neighbour> neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            if (neighbours.Count > 0)
            {
                positionStack.Push(current);
                Neighbour randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];

                Position neighbourPos = randomNeighbour.position;

                if (neighbourPos.x != current.x)
                {
                    SetDirectionOfNode(current, neighbourPos, WallState.Direction.Right, WallState.Direction.Left, maze);
                } else
                {
                    SetDirectionOfNode(current, neighbourPos, WallState.Direction.Up, WallState.Direction.Down, maze);
                }

                maze[neighbourPos.x, neighbourPos.y].visited = true;

                positionStack.Push(neighbourPos);
            }
        }

        return maze;
    }

    private static void SetDirectionOfNode(Position current, Position neighbourPos, WallState.Direction positiveDirection,
        WallState.Direction negativeDirection, WallState[,] maze)
    {
        if (neighbourPos.x > current.x)
        {
            maze[current.x, current.y].direction = positiveDirection;
            maze[neighbourPos.x, neighbourPos.y].direction = negativeDirection;
        }
        else
        {
            maze[current.x, current.y].direction = negativeDirection;
            maze[neighbourPos.x, neighbourPos.y].direction = positiveDirection;
        }
    }

    private static List<Neighbour> GetUnvisitedNeighbours(Position pos, WallState[,] maze, int width, int height)
    {
        List<Neighbour> unvisitedNeighbours = new List<Neighbour>();

        AddNeigbourToList(unvisitedNeighbours, new Position(pos.x - 1, pos.y), pos.x > 0, maze);
        AddNeigbourToList(unvisitedNeighbours, new Position(pos.x, pos.y + 1), pos.y < height - 1, maze);
        AddNeigbourToList(unvisitedNeighbours, new Position(pos.x + 1, pos.y), pos.x < width - 1, maze);
        AddNeigbourToList(unvisitedNeighbours, new Position(pos.x, pos.y - 1), pos.y > 0, maze);

        return unvisitedNeighbours;
    }

    private static void AddNeigbourToList(List<Neighbour> unvisitedNeighbours, Position posToCheck, bool shouldCheck, 
        WallState[,] maze)
    {
        if (shouldCheck)
        {
            if (!maze[posToCheck.x, posToCheck.y].visited)
            {
                unvisitedNeighbours.Add(new Neighbour(posToCheck));
            }
        }
    }
}
