using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Tilemaps;

public class GuiButtons : MonoBehaviour
{
    [System.Serializable]
    public class ButtonList {
            public PlayerController.Key index;
            public GameObject guiButton;
            public GameObject guiLayer;
            public bool actif;
    }
    
    public List<ButtonList> buttonList = new List<ButtonList>();
    public AudioSource  destroyButtonSound;
    public GameObject   dealPanel;
    public Animator     guiButtonsAnimator;
    private bool        isPaused = false;

    // TileMap:
    public TileBase     platTile;
    Tilemap             tilemap;

    Dictionary< Vector3Int, TileBase > previewTiles = new Dictionary<Vector3Int, TileBase>();

    void Start()
    {
        tilemap = GameObject.FindObjectOfType< Tilemap >();
    }

    void Update()
    {
        if (Input.GetKeyDown("e")) {
            TogglePauseMenu();
        }
    }

    public void SetButtonStatus(PlayerController.Key p_button, bool p_status)
    {
        ButtonList button = buttonList.Where(c => c.index == p_button).FirstOrDefault();
        button.actif = p_status;
        button.guiLayer.SetActive(!p_status);
    }

    public bool GetButtonStatus(PlayerController.Key p_button) {
        ButtonList truc;
        return ((truc = buttonList.Where(c => c.index == p_button).FirstOrDefault()) != null ? truc.actif : false);
    }

    public void DestroyButton(string p_key)
    {
        ButtonList button;
        button = buttonList.Where(c => c.index.ToString() == p_key).FirstOrDefault();
        if (button.actif)
        {
            button.actif = false;
            button.guiLayer.SetActive(true);
            destroyButtonSound.Play();
            guiButtonsAnimator.SetBool("EnableButtons", true);
        }
    }

    public void TogglePauseMenu()
    {
        if (dealPanel.activeSelf)
        {
            dealPanel.SetActive(false);
            Time.timeScale = 1.0f;
            isPaused = false;
        }
        else
        {
            dealPanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
    
/*****************************************************************************************************************
                                                        DEAL
*****************************************************************************************************************/
    [Header("Cusrsor setting")]
    public Texture2D mouseBase;
    public Texture2D mouseDestroy;
    public Texture2D mouseCreate;
    
    [Header("Deal settings")]
    public int DestroyRange = 1;

    void DestroyTileRepeat(int i, Vector3Int CellPos, Tilemap t)
    {
        t.SetTile(CellPos, null);
        if (i > 0)
        {
            i--;
            DestroyTileRepeat(i, new Vector3Int(CellPos.x - 1, CellPos.y - 1, CellPos.z), t);
            DestroyTileRepeat(i, new Vector3Int(CellPos.x - 1, CellPos.y, CellPos.z), t);
            DestroyTileRepeat(i, new Vector3Int(CellPos.x - 1, CellPos.y + 1, CellPos.z), t);
            DestroyTileRepeat(i, new Vector3Int(CellPos.x, CellPos.y - 1, CellPos.z), t);
            DestroyTileRepeat(i, new Vector3Int(CellPos.x, CellPos.y, CellPos.z), t);
            DestroyTileRepeat(i, new Vector3Int(CellPos.x, CellPos.y + 1, CellPos.z), t);
            DestroyTileRepeat(i, new Vector3Int(CellPos.x + 1, CellPos.y - 1, CellPos.z), t);
            DestroyTileRepeat(i, new Vector3Int(CellPos.x + 1, CellPos.y, CellPos.z), t);
            DestroyTileRepeat(i, new Vector3Int(CellPos.x + 1, CellPos.y + 1, CellPos.z), t);
        }
    }

    void DestroyThing()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] col = Physics2D.OverlapPointAll(mouseWorld);
        if (col.Count() == 0)
            return ;
        foreach (var c in col)
        {
            if (tilemap)
            {
                DestroyTileRepeat(DestroyRange, tilemap.WorldToCell(mouseWorld), tilemap);
                // tilemap.GetComponent<CompositeCollider2D>().GenerateGeometry();
            }
            else
                GameObject.Destroy(c.gameObject);
        }
    }

    public void DestroyBlock()
    {
        TogglePauseMenu();
        // TODO: cursor
        // TODO: preview
    }

    public void CreatePlatform()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tilemap.SetTile(tilemap.WorldToCell(mouseWorld), platTile);
        tilemap.SetTile(tilemap.WorldToCell(mouseWorld) + Vector3Int.right, platTile);
        // GameObject.Instantiate(PlatformToCreate, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), PlatformToCreate.transform.rotation);
    }

    public void Boost()
    {

    }

    public void InitializeCreatePlatformPreview()
    {
        previewTiles.Clear();
    }

    public void PreviewCreatePlatform()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos1 = tilemap.WorldToCell(mouseWorld);
        Vector3Int tilePos2 = tilemap.WorldToCell(mouseWorld) + Vector3Int.right;

        // Replace tiles if needed
        if (!previewTiles.ContainsKey(tilePos1))
            previewTiles[tilePos1] = tilemap.GetTile(tilePos1);
        if (!previewTiles.ContainsKey(tilePos2))
            previewTiles[tilePos2] = tilemap.GetTile(tilePos2);

        // Reset existing tiles:
        foreach (var tile in previewTiles)
        {
            if (!(tile.Key == tilePos1 || tile.Key == tilePos2))
                tilemap.SetTile(tile.Key, tile.Value);
        }
    }

    public void AddPlatform()
    {
        TogglePauseMenu();
    }

    void SetMouse()
    {
        // TODO
        // Cursor.SetCursor(mouseCreate, new Vector2(mouseCreate.width / 2, mouseCreate.height / 2), CursorMode.Auto);
        // Cursor.SetCursor(mouseDestroy, new Vector2(mouseDestroy.width / 2, mouseDestroy.height / 2), CursorMode.Auto);
        // Cursor.SetCursor(mouseBase, new Vector2(mouseBase.width / 2, mouseBase.height / 2), CursorMode.Auto);
    }
}
