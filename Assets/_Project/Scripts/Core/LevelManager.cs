using System.Collections;
using UnityEngine;

// Um por cena (Dia1, Dia2, Dia3). Fase termina quando todos os EnemyAI da cena morrem.
public class LevelManager : MonoBehaviour
{
    [Header("Fase")]
    public int dayNumber = 1;

    [Header("Referencias")]
    public Health playerHealth;           // se vazio, procura o objeto com tag Player
    public GameObject gameOverPanel;

    [Header("Dialogos")]
    public DialogueLine[] intro;          // toca ao comecar a fase
    public DialogueLine[] outro;          // toca depois do ultimo inimigo (ex.: a carta do Corisco)

    [Header("Fim de fase")]
    public float delayAfterLastEnemy = 1.5f;

    int remaining;
    bool finished;

    void Start()
    {
        GameManager.Instance.CurrentDay = dayNumber;
        if (gameOverPanel) gameOverPanel.SetActive(false);

        if (playerHealth == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) playerHealth = p.GetComponent<Health>();
        }
        if (playerHealth) playerHealth.OnDied += OnPlayerDied;

        remaining = FindObjectsByType<EnemyAI>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
        EnemyAI.AnyDied += OnEnemyDied;

        if (intro != null && intro.Length > 0 && DialogueManager.Instance != null)
            DialogueManager.Instance.Play(intro);
    }

    void OnDestroy()
    {
        EnemyAI.AnyDied -= OnEnemyDied;
        if (playerHealth) playerHealth.OnDied -= OnPlayerDied;
    }

    void OnEnemyDied(EnemyAI enemy)
    {
        remaining--;
        if (remaining <= 0 && !finished) StartCoroutine(FinishRoutine());
    }

    IEnumerator FinishRoutine()
    {
        finished = true;
        yield return new WaitForSeconds(delayAfterLastEnemy);

        if (outro != null && outro.Length > 0 && DialogueManager.Instance != null)
        {
            bool done = false;
            DialogueManager.Instance.Play(outro, () => done = true);
            yield return new WaitUntil(() => done);
        }
        GameManager.Instance.LoadNextDay();
    }

    void OnPlayerDied()
    {
        if (finished) return;
        GameManager.InputLocked = true;
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }
}
