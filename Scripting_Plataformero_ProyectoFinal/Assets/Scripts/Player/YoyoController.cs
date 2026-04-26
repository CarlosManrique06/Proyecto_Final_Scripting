using UnityEngine;

public class YoyoController : MonoBehaviour
{
    public KeyCode attackKey = KeyCode.Mouse0;

    private Yoyo activeYoyo;
    private SwingController swingController;

    void Awake()
    {
        swingController = GetComponent<SwingController>();
    }

    void Update()
    {
        if (swingController != null && (swingController.IsSwinging || swingController.IsLaunching)) return;
        if (Input.GetKeyDown(attackKey)) TryLaunch();
    }

    void TryLaunch()
    {
        if (activeYoyo != null && activeYoyo.isActive) return;

        Vector2 aimDir = GetAimDirection();
        Vector2 spawnPos = (Vector2)transform.position + aimDir * 0.4f;

        // Pool en vez de Instantiate
        GameObject go = YoyoPool.Instance.Get(spawnPos);
        activeYoyo = go.GetComponent<Yoyo>();
        activeYoyo.Launch(aimDir, transform);

        if (aimDir.x != 0)
            transform.localScale = new Vector3(aimDir.x < 0 ? -1f : 1f, 1f, 1f);
    }

    Vector2 GetAimDirection()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector2 dir = (mouseWorld - transform.position).normalized;
        if (dir.magnitude < 0.01f)
            dir = transform.localScale.x >= 0 ? Vector2.right : Vector2.left;
        return dir;
    }
}