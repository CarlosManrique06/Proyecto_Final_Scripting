using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly_Follow_Shoot : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed;
    public float distanceToFollow;
    public float shootingRange;
    public float fireRate = 1f;
    private float fireTime;

    public float bulletClock;
    public float bulletTime;

    public GameObject bullets;
    public GameObject bulletStartUp;

   private Transform player;
   

    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        EnemyPatrol Patrol = GetComponent<EnemyPatrol>();
        bulletClock = bulletTime;
    }

    // Update is called once per frame
    void Update()
    {
        followPlayer();
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    public void followPlayer()
    {
        EnemyPatrol Patrol = GetComponent<EnemyPatrol>();
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);

        Vector2 direction = (player.position - transform.position).normalized;

        int roundedDirection = Mathf.RoundToInt(direction.x);
       
        
        if(distanceFromPlayer < distanceToFollow && distanceFromPlayer > shootingRange)
        {


            if (roundedDirection < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (roundedDirection > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            
            Patrol.enabled = false;
            transform.position = Vector2.MoveTowards(this.transform.position, player.position, speed * Time.deltaTime);
            bulletClock = bulletTime;
               

            }
        else if (distanceFromPlayer > distanceToFollow )
        {

            Patrol.enabled = true;
            bulletClock = bulletTime;
        }

        if (distanceFromPlayer <= shootingRange)
        {
            if (roundedDirection < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (roundedDirection > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
           
            bulletClock -= Time.deltaTime;

        }
        if (distanceFromPlayer <= shootingRange && fireTime < Time.time)
        {
            if(bulletClock <= 0)
            {
                Instantiate(bullets, bulletStartUp.transform.position, Quaternion.identity);
                fireTime = Time.time + fireRate;
                bulletClock = bulletTime;
            }
            
        }

        
        
       

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanceToFollow);
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
