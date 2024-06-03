using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class TutorialMirrorScript : MonoBehaviour
{
    
    [SerializeField] private Tilemap my_tm;
    [SerializeField] private TileBase lighted_square;
    private int angleIndex = 0;

    [SerializeField] private TargetSelection targetSelection;
    [SerializeField] private int range;

    [SerializeField] GameObject wall_crash;
    [SerializeField] GameObject beam_1;
    [SerializeField] GameObject beam_2;
    [SerializeField] GameObject beam_3;
    [SerializeField] GameObject beam_4;
    [SerializeField] GameObject beam_5;

    private float startCrash;
    private float currTime;

    private Vector3Int my_targ;

    [SerializeField] private Color color1;
    [SerializeField] private Color color2;
    private SpriteRenderer my_glow;

    public Vector3Int GetMyTarg() {
        if (angleIndex == 0) { return targetSelection.GetRandomTile(); }
        return my_targ;
    }

    public bool doingChase() {
        return (angleIndex > 0);
    }

    void Start() {
        my_glow = transform.Find("Glow").gameObject.GetComponent<SpriteRenderer>();
        StartCoroutine(GlowRoutine());
    }

    void Update() {
        currTime = Time.time;
        if (angleIndex == 5) {
            if (currTime - startCrash > 0.40f) {
                wall_crash.SetActive(true);
                targetSelection.gameObject.transform.position += new Vector3(5*Time.deltaTime,-2*Time.deltaTime*(currTime-startCrash),0);
                targetSelection.gameObject.transform.Rotate(0,0,12.0f);
            }
            if (currTime - startCrash > 1.0f) {
                //Destroy(targetSelection);
            }
            if (currTime - startCrash > 5.0f) {
                SceneManager.LoadScene("TutorialFinish");
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
   }

    void OnMouseDown() {
        Debug.Log("mouse clicked on mirror");
        int[] my_angles = {0, 15, 30, 45, 60, 75};

        if (angleIndex < 5) {
            angleIndex++;
        }

        // Old
        //my_tm.ClearAllTiles();
        //my_tm.SetTile(new Vector3Int(9+angleIndex-1,5,0), lighted_square);

        // New
        Vector3Int selfCellPosition = my_tm.WorldToCell(targetSelection.transform.position);
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector3Int cellPosition = new(
                    selfCellPosition.x + x,
                    selfCellPosition.y + y,
                    selfCellPosition.z);
                TileBase tile = my_tm.GetTile(cellPosition);
                if (tile != null)
                {
                    targetSelection.ModifyForbiddenTile(cellPosition, true);
                }
            }
        }
        targetSelection.ModifyForbiddenTile(new Vector3Int(9+angleIndex-1,5,0), false); // Now whenever call targetSelection.GetRandomTile() should be target
        targetSelection.SelectTile(new Vector3Int(9+angleIndex-1,5,0)); 
        my_targ = new Vector3Int(9+angleIndex-1,5,0);

        int curr_angle = my_angles[angleIndex];
        if (angleIndex == 0) {
            //gameObject.transform.rotation = new Quaternion(0,0,0,1);
        } else if (angleIndex == 1) {
            //gameObject.transform.rotation = new Quaternion(0,0.130526155f,0,0.991444886f);
            beam_1.SetActive(true);
        } else if (angleIndex == 2) {
            //gameObject.transform.rotation = new Quaternion(0,0.258819103f,0,0.965925813f);
            beam_1.SetActive(false);
            beam_2.SetActive(true);
        } else if (angleIndex == 3) {
            //gameObject.transform.rotation = new Quaternion(0,0.382683426f,0,0.923879564f);
            beam_2.SetActive(false);
            beam_3.SetActive(true);
        } else if (angleIndex == 4) {
            //gameObject.transform.rotation = new Quaternion(0,0.5f,0,0.866025388f);
            beam_3.SetActive(false);
            beam_4.SetActive(true);
        } else if (angleIndex == 5) {
            //targetSelection.ModifyForbiddenTile(new Vector3Int(13,5,0), false);
            beam_4.SetActive(false);
            beam_5.SetActive(true);
            startCrash = Time.time;
            //wall_crash.SetActive(true);
        }

    }

}
