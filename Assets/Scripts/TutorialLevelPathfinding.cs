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

    [SerializeField] private List<SprinklerActivate> my_sprinklers;
    [SerializeField] private Tilemap secondTilemap;
    private bool doneClimb;
    private bool inTreeClimbAnim;
    private bool inJumpStreetAnim;

    [SerializeField] private Sprite baseSpr;
    [SerializeField] private Sprite climbSpr;

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
        inTreeClimbAnim = false;
        inJumpStreetAnim = false;
        doneClimb = false;
    }

    void Update()
    {
        if (inTreeClimbAnim == false && inJumpStreetAnim == false) {
            tileCoordPosition = tilemap.WorldToCell(transform.position);
            //if (doneClimb) { 
                //Debug.Log("xyz: " + tileCoordPosition.x + " " + tileCoordPosition.y + " " + tileCoordPosition.z);
                //Debug.Log("targetChosen: " + targetChosen);
            //}
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

            if (my_sprinklers != null && doneClimb == false) {
                bool doSwap = true;
                foreach (SprinklerActivate sprnk in my_sprinklers) {
                    doSwap = doSwap && sprnk.getTriggerVal();
                }
                if (doSwap == true) {
                    //tilemap = secondTilemap;
                    targetChosen = true;
                    tilemap.SetTile(chosenTilePosition, originalTile);
                    chosenTilePosition = new Vector3Int(4,0,1);
                    originalTile = tilemap.GetTile(chosenTilePosition);
                    chosenWorldPosition = tilemap.GetCellCenterWorld(chosenTilePosition);
                    if (Mathf.Abs(transform.position.x - chosenWorldPosition.x) < 0.01 && Mathf.Abs(transform.position.y - chosenWorldPosition.y) < 0.01) {
                        inTreeClimbAnim = true;
                        //Debug.Log("enter tree climb");
                    }
                }
            }
        } else if (inTreeClimbAnim) {
            //Debug.Log("inTreeClimbAnim");
            gameObject.GetComponent<SpriteRenderer>().sprite = climbSpr;
            gameObject.transform.position += new Vector3(0,Time.deltaTime,0);
            //Debug.Log(gameObject.transform.position.y);
            if (Mathf.Abs(gameObject.transform.position.y - 4) < 0.01) {
                //Debug.Log("exit tree climb");
                inTreeClimbAnim = false;
                targetChosen = false;
                gameObject.GetComponent<SpriteRenderer>().sprite = baseSpr;
                tilemap = secondTilemap;
                doneClimb = true;
                // 9 5 0 final tile coords
            }
        } else if (inJumpStreetAnim) {

        }
    }

    void MoveISO_CARD() {

        // INTERMEDIATE SELECTION IS MINOR BUGGED

        // Isometric cardinal direction movement
        intermediateTilePosition = new Vector3Int(chosenTilePosition.x, tileCoordPosition.y, tileCoordPosition.z);
        intermediateWorldPosition = tilemap.GetCellCenterWorld(intermediateTilePosition) + new Vector3(0f, -tilemap.cellSize.y / 2f, 0f);
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
                    //Debug.Log(tilesLen);
                }
            }
        }
        
        for (int i = 0; i < tilesLen; i++) {
            //Debug.Log(candidateTiles[i]);
        }

        chosenTilePosition = candidateTiles[Random.Range(0, tilesLen)];
        originalTile = tilemap.GetTile(chosenTilePosition);
        tilemap.SetTile(chosenTilePosition, debugTile);

        chosenWorldPosition = tilemap.GetCellCenterWorld(chosenTilePosition);
        chosenWorldPosition.z = transform.position.z;
        //chosenWorldPosition += new Vector3(0f, tilemap.cellSize.y / 2f, 0f);

        targetChosen = true;
    }
}
