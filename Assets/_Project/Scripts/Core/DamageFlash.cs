using System.Collections;
using UnityEngine;

// Pisca o sprite de vermelho ao levar dano. Feedback barato e importante.
[RequireComponent(typeof(Health))]
public class DamageFlash : MonoBehaviour
{
    public Color flashColor = Color.red;
    public float duration = 0.1f;

    SpriteRenderer sprite;
    Color original;

    void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite) original = sprite.color;
        GetComponent<Health>().OnDamaged += HandleDamaged;
    }

    void HandleDamaged()
    {
        if (sprite && isActiveAndEnabled) StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        sprite.color = flashColor;
        yield return new WaitForSeconds(duration);
        sprite.color = original;
    }
}
