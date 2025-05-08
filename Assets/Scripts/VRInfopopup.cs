using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class VRInfoPopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Button closeButton;

    [Header("Display Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private bool faceCamera = true;

    private Coroutine fadeRoutine;
    private Transform cameraTransform;

    private void Awake()
    {
        // Initialize references
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (closeButton != null) closeButton.onClick.AddListener(HidePopup);

        cameraTransform = Camera.main.transform;
        SetVisibility(0f, false);
    }

    private void LateUpdate()
    {
        if (faceCamera && canvasGroup.alpha > 0)
        {
            transform.LookAt(cameraTransform);
            transform.Rotate(0, 180f, 0); // Flip to face correctly
        }
    }

    public void ShowPopup(string information, Vector3 position)
    {
        transform.position = position;
        infoText.text = information;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadePopup(1f));
    }

    public void HidePopup()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadePopup(0f));
    }

    private IEnumerator FadePopup(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        SetVisibility(targetAlpha > 0.5f);

        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private void SetVisibility(bool isVisible)
    {
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }

    private void SetVisibility(float alpha, bool? isVisible = null)
    {
        canvasGroup.alpha = alpha;
        if (isVisible.HasValue)
        {
            canvasGroup.interactable = isVisible.Value;
            canvasGroup.blocksRaycasts = isVisible.Value;
        }
    }
}