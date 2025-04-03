using UnityEngine;
using UnityEngine.Events;

public class DotProductToggle : MonoBehaviour
{
    [SerializeField] float dotProductThreshold = 0.8f;
    [SerializeField] Transform m_LookAtTransform;
    [SerializeField] UnityEvent<bool> onToggle;

    private Transform playerCameraTransform;


    bool isLookingAt = true;

    void Awake()
    {
        playerCameraTransform = Camera.main.transform;
        if (m_LookAtTransform == null)
            m_LookAtTransform = transform;
    }
    void Update()
    {
        bool wasLookingAt = isLookingAt;
        isLookingAt = XRMultiplayer.Utils.IsPlayerLookingTowards(playerCameraTransform, m_LookAtTransform, dotProductThreshold);
        if (wasLookingAt != isLookingAt)
        {
            onToggle.Invoke(isLookingAt);
        }
    }
}
