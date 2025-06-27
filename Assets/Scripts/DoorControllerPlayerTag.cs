using UnityEngine;
using System.Collections;

public class DoorControllerPlayerTag : MonoBehaviour
{
    private Animator _Anim;
    private OcclusionPortal _occlusionPortal;
    private bool _isOpen = false;
    private bool _playerInZone = false;
    private Coroutine _closePortalCoroutine;

    [Header("Door Settings")]
    [SerializeField] private float _debounceTime = 0.1f; // Prevent rapid toggling
    private float _lastTriggerTime = 0f;

    void Start()
    {
        _Anim = GetComponent<Animator>();
        _occlusionPortal = GetComponentInChildren<OcclusionPortal>();

        if (_occlusionPortal != null)
        {
            _occlusionPortal.open = _isOpen;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Debounce rapid triggers
            if (Time.time - _lastTriggerTime < _debounceTime)
                return;

            _lastTriggerTime = Time.time;
            _playerInZone = true;

            // Only open if door is currently closed
            if (!_isOpen)
            {
                SetDoorState(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Debounce rapid triggers
            if (Time.time - _lastTriggerTime < _debounceTime)
                return;

            _lastTriggerTime = Time.time;
            _playerInZone = false;

            // Only close if door is currently open
            if (_isOpen)
            {
                SetDoorState(false);
            }
        }
    }

    private void SetDoorState(bool shouldOpen)
    {
        if (_isOpen == shouldOpen)
            return; // Already in desired state

        _isOpen = shouldOpen;
        _Anim.SetTrigger("DoorTrigger");

        if (_occlusionPortal != null)
        {
            if (_isOpen)
            {
                // Open immediately when door opens
                _occlusionPortal.open = true;

                // Cancel any pending close operations
                if (_closePortalCoroutine != null)
                {
                    StopCoroutine(_closePortalCoroutine);
                    _closePortalCoroutine = null;
                }
            }
            else
            {
                // Start coroutine to close after delay when door closes
                if (_closePortalCoroutine != null)
                {
                    StopCoroutine(_closePortalCoroutine);
                }
                _closePortalCoroutine = StartCoroutine(ClosePortalAfterDelay(3f));
            }
        }
    }

    private IEnumerator ClosePortalAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_occlusionPortal != null)
        {
            _occlusionPortal.open = false;
        }
        _closePortalCoroutine = null;
    }

    // Optional animation event methods - these now sync with the actual state
    public void OnDoorAnimationOpened()
    {
        // Animation confirms door is open
        if (_occlusionPortal != null)
        {
            _occlusionPortal.open = true;
        }
    }

    public void OnDoorAnimationClosed()
    {
        // Animation confirms door is closed
        if (_occlusionPortal != null && !_playerInZone)
        {
            // Only start delayed close if player is not in zone
            if (_closePortalCoroutine != null)
            {
                StopCoroutine(_closePortalCoroutine);
            }
            _closePortalCoroutine = StartCoroutine(ClosePortalAfterDelay(3f));
        }
    }

    // Debug method to check current state
    private void OnDrawGizmosSelected()
    {
        if (_playerInZone)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        }
    }
}