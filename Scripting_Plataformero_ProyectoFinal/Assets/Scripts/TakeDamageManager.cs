using UnityEngine;

public class TakeDamageManager : MonoBehaviour, IDamage // Firmamos el contrato aquí
{
    private HealthManager healthManager;
    private Animator playerAnimator;
    private SpriteRenderer spriteRenderer;
    private float invincibilityClock;

    [Header("Settings")]
    [SerializeField] private float invincibilityTime = 1.0f;
    [SerializeField] private ParticleSystem particlesCaller;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        healthManager = FindAnyObjectByType<HealthManager>();

        // Si no asignaste las partículas en el inspector, las buscamos
        if (particlesCaller == null)
            particlesCaller = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (invincibilityClock > 0)
        {
            invincibilityClock -= Time.deltaTime;
            spriteRenderer.color = Color.red; // Feedback visual de daño
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    public void TakeDamage(int damage)
    {
        // Si estamos en tiempo de invencibilidad, ignoramos el daño
        if (invincibilityClock > 0) return;

        // 1. Aplicar daño al HealthManager
        if (healthManager != null)
        {
            healthManager.playerHealth -= damage;
        }

        // 2. Feedback Visual
        if (playerAnimator != null) playerAnimator.SetTrigger("Hit");

        if (particlesCaller != null)
        {
            var main = particlesCaller.main;
            main.startColor = Color.red;
            particlesCaller.Play();
        }

        // 3. Iniciar invencibilidad
        invincibilityClock = invincibilityTime;
        PlayerController player = GetComponent<PlayerController>();
        if (player != null)
        {
            // Llamas solo a la parte física del golpe
            player.ApplyHit(5f, 2f, 0.3f, true);
        }
    }
}