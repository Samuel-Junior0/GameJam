using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Configurações de Destino")]
    [Tooltip("Nome exato da cena para onde o player vai")]
    [SerializeField] private string sceneToLoad;

    [Tooltip("Tag do objeto do jogador para evitar que outros objetos ativem a troca")]
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se quem entrou na borda foi o Player
        if (collision.CompareTag(playerTag))
        {
            // Carrega a nova cena pelo nome
            SceneManager.LoadScene("Compadre_Encontro");
        }
    }
}