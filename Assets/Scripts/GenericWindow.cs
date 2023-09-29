using UnityEngine;
using UnityEngine.EventSystems;

public class GenericWindow : MonoBehaviour
{
    public GameObject firstSelected;

    protected virtual void Awake()
    {
        Close();
    }

    public void OnFocus()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    protected void Display(bool active)
    {
        gameObject.SetActive(active);
    }

    public virtual void Open()
    {
        Display(true);
        OnFocus();
    }

    public virtual void Close()
    {
        Display(false);
    }
}