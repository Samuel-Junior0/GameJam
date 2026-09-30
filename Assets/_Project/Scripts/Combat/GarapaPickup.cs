using UnityEngine;

// Prefab da garapa: sprite + Collider2D (Is Trigger) + este script.
// So e consumida se o jogador realmente precisar de cura.
public class GarapaPickup : MonoBehaviour
{
    public int healAmount = 1;

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var health = other.GetComponentInParent<Health>();
        if (health != null && health.Heal(healAmount))
            Destroy(gameObject);
    }
}
