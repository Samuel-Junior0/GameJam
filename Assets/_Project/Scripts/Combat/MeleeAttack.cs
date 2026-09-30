using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Golpe corpo a corpo: acerta uma área circular na direção da mira.
// Pode ficar no Player (raiz) ou em um filho. O golpe parte sempre do centro do Player.
public class MeleeAttack : MonoBehaviour
{
    public int damage = 1;
    public float reach = 0.9f;          // distância do centro do golpe até o Player
    public float radius = 0.6f;         // tamanho da área
    public float cooldown = 0.4f;
    [Tooltip("Segundos entre apertar o botão e o dano sair. Ajuste para bater com o soco da animação.")]
    public float hitDelay = 0.1f;
    public LayerMask targetMask;        // Layer "Enemy" — se ficar vazio, nada é acertado!

    [Tooltip("Ponto de onde o golpe parte. Vazio = o Player.")]
    public Transform origin;
    public bool debugLog = true;        // escreve no Console se acertou ou não

    float nextTime;
    Vector2 lastCenter;
    float lastAttackTime = -10f;

    void Awake()
    {
        if (origin == null)
        {
            var player = GetComponentInParent<PlayerController>();
            origin = player != null ? player.transform : transform;
        }
    }

    public bool TryAttack(Vector2 direction)
    {
        if (Time.time < nextTime) return false;
        nextTime = Time.time + cooldown;

        StartCoroutine(HitRoutine(direction.normalized));
        return true;
    }

    IEnumerator HitRoutine(Vector2 direction)
    {
        if (hitDelay > 0f) yield return new WaitForSeconds(hitDelay);
        DealDamage(direction);
    }

    void DealDamage(Vector2 direction)
    {
        Vector2 center = (Vector2)origin.position + direction * reach;
        lastCenter = center;
        lastAttackTime = Time.time;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, targetMask);

        // Um inimigo pode ter vários colisores: só leva 1 dano por golpe
        var alreadyHit = new HashSet<Health>();
        int count = 0;
        foreach (var hit in hits)
        {
            var health = hit.GetComponentInParent<Health>();
            if (health != null && alreadyHit.Add(health))
            {
                health.TakeDamage(damage);
                count++;
            }
        }

        if (!debugLog) return;
        if (count > 0)
            Debug.Log($"[Melee] acertou {count} alvo(s).");
        else
            Debug.Log($"[Melee] nada na área. Colisores na máscara: {hits.Length}. " +
                      $"Target Mask vazio? {targetMask.value == 0}");
    }

    void OnDrawGizmos()
    {
        if (Time.time - lastAttackTime < 0.5f)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lastCenter, radius);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Transform o = origin != null ? origin : transform;
        Gizmos.DrawWireSphere(o.position + Vector3.right * reach, radius);
    }
}
