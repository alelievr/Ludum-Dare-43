using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.Audio;
using DG.Tweening;

public class GuiButtons : MonoBehaviour
{
    public enum Mode
    {
        Default,
        CreatePlatform,
        DestroyBlock,
    }

    [System.Serializable]
    public class ButtonList {
            public PlayerController.Key index;
            public GameObject guiButton;
            public GameObject guiLayer;
            public bool actif;
    }
    
    public List<ButtonList> buttonList = new List<ButtonList>();
    public AudioSource  guiEnterSound;
    public AudioSource  guiExitSound;
    public AudioMixer   backgroundMusicMixer;
    public GameObject   dealPanel;
    public Animator     guiButtonsAnimator;
    Mode               mode;
    
    [Header("Cusrsor setting")]
    public Texture2D mouseBase;
    public Texture2D mouseDestroy;
    public Texture2D mouseCreate;
    
    [Header("Deal settings")]
    public int DestroyRange = 1;

    // TileMap:
    public TileBase     platTile;
    bool                timeFreeze = false;
    bool                dealing = false;
    public Tilemap             tilemap;
    PlayerController    player;

    float defaultBackgroundMusicVolume;
    Dictionary< Vector3Int, TileBase > previewTiles = new Dictionary<Vector3Int, TileBase>();

    void Start()
    {
        backgroundMusicMixer.GetFloat("BackgroundMusicVolume", out defaultBackgroundMusicVolume);
        // tilemap = GameObject.FindObjectOfType< Tilemap >();
        player = GameObject.FindObjectOfType< PlayerController >();
    }

    void Update()
    {
        if (Input.GetKeyDown("e") && !dealing)
        {
            if (guiButtonsAnimator.GetBool("ShowGUI"))
                CloseDealMenu();
            else
                OpenDealMenu();
        }

        switch (mode)
        {
            case Mode.CreatePlatform:
                PreviewCreatePlatform();
                if (Input.GetMouseButtonDown(0))
                {
                    InstantiatePlatform();
                }
                break ;
            case Mode.DestroyBlock:
                PreviewDestroyPlatform();
                if (Input.GetMouseButtonDown(0))
                {
                    DestroyBlock();
                }
                break ; 
            default:
                break ;
        }

        Time.timeScale = (mode == Mode.Default && !timeFreeze) ? 1 : 0;
    }
    
    void UpdateMouse()
    {
        // TODO
        // Cursor.SetCursor(mouseCreate, new Vector2(mouseCreate.width / 2, mouseCreate.height / 2), CursorMode.Auto);
        // Cursor.SetCursor(mouseDestroy, new Vector2(mouseDestroy.width / 2, mouseDestroy.height / 2), CursorMode.Auto);
        // Cursor.SetCursor(mouseBase, new Vector2(mouseBase.width / 2, mouseBase.height / 2), CursorMode.Auto);
    }

    public bool GetButtonStatus(PlayerController.Key p_button)
    {
        ButtonList truc;
        return ((truc = buttonList.Where(c => c.index == p_button).FirstOrDefault()) != null ? truc.actif : false);
    }

    void OpenDealMenu()
    {
        backgroundMusicMixer.DOSetFloat("BackgroundMusicVolume", -25, .5f).SetUpdate(true);
        guiEnterSound.Play();
        timeFreeze = true;
        guiButtonsAnimator.SetBool("ShowGUI", true);
        guiButtonsAnimator.SetBool("SwitchEnable", false);
        guiButtonsAnimator.SetBool("HideGUI", false);
        guiButtonsAnimator.SetBool("HidePanel", false);
    }

    void HideDealOptions()
    {
        guiButtonsAnimator.SetBool("HideGUI", true);
    }

    void CloseDealMenu()
    {
        Debug.Log("defaultBackgroundMusicVolume: " + defaultBackgroundMusicVolume);
        backgroundMusicMixer.DOSetFloat("BackgroundMusicVolume", defaultBackgroundMusicVolume, .5f);
        guiExitSound.Play();
        timeFreeze = false;
        dealing = false;
        guiButtonsAnimator.SetBool("ShowGUI", false);
        guiButtonsAnimator.SetBool("HidePanel", true);
    }

    #region Actions

    IEnumerable< Vector3Int > GetDestroyPositions(Vector3Int pos)
    {
        yield return new Vector3Int(pos.x - 1, pos.y - 1, pos.z);
        yield return new Vector3Int(pos.x - 1, pos.y, pos.z);
        yield return new Vector3Int(pos.x - 1, pos.y + 1, pos.z);
        yield return new Vector3Int(pos.x, pos.y - 1, pos.z);
        yield return new Vector3Int(pos.x, pos.y, pos.z);
        yield return new Vector3Int(pos.x, pos.y + 1, pos.z);
        yield return new Vector3Int(pos.x + 1, pos.y - 1, pos.z);
        yield return new Vector3Int(pos.x + 1, pos.y, pos.z);
        yield return new Vector3Int(pos.x + 1, pos.y + 1, pos.z);
    }

    void DestroyTileRepeat(int i, Vector3Int CellPos, Tilemap t)
    {
        foreach (var pos in GetDestroyPositions(CellPos))
            t.SetTile(pos, null);
    }

    void DestroyBlock()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DestroyTileRepeat(DestroyRange, tilemap.WorldToCell(mouseWorld), tilemap);
        mode = Mode.Default;
        CloseDealMenu();
    }

    void InstantiatePlatform()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tilemap.SetTile(tilemap.WorldToCell(mouseWorld), platTile);
        tilemap.SetTile(tilemap.WorldToCell(mouseWorld) + Vector3Int.right, platTile);
        mode = Mode.Default;
        CloseDealMenu();
    }

    #endregion

    #region GUI setters

    public void DestroyBlockMode()
    {
        HideDealOptions();
        InitializePreview();
        mode = Mode.DestroyBlock;
        // TODO: cursor
    }

    public void CreatePlatformMode()
    {
        HideDealOptions();
        InitializePreview();
        mode = Mode.CreatePlatform;
        // TODO: cursor
    }

    public void Boost()
    {
        HideDealOptions();
        CloseDealMenu();
        player.Boost();
    }

    List<ButtonList> lastButton = new List<ButtonList>();

    public void DestroyButton(string p_key)
    {
        dealing = true;
        ButtonList button;
        button = buttonList.Where(c => c.index.ToString() == p_key).FirstOrDefault();
        lastButton.Add(button);
        if (button.actif)
        {
            button.actif = false;
            button.guiButton.GetComponent< Button >().interactable = false;
            button.guiLayer.SetActive(true);
            guiButtonsAnimator.SetBool("SwitchEnable", true);
        }
    }

    public void ReAddButton()
    {
        ButtonList button;
        button = lastButton.LastOrDefault();
        if (button != null)
        {
            button.actif = true;
            button.guiButton.GetComponent< Button >().interactable = true;
            button.guiLayer.SetActive(false);
            guiButtonsAnimator.SetBool("SwitchEnable", false);
            lastButton.RemoveAt(lastButton.Count - 1);
        }
    }

    #endregion

    #region Previews

    void InitializePreview()
    {
        previewTiles.Clear();
    }

    void PreviewCreatePlatform()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos1 = tilemap.WorldToCell(mouseWorld);
        Vector3Int tilePos2 = tilemap.WorldToCell(mouseWorld) + Vector3Int.right;

        // Replace tiles if needed
        if (!previewTiles.ContainsKey(tilePos1))
        {
            previewTiles[tilePos1] = tilemap.GetTile(tilePos1);
            tilemap.SetTile(tilePos1, platTile);
        }
        if (!previewTiles.ContainsKey(tilePos2))
        {
            previewTiles[tilePos2] = tilemap.GetTile(tilePos2);
            tilemap.SetTile(tilePos2, platTile);
        }

        // Reset existing tiles:
        List< Vector3Int > toRemove = new List<Vector3Int>();
        foreach (var tile in previewTiles)
        {
            if (!(tile.Key == tilePos1 || tile.Key == tilePos2))
            {
                tilemap.SetTile(tile.Key, tile.Value);
                toRemove.Add(tile.Key);
            }
        }

        foreach (var key in toRemove)
            previewTiles.Remove(key);
    }

    void PreviewDestroyPlatform()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        List< Vector3Int > destroyPositions = GetDestroyPositions(tilemap.WorldToCell(mouseWorld)).ToList();

        // Replace tiles if needed
        foreach (var pos in destroyPositions)
        {
            if (!previewTiles.ContainsKey(pos))
            {
                previewTiles[pos] = tilemap.GetTile(pos);
                tilemap.SetTile(pos, null);
            }
        }

        // Reset existing tiles:
        List< Vector3Int > toRemove = new List<Vector3Int>();
        foreach (var tile in previewTiles)
        {
            if (!(destroyPositions.Contains(tile.Key)))
            {
                tilemap.SetTile(tile.Key, tile.Value);
                toRemove.Add(tile.Key);
            }
        }

        foreach (var key in toRemove)
            previewTiles.Remove(key);
    }

    #endregion
}
