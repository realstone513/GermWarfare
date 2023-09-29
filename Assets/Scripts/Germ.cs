using UnityEngine;

public class Germ : MonoBehaviour
{
    public GermState germState;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    public void SetState(GermState state)
    {
        germState = state;
        if (state == GermState.Inactive)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(true);
            if (state == GermState.Player1)
                spriteRenderer.color = GameManager.Instance.GetColor(Colors.Red);
            else if (state == GermState.Player2)
                spriteRenderer.color = GameManager.Instance.GetColor(Colors.Blue);
        }
    }
}