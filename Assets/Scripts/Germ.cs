using UnityEngine;

public enum GermState
{
    Inactive,
    Player1,
    Player2,
}

public class Germ : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    public void SetState(GermState state)
    {
        if (state == GermState.Inactive)
            gameObject.SetActive(false);
        else
        {
            if (state != GermState.Player1)
                spriteRenderer.color = Color.red;
            else if (state != GermState.Player2)
                spriteRenderer.color = Color.blue;
            gameObject.SetActive(true);
        }
    }
}