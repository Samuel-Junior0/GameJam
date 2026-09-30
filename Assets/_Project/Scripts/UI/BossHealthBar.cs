using UnityEngine;
using UnityEngine.UI;

// Coloque num objeto FORA do painel. O painel (root) fica escondido ate o jogador chegar perto do boss.
public class BossHealthBar : MonoBehaviour
{
    public Health boss;
    public Slider slider;
    public GameObject root;             // painel com a barra
    public float revealDistance = 9f;

    Transform player;
    bool revealed;

    void Start()
    {
        if (root) root.SetActive(false);
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
        if (boss == null) { enabled = false; return; }

        boss.OnHealthChanged += Refresh;
        boss.OnDied += Hide;
        Refresh(boss.Current, boss.Max);
    }

    void Update()
    {
        if (revealed || player == null || boss == null) return;
        if (Vector2.Distance(player.position, boss.transform.position) <= revealDistance)
        {
            revealed = true;
            if (root) root.SetActive(true);
        }
    }

    void Refresh(int current, int max) { if (slider) slider.value = (float)current / max; }

    void Hide() { if (root) root.SetActive(false); enabled = false; }

    void OnDestroy()
    {
        if (boss != null) { boss.OnHealthChanged -= Refresh; boss.OnDied -= Hide; }
    }
}
