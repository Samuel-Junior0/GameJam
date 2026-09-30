using UnityEngine;

// Prefab: sprite + Collider2D (Is Trigger) + Rigidbody2D (Kinematic) + este script.
// Ajuste 'Sprite Angle Offset' conforme para onde o desenho do sprite aponta.
public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public LayerMask obstacleMask;      // Layer "Obstacle" (paredes, cercas)

    [Tooltip("Correção para o desenho do sprite. Virado para CIMA: -90 | para a DIREITA: 0 | " +
             "para BAIXO: 90 | para a ESQUERDA: 180. Deixe a rotação do sprite/objeto em 0.")]
    public float spriteAngleOffset = -90f;

    int damage;
    LayerMask targetMask;
    Vector2 direction;

    public void Launch(Vector2 dir, int dmg, LayerMask targets)
    {
        direction = dir.normalized;
        damage = dmg;
        targetMask = targets;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteAngleOffset);
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