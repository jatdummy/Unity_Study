using UnityEngine;

public class Tile : MonoBehaviour
{
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer가 필요해요 ");
        }

    }
    SpriteRenderer spriteRenderer;


    public Color color
    {
        set { spriteRenderer.color = value; }
        get { return spriteRenderer.color; }
    }

    public int sortingOrder
    {
        set
        {
            spriteRenderer.sortingOrder = value;
        }
        get
        {
            return spriteRenderer.sortingOrder;
        }

    }
}
