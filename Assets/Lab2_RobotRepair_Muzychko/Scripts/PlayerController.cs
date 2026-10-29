using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction LaunchAction;
    public GameObject projectilePrefab;

    public float speed = 3.0f;

    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;

    Rigidbody2D rigidbody2d;
    Animator animator;
    SpriteRenderer spriteRenderer;

    Vector2 moveInput;
    Vector2 lookDirection = new Vector2(1, 0);

    private void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;

        MoveAction.Enable();
        LaunchAction.Enable();

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", 0f);
    }

    void Update()
    {
        moveInput = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(moveInput.x, 0.0f) ||
            !Mathf.Approximately(moveInput.y, 0.0f))
        {
            lookDirection.Set(moveInput.x, moveInput.y);
            lookDirection.Normalize();
        }

        // Виправлене віддзеркалення спрайта
        if (lookDirection.x < -0.1f)
        {
            spriteRenderer.flipX = false; // Змінено на false для лівого напрямку
        }
        else if (lookDirection.x > 0.1f)
        {
            spriteRenderer.flipX = true;  // Змінено на true для правого напрямку
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", moveInput.magnitude);

        if (LaunchAction.WasPressedThisFrame())
        {
            Launch();
        }

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;

            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position
            + moveInput * speed * Time.fixedDeltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void Launch()
    {
        if (projectilePrefab == null)
            return;

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            rigidbody2d.position + lookDirection * 0.5f,
            Quaternion.identity
        );

        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile != null)
        {
            Vector2 direction = lookDirection;

            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }

            projectile.Launch(direction, 300f);
            animator.SetTrigger("Launch");
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            damageCooldown = timeInvincible;

            animator.SetTrigger("Hit");
        }

        currentHealth = Mathf.Clamp(
            currentHealth + amount,
            0,
            maxHealth
        );

        Debug.Log(currentHealth + "/" + maxHealth);
    }
}