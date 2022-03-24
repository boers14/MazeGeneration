using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MazeGenerator
{
    // Create empty maze
    public static WallState[,] GenerateMaze(int width, int height)
    {
        WallState[,] maze = new WallState[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = new WallState();
            }
        }

        // Fill maze
        maze = ApplyRecursiveBackTracking(maze, width, height);

        return maze;
    }

    // Return maze based on recursive backtracking
    private static WallState[,] ApplyRecursiveBackTracking(WallState[,] maze, int width, int height)
    {
        // Grab random pos and mark it as visited
        Stack<Position> positionStack = new Stack<Position>();
        Position position = new Position(Random.Range(0, width), Random.Range(0, height));

        maze[position.x, position.y].visited = true;
        positionStack.Push(position);

        while (positionStack.Count > 0)
        {
            // Grab last added position and check if it has unvisted neighbours
            Position current = positionStack.Pop();
            List<Neighbour> neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            // If there are neighbours continue else go to next position
            if (neighbours.Count > 0)
            {
                // Re-add position
                positionStack.Push(current);
                // Grab random neighbour to go to
                Neighbour randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];

                // Check which direction the neighbour is and mark that as the direction of the current node (neigbout gains opposite
                // direction, to make sure all positions have a wall)
                Position neighbourPos = randomNeighbour.position;
                if (neighbourPos.x != current.x)
                {
                    SetDirectionOfNode(current.x, neighbourPos.x, current, neighbourPos, WallState.Direction.Right, 
                        WallState.Direction.Left, maze);
                } else
                {
                    SetDirectionOfNode(current.y, neighbourPos.y, current, neighbourPos, WallState.Direction.Up, 
                        WallState.Direction.Down, maze);
                }

                // Mark position as visited
                maze[neighbourPos.x, neighbourPos.y].visited = true;

                // Add neighbour
                positionStack.Push(neighbourPos);
            }
        }

        return maze;
    }

    // Set the direction of a given node based on the given variables
    private static void SetDirectionOfNode(int currentPos, int neighbourPos, Position current, Position neighbour,
        WallState.Direction positiveDirection, WallState.Direction negativeDirection, WallState[,] maze)
    {
        if (neighbourPos > currentPos)
        {
            maze[current.x, current.y].direction = positiveDirection;
            maze[neighbour.x, neighbour.y].direction = negativeDirection;
        }
        else
        {
            maze[current.x, current.y].direction = negativeDirection;
            maze[neighbour.x, neighbour.y].direction = positiveDirection;
        }
    }

    // Add all unvisited neigbours to the list
    private static List<Neighbour> GetUnvisitedNeighbours(Position pos, WallState[,] maze, int width, int height)
    {
        List<Neighbour> unvisitedNeighbours = new List<Neighbour>();

        AddNeigbourToList(unvisitedNeighbours, new Position(pos.x - 1, pos.y), pos.x > 0, maze);
        AddNeigbourToList(unvisitedNeighbours, new Position(pos.x, pos.y + 1), pos.y < height - 1, maze);
        AddNeigbourToList(unvisitedNeighbours, new Position(pos.x + 1, pos.y), pos.x < width - 1, maze);
        AddNeigbourToList(unvisitedNeighbours, new Position(pos.x, pos.y - 1), pos.y > 0, maze);

        return unvisitedNeighbours;
    }

    // Check if position is within bounds and is not visited
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
