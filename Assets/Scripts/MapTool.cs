using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapTool : MonoBehaviour
{
    public GameObject tileObject;

    private int width;
    private int height;

    // WH Setting
    public Button submitButton;
    public Button returnButton;
    public Button playButton;
    public Slider widthSlider;
    public Slider heightSlider;
    public TextMeshProUGUI widthText;
    public TextMeshProUGUI heightText;

    // Tile Setting
    public Button tileButton;
    public Button blankButton;
    public Button player1Button;
    public Button player2Button;
    public TextMeshProUGUI currentTileText;
    private TileType currentButtonType;

    private List<List<Tile>> tiles = new ();
    private RaycastHit2D hit;
    private GameManager gm;
    private Queue<Tile> unuseTiles = new ();
    private Queue<Tile> useTiles = new ();

    private void Start()
    {
        submitButton.onClick.AddListener(SubmitButton);
        returnButton.onClick.AddListener(ReturnButton);
        playButton.onClick.AddListener(PlayButton);
        widthSlider.onValueChanged.AddListener(WidthChange);
        heightSlider.onValueChanged.AddListener(HeightChange);
        tileButton.onClick.AddListener(TileButton);
        blankButton.onClick.AddListener(BlankButton);
        player1Button.onClick.AddListener(Player1Button);
        player2Button.onClick.AddListener(Player2Button);
        SetCurrentTileText();

        int count = (int) (widthSlider.maxValue * heightSlider.maxValue);
        for (int i = 0; i < count; i++)
        {
            GameObject tileInstance = Instantiate(tileObject, transform);
            unuseTiles.Enqueue(tileInstance.GetComponent<Tile>());
            tileInstance.SetActive(false);
        }
        for (int i = 0; i < width; i++)
            tiles.Add(new List<Tile>());

        gm = GameManager.Instance;
        width = gm.width;
        height = gm.height;
        widthSlider.value = width;
        widthText.text = $"{widthSlider.value}";
        heightSlider.value = height;
        heightText.text = $"{heightSlider.value}";
        currentButtonType = TileType.Tile;

        GenerateTempBoard();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(pos, Vector2.zero);

            if (hit.collider != null)
            {
                Tile targetTile = hit.collider.gameObject.GetComponent<Tile>();
                targetTile.TileType = currentButtonType;
                //Debug.Log(hit.collider.gameObject.name);
            }
        }
    }

    private void SubmitButton()
    {
        // Generate Map
        width = (int)widthSlider.value;
        height = (int)heightSlider.value;
        GenerateTempBoard();
    }

    private void ReturnButton()
    {
        gm.LoadScene(Scenes.Title);
    }

    private void PlayButton()
    {
        gm.isDefault = false;
        gm.SetWH(width, height);

        List<List<TileType>> tileTypes = new ();
        for (int x = 0; x < width; x++)
        {
            tileTypes.Add(new List<TileType>());
            for (int y = 0; y < height; y++)
            {
                tileTypes[x].Add(tiles[x][y].TileType);
            }
        }
        gm.TileTypes = tileTypes;
        gm.LoadScene(Scenes.Game);
    }

    private void WidthChange(Single value)
    {
        widthText.text = $"{widthSlider.value}";
        width = (int)value;
    }

    private void HeightChange(Single value)
    {
        heightText.text = $"{heightSlider.value}";
        height = (int)value;
    }

    private void TileButton()
    {
        currentButtonType = TileType.Tile;
        SetCurrentTileText();
    }

    private void BlankButton()
    {
        currentButtonType = TileType.Blank;
        SetCurrentTileText();
    }

    private void Player1Button()
    {
        currentButtonType = TileType.Player1;
        SetCurrentTileText();
    }

    private void Player2Button()
    {
        currentButtonType = TileType.Player2;
        SetCurrentTileText();
    }

    private void SetCurrentTileText()
    {
        switch (currentButtonType)
        {
            case TileType.Tile:
                currentTileText.text = "Å¸ÀÏ";
                break;
            case TileType.Blank:
                currentTileText.text = "ºó Ä­";
                break;
            case TileType.Player1:
                currentTileText.text = "P1";
                break;
            case TileType.Player2:
                currentTileText.text = "P2";
                break;
        }
    }

    private void ClearTiles()
    {
        while (useTiles.Count > 0)
        {
            Tile tile = useTiles.Dequeue();
            tile.gameObject.SetActive(false);
            unuseTiles.Enqueue(tile);
        }
        tiles.Clear();
    }

    private void GenerateTempBoard()
    {
        ClearTiles();
        Color white = gm.GetColor(Colors.White);

        // Set Screen(Panel) Center Position
        float spawnPosXStart = width % 2 == 1 ? -width / 2 : -width / 2 + 0.5f;
        float spawnPosYStart = height % 2 == 1 ? -height / 2 : -height / 2 + 0.5f;

        for (int x = 0; x < width; x++)
        {
            tiles.Add(new List<Tile>());
            for (int y = 0; y < height; y++)
            {
                Vector2 spawnPos = new (spawnPosXStart + x, spawnPosYStart + y);
                Tile tile = unuseTiles.Dequeue();
                tile.SetTileData(x, y, white, TileType.Tile);
                tile.transform.position = spawnPos;
                tile.gameObject.SetActive(true);
                tiles[x].Add(tile);
                useTiles.Enqueue(tile);
            }
        }
    }
}