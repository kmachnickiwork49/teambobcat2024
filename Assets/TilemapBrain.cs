using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class ListWrapper
{
     public List<SpriteRenderer> values;
}

public class TilemapBrain : MonoBehaviour
{
    [SerializeField] List<Tilemap> tilemaps;
    [SerializeField] List<Transform> cameraTransforms;
    [SerializeField] List<float> cameraZooms;
    [SerializeField] float cameraMoveDuration = 2f;
    [SerializeField] TargetSelection bobTargetSelection;
    [SerializeField] Transform bobTransform;
    [SerializeField] int mapIdx = 0;
    [SerializeField] List<ListWrapper> srs = new();

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
            // entered next tilemap
            bobTargetSelection.setNewTilemap(tilemaps[mapIdx + 1]);
            StartCoroutine(CameraUtils.MoveAndZoomCamera(
                cameraTransforms[mapIdx].position, cameraTransforms[mapIdx + 1].position,
                cameraZooms[mapIdx], cameraZooms[mapIdx + 1], 
                cameraMoveDuration));
            foreach (SpriteRenderer sr in srs[mapIdx].values)
            {
                StartCoroutine(ChangeTopLevelOpacity(1f, 0.05f, cameraMoveDuration, sr));
            }
            mapIdx += 1;
        }
    }

    IEnumerator ChangeTopLevelOpacity(float iAlpha, float fAlpha, float duration, SpriteRenderer sr)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, iAlpha + (fAlpha - iAlpha) * normalizedTime);
            yield return null;
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, fAlpha);
    }
}
