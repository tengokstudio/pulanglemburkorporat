using UnityEngine;
using TMPro;

public class FloorNumberDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [Tooltip("Kalau nyala, tampilin teks \"EXIT\" pas di lantai terakhir alih-alih nomor lantai.")]
    [SerializeField] private bool showExitOnFinalFloor = false;

    private int lastDisplayedFloor = int.MinValue;

    private void Update()
    {
        if (FloorManager.Instance == null || textMesh == null) return;

        int floor = FloorManager.Instance.CurrentFloor;
        if (floor == lastDisplayedFloor) return;

        lastDisplayedFloor = floor;

        bool isFinalFloor = floor == FloorManager.Instance.FinalFloor;
        textMesh.text = (showExitOnFinalFloor && isFinalFloor) ? "EXIT" : floor.ToString();
    }
}
