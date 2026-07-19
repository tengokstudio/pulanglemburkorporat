using UnityEngine;
using TMPro;

public class FloorNumberDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;

    private int lastDisplayedFloor = int.MinValue;

    private void Update()
    {
        if (FloorManager.Instance == null || textMesh == null) return;

        int floor = FloorManager.Instance.CurrentFloor;
        if (floor == lastDisplayedFloor) return;

        lastDisplayedFloor = floor;
        textMesh.text = floor.ToString();
    }
}
