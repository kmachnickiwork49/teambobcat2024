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

    private void Start()
    {
        targetChosen = false;

        candidateTiles = new List<Vector3Int>();
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
	    } 
	    if (tileCoordPosition.x == chosenTilePosition.x && tileCoordPosition.y == tileCoordPosition.y) {
            tilemap.SetTile(chosenTilePosition, originalTile);
            targetChosen = false;
            return;
	    }
        transform.position = Vector3.MoveTowards(transform.position, chosenWorldPosition, speed * Time.deltaTime);
    }

    void GetTilesInRange()
    {
        int tilesLen = 0;
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector3Int cellPosition = new Vector3Int(
        		    tileCoordPosition.x + x, 
		            tileCoordPosition.y + y, 
		            tileCoordPosition.z - 1);

                TileBase tile = tilemap.GetTile(cellPosition);

                if (tile != null)
                {
                    candidateTiles[tilesLen] = cellPosition;
                    tilesLen += 1;
                    Debug.Log(tilesLen);
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
}
