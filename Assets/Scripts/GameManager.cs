using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public int width = 5;
    public int height = 5;
    public AZNotation test = new ();
    public long testInput;
    public int azIndex;

    private Color[] colors = new Color[4];

    protected override void Awake()
    {
        base.Awake();
        colors[0] = Color.white;
        colors[1] = Color.black;
        colors[2] = Color.red;
        colors[3] = Color.blue;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            test.amount += testInput;
            Debug.Log($"{test.MakeString()}");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            test.amount = 0;
            Debug.Log("clear");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log($"{test.GetAZString(azIndex)}");
        }
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

public enum Scenes
{
    Title,
    Game,
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