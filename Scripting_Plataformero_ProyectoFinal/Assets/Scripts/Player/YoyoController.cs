using UnityEngine;

public class YoyoController : MonoBehaviour
{
    public KeyCode attackKey = KeyCode.Mouse0; // tecla para lanzar el yoyo
    public GameObject yoyoPrefab;              // prefab del yoyo (asignar en Inspector)
    public GameObject yoyoVisual;
    private Yoyo activeYoyo;                   // referencia al yoyo activo
    private SwingController swingController;   // referencia al controlador de balanceo

    void Awake()
    {
        swingController = GetComponent<SwingController>(); // obtiene el componente SwingController del jugador
    }

    void Update()
    {
        // si el jugador está balanceándose o lanzando, no permite otro lanzamiento
        if (swingController != null && (swingController.IsSwinging || swingController.IsLaunching)) return;

        // si se presiona la tecla de ataque, intenta lanzar el yoyo
        if (Input.GetKeyDown(attackKey)) { TryLaunch(); }

        if(activeYoyo != null && activeYoyo.isActive == false && activeYoyo.isAnchored == false) yoyoVisual.SetActive(true);

       
    }

    void TryLaunch()
    {
        // si ya hay un yoyo activo, no lanza otro
        yoyoVisual.SetActive(false);
        if (activeYoyo != null && activeYoyo.isActive) return;

        // calcula la dirección hacia el mouse
        Vector2 aimDir = GetAimDirection();
        // posición inicial del yoyo, un poco delante del jugador
        Vector2 spawnPos = (Vector2)transform.position + aimDir * 0.4f;

        if (YoyoPool.Instance != null)
        {
            GameObject go = YoyoPool.Instance.Get(spawnPos);
            if (go != null)
            {
                activeYoyo = go.GetComponent<Yoyo>();
                activeYoyo.Launch(aimDir, transform);

                // Apagamos el visual de la mano inmediatamente al lanzar
                if (yoyoVisual != null) yoyoVisual.SetActive(false);
            }
        }

        // ajusta la escala del jugador según la dirección (para voltear el sprite)
        if (aimDir.x != 0)
            transform.localScale = new Vector3(aimDir.x < 0 ? -1f : 1f, 1f, 1f);
    }

    Vector2 GetAimDirection()
    {
        // convierte la posición del mouse a coordenadas del mundo
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // calcula la dirección normalizada hacia el mouse
        Vector2 dir = (mouseWorld - transform.position).normalized;

        // si la dirección es muy pequeña, usa izquierda/derecha según la escala del jugador
        if (dir.magnitude < 0.01f)
            dir = transform.localScale.x >= 0 ? Vector2.right : Vector2.left;

        return dir; // devuelve la dirección final
    }
}