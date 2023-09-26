using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector2 coord;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTileData(string name, float x, float y, Color color)
    {
        gameObject.name = name;
        coord.x = x;
        coord.y = y;
        spriteRenderer.color = color;
    }
}