using System;
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

    bool movingRight = false;
    bool movingUp = false;
 
    // From Pathfinder.cs
    [SerializeField] private TargetSelection targetSelection;
    [SerializeField] private Vector3Int initialTile;
    [SerializeField] private bool debugMode;
    private List<Vector3Int> routeTiles;
    private int routeIdx;
    private Vector3Int[] directions;
    private Vector3Int? targetTile;


    [SerializeField] private TutorialMirrorScript my_mirr;

    [SerializeField] private SpriteRenderer treefront;
    private float treefrontTimer;
    private float treefrontTimerStart;
    [SerializeField] Color color1;
    [SerializeField] Color color2;

    [SerializeField] float treeExtVanishTime;
    [SerializeField] SpriteRenderer treeExtSr;

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

        // Paste from Pathfinder.cs
        targetSelection.onForbiddenChanged += HandleForbiddenChanged;
        routeTiles = new List<Vector3Int>(); //starts off empty
        directions = new Vector3Int[]
          {
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
          };

        targetSelection.SelectTile(initialTile);
        targetTile = targetSelection.GetTarget();
        if (debugMode)
        {
            originalTile = tilemap.GetTile(targetTile.Value);
            tilemap.SetTile(targetTile.Value, debugTile);
        }
        routeTiles = GetRoute(
            tilemap.WorldToCell(transform.position - new Vector3(0,0,1.0f)),
		    targetTile.Value);
        routeIdx = 0;
        // End paste from Pathfinder.cs

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
                    doWait = false;
                }
            }
            if (curr_time - prev_time > restTime || tilemap.GetTile(tileCoordPosition - new Vector3Int(0,0,1))==null
            || targetSelection.GetForbiddenTiles().Contains(tileCoordPosition - new Vector3Int(0,0,1))
            || my_mirr.doingChase()) {
                //print(targetSelection.GetForbiddenTiles().Contains(tileCoordPosition - new Vector3Int(0,0,1)));
                doWait = false;
            } else {
                return;
            }
        }

        if (inTreeClimbAnim == false && inJumpStreetAnim == false) {
            tileCoordPosition = tilemap.WorldToCell(transform.position);
            if (tilemap.GetTile(tileCoordPosition - new Vector3Int(0,0,1)) == null
            || targetSelection.GetForbiddenTiles().Contains(tileCoordPosition - new Vector3Int(0,0,1))
            || my_mirr.doingChase()) {
                // Wet currently
                //print("currently wet");
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
            //MoveISO_CARD();

            if (doneClimb && !inTreeClimbAnim && !inJumpStreetAnim) {
                //targetSelection.ModifyForbiddenTile(new Vector3Int(13,5,0), true);
                chosenTilePosition = my_mirr.GetMyTarg();
            }

            targetSelection.SelectTile(chosenTilePosition);
            //print(chosenTilePosition);
            FollowPath();

            if (my_sprinklers != null && doneClimb == false) {
                bool doSwap = true;
                foreach (SprinklerActivatePF sprnk in my_sprinklers) {
                    doSwap = doSwap && sprnk.getTriggerVal();
                }
                if (doSwap == true) {
                    //print("doSwap true");
                    //tilemap = secondTilemap;
                    targetChosen = true;
                    //tilemap.SetTile(chosenTilePosition, originalTile);
                    //chosenTilePosition = new Vector3Int(4,0,1);
                    chosenTilePosition = new Vector3Int(4,0,0);
                    //originalTile = tilemap.GetTile(chosenTilePosition);
                    tilemap.SetTile(chosenTilePosition, baseOfTree);
                    chosenWorldPosition = tilemap.GetCellCenterWorld(chosenTilePosition);
                    targetSelection.SelectTile(chosenTilePosition); // NEW FROM Pathfinder
                    if (Mathf.Abs(transform.position.x - chosenWorldPosition.x) < 0.01 && Mathf.Abs(transform.position.y - chosenWorldPosition.y) < 0.01) {
                        StartCoroutine(VanishTreeExterior());
                        inTreeClimbAnim = true;
                        treefrontTimerStart = Time.time;
                        treefrontTimer = Time.time;
                        Debug.Log("enter tree climb");
                    }
                } else if (tilemap.GetTile(chosenTilePosition) == null
                            || targetSelection.GetForbiddenTiles().Contains(chosenTilePosition)) {
                    // Goal is wet, need new goal
                    print("goal got wet");
                    chosenTilePosition = targetSelection.GetRandomTile();
                }
            }
        } else if (inTreeClimbAnim) {
            //Debug.Log("inTreeClimbAnim");
            GetComponent<Animator>().enabled = false;
            GetComponent<BobOrientation>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().sprite = climbSpr;
            gameObject.transform.position += new Vector3(0, Time.deltaTime * 0.75f, 0);
            gameObject.transform.localScale = new Vector3(0.2f,0.2f,1);
            treefrontTimer += Time.deltaTime;
            print((treefrontTimer - treefrontTimerStart));
            float t = Mathf.Clamp01((treefrontTimer - treefrontTimerStart) / 2.5f);
            treefront.color = Color.Lerp(color1, color2, t);
            //Debug.Log(gameObject.transform.position.y);
            if (Mathf.Abs(gameObject.transform.position.y - 4) < 0.01) {
                Debug.Log("exit tree climb");
                gameObject.transform.localScale = new Vector3(0.1f,0.1f,1);
                inTreeClimbAnim = false;
                targetChosen = false;
                GetComponent<Animator>().enabled = true;
                GetComponent<BobOrientation>().enabled = true;
                gameObject.GetComponent<SpriteRenderer>().sprite = baseSpr;
                tilemap = secondTilemap;
                doneClimb = true;

                targetSelection.setNewTilemap(secondTilemap);
                chosenTilePosition = targetSelection.GetRandomTile();
                routeTiles = new List<Vector3Int>(); //starts off empty
                //targetSelection.SelectTile(initialTile);
                targetSelection.SelectTile(chosenTilePosition);
                targetTile = targetSelection.GetTarget();
                routeTiles = GetRoute(
                    tilemap.WorldToCell(transform.position - new Vector3(0,0,1.0f)),
                    targetTile.Value);
                routeIdx = 0;
                targetSelection.ModifyForbiddenTile(new Vector3Int(13,5,0), true); // Prevent accidental access special area
                print(chosenTilePosition);
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

    public bool GetMovingUp()
    {
        return movingUp;
    }

    public bool GetMovingRight()
    {
        return movingRight;
    }

    IEnumerator VanishTreeExterior() { 
        for (float t = 0f; t < treeExtVanishTime; t += Time.deltaTime)
        {
            float normalizedTime = t / treeExtVanishTime;
            treeExtSr.color = new Color(treeExtSr.color.r, treeExtSr.color.g, treeExtSr.color.b, 1 - normalizedTime);
            yield return null;
        }
        treeExtSr.color = new Color(treeExtSr.color.r, treeExtSr.color.g, treeExtSr.color.b, 0);
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

        /*
        chosenTilePosition = candidateTiles[UnityEngine.Random.Range(0, tilesLen)];
        while (targetSelection.GetForbiddenTiles().Contains(chosenTilePosition)) {
            // Do not select a wet tile
            chosenTilePosition = candidateTiles[UnityEngine.Random.Range(0, tilesLen)];
        }
        */
        chosenTilePosition = targetSelection.GetRandomTile();
        originalTile = tilemap.GetTile(chosenTilePosition);
        tilemap.SetTile(chosenTilePosition, debugTile);

        chosenWorldPosition = tilemap.GetCellCenterWorld(chosenTilePosition);
        chosenWorldPosition.z = transform.position.z;
        //chosenWorldPosition += new Vector3(0f, tilemap.cellSize.y / 2f, 0f);

        targetChosen = true;
    }

    void FollowPath()
    {
        if (routeTiles == null)
        {
            // Bob is dead
            return;
        }

        if (routeIdx >= routeTiles.Count) 
    	{
            //targetSelection.SelectTile(null);
            /*
            if (debugMode)
            {
                if (targetTile.HasValue)
                {
                    tilemap.SetTile(targetTile.Value, originalTile);
                }
                targetTile = targetSelection.GetTarget();
                originalTile = tilemap.GetTile(targetTile.Value);
                tilemap.SetTile(targetTile.Value, debugTile);
            } else
            {
                */
                targetTile = targetSelection.GetTarget();
            //}
            //print(transform.position); // 2.5 -0.5 1, exact centered looks correct
            //print( tilemap.WorldToCell(transform.position) ); // 0 -4 1
            //print(transform.position - new Vector3(0,0,1.0f));
            routeTiles = GetRoute(
		        //tilemap.WorldToCell(transform.position) - new Vector3Int(0,0,1),
                tilemap.WorldToCell(transform.position - new Vector3(0,0,1.0f)),
		        //targetTile.Value);
                chosenTilePosition);
            if (routeTiles == null)
            {
                // Bob is dead
                print("null routeTiles");
                return;
            }
            routeIdx = 0;
	    }

        Vector3 nextTileWorld = tilemap.GetCellCenterWorld(routeTiles[routeIdx]);

        Vector3 targetPosition = tilemap.GetCellCenterWorld(routeTiles[routeIdx]) + new Vector3(0, 0, 1);
        Vector3 direction = targetPosition - transform.position;
        movingRight = direction.x > 0;
        movingUp = direction.y > 0;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currSpeed * Time.deltaTime);

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
            //tilemap.WorldToCell(transform.position) - new Vector3Int(0, 0, 1), 
            tilemap.WorldToCell(transform.position - new Vector3(0,0,1.0f)),
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

                if (tilemap.GetTile(neighbor) != null && !seenFrom.ContainsKey(neighbor) && 
                    !targetSelection.GetForbiddenTiles().Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    seenFrom[neighbor] = currentCell;
                }
            }
        }

        // No path found, NOW TRY TO USE FORBIDDEN TO RECOVER
        queue = new();
        seenFrom = new(); //maps new cell:the cell where new cell came from
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
                //foreach( Vector3Int x in path) {
                    //print( x );
                //}
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

    bool IsCloseTo(Vector3 position, Vector3 targetPosition, float threshold=0.002f)
    {
        Vector2 position2D = new(position.x, position.y);
        Vector2 targetPosition2D = new(targetPosition.x, targetPosition.y);
        float distance = Vector2.Distance(position2D, targetPosition2D);
        return distance <= threshold;
    }

}
