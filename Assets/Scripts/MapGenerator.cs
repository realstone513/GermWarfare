using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject tile;

    public int row = 5;
    public int col = 5;

    private Color black;
    private Color white;
    private List<List<GameObject>> tiles;
    private List<GameObject> objects;

    private void Awake()
    {
        black = Color.black;
        white = Color.white;
    }
    
    private void GenerateMap()
    {
        if (tiles != null)
        {
            for (int x = 0; x < row; x++)
                tiles[x].Clear();

            tiles.Clear();
        }

        float spawnPosRow = row % 2 == 1 ? -row / 2 : -row / 2 + 0.5f;
        float spawnPosCol = col % 2 == 1 ? -col / 2 : -col / 2 + 0.5f;

        tiles = new List<List<GameObject>>();
        for (int x = 0; x < row; x++)
        {
            tiles.Add(new List<GameObject>());
            for (int y = 0; y < col; y++)
            {
                Vector2 spawnPos = new (spawnPosRow + x, spawnPosCol + y);
                tiles[x].Add(Instantiate(tile, spawnPos, Quaternion.identity, transform));
                tiles[x][y].name = $"tile {x}, {y}";
                tiles[x][y].GetComponent<SpriteRenderer>().color =
                    ((x + y) % 2 == 1) ? black : white;

                objects.Add(tiles[x][y]);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GenerateMap();
        }
    }
}