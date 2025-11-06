using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathNode
{
    public Grid<PathNode> grid;
    public int x;
    public int z;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;
    public PathNode cameFromNode;

    public PathNode(Grid<PathNode> grid, int x, int z) {
        this.grid = grid;
        this.x = x;
        this.z = z;
        isWalkable = true;

        // Liste der nicht begehbaren Koordinaten
        HashSet<(int, int)> nonWalkableCells = new HashSet<(int, int)>()
        {
            (0, 4), (0, 11),
            (1, 4), (1, 11),
            (2, 4), (2, 11), (2, 12), (2, 13), (2, 14), (2, 15), (2, 16),
            (3, 4), (3, 8),
            (4, 0), (4, 1), (4, 2), (4, 4), (4, 7), (4, 8), (4, 9), (4, 10), (4, 11), (4, 12), (4, 13), (4, 14), (4, 15), (4, 16),
            (5, 0), (5, 1), (5, 2), (5, 7), (5, 8), (5, 9), (5, 10), (5, 11), (5, 12), (5, 13), (5, 14), (5, 15), (5, 16),
            (6, 7), (6, 9),
            (7, 2), (7, 10),
            (8, 2), (8, 5),
            (9, 0), (9, 1), (9, 2), (9, 5), (9, 8), (9, 9), (9, 10), (9, 11), (9, 12), (9, 13), (9, 14), (9, 15), (9, 16),
            (10, 5), (10, 8), (10, 10),
            (11, 0), (11, 1), (11, 2), (11, 3), (11, 5), (11, 6), (11, 8), (11, 10),
            (12, 0), (12, 1), (12, 8),
            (13, 0), (13, 1), (13, 8), (13, 10),
            (14, 8), (14, 10),
            (15, 8), (15, 10),
            (16, 8), (16, 10)
        };

        // Setze isWalkable auf false, falls die Koordinate in der Liste ist
        if (nonWalkableCells.Contains((x, z))) {
            isWalkable = false;
        }
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable) {
        this.isWalkable = isWalkable;
        grid.TriggerGridObjectChanged(x, z);
    }

    public override string ToString() {
        return x + ", " + z;
    }
}
