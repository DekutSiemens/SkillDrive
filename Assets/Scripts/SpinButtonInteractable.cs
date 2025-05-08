using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SpinButtonInteractable : MonoBehaviour
{
    [Header("Button Type")]
    public bool isStartButton = true; // Set this to true for the start button, false for the stop button

    [Header("Spin Controller")]
    public SpinController spinController; // Reference to the SpinController

    private XRSimpleInteractable simpleInteractable;

    void Start()
    {
        // Get the XRSimpleInteractable component
        simpleInteractable = GetComponent<XRSimpleInteractable>();

        if (simpleInteractable == null)
        {
            Debug.LogError("XRSimpleInteractable component not found on this GameObject.");
            return;
        }

        // Subscribe to the Select Entered event
        simpleInteractable.selectEntered.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        if (isStartButton)
        {
            spinController.StartSpinning();
        }
        else
        {
            spinController.StopSpinning();
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.RemoveListener(OnButtonPressed);
        }
    }
}