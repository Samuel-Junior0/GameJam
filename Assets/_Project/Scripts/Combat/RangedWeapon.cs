using System;
using System.Collections;
using UnityEngine;

// Arma de fogo. Jogador: munição limitada + recarga. Inimigo: marque infiniteAmmo.
public class RangedWeapon : MonoBehaviour
{
    public Projectile projectilePrefab;
    public int damage = 2;
    public LayerMask targetMask;        // jogador -> Enemy | inimigo -> Player
    public int maxAmmo = 6;
    public float reloadTime = 1.5f;
    public float cooldown = 0.25f;
    public bool infiniteAmmo;
    public float spawnOffset = 0.6f;    // evita o tiro nascer dentro do colisor de quem atira

    public int Ammo { get; private set; }
    public bool IsReloading { get; private set; }
    public event Action<int, int> OnAmmoChanged;

    float nextShot;

    void Awake() { Ammo = maxAmmo; }

    public bool TryShoot(Vector2 direction)
    {
        if (IsReloading || Time.time < nextShot || projectilePrefab == null) return false;
        nextShot = Time.time + cooldown;

        Vector2 pos = (Vector2)transform.position + direction.normalized * spawnOffset;
        Projectile p = Instantiate(projectilePrefab, pos, Quaternion.identity);
        p.Launch(direction, damage, targetMask);

        if (!infiniteAmmo)
        {
            Ammo--;
            OnAmmoChanged?.Invoke(Ammo, maxAmmo);
            if (Ammo <= 0) StartCoroutine(Reload());
        }
        return true;
    }

    IEnumerator Reload()
    {
        IsReloading = true;
        yield return new WaitForSeconds(reloadTime);
        Ammo = maxAmmo;
        IsReloading = false;
        OnAmmoChanged?.Invoke(Ammo, maxAmmo);
    }
}
