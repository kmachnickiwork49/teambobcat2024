using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SprinklerActivatePF : MonoBehaviour
{
    [SerializeField] private int range;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TargetSelection targetSelection;
    [SerializeField] private bool debugMode;
    private GameObject sprinkles;
    private WetSpot wetSpot;

    private SpriteRenderer my_glow;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;

    void Start()
    {
        sprinkles = transform.Find("Sprinkles").gameObject;
        wetSpot = GetComponentInChildren<WetSpot>();
        sprinkles.SetActive(false);
        wetSpot.SetIsOn(false);
        
        my_glow = transform.Find("Glow").gameObject.GetComponent<SpriteRenderer>();
        StartCoroutine(GlowRoutine());
    }

    private bool triggered = false;
    void OnMouseDown() {
        Debug.Log("mouse clicked sprinkler activate");
        triggered = !triggered;
        sprinkles.SetActive(triggered);
        wetSpot.SetIsOn(triggered);
		Vector3Int selfCellPosition = tilemap.WorldToCell(transform.position);
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector3Int cellPosition = new(
                    selfCellPosition.x + x,
                    selfCellPosition.y + y,
                    selfCellPosition.z);
                TileBase tile = tilemap.GetTile(cellPosition);
                if (tile != null)
                {
                    targetSelection.ModifyForbiddenTile(cellPosition, triggered);
                }
                if (debugMode)
                {
                    TileFlags currentFlags = tilemap.GetTileFlags(cellPosition);
                    tilemap.SetTileFlags(cellPosition, currentFlags & ~TileFlags.LockColor);
                    tilemap.SetColor(cellPosition, triggered ? new Color(0.95f, 0.95f, 1f) : new Color(1f, 1f, 1f));
                }
            }
        }
    }

    IEnumerator GlowRoutine()
   {
        yield return new WaitForSeconds(5);
        for (int i = 0; i < 100; i++) {
            my_glow.color = Color.Lerp(color1, color2, Mathf.Clamp01(i / 100.0f));
            yield return new WaitForSeconds(0.05f);
        }
        //float t_fade = 0;
        //t_fade = Mathf.Clamp01(t_fade + Time.deltaTime / 5.0f);
        //my_glow.color = Color.Lerp(color1, color2, t_fade);
        //my_glow.color = color2;
   }

    public bool getTriggerVal() {
        return triggered;
    }
}
