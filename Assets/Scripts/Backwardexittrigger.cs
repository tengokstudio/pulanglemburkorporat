using UnityEngine;

public class BackwardExitTrigger : MonoBehaviour
{
    private bool playerInRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

    private void Update()
    {
        if (!playerInRange || !Input.GetKeyDown(KeyCode.Space)) return;

        if (FloorManager.Instance == null)
        {
            Debug.LogError("[BackwardExitTrigger] FloorManager.Instance null — cek apakah FloorManager ada di scene dan aktif.");
            return;
        }

        FloorManager.Instance.PlayerReachedBackwardExit();
    }
}
