using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapBrain : MonoBehaviour
{
    [SerializeField] List<Tilemap> tilemaps;
    [SerializeField] List<Transform> cameraTransforms;
    [SerializeField] List<float> cameraZooms;
    [SerializeField] float cameraMoveDuration = 2f;
    [SerializeField] TargetSelection bobTargetSelection;
    [SerializeField] Transform bobTransform;
    [SerializeField] int mapIdx = 0;

    void Start()
    {
        Camera cam = Camera.main;
        StartCoroutine(CameraUtils.MoveAndZoomCamera(
            cam.transform.position, cameraTransforms[mapIdx].position,
            cam.orthographicSize, cameraZooms[mapIdx], 0.1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (mapIdx >= tilemaps.Count - 1)
        {
            return;
        }
        Vector3Int bobTile = tilemaps[mapIdx + 1].WorldToCell(bobTransform.position - new Vector3(0, 0, 1.0f));
        if (tilemaps[mapIdx + 1].HasTile(bobTile))
        {
            mapIdx += 1;
            bobTargetSelection.setNewTilemap(tilemaps[mapIdx]);
            StartCoroutine(CameraUtils.MoveAndZoomCamera(
                cameraTransforms[mapIdx - 1].position, cameraTransforms[mapIdx].position,
                cameraZooms[mapIdx - 1], cameraZooms[mapIdx], 
                cameraMoveDuration));
        }
    }
}
