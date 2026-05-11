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
        if (collision.gameObject.CompareTag("Player"))
        {
            // 2. Intentamos obtener la interfaz IDamage del objeto con el que chocamos
            IDamage damageable = collision.gameObject.GetComponent<IDamage>();

            if (damageable != null)
            {
                // 3. Aplicamos el daño a través de la interfaz
                damageable.TakeDamage((int)enemyDamage);

                // 4. OPCIONAL: Aplicar el empuje físico si el objeto tiene PlayerController
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    // Calculamos si el enemigo está a la derecha o izquierda para el empuje
                    bool hitFromRight = transform.position.x > collision.transform.position.x;
                    player.ApplyHit(hitForceX, hitForceY, hitDuration, hitFromRight);
                }
            }
        }
    }
}