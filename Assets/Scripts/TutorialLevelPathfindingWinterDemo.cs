using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TutorialLevelPathfindingWinterDemo : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private int range;
    [SerializeField] private TileBase debugTile;
    [SerializeField] private float speed;
    [SerializeField] private float wetSpeed;
    private float currSpeed;
    private TileBase originalTile;
    private Vector3Int chosenTilePosition;
    private Vector3 chosenWorldPosition;
    private Vector3Int tileCoordPosition;
    private bool targetChosen;
    private List<Vector3Int> candidateTiles;

    private Vector3Int intermediateTilePosition;
    private Vector3 intermediateWorldPosition;
    private bool hasHitIntermediate;

    [SerializeField] private List<SprinklerActivatePF> my_sprinklers;
    [SerializeField] private Tilemap secondTilemap;
    private bool doneClimb;
    private bool inTreeClimbAnim;
    private bool inJumpStreetAnim;

    [SerializeField] private Sprite baseSpr;
    [SerializeField] private Sprite climbSpr;

    private float curr_time;
    private float prev_time;
    private bool doWait;

    [SerializeField] private TileBase baseOfTree;

    [SerializeField] private float restTime;

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

        curr_time = Time.time;
        if (doWait) {
            if (my_sprinklers != null && doneClimb == false) {
                bool doSwap = true;
                foreach (SprinklerActivatePF sprnk in my_sprinklers) {
                    doSwap = doSwap && sprnk.getTriggerVal();
                }
                if (doSwap == true) {
                    prev_time = prev_time - restTime - 0.5f; // Interrupt rest states if any
                }
            }
            if (curr_time - prev_time > restTime || tilemap.GetTile(tileCoordPosition - new Vector3Int(0,0,1))==null) {
                doWait = false;
            } else {
                return;
            }
        }

        if (inTreeClimbAnim == false && inJumpStreetAnim == false) {
            tileCoordPosition = tilemap.WorldToCell(transform.position);
            if (tilemap.GetTile(tileCoordPosition - new Vector3Int(0,0,1)) == null) {
                // Wet currently
                currSpeed = wetSpeed;
            } else {
                currSpeed = speed;
            }
            /*
            if (doneClimb) { 
                Debug.Log("xyz: " + tileCoordPosition.x + " " + tileCoordPosition.y + " " + tileCoordPosition.z);
                Debug.Log("targetChosen: " + targetChosen);
            }
            */
            if (!targetChosen || tilemap.GetTile(chosenTilePosition) == null) {
                GetTilesInRange();
                hasHitIntermediate = false;
            } 
            if (tileCoordPosition.x == chosenTilePosition.x 
                    && tileCoordPosition.y == chosenTilePosition.y
                    && Mathf.Abs(transform.position.x - chosenWorldPosition.x) < 0.01
                    && Mathf.Abs(transform.position.y - chosenWorldPosition.y) < 0.01) {
                tilemap.SetTile(chosenTilePosition, originalTile);
                targetChosen = false;
                hasHitIntermediate = false;
                doWait = true;
                prev_time = curr_time;
                return;
            }
            //transform.position = Vector3.MoveTowards(transform.position, chosenWorldPosition, currSpeed * Time.deltaTime);
            MoveISO_CARD();

            if (my_sprinklers != null && doneClimb == false) {
                bool doSwap = true;
                foreach (SprinklerActivatePF sprnk in my_sprinklers) {
                    doSwap = doSwap && sprnk.getTriggerVal();
                }
                if (doSwap == true) {
                    //tilemap = secondTilemap;
                    targetChosen = true;
                    //tilemap.SetTile(chosenTilePosition, originalTile);
                    chosenTilePosition = new Vector3Int(4,0,1);
                    //originalTile = tilemap.GetTile(chosenTilePosition);
                    tilemap.SetTile(chosenTilePosition, baseOfTree);
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
            gameObject.transform.position += new Vector3(0, Time.deltaTime * 0.75f, 0);
            gameObject.transform.localScale = new Vector3(0.2f,0.2f,1);
            //Debug.Log(gameObject.transform.position.y);
            if (Mathf.Abs(gameObject.transform.position.y - 4) < 0.01) {
                Debug.Log("exit tree climb");
                gameObject.transform.localScale = new Vector3(0.1f,0.1f,1);
                inTreeClimbAnim = false;
                targetChosen = false;
                gameObject.GetComponent<SpriteRenderer>().sprite = baseSpr;
                tilemap = secondTilemap;
                doneClimb = true;
                // 9 5 0 final tile coords
            }
        } else if (inJumpStreetAnim) {
            // prev_time has been set
            // Use curr_time - prev_time to model jump descent
            // Trigger jump_in_the_caac audio?
        }
    }

    void MoveISO_CARD() {

        // INTERMEDIATE SELECTION IS MINOR BUGGED
        // Also does not account for avoiding non-tile while moving on path

        // Isometric cardinal direction movement
        intermediateTilePosition = new Vector3Int(chosenTilePosition.x, tileCoordPosition.y, tileCoordPosition.z);
        if (tilemap.GetTile(intermediateTilePosition) == null) {
            intermediateTilePosition = new Vector3Int(tileCoordPosition.x, chosenTilePosition.y, tileCoordPosition.z);
        }
        intermediateWorldPosition = tilemap.GetCellCenterWorld(intermediateTilePosition) + new Vector3(0f, -tilemap.cellSize.y / 2f, 0f);
        if (hasHitIntermediate == false) {
            // Intermediate first
            transform.position = Vector3.MoveTowards(transform.position, intermediateWorldPosition, currSpeed * Time.deltaTime);
            if (tileCoordPosition.x == intermediateTilePosition.x 
                && tileCoordPosition.y == intermediateTilePosition.y
                && Mathf.Abs(transform.position.x - intermediateWorldPosition.x) < 0.01
                && Mathf.Abs(transform.position.y - intermediateWorldPosition.y) < 0.01) { hasHitIntermediate = true; } 
        } else {
            // Already reached intermediate
            transform.position = Vector3.MoveTowards(transform.position, chosenWorldPosition, currSpeed * Time.deltaTime);
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
        
        /*
        for (int i = 0; i < tilesLen; i++) {
            //Debug.Log(candidateTiles[i]);
        }
        */

        chosenTilePosition = candidateTiles[Random.Range(0, tilesLen)];
        originalTile = tilemap.GetTile(chosenTilePosition);
        tilemap.SetTile(chosenTilePosition, debugTile);

        chosenWorldPosition = tilemap.GetCellCenterWorld(chosenTilePosition);
        chosenWorldPosition.z = transform.position.z;
        //chosenWorldPosition += new Vector3(0f, tilemap.cellSize.y / 2f, 0f);

        targetChosen = true;
    }

    /*
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

    bool IsCloseTo(Vector3 position, Vector3 targetPosition, float threshold=0.01f)
    {
        Vector2 position2D = new Vector2(position.x, position.y);
        Vector2 targetPosition2D = new Vector2(targetPosition.x, targetPosition.y);
        float distance = Vector2.Distance(position2D, targetPosition2D);
        return distance <= threshold;
    }
    */
}
