using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float force)
    {
        if (rigidbody2d == null)
        {
            rigidbody2d = GetComponent<Rigidbody2D>();
        }

        Debug.Log("Снаряд отримав напрямок: " + direction + " та силу: " + force);

        if (rigidbody2d != null)
        {
            // Використовуємо пряме задання швидкості, щоб обійти будь-які баги з AddForce
            rigidbody2d.linearVelocity = direction * 8f; 
        }
        else
        {
            Debug.LogError("Rigidbody2D НЕ ЗНАЙДЕНО на снаряді!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController e = other.GetComponent<EnemyController>();
        if (e != null)
        {
            e.Fix();
            Destroy(gameObject);
        }
    }
}