using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
   

    public float enemyHealth;
    
 
    public float EnemyHitStrengthX;
    public float EnemyHitStrengthY;
    public float EnemyHitTime;
    public float enemyDamage;

    

    

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
        if (enemyHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void enemyTakeDamage(float damage)
    {
       

        enemyHealth -= damage;
        

    }
    

   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           
            TakeDamageManager damage = collision.gameObject.GetComponent<TakeDamageManager>();
           
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                player.hitTime = EnemyHitTime;
                player.hitForceX = EnemyHitStrengthX;
                player.hitForceY = EnemyHitStrengthY;


                if (collision.transform.position.x <= transform.position.x)
                {
                    player.hitFromRight = true;
                }
                else if (collision.transform.position.x > transform.position.x)
                {
                    player.hitFromRight = false;
                }
            }
            damage.TakeDamage(enemyDamage);
      

            
        }
         
        

    }
}
