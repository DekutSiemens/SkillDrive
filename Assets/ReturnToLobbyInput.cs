// Enhanced VR Input Handler
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ReturnToLobbyInput : MonoBehaviour
{
    [Header("Input Settings")]
    public float buttonHoldTime = 0.5f; // Require holding button to prevent accidental presses

    private InputDevice leftController;
    private bool isInitialized = false;
    private float menuButtonHoldTime = 0f;
    private bool wasMenuPressed = false;

    void Start()
    {
        TryInitialize();
    }

    void TryInitialize()
    {
        var leftHandedControllers = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandedControllers);

        if (leftHandedControllers.Count > 0)
        {
            leftController = leftHandedControllers[0];
            isInitialized = true;
            Debug.Log($"Left controller initialized: {leftController.name}");
        }
    }

    void Update()
    {
        if (!isInitialized || !leftController.isValid)
        {
            TryInitialize();
            return;
        }

        // Menu button with hold requirement
        bool menuPressed = false;
        if (leftController.TryGetFeatureValue(CommonUsages.menuButton, out menuPressed))
        {
            if (menuPressed)
            {
                if (!wasMenuPressed)
                {
                    menuButtonHoldTime = 0f;
                }

                menuButtonHoldTime += Time.unscaledDeltaTime;

                if (menuButtonHoldTime >= buttonHoldTime)
                {
                    if (SceneTransitionManager.Instance != null)
                    {
                        SceneTransitionManager.Instance.ReturnToLobby();
                    }
                    menuButtonHoldTime = 0f; // Reset to prevent multiple triggers
                }
            }
            else
            {
                menuButtonHoldTime = 0f;
            }

            wasMenuPressed = menuPressed;
        }
    }
}