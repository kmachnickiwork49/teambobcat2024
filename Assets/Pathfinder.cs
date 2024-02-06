using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private int range;
    [SerializeField] private TileBase debugTile;
    [SerializeField] private float speed;
    private TileBase originalTile;
    private Vector3Int chosenTilePosition;
    private Vector3 chosenWorldPosition;
    private Vector3Int tileCoordPosition;
    private bool targetChosen;
    private List<Vector3Int> candidateTiles;
    private List<Vector3Int> routeTiles;
    private int routeIdx;
    private Vector3Int[] directions;

    private void Start()
    {
        targetChosen = false;

        candidateTiles = new List<Vector3Int>();
        routeTiles = new List<Vector3Int>(); //starts off empty
        directions = new Vector3Int[]
          {
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(1, 1, 0),
                new Vector3Int(1, -1, 0),
                new Vector3Int(-1, 1, 0),
                new Vector3Int(-1, -1, 0)
          };
        for (int x = -range; x <= range; x++) {
            for (int y = -range; y <= range; y++) {
                candidateTiles.Add(new Vector3Int(0,0,0));
            }
        }
    }

    void Update()
    {
        tileCoordPosition = tilemap.WorldToCell(transform.position);
        if (!targetChosen) {
            GetTilesInRange();
            //get route sets routeTiles
            routeTiles = GetRoute(tileCoordPosition, chosenTilePosition);
        }

        if (tileCoordPosition.x == routeTiles[routeIdx].x && tileCoordPosition.y == routeTiles[routeIdx].y)
        {
            routeIdx += 1;
            if (routeIdx >= routeTiles.Count)
            {
                tilemap.SetTile(chosenTilePosition, originalTile);
                targetChosen = false;
                return;
            }
        }

        Vector3 targetPosition = tilemap.GetCellCenterWorld(routeTiles[routeIdx]) + new Vector3(0, 0, 1);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    void GetTilesInRange()
    {
        int tilesLen = 0;
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                if (x==0 && y==0)
                {
                    continue;
                }
                Vector3Int cellPosition = new Vector3Int(
        		    tileCoordPosition.x + x, 
		            tileCoordPosition.y + y, 
		            tileCoordPosition.z - 1);

                TileBase tile = tilemap.GetTile(cellPosition);

                if (tile != null)
                {
                    candidateTiles[tilesLen] = cellPosition;
                    tilesLen += 1;
                    //Debug.Log(tilesLen);
                }
            }
        }
        chosenTilePosition = candidateTiles[Random.Range(0, tilesLen)];
        originalTile = tilemap.GetTile(chosenTilePosition);
        tilemap.SetTile(chosenTilePosition, debugTile);

        chosenWorldPosition = tilemap.GetCellCenterWorld(chosenTilePosition);
        chosenWorldPosition.z = transform.position.z;
        chosenWorldPosition += new Vector3(0f, tilemap.cellSize.y / 2f, 0f);

        targetChosen = true;
    }

    List<Vector3Int> GetRoute(Vector3Int startCell, Vector3Int targetCell)
    {
        //set routeTiles to a list generated here
        startCell -= new Vector3Int(0, 0, 1);
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> seenFrom = new Dictionary<Vector3Int, Vector3Int>(); //maps new cell:the cell where new cell came from
        queue.Enqueue(startCell);
        while (queue.Count > 0)
        {
            Vector3Int currentCell = queue.Dequeue();

            if (currentCell == targetCell)
            {
                // Reconstruct the path
                List<Vector3Int> path = new List<Vector3Int>();
                while (currentCell != startCell)
                {
                    path.Add(currentCell);
                    currentCell = seenFrom[currentCell];
                }
                path.Reverse();
                routeIdx = 0;
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
}
