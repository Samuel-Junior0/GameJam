using UnityEngine;

// Prefab: sprite + Collider2D (Is Trigger) + Rigidbody2D (Kinematic) + este script.
// Sprite desenhado virado para a direita.
public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 1.25f;
    public LayerMask obstacleMask;      // Layer "Obstacle" (paredes, cercas)

    int damage;
    LayerMask targetMask;
    Vector2 direction;

    public void Launch(Vector2 dir, int dmg, LayerMask targets)
    {
        direction = dir.normalized;
        damage = dmg;
        targetMask = targets;
        transform.right = direction;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        int bit = 1 << other.gameObject.layer;

        if ((targetMask.value & bit) != 0)
        {
            var health = other.GetComponentInParent<Health>();
            if (health != null) health.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if ((obstacleMask.value & bit) != 0)
        {
            Destroy(gameObject);
        }
    }
}
