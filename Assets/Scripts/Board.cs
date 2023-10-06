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

    private int player1Count = 0;
    private int player2Count = 0;
    public int curBoardTileCount = 0;
    public int curBoardBlankCount = 0;

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
        ChangeTurn(Players.Player1);
    }

    private void ShowOverlays(Tile target)
    {
        Vector2 targetCoord = target.Coord;
        foreach (Vector2 near in nearNeighborList)
        {
            if (CheckMovable(near + targetCoord))
                SetNearOverlay(target, near);
        }

        foreach (Vector2 far in farNeighborList)
        {
            if (CheckMovable(far + targetCoord))
                SetFarOverlay(target, far);
        }
    }

    private void SetNearOverlay(Tile target, Vector2 near)
    {
        GameObject nearObj = unuseNearOverlayQueue.Dequeue();
        Vector3 dest = target.transform.position;
        dest.x += near.x;
        dest.y += near.y;
        nearObj.transform.position = dest;
        nearObj.SetActive(true);
        useNearOverlayQueue.Enqueue(nearObj);
    }

    private void SetFarOverlay(Tile target, Vector2 far)
    {
        GameObject farObj = unuseFarOverlayQueue.Dequeue();
        Vector3 dest = target.transform.position;
        dest.x += far.x;
        dest.y += far.y;
        farObj.transform.position = dest;
        farObj.SetActive(true);
        useFarOverlayQueue.Enqueue(farObj);
    }

    private bool CheckMovable(Vector2 target)
    {
        int x = (int)target.x;
        int y = (int)target.y;

        if (CheckXBorder(x) == false || CheckYBorder(y) == false) // Check Border
            return false;
        if (tiles[x][y].TileType != TileType.Tile)
            return false;
        return true;
    }

    private bool CheckXBorder(int x)
    {
        return x >= 0 && x <= width - 1;
    }

    private bool CheckYBorder(int y)
    {
        return y >= 0 && y <= height - 1;
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
                            curSelectTile.TileType = TileType.Tile;
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
                    if ((isPlayer1 && targetTile.TileType == TileType.Player1) ||
                        (!isPlayer1 && targetTile.TileType  == TileType.Player2))
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
        TileType tileType = isPlayer1 ? TileType.Player1 : TileType.Player2; ;
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
            tile.TileType = tileType;
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
        player1Count = 0;
        player2Count = 0;

        bool canMove = false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x][y];
                if (tile.TileType == TileType.Player1)
                {
                    player1Count++;

                    if (canMove == false && isPlayer1 == true)
                        canMove = CheckCanMove(tile.Coord);
                }
                else if (tile.TileType == TileType.Player2)
                {
                    player2Count++;

                    if (canMove == false && isPlayer1 == false)
                        canMove = CheckCanMove(tile.Coord);
                }
            }
        }
        player1Score.text = $"{player1Count}";
        player2Score.text = $"{player2Count}";

        // End Condition
        if (canMove == false ||                                 // When there are no movable germ
            player1Count + player2Count == curBoardTileCount || // When the board is full
            player1Count == 0 || player2Count == 0)             // When destroy one player
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

    private bool CheckCanMove(Vector2 targetCoord)
    {
        foreach (Vector2 near in nearNeighborList)
        {
            if (CheckMovable(near + targetCoord))
                return true;
        }
        foreach (Vector2 far in farNeighborList)
        {
            if (CheckMovable(far + targetCoord))
                return true;
        }
        return false;
    }

    private void SetCustomBoard()
    {
        List<List<TileType>> tileTypes = gm.TileTypes;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x][y].TileType = tileTypes[x][y];
                if (tileTypes[x][y] == TileType.Blank)
                    curBoardBlankCount++;
            }
        }
        curBoardTileCount = width * height - curBoardBlankCount;
    }

    private void SetDefaultBoard()
    {
        tiles[0][height - 1].TileType = TileType.Player1;
        tiles[width - 1][0].TileType = TileType.Player1;

        tiles[0][0].TileType = TileType.Player2;
        tiles[width - 1][height - 1].TileType = TileType.Player2;
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