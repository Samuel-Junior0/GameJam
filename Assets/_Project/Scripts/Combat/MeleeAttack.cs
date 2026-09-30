using UnityEngine;

// Facao: acerta uma area circular na frente de quem ataca. Usado pelo jogador.
public class MeleeAttack : MonoBehaviour
{
    public int damage = 1;
    public float reach = 0.9f;      // distancia do centro do golpe
    public float radius = 0.6f;     // tamanho da area
    public float cooldown = 0.4f;
    public LayerMask targetMask;    // Layer "Enemy"

    float nextTime;

    public bool TryAttack(Vector2 direction)
    {
        if (Time.time < nextTime) return false;
        nextTime = Time.time + cooldown;

        Vector2 center = (Vector2)transform.position + direction.normalized * reach;
        foreach (var hit in Physics2D.OverlapCircleAll(center, radius, targetMask))
        {
            var health = hit.GetComponentInParent<Health>();
            if (health != null) health.TakeDamage(damage);
        }
        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.right * reach, radius);
    }
}
