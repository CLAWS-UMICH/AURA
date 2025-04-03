using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class TableInitializer : MonoBehaviour
{
    [SerializeField]
    Vector3 m_SpawnOffset;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var m_Head = Camera.main.transform;

        var table = FindFirstObjectByType<TableSeatSystem>(FindObjectsInactive.Include);
        table.gameObject.SetActive(true);
        // Calculate inverse position difference and apply to the player to make the table to feel like it's in the same place.
        var spawnPos = transform.position - table.tableTop.seats[0].seatTransform.forward * m_SpawnOffset.z + Vector3.up * m_SpawnOffset.y;
        var inversePositionDifference = m_Head.position - spawnPos + (Vector3.down * m_Head.localPosition.y);

        TeleportRequest teleportRequest = new TeleportRequest
        {
            destinationPosition = inversePositionDifference,
        };
        FindFirstObjectByType<TeleportationProvider>().QueueTeleportRequest(teleportRequest);

        Destroy(gameObject);
    }
}
