using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TMPButtonTextColorChanger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Color normalTextColor = Color.white;
    public Color pressedTextColor = Color.red;
    public Color selectedTextColor = Color.yellow;

    private TMP_Text buttonText;
    private Selectable selectable;
    private bool isPressed = false;

    void Start()
    {
        buttonText = GetComponentInChildren<TMP_Text>();
        selectable = GetComponent<Selectable>();

        if (buttonText == null)
        {
            Debug.LogError("No TextMeshPro component found in children");
            return;
        }

        buttonText.color = normalTextColor;
    }

    void Update()
    {
        // Don't update color if button is being pressed
        if (isPressed)
            return;

        // Check the current selection state
        if (selectable != null && EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject == gameObject)
        {
            buttonText.color = selectedTextColor;
        }
        else
        {
            buttonText.color = normalTextColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        buttonText.color = pressedTextColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;

        // Check if the button is currently selected
        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject == gameObject)
        {
            buttonText.color = selectedTextColor;
        }
        else
        {
            buttonText.color = normalTextColor;
        }
    }
}