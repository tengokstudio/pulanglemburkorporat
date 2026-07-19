using UnityEngine;

public class ForwardExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (FloorManager.Instance == null)
        {
            Debug.LogError("[ForwardExitTrigger] FloorManager.Instance null — cek apakah FloorManager ada di scene dan aktif.");
            return;
        }

        FloorManager.Instance.PlayerReachedForwardExit();
    }
}