using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector2 coord;
    public Germ germ;

    public bool GermActive
    {
        get { return germ.gameObject.activeSelf; }
    }

    public Vector2 Coord
    {
        get { return coord; }
    }

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