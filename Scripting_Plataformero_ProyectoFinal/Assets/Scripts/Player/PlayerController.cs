using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Estados
    public enum PlayerState { Idle, Running, Jumping, Swinging, Hit }

    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

    void SetState(PlayerState next)
    {
        if (CurrentState == next) return;
        CurrentState = next;
    }

   

    public float speed = 8f;
    public float jumpForce = 12f;
    public float jumpTime = 0.2f;
    public float gravityScale = 3f;
    public Transform startPosition;

    public float hitForceX = 5f;
    public float hitForceY = 2f;
    public float hitDuration = 0.3f;

    public float playerHealth = 100f;
    private float maxHealth = 100f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator playerAnimator;

    public bool controlsInverted = false;
    public float invertTimer = 0f;


    private float movement;
    private bool isJumping;
    private float jumpClock;
    public float hitTime;
    public bool hitFromRight;
    private SwingController swingController;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        swingController = GetComponent<SwingController>();
        playerHealth = maxHealth;
        jumpClock = jumpTime;
        if (startPosition != null) transform.position = startPosition.position;
    }

    void Update()
    {
        if (invertTimer > 0)
        {
            invertTimer -= Time.deltaTime;
            if (invertTimer <= 0) controlsInverted = false;
        }

        UpdateState();
        HandleByState();
    }

   
    //  Determinar estado

    void UpdateState()
    {
       
        if (hitTime > 0)
        {
            SetState(PlayerState.Hit);
            return;
        }

        // Columpio
        if (swingController != null && (swingController.IsSwinging || swingController.IsLaunching))
        {
            SetState(PlayerState.Swinging);
            return;
        }

        // Salto / caída
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        if (!grounded)
        {
            SetState(PlayerState.Jumping);
            return;
        }

        // En el suelo
        movement = Input.GetAxisRaw("Horizontal");
        SetState(movement != 0 ? PlayerState.Running : PlayerState.Idle);
    }

   

    void HandleByState()
    {
        switch (CurrentState)
        {
            case PlayerState.Idle:
            case PlayerState.Running:
                HandleMovement();
                HandleJumpInput();
                break;

            case PlayerState.Jumping:
                HandleMovement();   // control aéreo normal
                HandleJumpInput();
                break;

            case PlayerState.Swinging:
                // NO sobreescribir velocidad — la física del péndulo manda
                HandleSwingAirControl();
                break;

            case PlayerState.Hit:
                HandleHitKnockback();
                break;
        }

        UpdateAnimator();
    }

   

    void HandleMovement()
    {
        movement = Input.GetAxisRaw("Horizontal");
        if (controlsInverted) movement = -movement;
        rb.linearVelocity = new Vector2(movement * speed, rb.linearVelocity.y);

        if (movement < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (movement > 0) transform.localScale = new Vector3(1, 1, 1);
    }

    void HandleJumpInput()
    {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        bool jumpPressed = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space);
        bool jumpHeld = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space);
        bool jumpReleased = Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space);

        if (grounded && jumpPressed && !isJumping && CurrentState != PlayerState.Swinging)
        {
            isJumping = true;
            jumpClock = jumpTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (isJumping && jumpHeld && jumpClock > 0)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpClock -= Time.deltaTime;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

        if (jumpReleased)
        {
            rb.gravityScale = gravityScale;
            jumpClock = 0;
            isJumping = false;
        }
    }

    // Permite girar el sprite en el aire durante el columpio, sin tocar velocity
    void HandleSwingAirControl()
    {
        movement = Input.GetAxisRaw("Horizontal");
        if (movement < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (movement > 0) transform.localScale = new Vector3(1, 1, 1);
    }

    void HandleHitKnockback()
    {
        rb.linearVelocity = hitFromRight
            ? new Vector2(-hitForceX, hitForceY)
            : new Vector2(hitForceX, hitForceY);

        hitTime -= Time.deltaTime;
    }

    void UpdateAnimator()
    {
        if (playerAnimator == null) return;
        playerAnimator.SetFloat("Move", Mathf.Abs(movement));
        playerAnimator.SetBool("IsJumping", CurrentState == PlayerState.Jumping);
        playerAnimator.SetBool("IsSwinging", CurrentState == PlayerState.Swinging);
    }

   
    //  Salud y daño
   

    public void TakeDamage(float damage)
    {
        playerHealth -= damage;
        hitTime = hitDuration;
        hitFromRight = true;
        if (playerAnimator != null) playerAnimator.SetTrigger("Hit");

        if (playerHealth <= 0)
        {
            playerHealth = maxHealth;
            if (startPosition != null) transform.position = startPosition.position;
        }
    }

    public void AddHealth(float amount)
    {
        playerHealth = Mathf.Min(playerHealth + amount, maxHealth);
    }
}