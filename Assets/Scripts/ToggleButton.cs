using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    private int index;
    private bool isSelected = false;
    private Button button;
    private Image background;

    private System.Action<int, bool> callback;

    // Custom colors
    private Color normalColor = new Color(0.627f, 0.812f, 1.0f);        // A0CFFF
    private Color highlightedColor = new Color(0.569f, 0.741f, 0.914f); // 91BDE9 (hover - optional)
    private Color selectedColor = new Color(0.0f, 0.211f, 0.427f);      // 00366D

    public void Init(int idx, System.Action<int, bool> onToggle)
    {
        index = idx;
        callback = onToggle;

        button = GetComponent<Button>();
        background = GetComponent<Image>();

        button.onClick.AddListener(() =>
        {
            isSelected = !isSelected;
            UpdateVisual();
            callback?.Invoke(index, isSelected);
        });

        UpdateVisual();
    }

    public void SetSelected(bool value)
    {
        isSelected = value;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (background != null)
        {
            background.color = isSelected ? selectedColor : normalColor;
        }
    }
}
