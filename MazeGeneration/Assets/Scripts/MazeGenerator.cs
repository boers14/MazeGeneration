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
        WallState initial = WallState.Down | WallState.Left | WallState.Right | WallState.Up;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = initial;
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
        Position position = new Position { X = Random.Range(0, width), Y = Random.Range(0, height) };

        maze[position.X, position.Y] |= WallState.Visited;
        positionStack.Push(position);

        while (positionStack.Count > 0)
        {
            Position current = positionStack.Pop();
            List<Neighbour> neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            if (neighbours.Count > 0)
            {
                positionStack.Push(current);
                Neighbour randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];

                Position neighbourPos = randomNeighbour.Position;
                maze[current.X, current.Y] &= ~randomNeighbour.SharedWall;
                maze[neighbourPos.X, neighbourPos.Y] &= ~GetOppositeWall(randomNeighbour.SharedWall);

                maze[neighbourPos.X, neighbourPos.Y] |= WallState.Visited;

                positionStack.Push(neighbourPos);
            }
        }

        return maze;
    }

    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.Down: return WallState.Up;
            case WallState.Up: return WallState.Down;
            case WallState.Left: return WallState.Right;
            case WallState.Right: return WallState.Left;
            default: return WallState.Left;
        }
    }

    private static List<Neighbour> GetUnvisitedNeighbours(Position pos, WallState[,] maze, int width, int height)
    {
        List<Neighbour> unvisitedNeighbours = new List<Neighbour>();

        AddNeigbourToList(unvisitedNeighbours, new Position {X = pos.X - 1, Y = pos.Y }, WallState.Left, pos.X > 0, maze);
        AddNeigbourToList(unvisitedNeighbours, new Position { X = pos.X, Y = pos.Y + 1 }, WallState.Up, pos.Y < height - 1, maze);
        AddNeigbourToList(unvisitedNeighbours, new Position { X = pos.X + 1, Y = pos.Y }, WallState.Right, pos.X < width - 1, maze);
        AddNeigbourToList(unvisitedNeighbours, new Position { X = pos.X, Y = pos.Y -1 }, WallState.Down, pos.Y > 0, maze);

        return unvisitedNeighbours;
    }

    private static void AddNeigbourToList(List<Neighbour> unvisitedNeighbours, Position posToCheck, WallState sharedWall, 
        bool shouldCheck, WallState[,] maze)
    {
        if (shouldCheck)
        {
            if (!maze[posToCheck.X, posToCheck.Y].HasFlag(WallState.Visited))
            {
                unvisitedNeighbours.Add(new Neighbour { Position = posToCheck, SharedWall = sharedWall });
            }
        }
    }
}
