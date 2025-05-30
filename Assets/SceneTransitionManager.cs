using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    public FadeScreen fadeScreen;

    [Header("Lobby Settings")]
    public string lobbySceneName = "Lobby";

    [Header("Loading Settings")]
    public float minLoadingTime = 1f; // Minimum time to show loading

    // Events for external systems to hook into
    public UnityEvent<string> OnSceneTransitionStart;
    public UnityEvent<string> OnSceneTransitionComplete;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Rebind fade screen
        if (fadeScreen == null)
        {
            fadeScreen = FindObjectOfType<FadeScreen>();
        }

        // Notify that transition is complete
        OnSceneTransitionComplete?.Invoke(scene.name);
        isTransitioning = false;
    }

    public void TransitionToScene(string sceneName)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("Scene transition already in progress.");
            return;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name cannot be null or empty.");
            return;
        }

        StartCoroutine(TransitionRoutine(sceneName));
    }

    public void ReturnToLobby()
    {
        if (!string.IsNullOrEmpty(lobbySceneName))
        {
            TransitionToScene(lobbySceneName);
        }
        else
        {
            Debug.LogWarning("Lobby scene name not set.");
        }
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;
        OnSceneTransitionStart?.Invoke(sceneName);

        float startTime = Time.unscaledTime;

        // Fade out
        if (fadeScreen != null)
        {
            fadeScreen.FadeOut();
            yield return new WaitForSecondsRealtime(fadeScreen.fadeDuration);
        }

        // Start loading
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Wait until scene is ~90% loaded
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Ensure minimum loading time for better UX
        float elapsedTime = Time.unscaledTime - startTime;
        if (elapsedTime < minLoadingTime)
        {
            yield return new WaitForSecondsRealtime(minLoadingTime - elapsedTime);
        }

        // Activate the scene
        asyncLoad.allowSceneActivation = true;
        yield return new WaitUntil(() => asyncLoad.isDone);
        yield return null; // Allow one frame for UI to initialize

        // Fade in
        if (fadeScreen != null)
        {
            fadeScreen.FadeIn();
        }
    }
}