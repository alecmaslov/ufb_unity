using UnityEngine;
using System;
using System.Collections.Generic;
using UFB.Entities;
using System.Linq;

public static class Pathfinder
{

    public static List<TileEntity> FindTilePath(TileEntity startTile, TileEntity endTile, GameBoard gameBoard)
    {
        Debug.Log($"Finding path from {startTile} to {endTile}");

        var openSet = new List<TileEntity> { startTile };
        var cameFrom = new Dictionary<TileEntity, TileEntity>();
        var gScore = new Dictionary<TileEntity, float> { { startTile, 0 } };
        var fScore = new Dictionary<TileEntity, float> { { startTile, startTile.DistanceTo(endTile) } };

        while (openSet.Count > 0)
        {
            TileEntity current = openSet.OrderBy(t => fScore.ContainsKey(t) ? fScore[t] : float.MaxValue).First();

            Debug.Log($"Current tile: {current} with fScore: {fScore[current]}");

            if (current == endTile)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            foreach (var neighbor in gameBoard.GetAdjacentTiles(current))
            {


                if (current.BlockedByWall(neighbor)) {
                    continue;
                }

                var tentativeGScore = (gScore.ContainsKey(current) ? gScore[current] : float.MaxValue) + current.DistanceTo(neighbor);
                Debug.Log($"Evaluating neighbor: {neighbor}. Tentative gScore: {tentativeGScore}. Is it new or a better path? {!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor]}");

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    if (cameFrom.ContainsKey(neighbor))
                    {
                        cameFrom[neighbor] = current;
                    }
                    else
                    {
                        cameFrom.Add(neighbor, current);
                    }

                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + neighbor.DistanceTo(endTile);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        Debug.Log("No path found");
        return null; // Return null if no path is found
    }

    private static List<TileEntity> ReconstructPath(Dictionary<TileEntity, TileEntity> cameFrom, TileEntity current)
    {
        var path = new List<TileEntity> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();

        Debug.Log($"Path found: {string.Join(" => ", path.Select(t => t.ToString()))}");

        return path;
    }


    // public static List<TileEntity> FindPath(TileEntity tileStart, TileEntity tileEnd, GameBoard gameBoard) {

    //     List<TileEntity> traversedPaths = new List<TileEntity>();

    //     List<TileEntity> openList = new List<TileEntity>();
    //     List<TileEntity> closedList = new List<TileEntity>();

    //     TileEntity currentTile = tileStart;

    //     while (true) {


    //         TileEntity[] adjacentTiles = gameBoard.GetAdjacentTiles(currentTile, false);
    //         if (adjacentTiles.Length == 0) {
    //             return null;
    //         }
    //         var lowestCostTile = adjacentTiles.OrderBy(t => t.DistanceTo(tileEnd)).First();

    //     }


    //     return null;
    // }



}





































// public static List<TileEntity> FindPath(TileEntity start, TileEntity end) {
//     List<TileEntity> path = new List<TileEntity>();
//     List<TileEntity> openList = new List<TileEntity>();
//     List<TileEntity> closedList = new List<TileEntity>();

//     openList.Add(start);

//     while (openList.Count > 0) {
//         TileEntity currentTile = openList[0];
//         for (int i = 1; i < openList.Count; i++) {
//             if (openList[i].FCost < currentTile.FCost || openList[i].FCost == currentTile.FCost && openList[i].HCost < currentTile.HCost) {
//                 currentTile = openList[i];
//             }
//         }

//         openList.Remove(currentTile);
//         closedList.Add(currentTile);

//         if (currentTile == end) {
//             path = RetracePath(start, end);
//             return path;
//         }

//         foreach (TileEntity neighbor in currentTile.Neighbors) {
//             if (neighbor == null || closedList.Contains(neighbor)) {
//                 continue;
//             }

//             int newMovementCostToNeighbor = currentTile.GCost + GetDistance(currentTile, neighbor);
//             if (newMovementCostToNeighbor < neighbor.GCost || !openList.Contains(neighbor)) {
//                 neighbor.GCost = newMovementCostToNeighbor;
//                 neighbor.HCost = GetDistance(neighbor, end);
//                 neighbor.Parent = currentTile;

//                 if (!openList.Contains(neighbor)) {
//                     openList.Add(neighbor);
//                 }
//             }
//         }
//     }

//     return path;
// }