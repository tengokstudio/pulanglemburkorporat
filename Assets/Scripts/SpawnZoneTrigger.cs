// SpawnZoneTrigger.cs — baru, taruh persis di SpawnPoint_Lift
using UnityEngine;

public class SpawnZoneTrigger : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (FloorManager.Instance == null)
        {
            Debug.LogError("[SpawnZoneTrigger] FloorManager.Instance null.");
            return;
        }

        FloorManager.Instance.MarkLeftSpawnZone();
        //Debug.Log("Left spawn zone");
    }
}