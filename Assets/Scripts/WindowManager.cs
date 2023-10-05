using UnityEngine;

public enum Windows
{
    None = -1,
    Main,
    Play,
}

public class WindowManager : MonoBehaviour
{
    public static WindowManager instance;

    public GenericWindow[] windows;
    public int currentWndId = -1;

    private void Awake()
    {
        instance = this;
    }

    public GenericWindow GetWindow(int id)
    {
        return windows[id];
    }

    public void ToggleWindow(int id)
    {
        for (int i = 0; i < windows.Length; i++)
        {
            if (i == id)
                windows[i].Open();
            else if (windows[i].gameObject.activeSelf)
                windows[i].Close();
        }
    }

    public void Open(Windows id)
    {
        currentWndId = (int)id;
        ToggleWindow(currentWndId);
    }

    public void AllClose()
    {
        foreach (var window in windows)
            window.Close();

        currentWndId = -1;
    }
}