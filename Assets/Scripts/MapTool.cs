using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CurrentButtonType
{
    Tile,
    Blank,
    Player1,
    Player2,
}

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
    private CurrentButtonType currentButtonType;

    private List<List<Tile>> tiles = new ();
    private GameManager gm;

    private void Start()
    {
        gm = GameManager.Instance;
        width = gm.width;
        height = gm.height;
        widthSlider.value = width;
        widthText.text = $"{widthSlider.value}";
        heightSlider.value = height;
        heightText.text = $"{heightSlider.value}";
        currentButtonType = CurrentButtonType.Tile;

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
    }

    private void SubmitButton()
    {
        // Tiles Clear
        foreach (List<Tile> tilesRow in tiles)
        {
            foreach (Tile tile in tilesRow)
            {
                Destroy(tile.gameObject);
            }
            tilesRow.Clear();
        }
        tiles.Clear();

        // Generate Map
        width = (int)widthSlider.value;
        height = (int)heightSlider.value;
        GenerateMap();
    }

    private void ReturnButton()
    {
        gm.LoadScene(Scenes.Title);
    }

    private void PlayButton()
    {
        gm.SetWH(width, height);
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
        currentButtonType = CurrentButtonType.Tile;
        SetCurrentTileText();
    }

    private void BlankButton()
    {
        currentButtonType = CurrentButtonType.Blank;
        SetCurrentTileText();
    }

    private void Player1Button()
    {
        currentButtonType = CurrentButtonType.Player1;
        SetCurrentTileText();
    }

    private void Player2Button()
    {
        currentButtonType = CurrentButtonType.Player2;
        SetCurrentTileText();
    }

    private void SetCurrentTileText()
    {
        switch (currentButtonType)
        {
            case CurrentButtonType.Tile:
                currentTileText.text = "Å¸ÀÏ";
                break;
            case CurrentButtonType.Blank:
                currentTileText.text = "ºó Ä­";
                break;
            case CurrentButtonType.Player1:
                currentTileText.text = "P1";
                break;
            case CurrentButtonType.Player2:
                currentTileText.text = "P2";
                break;
        }
    }

    private void GenerateMap()
    {
        //Color black = gm.GetColor(Colors.Black);
        Color white = gm.GetColor(Colors.White);

        float spawnPosXStart = width % 2 == 1 ? -width / 2 : -width / 2 + 0.5f;
        float spawnPosYStart = height % 2 == 1 ? -height / 2 : -height / 2 + 0.5f;

        //tiles = new List<List<Tile>>();
        for (int x = 0; x < width; x++)
        {
            tiles.Add(new List<Tile>());
            for (int y = 0; y < height; y++)
            {
                Vector2 spawnPos = new(spawnPosXStart + x, spawnPosYStart + y);
                GameObject tileInstance = Instantiate(tileObject, spawnPos, Quaternion.identity, transform);
                Tile tile = tileInstance.GetComponent<Tile>();
                //tile.SetTileData(x, y, (x + y) % 2 == 1 ? black : white);
                tile.SetTileData(x, y, white);
                tiles[x].Add(tile);
            }
        }
    }
}