using UnityEngine;

// Coloque nos inimigos: chance de soltar uma garapa ao morrer.
[RequireComponent(typeof(Health))]
public class DropOnDeath : MonoBehaviour
{
    public GameObject dropPrefab;               // prefab da garapa
    [Range(0f, 1f)] public float chance = 0.3f;

    void Awake() { GetComponent<Health>().OnDied += Drop; }

    void Drop()
    {
        if (dropPrefab != null && Random.value <= chance)
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
    }
}
