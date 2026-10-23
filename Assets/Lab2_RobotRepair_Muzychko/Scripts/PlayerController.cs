using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction LaunchAction; // Дія для стрільби
    public GameObject projectilePrefab; // Поле для префаба снаряда

    public float speed = 3.0f;

    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;

    Rigidbody2D rigidbody2d;
    Vector2 moveInput;
    Vector2 lookDirection = new Vector2(1, 0); // За замовчуванням дивимося вправо

    private void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // Вмикаємо Input Actions
        MoveAction.Enable();
        LaunchAction.Enable();
    }

    void Update()
    {
        moveInput = MoveAction.ReadValue<Vector2>();

        // Оновлюємо напрямок погляду, якщо персонаж рухається
        if (!Mathf.Approximately(moveInput.x, 0.0f) || !Mathf.Approximately(moveInput.y, 0.0f))
        {
            lookDirection.Set(moveInput.x, moveInput.y);
            lookDirection.Normalize();
        }

        // Перевірка натискання кнопки стрільби
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
        Vector2 position = rigidbody2d.position + moveInput * speed * Time.fixedDeltaTime;
        rigidbody2d.MovePosition(position);
    }

    // ЄДИНИЙ метод Launch()
    public void Launch()
    {
        if (projectilePrefab == null) return;

        // Створюємо снаряд трохи вище центру гравця
        GameObject projectileObject = Instantiate(
            projectilePrefab, 
            rigidbody2d.position + Vector2.up * 0.5f, 
            Quaternion.identity
        );

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            // Якщо напрямок з якоїсь причини 0, задаємо напрямок вправо
            Vector2 direction = lookDirection;
            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }

            // Запускаємо снаряд з силою 300
            projectile.Launch(direction, 300f);
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damageCooldown = timeInvincible;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }
}