using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public Button gameButton;
    public Button mapToolButton;
    public Button exitButton;
    public Button playButton;
    public Button returnButton;
    public Slider widthSlider;
    public Slider heightSlider;
    public TextMeshProUGUI widthText;
    public TextMeshProUGUI heightText;

    private int width;
    private int height;
    private WindowManager wm;
    private GameManager gm;

    private void Awake()
    {
        gm = GameManager.Instance;
        wm = gameObject.GetComponent<WindowManager>();
        width = 5;
        height = 5;
    }

    private void Start()
    {
        wm.Open(Windows.Main);
        gameButton.onClick.AddListener(GameButton);
        mapToolButton.onClick.AddListener(MapToolButton);
        exitButton.onClick.AddListener(ExitButton);
        playButton.onClick.AddListener(PlayButton);
        returnButton.onClick.AddListener(ReturnButton);
        widthSlider.onValueChanged.AddListener(WidthChange);
        heightSlider.onValueChanged.AddListener(HeightChange);
    }

    private void WidthChange(Single value)
    {
        widthText.text = $"{widthSlider.value}";
        width = (int)value;
    }

    private void HeightChange(Single value)
    {
        heightText.text = $"{heightSlider.value}";
        height = (int)value;
    }

    private void GameButton()
    {
        wm.Open(Windows.Play);
    }

    private void MapToolButton()
    {
        gm.LoadScene(Scenes.MapTool);
    }

    private void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void PlayButton()
    {
        gm.SetWH(width, height);
        gm.LoadScene(Scenes.Game);
    }

    private void ReturnButton()
    {
        wm.Open(Windows.Main);
    }
}