using UnityEngine;

public class YoyoController : MonoBehaviour
{
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;


    [SerializeField] private GameObject yoyoPrefab;
    [SerializeField] private GameObject yoyoVisual;

    private Yoyo _activeYoyo;
    private SwingController _swingController;

    
 private bool CanLaunch 
{
    get { return !IsYoyoBusy && !IsSwinging; }
}

private bool IsYoyoBusy 
{
    get { return _activeYoyo != null && _activeYoyo.IsActive; }
}

private bool IsSwinging 
{
    get { return _swingController != null && (_swingController.IsSwinging || _swingController.IsLaunching); }
}

    private void Awake()
    {
        _swingController = GetComponent<SwingController>();
    }

    private void Update()
    {
        UpdateHandVisual();

        if (Input.GetKeyDown(attackKey) && CanLaunch)
        {
            TryLaunch();
        }
    }

    
    private void UpdateHandVisual()
    {
        if (yoyoVisual == null) return;

        // El visual de la mano se apaga si hay un yoyo 
        bool shouldShowVisual = !IsYoyoBusy && !IsSwinging;

        if (yoyoVisual.activeSelf != shouldShowVisual)
        {
            yoyoVisual.SetActive(shouldShowVisual);
        }
    }

    private void TryLaunch()
    {
        Vector2 aimDir = GetAimDirection();
        Vector2 spawnPos = (Vector2)transform.position + aimDir * 0.4f;

        if (YoyoPool.Instance != null)
        {
            GameObject go = YoyoPool.Instance.Get(spawnPos);
            if (go != null)
            {
                _activeYoyo = go.GetComponent<Yoyo>();
                _activeYoyo.Launch(aimDir, transform);
            }
        }

        UpdatePlayerDirection(aimDir);
    }

    private void UpdatePlayerDirection(Vector2 aimDir)
    {
        if (Mathf.Abs(aimDir.x) > 0.1f)
        {
            float direction = aimDir.x < 0 ? -1f : 1f;
            transform.localScale = new Vector3(direction, 1f, 1f);
        }
    }

    private Vector2 GetAimDirection()
    {
        if (Camera.main == null) return Vector2.right;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector2 dir = (mouseWorld - transform.position).normalized;

        // Si el mouse está sobre el jugador, lanzamos hacia donde mira el sprite
        if (dir.sqrMagnitude < 0.001f)
        {
            dir = transform.localScale.x >= 0 ? Vector2.right : Vector2.left;
        }

        return dir;
    }

   
    public void SetActiveYoyo(Yoyo yoyo)
    {
        _activeYoyo = yoyo;
    }
}