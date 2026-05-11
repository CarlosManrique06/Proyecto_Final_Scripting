using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float hitForceX = 5f;
    [SerializeField] private float hitForceY = 2f;
    [SerializeField] private float hitDuration = 0.3f;
    [SerializeField] private float enemyDamage = 10f;
    [SerializeField] private float invertDuration = 3f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null) return;

        bool fromRight = collision.transform.position.x <= transform.position.x;

        // En vez de modificar los campos directamente, usamos los métodos públicos
        player.TakeDamage(enemyDamage);
        player.ApplyHit(hitForceX, hitForceY, hitDuration, fromRight);
        player.ApplyControlsInverted(invertDuration);
    }
}