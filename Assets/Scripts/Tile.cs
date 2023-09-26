using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector2 coord;
    public Germ germ;

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

    public void SetGerm(GermState state)
    {
        germ.SetState(state);
    }
}