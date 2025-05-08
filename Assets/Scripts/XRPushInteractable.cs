using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRPushButton : XRBaseInteractable
{
    [Header("Button Settings")]
    public float pushDepth = 0.05f; // How far the button can be pressed
    public float returnSpeed = 5f; // How fast the button returns to its original position
    public bool isPressed = false; // Whether the button is currently pressed

    private Vector3 initialPosition;
    private XRBaseInteractor currentInteractor;

    protected override void Awake()
    {
        base.Awake();
        initialPosition = transform.localPosition;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        currentInteractor = args.interactorObject as XRBaseInteractor;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        currentInteractor = null;
        isPressed = false;
    }

    private void Update()
    {
        if (currentInteractor != null)
        {
            // Calculate the button's position based on the interactor's position
            Vector3 interactorPosition = transform.parent.InverseTransformPoint(currentInteractor.transform.position);
            float pressDistance = Mathf.Clamp(interactorPosition.y, initialPosition.y - pushDepth, initialPosition.y);

            // Move the button
            transform.localPosition = new Vector3(initialPosition.x, pressDistance, initialPosition.z);

            // Check if the button is fully pressed
            if (!isPressed && pressDistance <= initialPosition.y - pushDepth)
            {
                isPressed = true;
                OnButtonPressed();
            }
        }
        else
        {
            // Return the button to its initial position
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, returnSpeed * Time.deltaTime);
        }
    }

    private void OnButtonPressed()
    {
        Debug.Log("Button Pressed!");
        // Add your custom logic here for when the button is pressed
    }
}