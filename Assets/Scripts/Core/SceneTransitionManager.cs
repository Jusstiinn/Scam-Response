using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    private bool loading;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (fadeCanvasGroup != null) { fadeCanvasGroup.alpha = 0; fadeCanvasGroup.blocksRaycasts = false; }
    }

    public void LoadScene(string sceneName) { if (!loading) StartCoroutine(LoadRoutine(sceneName)); }
    private IEnumerator LoadRoutine(string sceneName)
    {
        loading = true;
        yield return FadeTo(1);
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;
        yield return FadeTo(0);
        loading = false;
    }
    private IEnumerator FadeTo(float target)
    {
        if (fadeCanvasGroup == null) yield break;
        fadeCanvasGroup.blocksRaycasts = true;
        float start = fadeCanvasGroup.alpha, elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(start, target, elapsed / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = target;
        fadeCanvasGroup.blocksRaycasts = target > 0.01f;
    }
}
