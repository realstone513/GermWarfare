using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public GameObject tileObject;

    private int width;
    private int height;

    public bool isPlayer1 = true;
    public bool canPut = false;

    public TextMeshProUGUI player1Score;
    public TextMeshProUGUI player2Score;
    public TextMeshProUGUI turnDisplay;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public Button retryButton;
    public Button exitButton;

    private List<List<Tile>> tiles;
    private RaycastHit2D hit;
    private GameManager gm;
    private Tile curSelectTile;
    private List<Vector2> neighborList;

    private void Start()
    {
        gm = GameManager.Instance;
        height = gm.height;
        width = gm.width;
        GenerateMap();
        SetStartPoint();
        ChangeTurn(1);

        neighborList = new();
        for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
                neighborList.Add(new Vector2(i, j));

        resultPanel.SetActive(false);
        retryButton.onClick.AddListener(RetryButton);
        exitButton.onClick.AddListener(ExitButton);
    }

    private void RetryButton()
    {
        Debug.Log("Load game scene");
        SceneManager.LoadScene(0);
    }

    private void ExitButton()
    {
        Debug.Log("Load title scene");
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
                    if (targetTile == curSelectTile) // cancel
                    {
                        SwitchCanPut(false);
                        // overlay off
                        return;
                    }

                    if (!targetTile.GermActive) // empty tile
                    {
                        Vector2 originCoord = curSelectTile.Coord;
                        Vector2 curCoord = targetTile.Coord;

                        int diffX = Mathf.Abs((int)(originCoord.x - curCoord.x));
                        int diffY = Mathf.Abs((int)(originCoord.y - curCoord.y));

                        if (CheckNearTile(diffX, diffY)) // Near
                        {
                            ChangeNeighborGerm(targetTile);
                            ChangeTurn(isPlayer1 ? 2 : 1);
                        }
                        else if (CheckFarTile(diffX, diffY)) // Far
                        {
                            curSelectTile.SetGerm(GermState.Inactive);
                            ChangeNeighborGerm(targetTile);
                            ChangeTurn(isPlayer1 ? 2 : 1);
                        }
                        else
                        {
                            SwitchCanPut(false);
                            // overlay off
                        }
                    }
                }
                else
                {
                    if ((isPlayer1 && targetTile.germ.germState == GermState.Player1) ||
                        (!isPlayer1 && targetTile.germ.germState == GermState.Player2))
                    {
                        SwitchCanPut(true, targetTile);
                        // overlay on
                    }
                }
                // Debug.Log(hit.collider.gameObject.name);
            }
        }
    }

    private void ChangeNeighborGerm(Tile dest)
    {
        GermState targetState = isPlayer1 ? GermState.Player1 : GermState.Player2;
        Vector2 coord = dest.Coord;

        foreach (Vector2 neighbor in neighborList)
        {
            Vector2 neighborCoord = coord + neighbor;
            Tile target = GetTile(neighborCoord);
            if (target != null && target.GermActive)
            {
                GetTile(neighborCoord).SetGerm(targetState);
            }
        }
        dest.SetGerm(targetState);
    }

    private Tile GetTile(Vector2 coord)
    {
        if (coord.x < 0 || coord.x > width - 1 || coord.y < 0 || coord.y > height - 1)
            return null;
        return tiles[(int)coord.x][(int)coord.y];
    }

    private bool CheckNearTile(int diffX, int diffY)
    {
        if ((diffX == 1 && diffY == 0) || (diffX == 0 && diffY == 1)) // up down left right 1
            return true;
        if (diffX == 1 && diffY == 1) // diagonal 1
            return true;
        return false;
    }

    private bool CheckFarTile(int diffX, int diffY)
    {
        if ((diffX == 2 && diffY == 0) || (diffX == 0 && diffY == 2)) // up down left right 2
            return true;
        if (diffX + diffY == 3 && !(diffX == 0 || diffY == 0)) // 1,2 2,1
            return true;
        if (diffX == 2 && diffY == 2) // diagonal 2
            return true;
        return false;
    }

    private void SwitchCanPut(bool value, Tile curTile = null)
    {
        if (value)
        {
            curSelectTile = curTile;
            canPut = true;
        }
        else
        {
            curSelectTile = null;
            canPut = false;
        }
    }

    private void ChangeTurn(int num)
    {
        if (num == 1) // Player1
        {
            turnDisplay.text = "P1";
            turnDisplay.color = gm.GetColor(Colors.Red);
            isPlayer1 = true;
        }
        else if (num == 2) // Player2
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
        if (player1Count + player2Count == width * height || player1Count == 0 || player2Count == 0)
        {
            resultPanel.SetActive(true);
            if (player1Count > player2Count)
                resultText.text = $"Player1 Win!";
            else if (player1Count < player2Count)
                resultText.text = $"Player2 Win!";
            else
                resultText.text = $"Draw!";
        }
    }

    private void SetStartPoint()
    {
        // from map info. this is test
        tiles[0][height - 1].SetGerm(GermState.Player1);
        tiles[width - 1][0].SetGerm(GermState.Player1);

        tiles[0][0].SetGerm(GermState.Player2);
        tiles[width - 1][height - 1].SetGerm(GermState.Player2);
    }

    private void GenerateMap()
    {
        //Color black = gm.GetColor(Colors.Black);
        Color white = gm.GetColor(Colors.White);

        float spawnPosXStart = width % 2 == 1 ? -width / 2 : -width / 2 + 0.5f;
        float spawnPosYStart = height % 2 == 1 ? -height / 2 : -height / 2 + 0.5f;

        tiles = new List<List<Tile>>();
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