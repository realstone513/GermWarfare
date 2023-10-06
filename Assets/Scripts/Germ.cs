using UnityEngine;

public class Germ : MonoBehaviour
{
    private GermState germState;
    public GermState GermState
    {
        get { return germState; }
        set {
            germState = value;
            if (germState == GermState.Inactive)
                gameObject.SetActive(false);
            else
            {
                gameObject.SetActive(true);
                if (germState == GermState.Player1)
                    spriteRenderer.color = GameManager.Instance.GetColor(Colors.Red);
                else if (germState == GermState.Player2)
                    spriteRenderer.color = GameManager.Instance.GetColor(Colors.Blue);
            }
        }
    }
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }
}