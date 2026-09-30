using UnityEngine;
using UnityEngine.Events;

// Zona (Collider2D Is Trigger) que dispara um dialogo quando o jogador entra. Ex.: encontro com o compadre.
public class DialogueTrigger : MonoBehaviour
{
    public DialogueLine[] lines;
    public bool onlyOnce = true;
    public UnityEvent onFinished;       // ex.: ativar inimigos, liberar o compadre pra seguir

    bool played;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (played && onlyOnce) return;
        if (DialogueManager.Instance == null) return;

        played = true;
        DialogueManager.Instance.Play(lines, () => onFinished?.Invoke());
    }
}
