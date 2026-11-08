using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller =
            other.GetComponent<PlayerController>();

        if (controller != null)
        {
            if (controller.health < controller.MaxHealth)
            {
                controller.ChangeHealth(1);
                Destroy(gameObject);
            }
        }
    }
}