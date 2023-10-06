using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector2 coord;
    private TileType tileType;
    public TileType TileType
    {
        get { return tileType; }
        set
        {
            tileType = value;
            switch (tileType)
            {
                case TileType.Tile:
                case TileType.Player1:
                case TileType.Player2:
                    SwitchIsBlank(false);
                    SetGerm((GermState)value);
                    break;
                case TileType.Blank:
                    SetGerm(GermState.Inactive);
                    SwitchIsBlank(true);
                    break;
            }
        }
    }
    public Germ germ;
    private bool isBlank;
    
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

    public void SwitchIsBlank(bool value)
    {
        isBlank = value;
        spriteRenderer.color = GameManager.Instance.GetColor(value ? Colors.Black : Colors.White);
    }

    public void SetTileData(float x, float y, Color color)
    {
        gameObject.name = $"Tile ({x}, {y})";
        coord.x = x;
        coord.y = y;
        spriteRenderer.color = color;
    }

    public void SetGerm(GermState state)
    {
        if (isBlank)
            return;

        germ.SetState(state);
    }
}