using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public int width = 5;
    public int height = 5;
    public AZNotation test = new ();

    private Color[] colors = new Color[4];

    protected override void Awake()
    {
        base.Awake();
        colors[0] = Color.white;
        colors[1] = Color.black;
        colors[2] = Color.red;
        colors[3] = Color.blue;
    }

    private float timer = 0;
    private int it = 0;
    private bool stop = false;

    private void Update()
    {
        if (it >= 705)
            stop = true;

        if (!stop)
            timer += Time.deltaTime;

        if (timer >= 0.02f)
        {
            timer = 0f;
            Debug.Log($"{it}: {test.GetAZString(it++)}");
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