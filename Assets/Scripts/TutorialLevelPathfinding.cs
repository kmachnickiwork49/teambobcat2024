using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TutorialLevelPathfinding : MonoBehaviour
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

    private Vector3Int intermediateTilePosition;
    private Vector3 intermediateWorldPosition;
    private bool hasHitIntermediate;

    private void Start()
    {
        targetChosen = false;

        candidateTiles = new List<Vector3Int>();
        for (int x = -range; x <= range; x++) {
            for (int y = -range; y <= range; y++) {
                candidateTiles.Add(new Vector3Int(0,0,0));
            }
        }

        hasHitIntermediate = false;
    }

    void Update()
    {
        tileCoordPosition = tilemap.WorldToCell(transform.position);
        if (!targetChosen) {
            GetTilesInRange();
	    } 
	    if (tileCoordPosition.x == chosenTilePosition.x 
                && tileCoordPosition.y == chosenTilePosition.y
                && Mathf.Abs(transform.position.x - chosenWorldPosition.x) < 0.01
                && Mathf.Abs(transform.position.y - chosenWorldPosition.y) < 0.01) {
            tilemap.SetTile(chosenTilePosition, originalTile);
            targetChosen = false;
            hasHitIntermediate = false;
            return;
	    }
        //transform.position = Vector3.MoveTowards(transform.position, chosenWorldPosition, speed * Time.deltaTime);
        MoveISO_CARD();
    }

    void MoveISO_CARD() {
        // Isometric cardinal direction movement
        intermediateTilePosition = new Vector3Int(chosenTilePosition.x, tileCoordPosition.y, tileCoordPosition.z);
        intermediateWorldPosition = tilemap.GetCellCenterWorld(intermediateTilePosition);
        if (hasHitIntermediate == false) {
            // Intermediate first
            transform.position = Vector3.MoveTowards(transform.position, intermediateWorldPosition, speed * Time.deltaTime);
            if (tileCoordPosition.x == intermediateTilePosition.x 
                && tileCoordPosition.y == intermediateTilePosition.y
                && Mathf.Abs(transform.position.x - intermediateWorldPosition.x) < 0.01
                && Mathf.Abs(transform.position.y - intermediateWorldPosition.y) < 0.01) { hasHitIntermediate = true; } 
        } else {
            // Already reached intermediate
            transform.position = Vector3.MoveTowards(transform.position, chosenWorldPosition, speed * Time.deltaTime);
            if (tileCoordPosition.x == chosenTilePosition.x 
            && tileCoordPosition.y == chosenTilePosition.y
            && Mathf.Abs(transform.position.x - chosenWorldPosition.x) < 0.01
            && Mathf.Abs(transform.position.y - chosenWorldPosition.y) < 0.01) { hasHitIntermediate = false; }
        }
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
