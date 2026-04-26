using UnityEngine;

public class Collectibles : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private HealthManager healthManager;
    void Start()
    {
        
        healthManager = GameObject.FindGameObjectWithTag("HealthManager").GetComponent<HealthManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            healthManager.coinsNumber += 1;
            Destroy(gameObject);

        }



    }
}
