using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private TargetSelection targetSelection;
    [SerializeField] private Vector3Int initialTile;
    [SerializeField] private TileBase debugTile;
    [SerializeField] private float speed;
    [SerializeField] private bool debugMode;
    [SerializeField] private bool debug2 = false;
    private List<Vector3Int> routeTiles;
    private int routeIdx;
    private Vector3Int[] directions;
    private Vector3Int? targetTile;
    private TileBase originalTile;

    bool movingRight = false;
    bool movingUp = false;

    [SerializeField] private BobReportMovement brm;

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        targetSelection.onForbiddenChanged += HandleForbiddenChanged;
        routeTiles = new List<Vector3Int>(); //starts off empty
        directions = new Vector3Int[]
          {
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
          };

        targetSelection.SelectTile(tilemap().WorldToCell(transform.position) - new Vector3Int(0,0,1));
        if (initialTile != new Vector3(0,0,0)) { targetSelection.SelectTile(initialTile); }
        targetTile = targetSelection.GetTarget();
        if (debugMode)
        {
            originalTile = tilemap().GetTile(targetTile.Value);
            tilemap().SetTile(targetTile.Value, debugTile);
        }
        if (debug2) {
            tilemap().SetTile(targetTile.Value, debugTile);
        }
        routeTiles = GetRoute(
		    tilemap().WorldToCell(transform.position) - new Vector3Int(0,0,1),
		    targetTile.Value);
        routeIdx = 0;
    }
    
    void Update()
    {
        if (routeTiles == null)
        {
            // Bob is dead
            // Try to pick a new tile
            targetTile = targetSelection.GetTarget();
            if (debug2) {
                tilemap().SetTile(targetTile.Value, debugTile);
            }
            routeIdx = 0;
            routeTiles = GetRoute(
		        //tilemap.WorldToCell(transform.position) - new Vector3Int(0,0,1),
                tilemap().WorldToCell(transform.position - new Vector3(0,0,1.0f)),
		        targetTile.Value);
            return;
        }

        if (routeIdx >= routeTiles.Count) 
    	{
            targetSelection.SelectTile(null);
            if (debugMode)
            {
                if (targetTile.HasValue)
                {
                    tilemap().SetTile(targetTile.Value, originalTile);
                }
                targetTile = targetSelection.GetTarget();
                originalTile = tilemap().GetTile(targetTile.Value);
                tilemap().SetTile(targetTile.Value, debugTile);
            } else
            {
                targetTile = targetSelection.GetTarget();
                if (debug2) {
                    tilemap().SetTile(targetTile.Value, debugTile);
                }
            }
            routeTiles = GetRoute(
                tilemap().WorldToCell(transform.position - new Vector3(0,0,1.0f)),
		        targetTile.Value);
            if (routeTiles == null)
            {
                // Bob is dead
                // Try to pick a new tile
                targetTile = targetSelection.GetTarget();
                if (debug2) {
                    tilemap().SetTile(targetTile.Value, debugTile);
                }
                routeIdx = 0;
                return;
            }
            routeIdx = 0;
	    }

        Vector3 nextTileWorld = tilemap().GetCellCenterWorld(routeTiles[routeIdx]);

        Vector3 targetPosition = tilemap().GetCellCenterWorld(routeTiles[routeIdx]) + new Vector3(0, 0, 1);
        Vector3 direction = targetPosition - transform.position;
        movingRight = direction.x > 0;
        movingUp = direction.y > 0;
        if (brm != null) {
            brm.SetMovingRight(movingRight);
            brm.SetMovingUp(movingUp);
            brm.SetStopped(false);
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (IsCloseTo(nextTileWorld, transform.position)) // xy difference magnitude within threshold
        {
            routeIdx += 1;
            return;
        }
    }
    private void HandleForbiddenChanged(object sender, EventArgs e)
    {
        if (targetTile.HasValue)
        {
            if (targetSelection.GetForbiddenTiles().Contains(targetTile.Value))
            {
                routeTiles = new();
                return;
            }
        }
        routeTiles = GetRoute(
            tilemap().WorldToCell(transform.position) - new Vector3Int(0, 0, 1), 
            targetTile.Value);
        if (routeTiles == null)
        {
            routeTiles = new();
        }
    }

    List<Vector3Int> GetRoute(Vector3Int startCell, Vector3Int targetCell)
    {
        Queue<Vector3Int> queue = new();
        Dictionary<Vector3Int, Vector3Int> seenFrom = new(); //maps new cell:the cell where new cell came from
        queue.Enqueue(startCell);
        while (queue.Count > 0)
        {
            Vector3Int currentCell = queue.Dequeue();
            //Debug.Log("" + currentCell.ToString());

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
                //foreach( Vector3Int x in path) {
                    //print( x );
                //}
                return path;
            }

            foreach (Vector3Int direction in directions)
            {
                Vector3Int neighbor = currentCell + direction;

                if (tilemap().GetTile(neighbor) != null && !seenFrom.ContainsKey(neighbor) && 
                    !targetSelection.GetForbiddenTiles().Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    seenFrom[neighbor] = currentCell;
                }
            }
        }

        // No path found
        return null;
    }

    public bool IsCloseTo(Vector3 position, Vector3 targetPosition, float threshold=0.002f)
    {
        Vector2 position2D = new(position.x, position.y);
        Vector2 targetPosition2D = new(targetPosition.x, targetPosition.y);
        float distance = Vector2.Distance(position2D, targetPosition2D);
        return distance <= threshold;
    }

    private Tilemap tilemap()
    {
        return targetSelection.GetTilemap();
    }
}
