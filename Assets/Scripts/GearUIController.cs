using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class GearUIController : MonoBehaviour
{
    public GearController gear1;
    public GearController gear2;

    public TMP_InputField gear1SpeedInput;
    public TMP_InputField gear2SpeedInput;

    public Button startButton;
    public Button stopButton;
    public Button resetButton;

    [SerializeField] private XRKeyboardManager keyboardManager; // Reference to your VR keyboard system

    void Start()
    {
        // Set up button click listeners
        startButton.onClick.AddListener(StartRotation);
        stopButton.onClick.AddListener(StopRotation);
        resetButton.onClick.AddListener(ResetRotation);

        // Set up input field listeners
        gear1SpeedInput.onSelect.AddListener((text) => ShowKeyboard(gear1SpeedInput));
        gear2SpeedInput.onSelect.AddListener((text) => ShowKeyboard(gear2SpeedInput));

        // Set default values
        gear1SpeedInput.text = gear1.rotationSpeed.ToString();
        gear2SpeedInput.text = gear2.rotationSpeed.ToString();
    }

    private void ShowKeyboard(TMP_InputField inputField)
    {
        if (keyboardManager != null)
        {
            keyboardManager.ShowKeyboard(inputField);
        }
        else
        {
            Debug.LogWarning("Keyboard manager reference not set!");
            // Fallback to Unity's system keyboard if available
            TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad);
        }
    }


    // ... rest of your existing methods ...
    public void StartRotation()
    {
        UpdateGearSpeeds();
        ToggleRotation(true);
    }

    public void StopRotation()
    {
        ToggleRotation(false);
    }

    public void ResetRotation()
    {
        gear1.ResetRotation();
        gear2.ResetRotation();
    }

    private void UpdateGearSpeeds()
    {
        if (float.TryParse(gear1SpeedInput.text, out float speed1))
            gear1.SetRotationSpeed(speed1);

        if (float.TryParse(gear2SpeedInput.text, out float speed2))
            gear2.SetRotationSpeed(speed2);
    }

    private void ToggleRotation(bool shouldRotate)
    {
        if (shouldRotate)
        {
            gear1.StartRotation();
            gear2.StartRotation();
        }
        else
        {
            gear1.StopRotation();
            gear2.StopRotation();
        }
    }
}

