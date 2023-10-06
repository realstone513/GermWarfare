using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public int width = 5;
    public int height = 5;
    public bool isDefault = true;

    private Color[] colors = new Color[4];
    private List<List<TileType>> tileTypes = new ();
    public List<List<TileType>> TileTypes
    {
        set
        {
            tileTypes = value;
        }
        get
        {
            return tileTypes;
        }
    }

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

    public void SetWH(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public void LoadScene(Scenes index)
    {
        SceneManager.LoadScene((int)index);
    }
}

public enum Players
{
    Player1,
    Player2,
}

public enum Scenes
{
    Title,
    Game,
    MapTool,
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

public enum TileType
{
    Tile,
    Player1,
    Player2,
    Blank,
}