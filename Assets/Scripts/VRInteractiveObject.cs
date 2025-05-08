using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class VRInteractiveObject : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string objectInfo;

    [Header("Popup Settings")]
    [SerializeField] private VRInfoPopup infoPopup;  // Assign specific canvas in Inspector
    [SerializeField] private Vector3 popupOffset = new Vector3(0, 0.2f, 0);

    private XRSimpleInteractable simpleInteractable;

    private void Awake()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();

        // Validate assigned popup
        if (infoPopup == null)
        {
            Debug.LogError($"No VRInfoPopup assigned to {gameObject.name}!", this);
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        simpleInteractable.selectEntered.AddListener(OnObjectSelected);
    }

    private void OnDisable()
    {
        simpleInteractable.selectEntered.RemoveListener(OnObjectSelected);
    }

    private void OnObjectSelected(SelectEnterEventArgs args)
    {
        Vector3 popupPosition = transform.position + popupOffset;
        infoPopup.ShowPopup(objectInfo, popupPosition);
    }
}