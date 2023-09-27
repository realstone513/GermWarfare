using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject tileObject;

    private int row;
    private int col;

    public bool isPlayer1 = true;
    public bool canPut = false;

    private List<List<GameObject>> tiles;
    private RaycastHit2D hit;
    private GameManager gm;
    private Tile curSelectTile;

    private void Start()
    {
        gm = GameManager.Instance;
        row = gm.row;
        col = gm.col;
        GenerateMap();
        SetStartPoint();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(pos, Vector2.zero);

            if (hit.collider != null)
            {
                Tile curTile = hit.collider.gameObject.GetComponent<Tile>();

                if (canPut)
                {
                    if (curTile == curSelectTile) // cancel
                    {
                        SwitchCanPut(false);
                    }

                    if (!curTile.GermActive)
                    {
                        Vector2 originCoord = curSelectTile.Coord;
                        Vector2 curCoord = curTile.Coord;

                        int diffX = Mathf.Abs((int)(originCoord.x - curCoord.x));
                        int diffY = Mathf.Abs((int)(originCoord.y - curCoord.y));

                        if (CheckNearTile(diffX, diffY))
                        {
                            Debug.Log("near");

                            curTile.SetGerm(isPlayer1 ? GermState.Player1 : GermState.Player2);
                            isPlayer1 = !isPlayer1;
                            SwitchCanPut(false);

                        }
                        else if (CheckFarTile(diffX, diffY))
                        {
                            Debug.Log("far");
                            curSelectTile.SetGerm(GermState.Inactive);

                            curTile.SetGerm(isPlayer1 ? GermState.Player1 : GermState.Player2);
                            isPlayer1 = !isPlayer1;
                            SwitchCanPut(false);
                        }
                        else
                        {
                            Debug.Log("can not put");
                            SwitchCanPut(false);
                        }
                    }
                }
                else
                {
                    if (curTile.GermActive)
                    {
                        SwitchCanPut(true, curTile);
                    }
                }
                // Debug.Log(hit.collider.gameObject.name);
            }
        }
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
        if (diffX + diffY == 3 && (diffX != 0 && diffY != 0)) // diagonal 2
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
            Debug.Log("can put mode on");
        }
        else
        {
            curSelectTile = null;
            canPut = false;
            Debug.Log("can put mode off");
        }
    }

    private void SetStartPoint()
    {
        tiles[0][0].GetComponent<Tile>().SetGerm(GermState.Player1);
        tiles[row - 1][col - 1].GetComponent<Tile>().SetGerm(GermState.Player1);

        tiles[0][col - 1].GetComponent<Tile>().SetGerm(GermState.Player2);
        tiles[row - 1][0].GetComponent<Tile>().SetGerm(GermState.Player2);
    }

    private void GenerateMap()
    {
        Color black = gm.GetColor(Colors.Black);
        Color white = gm.GetColor(Colors.White);

        float spawnPosXStart = row % 2 == 1 ? -row / 2 : -row / 2 + 0.5f;
        float spawnPosYStart = col % 2 == 1 ? -col / 2 : -col / 2 + 0.5f;

        tiles = new List<List<GameObject>>();
        for (int x = 0; x < row; x++)
        {
            tiles.Add(new List<GameObject>());
            for (int y = 0; y < col; y++)
            {
                Vector2 spawnPos = new(spawnPosXStart + x, spawnPosYStart + y);
                tiles[x].Add(Instantiate(tileObject, spawnPos, Quaternion.identity, transform));
                Tile tile = tiles[x][y].GetComponent<Tile>();
                tile.SetTileData($"tile {x}, {y}", x, y, (x + y) % 2 == 1 ? black : white);
            }
        }
    }
}