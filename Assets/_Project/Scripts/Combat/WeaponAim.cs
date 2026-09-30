using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Pega a câmara principal da cena
        mainCamera = Camera.main;
    }

    void Update()
    {
        AimAtMouse();
    }

    void AimAtMouse()
    {
        // 1. Obtém a posição do rato no mundo 2D
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 2. Calcula a direção da arma até ao rato
        Vector2 aimDirection = mousePosition - transform.position;

        // 3. Calcula o ângulo em graus (Mathf.Atan2)
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // 4. Aplica a rotação no eixo Z do WeaponPoint
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 5. Inverte a arma na vertical (Flip) para não ficar de cabeça para baixo ao mirar para a esquerda
        Vector3 localScale = Vector3.one;
        if (angle > 90f || angle < -90f)
        {
            localScale.y = -1f; // Inverte o eixo Y quando aponta para a esquerda
        }
        else
        {
            localScale.y = 1f;  // Normal quando aponta para a direita
        }
        transform.localScale = localScale;
    }
}