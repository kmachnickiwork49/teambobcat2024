using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private TargetSelection targetSelection;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase debugTile;
    [SerializeField] private float speed;
    private List<Vector3Int> routeTiles;
    private int routeIdx;
    private Vector3Int[] directions;
    private Vector3Int? targetTile;
    private TileBase originalTile;

    private void Start()
    {
        routeTiles = new List<Vector3Int>(); //starts off empty
        directions = new Vector3Int[]
          {
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
          };
        targetSelection.SelectTile(new Vector3Int(9, 0, 0));
        targetTile = targetSelection.GetTarget();
        originalTile = tilemap.GetTile(targetTile.Value);
        tilemap.SetTile(targetTile.Value, debugTile);
        routeTiles = GetRoute(
		    tilemap.WorldToCell(transform.position) - new Vector3Int(0,0,1),
		    targetTile.Value);
        routeIdx = 0;
    }
    
    void Update()
    {
        if (routeIdx >= routeTiles.Count) 
    	{
            targetSelection.SelectTile(null);
            if (targetTile.HasValue)
            {
                tilemap.SetTile(targetTile.Value, originalTile);
            }
            targetTile = targetSelection.GetTarget();
            originalTile = tilemap.GetTile(targetTile.Value);
            tilemap.SetTile(targetTile.Value, debugTile);
            routeTiles = GetRoute(
		        tilemap.WorldToCell(transform.position) - new Vector3Int(0,0,1),
		        targetTile.Value);
            routeIdx = 0;
	    }

        Vector3 nextTileWorld = tilemap.GetCellCenterWorld(routeTiles[routeIdx]);
        if (IsCloseTo(nextTileWorld, transform.position))
        {
            routeIdx += 1;
            return;
        }

        Vector3 targetPosition = tilemap.GetCellCenterWorld(routeTiles[routeIdx]) + new Vector3(0, 0, 1);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    List<Vector3Int> GetRoute(Vector3Int startCell, Vector3Int targetCell)
    {
        Queue<Vector3Int> queue = new();
        Dictionary<Vector3Int, Vector3Int> seenFrom = new(); //maps new cell:the cell where new cell came from
        queue.Enqueue(startCell);
        while (queue.Count > 0)
        {
            Vector3Int currentCell = queue.Dequeue();

            if (currentCell == targetCell)
            {
                // Reconstruct the path
                List<Vector3Int> path = new();
                while (currentCell != startCell)
                {
                    path.Add(currentCell);
                    currentCell = seenFrom[currentCell];
                }
                path.Add(startCell);
                path.Reverse();
                return path;
            }

            foreach (Vector3Int direction in directions)
            {
                Vector3Int neighbor = currentCell + direction;

                if (tilemap.GetTile(neighbor) != null && !seenFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    seenFrom[neighbor] = currentCell;
                }
            }
        }

        // No path found
        return null;
    }

    bool IsCloseTo(Vector3 position, Vector3 targetPosition, float threshold=0.02f)
    {
        Vector2 position2D = new(position.x, position.y);
        Vector2 targetPosition2D = new(targetPosition.x, targetPosition.y);
        float distance = Vector2.Distance(position2D, targetPosition2D);
        return distance <= threshold;
    }
}
