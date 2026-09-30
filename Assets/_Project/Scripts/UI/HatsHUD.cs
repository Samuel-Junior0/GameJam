using UnityEngine;
using UnityEngine.UI;

// 5 Images na UI (um por chapeu, da esquerda pra direita). Cheio = vida, vazio = vida perdida.
public class HatsHUD : MonoBehaviour
{
    public Health playerHealth;     // se vazio, procura a tag Player
    public Image[] hats;
    public Sprite fullHat;
    public Sprite emptyHat;

    void Start()
    {
        if (playerHealth == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) playerHealth = p.GetComponent<Health>();
        }
        if (playerHealth == null) return;

        playerHealth.OnHealthChanged += Refresh;
        Refresh(playerHealth.Current, playerHealth.Max);
    }

    void OnDestroy()
    {
        if (playerHealth != null) playerHealth.OnHealthChanged -= Refresh;
    }

    void Refresh(int current, int max)
    {
        for (int i = 0; i < hats.Length; i++)
        {
            hats[i].enabled = i < max;
            hats[i].sprite = i < current ? fullHat : emptyHat;
        }
    }
}
