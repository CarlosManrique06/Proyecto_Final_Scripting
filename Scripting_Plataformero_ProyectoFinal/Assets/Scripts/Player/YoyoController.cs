using UnityEngine;


public class YoyoController : MonoBehaviour
{
    public GameObject yoyoPrefab;

    
    public Vector2 spawnOffset = new Vector2(0.5f, 0f);

    
    public KeyCode attackKey = KeyCode.Mouse0;

    
    private Yoyo activeYoyo;
    private SpriteRenderer spriteRenderer; 

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(attackKey))
            TryLaunchYoyo();
    }

    void TryLaunchYoyo()
    {
        // Un solo yoyo activo a la vez
        if (activeYoyo != null && activeYoyo.isActive)
            return;

       
        float facingDir = GetFacingDirection();
        Vector2 direction = new Vector2(facingDir, 0f);

        
        Vector2 spawnPos = (Vector2)transform.position + new Vector2(spawnOffset.x * facingDir, spawnOffset.y);

        GameObject go = Instantiate(yoyoPrefab, spawnPos, Quaternion.identity);
        activeYoyo = go.GetComponent<Yoyo>();
        activeYoyo.Launch(direction, transform); 
    }

    float GetFacingDirection()
    {
        
        if (transform.localScale.x < 0f) return -1f;
        return 1f;

       
    }
}