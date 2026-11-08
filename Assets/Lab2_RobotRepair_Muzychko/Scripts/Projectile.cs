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
        if (rigidbody2d != null)
        {
            rigidbody2d.AddForce(direction * force);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController enemy =
            other.GetComponent<EnemyController>();

        if (enemy != null)
        {
            enemy.Fix();
            Destroy(gameObject);
        }
    }
}