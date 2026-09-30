using UnityEngine;
using UnityEngine.Events;

// Adicione ao boss (junto do EnemyAI). Abaixo de X% de vida ele "enfurece": mais rapido e ataca mais.
// Ligue o evento onEnrage a um som/efeito se quiser.
[RequireComponent(typeof(EnemyAI), typeof(Health))]
public class BossEnemy : MonoBehaviour
{
    [Range(0.1f, 0.9f)] public float enrageAtHealthPercent = 0.5f;
    public float speedMultiplier = 1.5f;
    public float cooldownMultiplier = 0.6f;
    public UnityEvent onEnrage;

    EnemyAI ai;
    Health health;
    bool enraged;

    void Awake()
    {
        ai = GetComponent<EnemyAI>();
        health = GetComponent<Health>();
        health.OnHealthChanged += OnHealthChanged;
    }

    void OnHealthChanged(int current, int max)
    {
        if (enraged || current <= 0) return;
        if (current <= max * enrageAtHealthPercent)
        {
            enraged = true;
            ai.moveSpeed *= speedMultiplier;
            ai.attackCooldown *= cooldownMultiplier;
            onEnrage?.Invoke();
        }
    }
}
