using UnityEngine;

public class Patrol_Jump_Enemies : MonoBehaviour
{
    public Transform[] enemyMovementPoints;

    [SerializeField] private Transform actualObjective;
    [SerializeField] private Rigidbody2D rb;
  

    private bool canJump;
    public float jumpForce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius;
    [SerializeField] private LayerMask groundLayer;
    public float timetoJump;
    private float clock;

    public float enemySpeed;
    public float detectionRadius = 0.5f;

   


   

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        actualObjective = enemyMovementPoints[1];
        clock = timetoJump;

    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
        Jump();
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
    public void Jump()
    {
        canJump = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
       
        if (canJump == true && clock <= 0)
        {

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            clock = timetoJump;



        }
      

        if (clock >= 0)
        {
            clock -= Time.deltaTime;
        }





    }
}
