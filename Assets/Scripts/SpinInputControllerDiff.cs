using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpinInputControllerDiff : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField[] spinSpeedInputFields = new TMP_InputField[12]; // Array for 12 input fields
    public Button startButton;        // Button to start spinning
    public Button stopButton;         // Button to stop spinning
    public Button resetButton;        // Button to reset rotations

    [Header("Spin Controller Reference")]
    public SpinController spinController; // Reference to your existing SpinController

    private Quaternion[] initialRotations; // To store initial rotations for reset

    void Start()
    {
        // Validate that we have enough input fields
        if (spinSpeedInputFields.Length != 12)
        {
            Debug.LogError("SpinInputController requires exactly 12 input fields!");
            return;
        }

        // Store initial rotations
        initialRotations = new Quaternion[spinController.objectsToSpin.Length];
        for (int i = 0; i < spinController.objectsToSpin.Length; i++)
        {
            if (spinController.objectsToSpin[i].objectToSpin != null)
            {
                initialRotations[i] = spinController.objectsToSpin[i].objectToSpin.rotation;
            }
        }

        // Set up button click listeners
        startButton.onClick.AddListener(StartSpinning);
        stopButton.onClick.AddListener(StopSpinning);
        resetButton.onClick.AddListener(ResetRotations);

        // Set up input field validation and listeners
        foreach (var inputField in spinSpeedInputFields)
        {
            if (inputField != null)
            {
                inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                inputField.onEndEdit.AddListener((value) => UpdateSpinSpeeds());
            }
        }
    }

    public void UpdateSpinSpeeds()
    {
        // Update spin speeds for all objects with valid input
        for (int i = 0; i < Mathf.Min(spinSpeedInputFields.Length, spinController.objectsToSpin.Length); i++)
        {
            if (spinSpeedInputFields[i] != null &&
                float.TryParse(spinSpeedInputFields[i].text, out float speed))
            {
                spinController.SetObjectSpinSpeed(i, speed);
            }
        }
    }

    public void StartSpinning()
    {
        // Update speeds from input fields before starting
        UpdateSpinSpeeds();
        spinController.StartSpinning();
    }

    public void StopSpinning()
    {
        spinController.StopSpinning();
    }

    public void ResetRotations()
    {
        spinController.StopSpinning();

        // Reset each object to its initial rotation
        for (int i = 0; i < Mathf.Min(spinController.objectsToSpin.Length, initialRotations.Length); i++)
        {
            if (spinController.objectsToSpin[i].objectToSpin != null)
            {
                spinController.objectsToSpin[i].objectToSpin.rotation = initialRotations[i];
            }
        }

        // Optional: Clear all input fields
        foreach (var inputField in spinSpeedInputFields)
        {
            if (inputField != null) inputField.text = "";
        }
    }
}