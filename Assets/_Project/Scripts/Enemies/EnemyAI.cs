using System;
using System.Collections;
using UnityEngine;

// IA simples: persegue o jogador, ataca de perto e (opcional) atira de longe se tiver RangedWeapon.
// Serve para cangaceiro comum, boiadeiro e Corisco — muda so os numeros no Inspector.
[RequireComponent(typeof(Rigidbody2D), typeof(Health))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 2f;
    public float detectionRange = 8f;

    [Header("Corpo a corpo")]
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public float attackWindup = 0.3f;   // tempo de "preparo": da pro jogador reagir
    public int damage = 1;

    [Header("Distancia (opcional)")]
    public RangedWeapon ranged;         // deixe vazio para inimigo so corpo a corpo
    public float rangedRange = 6f;

    public static event Action<EnemyAI> AnyDied;

    Rigidbody2D rb;
    Health health;
    SpriteRenderer sprite;
    Transform target;
    Health targetHealth;
    float nextAttack;
    bool dead;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        health = GetComponent<Health>();
        health.OnDied += Die;
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            target = player.transform;
            targetHealth = player.GetComponent<Health>();
        }
    }

    void FixedUpdate()
    {
        if (dead || target == null || GameManager.InputLocked)
        {
            rb.SetVelocity(Vector2.zero);
            return;
        }

        Vector2 toTarget = (Vector2)target.position - rb.position;
        float dist = toTarget.magnitude;
        if (dist > detectionRange || dist < 0.001f) { rb.SetVelocity(Vector2.zero); return; }

        Vector2 dir = toTarget / dist;
        if (sprite && Mathf.Abs(dir.x) > 0.05f) sprite.flipX = dir.x < 0f;

        // Atira de longe
        if (ranged != null && dist <= rangedRange && dist > attackRange)
        {
            rb.SetVelocity(Vector2.zero);
            ranged.TryShoot(dir);
            return;
        }

        // Aproxima
        if (dist > attackRange)
        {
            rb.SetVelocity(dir * moveSpeed);
            return;
        }

        // Ataca de perto
        rb.SetVelocity(Vector2.zero);
        if (Time.time >= nextAttack)
        {
            nextAttack = Time.time + attackCooldown;
            StartCoroutine(MeleeRoutine());
        }
    }

    IEnumerator MeleeRoutine()
    {
        yield return new WaitForSeconds(attackWindup);
        if (dead || target == null || targetHealth == null) yield break;

        // So acerta se o jogador ainda estiver perto (permite esquivar durante o preparo)
        if (Vector2.Distance(transform.position, target.position) <= attackRange * 1.3f)
            targetHealth.TakeDamage(damage);
    }

    void Die()
    {
        dead = true;
        rb.SetVelocity(Vector2.zero);
        foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = false;
        AnyDied?.Invoke(this);
        Destroy(gameObject, 0.3f);
    }
}
