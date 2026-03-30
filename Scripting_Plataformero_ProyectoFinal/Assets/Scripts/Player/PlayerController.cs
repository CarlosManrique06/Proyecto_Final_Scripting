using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NUnit.Framework;

public class PlayerController : MonoBehaviour
{

    public float speed;
    public float movement;

    //Jump Variables
    private bool canJump;
    public float jumpForce;
    public float jumpTime;
    public float jumpClock;
    private bool isJumping = false;
    public float gravityScale;
    public Transform startPosition;
    

    //Player Health

    public float playerHealth;
    private float maxHealth = 100;
    //Components

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius;
    [SerializeField] private LayerMask groundLayer;

  

    //public TextMeshProUGUI Health;

 

    [SerializeField] private Animator playerAnimator;

    //Hit Variables Hit

    public float hitTime;
    public float hitForceX;
    public float hitForceY;


    public bool hitFromRight;

    


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
      
        playerHealth = maxHealth;
        //Health.text = "S a l u d : " + maxHealth;
        jumpClock = jumpTime;
        transform.position = startPosition.position;
       
    }

    // Update is called once per frame
    void Update()
    {


        MovementPlayer();
        Jump();


        if (hitTime <= 0)
        {
            transform.Translate(Time.deltaTime * (Vector2.right * movement) * speed);

        }
        else
        {
            if (hitFromRight)
            {
                rb.linearVelocity = new Vector2(-hitForceX, hitForceY);
            }
            else if (!hitFromRight)
            {
                rb.linearVelocity = new Vector2(hitForceX, hitForceY);
            }
            hitTime -= Time.deltaTime;
        }

        if (playerHealth> maxHealth)
        {
            playerHealth = maxHealth;
        }

        if (playerHealth < 0)
        {
            playerHealth = 0;
           // Health.text = "Game Over " + playerHealth;
            transform.position = startPosition.position;
            playerHealth = maxHealth;
        }
        else if (playerHealth > 0)
        {

            //Health.text = "S a l u d : " + playerHealth;
        }
        

    }
    
    public void MovementPlayer()
    {

        movement = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(movement * speed, rb.linearVelocity.y);
        if (movement < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (movement > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        playerAnimator.SetFloat("Move", movement);

    }
    public void Jump()
    {
        canJump = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
      
        if (canJump == true && Input.GetKeyDown(KeyCode.W) && isJumping == false || canJump == true  && (Input.GetKeyDown(KeyCode.Space) && isJumping == false))
        {

            isJumping = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            
        }


        if (isJumping == true && Input.GetKey(KeyCode.W)||  (Input.GetKey(KeyCode.Space) && isJumping == true))
        {
           
            if (jumpClock > 0)
            {
                rb.gravityScale = 0;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpClock -= Time.deltaTime;
            }
            else 
            {
                rb.gravityScale = gravityScale;
                isJumping = false;

            }
        }

        if (Input.GetKeyUp(KeyCode.W) ||  (Input.GetKeyUp(KeyCode.Space)))
        {
            rb.gravityScale = gravityScale;
            jumpClock = jumpTime;
            isJumping = false;
        }

        


    }
    public void TakeDamage(float damage)
    {
      

        playerHealth -= damage;
        playerAnimator.SetTrigger("Hit");
      

    }

    public void AddHealth(float health)
    {
       
        playerHealth += 5;
    
    }

    public void DamageButton()
    {
      
        hitTime = 3;
        hitForceX = 5f;
        hitForceY = 2f;
        playerHealth -= 10;
        hitFromRight = true;

        playerAnimator.SetTrigger("Hit");


    }
    

}
