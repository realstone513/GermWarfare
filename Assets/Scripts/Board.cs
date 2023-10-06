using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public GameObject tileObject;

    private int width;
    private int height;

    public bool isPlayer1 = true;
    public bool canPut = false;
    private bool isPause = false;
    private AZNotation player1Gold = new();
    private AZNotation player2Gold = new();
    private float gainGoldMin = 100000000;
    private float gainGoldMax = 10000000000;

    public TextMeshProUGUI player1Score;
    public TextMeshProUGUI player2Score;
    public TextMeshProUGUI player1GoldText;
    public TextMeshProUGUI player2GoldText;
    public TextMeshProUGUI turnDisplay;
    public GameObject announcePanel;
    public TextMeshProUGUI announceText;
    public Button titleButton;
    public Button retryButton;
    public Button optionButton;
    public TextMeshProUGUI retryText;

    public GameObject nearOverlay;
    public GameObject farOverlay;
    public Transform nearTransform;
    public Transform farTransform;

    private Queue<GameObject> unuseNearOverlayQueue = new();
    private Queue<GameObject> unuseFarOverlayQueue = new();
    private Queue<GameObject> useNearOverlayQueue = new();
    private Queue<GameObject> useFarOverlayQueue = new();

    private List<List<Tile>> tiles = new();
    private RaycastHit2D hit;
    private GameManager gm;
    private Tile curSelectTile;
    private List<Vector2> nearNeighborList;
    private List<Vector2> farNeighborList;

    private void Start()
    {
        gm = GameManager.Instance;
        height = gm.height;
        width = gm.width;
        GenerateMap();
        if (gm.isDefault)
            SetDefaultBoard();
        else
            SetCustomBoard();
        ChangeTurn(Players.Player1);

        nearNeighborList = new List<Vector2>();
        farNeighborList = new List<Vector2>();
        for (int i = -2; i <= 2; i++)
            for (int j = -2; j <= 2; j++)
            {
                Vector2 vector = new (i, j);
                if (i >= -1 && i <= 1 && j >= -1 && j <= 1) // i (-1, 1) && j (-1, 1)
                    nearNeighborList.Add(vector);
                else
                    farNeighborList.Add(vector);
            }
        nearNeighborList.Remove(Vector2.zero);

        announcePanel.SetActive(false);
        retryButton.onClick.AddListener(RetryButton);
        titleButton.onClick.AddListener(TitleButton);
        optionButton.onClick.AddListener(OptionButton);

        int nearCount = nearNeighborList.Count;
        for (int i = 0; i < nearCount; i++)
        {
            GameObject nearObj = Instantiate(nearOverlay, nearTransform);
            unuseNearOverlayQueue.Enqueue(nearObj);
            nearObj.SetActive(false);
        }

        int farCount = farNeighborList.Count;
        for (int i = 0; i < farCount; i++)
        {
            GameObject farObj = Instantiate(farOverlay, farTransform);
            unuseFarOverlayQueue.Enqueue(farObj);
            farObj.SetActive(false);
        }
    }

    private void ShowOverlays(Tile target)
    {
        Vector2 targetCoord = target.Coord;
        foreach (Vector2 near in nearNeighborList)
        {
            Vector2 neighborCoord = near + targetCoord;

            if (CheckBorder(neighborCoord))
            {
                if (tiles[(int)neighborCoord.x][(int)neighborCoord.y].TileType == TileType.Blank)
                    continue;

                GameObject nearObj = unuseNearOverlayQueue.Dequeue();
                Vector3 dest = target.transform.position;
                dest.x += near.x;
                dest.y += near.y;
                nearObj.transform.position = dest;
                nearObj.SetActive(true);
                useNearOverlayQueue.Enqueue(nearObj);
            }
        }

        foreach (Vector2 far in farNeighborList)
        {
            Vector2 neighborCoord = far + targetCoord;

            if (CheckBorder(neighborCoord))
            {
                if (tiles[(int)neighborCoord.x][(int)neighborCoord.y].TileType == TileType.Blank)
                    continue;

                GameObject farObj = unuseFarOverlayQueue.Dequeue();
                Vector3 dest = target.transform.position;
                dest.x += far.x;
                dest.y += far.y;
                farObj.transform.position = dest;
                farObj.SetActive(true);
                useFarOverlayQueue.Enqueue(farObj);
            }
        }
    }

    private bool CheckBorder(Vector2 target)
    {
        if (target.x >= 0 && target.x <= width - 1 && target.y >= 0 && target.y <= height - 1)
            return true;
        return false;
    }

    private void HideOverlays()
    {
        while (useNearOverlayQueue.Count > 0)
        {
            GameObject near = useNearOverlayQueue.Dequeue();
            near.SetActive(false);
            unuseNearOverlayQueue.Enqueue(near);
        }
        useNearOverlayQueue.Clear();

        while (useFarOverlayQueue.Count > 0)
        {
            GameObject far = useFarOverlayQueue.Dequeue();
            far.SetActive(false);
            unuseFarOverlayQueue.Enqueue(far);
        }
        useFarOverlayQueue.Clear();
    }

    private void OptionButton()
    {
        announcePanel.SetActive(true);
        announceText.text = "Pause";
        isPause = true;
    }

    private void TitleButton()
    {
        gm.LoadScene(Scenes.Title);
    }

    private void RetryButton()
    {
        if (isPause)
        {
            announcePanel.SetActive(false);
            isPause = false;
        }
        else
            gm.LoadScene(Scenes.Game);
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

                if (canPut)
                {
                    if (targetTile == curSelectTile) // Cancel
                    {
                        SwitchCanPut(false);
                        HideOverlays();
                        return;
                    }

                    if (!targetTile.GermActive) // Empty Tile
                    {
                        Vector2 originCoord = curSelectTile.Coord;
                        Vector2 curCoord = targetTile.Coord;

                        int diffX = Mathf.Abs((int)(originCoord.x - curCoord.x));
                        int diffY = Mathf.Abs((int)(originCoord.y - curCoord.y));

                        if (targetTile.TileType == TileType.Blank)
                            SwitchCanPut(false);
                        else if (CheckNearTile(diffX, diffY)) // Near
                        {
                            ChangeNeighborGerm(targetTile, true);
                            ChangeTurn(isPlayer1 ? Players.Player2 : Players.Player1);
                        }
                        else if (CheckFarTile(diffX, diffY)) // Far
                        {
                            curSelectTile.SetGerm(GermState.Inactive);
                            ChangeNeighborGerm(targetTile, false);
                            ChangeTurn(isPlayer1 ? Players.Player2 : Players.Player1);
                        }
                        else
                        {
                            SwitchCanPut(false);
                        }
                        HideOverlays();
                    }
                }
                else
                {
                    if ((isPlayer1 && targetTile.germ.germState == GermState.Player1) ||
                        (!isPlayer1 && targetTile.germ.germState == GermState.Player2))
                    {
                        SwitchCanPut(true, targetTile);
                        ShowOverlays(targetTile);
                    }
                }
                // Debug.Log(hit.collider.gameObject.name);
            }
        }
    }

    private void ChangeNeighborGerm(Tile dest, bool isNear)
    {
        GermState targetState = isPlayer1 ? GermState.Player1 : GermState.Player2;
        Vector2 coord = dest.Coord;
        List<Tile> changeTiles = new() { dest };
        foreach (Vector2 neighbor in nearNeighborList)
        {
            Vector2 neighborCoord = coord + neighbor;
            Tile target = GetTile(neighborCoord);
            if (target != null && target.GermActive)
                changeTiles.Add(target);
        }

        foreach (Tile tile in changeTiles)
        {
            tile.SetGerm(targetState);
            if (tile == dest && isNear == false)
                continue;
            if (isPlayer1)
                player1Gold += GetGold(gainGoldMin, gainGoldMax);
            else
                player2Gold += GetGold(gainGoldMin, gainGoldMax);
        }
        player1GoldText.text = $"{player1Gold.Text}";
        player2GoldText.text = $"{player2Gold.Text}";
    }

    private Tile GetTile(Vector2 coord)
    {
        if (coord.x < 0 || coord.x > width - 1 || coord.y < 0 || coord.y > height - 1)
            return null;
        return tiles[(int)coord.x][(int)coord.y];
    }

    private float GetGold(float min, float max)
    {
        return Random.Range(min, max);
    }

    private bool CheckNearTile(int diffX, int diffY)
    {
        if ((diffX == 1 && diffY == 0) || (diffX == 0 && diffY == 1)) // Up Down Left Right 1
            return true;
        if (diffX == 1 && diffY == 1) // Diagonal 1
            return true;
        return false;
    }

    private bool CheckFarTile(int diffX, int diffY)
    {
        if ((diffX == 2 && diffY == 0) || (diffX == 0 && diffY == 2)) // Up Down Left Right 2
            return true;
        if (diffX + diffY == 3 && !(diffX == 0 || diffY == 0)) // 1,2 2,1
            return true;
        if (diffX == 2 && diffY == 2) // Diagonal 2
            return true;
        return false;
    }

    private void SwitchCanPut(bool value, Tile curTile = null)
    {
        canPut = value;
        curSelectTile = canPut ? curTile : null;
    }

    private void ChangeTurn(Players playerIndex)
    {
        if (playerIndex == Players.Player1) // Player1
        {
            turnDisplay.text = "P1";
            turnDisplay.color = gm.GetColor(Colors.Red);
            isPlayer1 = true;
        }
        else // Player2
        {
            turnDisplay.text = "P2";
            turnDisplay.color = gm.GetColor(Colors.Blue);
            isPlayer1 = false;
        }

        SwitchCanPut(false);
        CheckBoardScore();
    }

    private void CheckBoardScore()
    {
        int player1Count = 0;
        int player2Count = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GermState state = tiles[x][y].germ.germState;
                if (state == GermState.Player1)
                    player1Count++;
                else if (state == GermState.Player2)
                    player2Count++;
            }
        }
        player1Score.text = $"{player1Count}";
        player2Score.text = $"{player2Count}";

        // End Condition
        // TODO: 이동 불가능 할 때 추가
        if (player1Count + player2Count == width * height || player1Count == 0 || player2Count == 0)
        {
            announcePanel.SetActive(true);
            if (player1Count > player2Count)
                announceText.text = $"Player1 Win!";
            else if (player1Count < player2Count)
                announceText.text = $"Player2 Win!";
            else
                announceText.text = $"Draw!";
            retryText.text = "다시하기";
        }
    }

    private void SetCustomBoard()
    {
        List<List<TileType>> tileTypes = gm.TileTypes;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x][y].TileType = tileTypes[x][y];
            }
        }
    }

    private void SetDefaultBoard()
    {
        tiles[0][height - 1].SetGerm(GermState.Player1);
        tiles[width - 1][0].SetGerm(GermState.Player1);

        tiles[0][0].SetGerm(GermState.Player2);
        tiles[width - 1][height - 1].SetGerm(GermState.Player2);
    }

    private void GenerateMap()
    {
        Color white = gm.GetColor(Colors.White);

        // Set Screen(Panel) Center Position
        float spawnPosXStart = width % 2 == 1 ? -width / 2 : -width / 2 + 0.5f;
        float spawnPosYStart = height % 2 == 1 ? -height / 2 : -height / 2 + 0.5f;

        for (int x = 0; x < width; x++)
        {
            tiles.Add(new List<Tile>());
            for (int y = 0; y < height; y++)
            {
                Vector2 spawnPos = new(spawnPosXStart + x, spawnPosYStart + y);
                GameObject tileInstance = Instantiate(tileObject, spawnPos, Quaternion.identity, transform);
                Tile tile = tileInstance.GetComponent<Tile>();
                tile.SetTileData(x, y, white);
                tiles[x].Add(tile);
            }
        }
    }
}