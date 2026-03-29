using UnityEngine;

public class SwingPoint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
       
        SwingController swingcontroller = other.GetComponent<SwingController>();

       
        if (swingcontroller != null)
        {
            swingcontroller.RegisterPoint(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
      
        SwingController swingcontroller = other.GetComponent<SwingController>();

        
        if (swingcontroller != null)
        {
            swingcontroller.UnregisterPoint(this);
        }
    }

    public void SetHighlight(bool on)
    {
       
        if (spriteRenderer != null)
        {
            if (on == true)
            {
                spriteRenderer.color = Color.yellow;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }
}