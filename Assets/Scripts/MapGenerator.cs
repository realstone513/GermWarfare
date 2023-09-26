using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject tileObject;

    public int row = 5;
    public int col = 5;

    private List<List<GameObject>> tiles;
    private RaycastHit2D hit;

    private void Awake()
    {
        GenerateMap();
    }
    
    private void GenerateMap()
    {
        Color black = Color.black;
        Color white = Color.white;

        float spawnPosXStart = row % 2 == 1 ? -row / 2 : -row / 2 + 0.5f;
        float spawnPosYStart = col % 2 == 1 ? -col / 2 : -col / 2 + 0.5f;

        tiles = new List<List<GameObject>>();
        for (int x = 0; x < row; x++)
        {
            tiles.Add(new List<GameObject>());
            for (int y = 0; y < col; y++)
            {
                Vector2 spawnPos = new (spawnPosXStart + x, spawnPosYStart + y);
                tiles[x].Add(Instantiate(tileObject, spawnPos, Quaternion.identity, transform));
                Tile tile = tiles[x][y].GetComponent<Tile>();
                tile.SetTileData($"tile {x}, {y}", x, y, (x + y) % 2 == 1 ? black : white);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(pos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
            }
        }
    }
}