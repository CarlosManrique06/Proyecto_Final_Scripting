using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnAfterFall : MonoBehaviour
{
   
    public float fallDamage;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            TakeDamageManager fall = collision.gameObject.GetComponent<TakeDamageManager>();
        

            collision.gameObject.transform.position = RespawnPoint.fallSpawn;
            fall.TakeDamage(fallDamage);
        }

    }
}
