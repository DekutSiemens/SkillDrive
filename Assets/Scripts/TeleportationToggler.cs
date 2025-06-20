using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class TeleportationToggler : MonoBehaviour
{
    [SerializeField] private GameObject teleportationAnchorsParent;
    [SerializeField] private float checkInterval = 0.5f;

    private bool lastControllerState = true;

    private void Start()
    {
        InvokeRepeating(nameof(CheckInputMethod), 0f, checkInterval);
    }

    private void CheckInputMethod()
    {
        bool controllersActive = AreControllersActive();

        if (controllersActive != lastControllerState)
        {
            teleportationAnchorsParent.SetActive(controllersActive);
            lastControllerState = controllersActive;
        }
    }

    private bool AreControllersActive()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand, devices);

        foreach (var device in devices)
        {
            if (device.isValid && device.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked) && isTracked)
            {
                return true;
            }
        }
        return false;
    }
}