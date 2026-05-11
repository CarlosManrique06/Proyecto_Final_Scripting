using UnityEngine;

public class PlayerController : MonoBehaviour
{
   
    public enum PlayerState { Idle, Running, Jumping, Swinging, Hit }
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;

    void SetState(PlayerState next)
    {
        if (CurrentState == next) return;
        CurrentState = next;
    }


    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float jumpTime = 0.2f;
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private Transform startPosition;

    [SerializeField] private float hitForceX = 5f;
    [SerializeField] private float hitForceY = 2f;
    [SerializeField] private float hitDuration = 0.3f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator playerAnimator;

  

    public float Health { get; private set; }
    private const float MaxHealth = 100f;

  

    private float movement;
    private bool isJumping;
    private float jumpClock;
    private float hitTimer;
    private bool hitFromRight;
    private bool controlsInverted;
    private float invertTimer;

    private SwingController swingController;

    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        swingController = GetComponent<SwingController>();
        Health = MaxHealth;
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

  
    //  Estado
  

    void UpdateState()
    {
        if (hitTimer > 0) { SetState(PlayerState.Hit); return; }
        if (swingController != null &&
            (swingController.IsSwinging ||
             swingController.IsLaunching)) { SetState(PlayerState.Swinging); return; }

        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        if (!grounded) { SetState(PlayerState.Jumping); return; }

        movement = Input.GetAxisRaw("Horizontal");
        SetState(movement != 0 ? PlayerState.Running : PlayerState.Idle);
    }

    void HandleByState()
    {
        switch (CurrentState)
        {
            case PlayerState.Idle:
            case PlayerState.Running:
            case PlayerState.Jumping:
                HandleMovement();
                HandleJumpInput();
                break;

            case PlayerState.Swinging:
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

        if (jumpReleased) { rb.gravityScale = gravityScale; jumpClock = 0; isJumping = false; }
    }

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

        hitTimer -= Time.deltaTime;
    }

    void UpdateAnimator()
    {
        if (playerAnimator == null) return;
        playerAnimator.SetFloat("Move", Mathf.Abs(movement));
        playerAnimator.SetBool("IsJumping", CurrentState == PlayerState.Jumping);
        playerAnimator.SetBool("IsSwinging", CurrentState == PlayerState.Swinging);
    }

   

    public void TakeDamage(float damage)
    {
        Health -= damage;
        hitTimer = hitDuration;
        hitFromRight = true;

        if (playerAnimator != null) playerAnimator.SetTrigger("Hit");

        if (Health <= 0)
        {
            Health = MaxHealth;
            if (startPosition != null) transform.position = startPosition.position;
        }
    }

    
    // Llamado por EnemyController para aplicar knockback con parámetros propios del enemigo.
   
    
    public void ApplyHit(float forceX, float forceY, float duration, bool fromRight)
    {
        hitForceX = forceX;
        hitForceY = forceY;
        hitTimer = duration;
        hitFromRight = fromRight;
    }

   
    // Invierte los controles por un tiempo determinado.
   
    public void ApplyControlsInverted(float duration)
    {
        controlsInverted = true;
        invertTimer = duration;
    }

    public void AddHealth(float amount)
    {
        Health = Mathf.Min(Health + amount, MaxHealth);
    }
}