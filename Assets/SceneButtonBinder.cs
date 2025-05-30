using UnityEngine;
using UnityEngine.UI;

public class SceneButtonBinder : MonoBehaviour
{
    [Header("Scene Settings")]
    public string targetSceneName;

    [Header("Validation")]
    public bool validateSceneExists = true;

    void Start()
    {
        // Validate scene name
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError($"Target scene name not set on {gameObject.name}");
            return;
        }

        // Optional: Validate scene exists in build settings
        if (validateSceneExists && !IsSceneInBuildSettings(targetSceneName))
        {
            Debug.LogError($"Scene '{targetSceneName}' not found in build settings on {gameObject.name}");
            return;
        }

        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError($"No Button component found on {gameObject.name}");
        }
    }

    private void OnButtonClick()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("SceneTransitionManager not found.");
        }
    }

    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameFromPath == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}