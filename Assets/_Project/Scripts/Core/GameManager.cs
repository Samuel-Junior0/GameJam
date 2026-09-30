using UnityEngine;
using UnityEngine.SceneManagement;

// Persiste entre cenas. Se nao existir na cena, e criado sozinho (da pra testar qualquer cena isolada).
public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                new GameObject("GameManager").AddComponent<GameManager>();
            return instance;
        }
    }

    // Trava movimento/ataque/IA durante dialogos e tela de derrota.
    public static bool InputLocked;

    public int CurrentDay = 2; //Mudar para testes
    public bool HasFirearm => CurrentDay >= 2; // arma de fogo so a partir do dia 2

    const string MenuScene = "Menu";
    const string EndScene = "Fim";

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InputLocked = false;
        Time.timeScale = 1f;
    }

    public void StartGame() { CurrentDay = 1; SceneManager.LoadScene("Dia1"); }
    public void RestartDay() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void LoadMenu() { SceneManager.LoadScene(MenuScene); }
    public void Quit() { Application.Quit(); }

    public void LoadNextDay()
    {
        if (CurrentDay >= 3) SceneManager.LoadScene(EndScene);
        else SceneManager.LoadScene("Dia" + (CurrentDay + 1));
    }
}
