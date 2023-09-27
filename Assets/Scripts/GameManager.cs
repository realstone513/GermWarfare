using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int row = 5;
    public int col = 5;

    private Color[] colors = new Color[4];

    protected override void Awake()
    {
        base.Awake();
        colors[0] = Color.white;
        colors[1] = Color.black;
        colors[2] = Color.red;
        colors[3] = Color.blue;
    }

    public Color GetColor(Colors color)
    {
        return colors[(int)color];
    }
}

public enum GermState
{
    Inactive,
    Player1,
    Player2,
}

public enum Colors
{
    White,
    Black,
    Red,
    Blue,
}