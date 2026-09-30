using UnityEngine;

// Coloque num objeto qualquer e ligue nos botoes (OnClick) do menu, da tela de derrota e da tela final.
public class UIButtons : MonoBehaviour
{
    public void PlayGame() { GameManager.Instance.StartGame(); }
    public void RestartDay() { GameManager.Instance.RestartDay(); }
    public void LoadMenu() { GameManager.Instance.LoadMenu(); }
    public void Quit() { GameManager.Instance.Quit(); }
}
