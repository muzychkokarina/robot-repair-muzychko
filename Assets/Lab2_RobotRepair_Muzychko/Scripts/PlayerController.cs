using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    public float speed = 3.0f;

    private Rigidbody2D rigidbody2d;
    private Vector2 moveInput;

    void Start()
    {
        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Зчитуємо ввід від гравця у звичайному Update
        moveInput = MoveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Переміщуємо фізичне тіло у FixedUpdate без трясіння
        Vector2 position = rigidbody2d.position;
        position += moveInput * speed * Time.fixedDeltaTime;
        rigidbody2d.MovePosition(position);
    }
}