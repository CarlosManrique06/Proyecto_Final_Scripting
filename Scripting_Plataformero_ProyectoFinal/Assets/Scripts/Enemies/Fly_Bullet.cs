using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly_Bullet : MonoBehaviour
{
    GameObject target;
    public float speed;
    public float bulletDamage;
    Rigidbody2D bulletRB;
    
    
   

    // Start is called before the first frame update
    void Start()
    {
        bulletRB = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player");
        Vector2 moveDir = (target.transform.position - transform.position).normalized * speed;
        bulletRB.linearVelocity = new Vector2(moveDir.x, moveDir.y);
        Destroy(this.gameObject, 1.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TakeDamageManager player = collision.gameObject.GetComponent<TakeDamageManager>();


            player.TakeDamage(bulletDamage);
            Destroy(this.gameObject);


        }



    }
}
