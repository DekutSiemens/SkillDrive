using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class XRKeyboardManager : MonoBehaviour
{
    public GameObject keyboardPrefab;
    public float keyboardDistance = 0.5f;
    public float keyboardHeight = -0.2f;

    private GameObject currentKeyboard;
    private TMP_InputField activeInputField;

    public void ShowKeyboard(TMP_InputField inputField)
    {
        activeInputField = inputField;

        // Destroy existing keyboard if any
        if (currentKeyboard != null)
        {
            Destroy(currentKeyboard);
        }

        // Create new keyboard
        currentKeyboard = Instantiate(keyboardPrefab);
        currentKeyboard.SetActive(true);

        // Position keyboard relative to the VR camera/head
        Transform headTransform = Camera.main.transform;
        Vector3 position = headTransform.position +
                         headTransform.forward * keyboardDistance +
                         Vector3.up * keyboardHeight;

        currentKeyboard.transform.position = position;
        currentKeyboard.transform.LookAt(headTransform);
        currentKeyboard.transform.Rotate(0, 180, 0); // Flip to face user

        // Setup all keyboard buttons
        var keys = currentKeyboard.GetComponentsInChildren<XRSimpleInteractable>();
        foreach (var key in keys)
        {
            key.selectEntered.AddListener(OnKeyPressed);
        }
    }

    private void OnKeyPressed(SelectEnterEventArgs args)
    {
        XRSimpleInteractable key = args.interactableObject as XRSimpleInteractable;
        string character = key.name; // Name your keyboard buttons with the characters they represent

        if (character == "Backspace")
        {
            if (activeInputField.text.Length > 0)
            {
                activeInputField.text = activeInputField.text.Substring(0, activeInputField.text.Length - 1);
            }
        }
        else if (character == "Enter")
        {
            Destroy(currentKeyboard);
        }
        else
        {
            activeInputField.text += character;
        }
    }
}