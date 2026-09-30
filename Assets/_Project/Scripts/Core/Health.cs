using System;
using UnityEngine;

// Vida generica. No Tôin, cada ponto de vida = 1 chapeu (maxHealth = 5).
public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 5;
    [SerializeField] float invulnerabilityTime = 0.5f; // use ~0.1 nos inimigos

    public int Current { get; private set; }
    public int Max => maxHealth;
    public bool IsDead { get; private set; }

    public event Action<int, int> OnHealthChanged; // (atual, maximo)
    public event Action OnDamaged;
    public event Action OnDied;

    float invulnerableUntil;

    void Awake() { Current = maxHealth; }

    public void TakeDamage(int amount)
    {
        if (IsDead || Time.time < invulnerableUntil) return;

        Current = Mathf.Max(0, Current - amount);
        invulnerableUntil = Time.time + invulnerabilityTime;
        OnHealthChanged?.Invoke(Current, maxHealth);
        OnDamaged?.Invoke();

        if (Current == 0)
        {
            IsDead = true;
            OnDied?.Invoke();
        }
    }

    // Retorna false se nao curou (morto ou vida cheia) — a garapa usa isso pra nao ser "gasta" a toa.
    public bool Heal(int amount)
    {
        if (IsDead || Current >= maxHealth) return false;
        Current = Mathf.Min(maxHealth, Current + amount);
        OnHealthChanged?.Invoke(Current, maxHealth);
        return true;
    }
}
