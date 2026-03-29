using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{

    public Transform[] enemyMovementPoints;

    [SerializeField] private Transform actualObjective;
    [SerializeField] private Rigidbody2D rb;
 
   
    
    public float enemySpeed;
    public float detectionRadius = 0.5f;

   
   

    private Transform player;
    Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        actualObjective = enemyMovementPoints[1];
       
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
    }
    public void Patrol()
    {
        float distanceToObjective = Vector2.Distance(actualObjective.position, transform.position);

        if (distanceToObjective < detectionRadius)
        {
            if (actualObjective == enemyMovementPoints[0])
            {
                actualObjective = enemyMovementPoints[1];
            }
            else if (actualObjective == enemyMovementPoints[1])
            {
                actualObjective = enemyMovementPoints[0];
            }
        }
        transform.position = Vector2.MoveTowards(this.transform.position, actualObjective.position, enemySpeed * Time.deltaTime);
        Vector2 direction = (actualObjective.position - transform.position).normalized;

        int roundedDirection = Mathf.RoundToInt(direction.x);

        

        if (roundedDirection < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (roundedDirection > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

  
    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
