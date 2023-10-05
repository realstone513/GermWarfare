using UnityEngine;

public class GenericWindow : MonoBehaviour
{
    protected virtual void Awake()
    {
        Close();
    }

    protected void Display(bool active)
    {
        gameObject.SetActive(active);
    }

    public virtual void Open()
    {
        Display(true);
    }

    public virtual void Close()
    {
        Display(false);
    }
}