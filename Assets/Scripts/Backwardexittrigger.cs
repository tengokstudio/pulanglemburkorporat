using UnityEngine;

public class BackwardExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (FloorManager.Instance == null)
        {
            Debug.LogError("[BackwardExitTrigger] FloorManager.Instance null — cek apakah FloorManager ada di scene dan aktif.");
            return;
        }

        FloorManager.Instance.PlayerReachedBackwardExit();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (FloorManager.Instance == null)
        {
            Debug.LogError("[BackwardExitTrigger] FloorManager.Instance null — cek apakah FloorManager ada di scene dan aktif.");
            return;
        }

        FloorManager.Instance.MarkLeftSpawnZone();
    }
}