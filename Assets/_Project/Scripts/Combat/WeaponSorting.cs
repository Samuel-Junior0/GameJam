using UnityEngine;

// Coloque no WeaponPoint (o objeto que o WeaponAim gira).
// Arma NA FRENTE do Tôin ao mirar para baixo/lados e ATRÁS ao mirar para cima.
public class WeaponSorting : MonoBehaviour
{
    public SpriteRenderer playerSprite;     // sprite do Tôin (se vazio, procura sozinho)
    public SpriteRenderer weaponSprite;     // sprite da arma (se vazio, procura nos filhos)

    [Tooltip("Mira com Y acima desse valor = arma atrás. 0.3 deixa a mira lateral na frente.")]
    [Range(-1f, 1f)] public float behindThreshold = 0.3f;

    void Awake()
    {
        if (weaponSprite == null)
            weaponSprite = GetComponentInChildren<SpriteRenderer>(true);

        if (playerSprite == null)
        {
            // Primeiro SpriteRenderer do Player que NÃO seja da arma
            foreach (var sr in transform.root.GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (!sr.transform.IsChildOf(transform)) { playerSprite = sr; break; }
            }
        }
    }

    void LateUpdate()
    {
        if (playerSprite == null || weaponSprite == null) return;

        bool behind = transform.right.y > behindThreshold;

        weaponSprite.sortingLayerID = playerSprite.sortingLayerID;
        weaponSprite.sortingOrder = playerSprite.sortingOrder + (behind ? -1 : 1);
    }
}