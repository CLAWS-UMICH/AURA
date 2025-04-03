using UnityEngine;
using XRMultiplayer;

public class InteractableSpawner : MonoBehaviour
{
    public Transform spawnTransform;
    public NetworkBaseInteractable spawnInteractablePrefab;

    public int panelId;
    public int slotId;

    public bool freezeOnSpawn = true;
    public float distanceToSpawnNew = .5f;
    public float spawnCooldown = .5f;

    internal float m_SpawnCooldownTimer = 0f;

    /// <summary>
    /// The current network interactable object in the dispenser slot.
    /// </summary>
    [HideInInspector] public NetworkBaseInteractable currentInteractable;

    public bool CheckInteractablePosition()
    {
        if (currentInteractable == null)
            return false;

        float currentDistance = Vector3.Distance(currentInteractable.transform.position, spawnTransform.position);
        return currentDistance > distanceToSpawnNew;
    }

    public bool CanSpawn(float deltaTime)
    {
        if (currentInteractable != null) return false;
        if (m_SpawnCooldownTimer > 0)
        {
            UpdateCooldown(m_SpawnCooldownTimer - deltaTime);
            return false;
        }

        UpdateCooldown(spawnCooldown);
        return true;
    }

    void UpdateCooldown(float newTime)
    {
        m_SpawnCooldownTimer = newTime;
    }

    public NetworkBaseInteractable SpawnInteractablePrefab(Transform spawnerTransform)
    {
        UpdateCooldown(spawnCooldown);
        NetworkBaseInteractable spawnedInteractable = UnityEngine.Object.Instantiate
        (
            spawnInteractablePrefab,
            spawnerTransform.position,
            spawnerTransform.rotation
        );
        spawnedInteractable.transform.localScale = spawnerTransform.localScale;

        return spawnedInteractable;
    }
}
