using System;
using System.Collections;
using TMPro;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea(2, 4)] public string text;
}

// Um por cena. Painel com dois TMP_Text (nome e fala). Avanca com Espaco, Enter, E ou clique.
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public GameObject panel;
    public TMP_Text speakerText;
    public TMP_Text bodyText;
    public float charDelay = 0.03f;

    public bool IsPlaying { get; private set; }

    void Awake()
    {
        Instance = this;
        if (panel) panel.SetActive(false);
    }

    public void Play(DialogueLine[] lines, Action onFinished = null)
    {
        if (IsPlaying) return;
        if (lines == null || lines.Length == 0) { onFinished?.Invoke(); return; }
        StartCoroutine(Run(lines, onFinished));
    }

    static bool AdvancePressed()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0);
    }

    IEnumerator Run(DialogueLine[] lines, Action onFinished)
    {
        IsPlaying = true;
        GameManager.InputLocked = true;
        panel.SetActive(true);
        yield return null; // evita que a tecla que abriu o dialogo ja avance a primeira fala

        foreach (var line in lines)
        {
            speakerText.text = line.speaker;
            bodyText.text = "";

            float timer = 0f;
            int shown = 0;
            while (shown < line.text.Length)
            {
                if (AdvancePressed()) break;    // pula a digitacao
                timer += Time.deltaTime;
                while (timer >= charDelay && shown < line.text.Length) { timer -= charDelay; shown++; }
                bodyText.text = line.text.Substring(0, shown);
                yield return null;
            }

            bodyText.text = line.text;
            yield return null;
            while (!AdvancePressed()) yield return null;
        }

        panel.SetActive(false);
        GameManager.InputLocked = false;
        IsPlaying = false;
        onFinished?.Invoke();
    }
}
