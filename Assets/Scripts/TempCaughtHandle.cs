using UnityEngine;

public class TempCaughtHandler : MonoBehaviour
{
    private void Start()
    {
        FloorManager.Instance.OnPlayerCaught += HandleCaught;
    }

    private void OnDisable()
    {
        if (FloorManager.Instance != null)
            FloorManager.Instance.OnPlayerCaught -= HandleCaught;
    }

    private void HandleCaught()
    {
        Debug.Log("[TempCaughtHandler] Player ketangkep! (nanti diganti circle wipe animation)");
        FloorManager.Instance.BeginFloor(); // sementara langsung reset visual, ga ada animasi dulu
    }
}