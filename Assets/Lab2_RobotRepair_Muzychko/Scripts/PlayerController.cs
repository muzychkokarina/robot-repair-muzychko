
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction LaunchAction;
    public InputAction TalkAction;

    public GameObject ProjectilePrefab;

    public float Speed = 3.0f;
    public int MaxHealth = 5;
    public float TimeInvincible = 2.0f;

    public int health
    {
        get { return currentHealth; }
    }

    private int currentHealth;
    private bool isInvincible;
    private float invincibleTimer;

    private Rigidbody2D rigidbody2d;
    private Vector2 move;
    private Vector2 lookDirection = new Vector2(1, 0);
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private NonPlayerCharacter lastNonPlayerCharacter;

    private void OnEnable()
    {
        MoveAction.Enable();
        LaunchAction.Enable();
        TalkAction.Enable();
    }

    private void OnDisable()
    {
        MoveAction.Disable();
        LaunchAction.Disable();
        TalkAction.Disable();
    }

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        currentHealth = MaxHealth;
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0) ||
            !Mathf.Approximately(move.y, 0))
        {
            lookDirection = move.normalized;
        }

        animator.SetFloat("Look X", -lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;

            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (LaunchAction.WasPressedThisFrame())
        {
            Launch();
        }

        RaycastHit2D hit = Physics2D.Raycast(
            rigidbody2d.position + Vector2.up * 0.2f,
            lookDirection,
            1.5f,
            LayerMask.GetMask("NPC")
        );

        if (hit.collider != null)
        {
            NonPlayerCharacter npc =
                hit.collider.GetComponent<NonPlayerCharacter>();

            if (npc != null)
            {
                npc.dialogueBubble.SetActive(true);
                lastNonPlayerCharacter = npc;
                FindFriend();
            }
        }
        else
        {
            if (lastNonPlayerCharacter != null)
            {
                lastNonPlayerCharacter.dialogueBubble.SetActive(false);
                lastNonPlayerCharacter = null;
            }
        }
    }

    void LateUpdate()
    {
        if (spriteRenderer != null &&
            !Mathf.Approximately(lookDirection.x, 0))
        {
            spriteRenderer.flipX = lookDirection.x > 0;
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;

        position.x += Speed * move.x * Time.fixedDeltaTime;
        position.y += Speed * move.y * Time.fixedDeltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = TimeInvincible;
        }

        currentHealth = Mathf.Clamp(
            currentHealth + amount,
            0,
            MaxHealth
        );

        if (UIHandler.instance != null)
        {
            UIHandler.instance.SetHealthValue(
                currentHealth / (float)MaxHealth
            );
        }
    }

    void Launch()
  {
        Debug.Log("СТРІЛЬБУ ВИКЛИКАНО!");

        if (ProjectilePrefab == null)
        {
            Debug.LogError("ProjectilePrefab НЕ ПІДКЛЮЧЕНИЙ!");
            return;
        }

        Debug.Log("Префаб снаряда знайдено!");

        GameObject projectileObject = Instantiate(
            ProjectilePrefab,
            rigidbody2d.position + lookDirection * 0.5f,
            Quaternion.identity
        );

        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Launch(lookDirection, 300);
        }

        animator.SetTrigger("Launch");
    }

    void FindFriend()
    {
        if (TalkAction.WasPressedThisFrame())
        {
            if (UIHandler.instance != null)
            {
                UIHandler.instance.DisplayDialogue();
            }
        }
    }
}
