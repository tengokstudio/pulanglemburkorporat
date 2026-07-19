using UnityEngine;

public class LemariInteract : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject interactUI;
    public Transform playerTransform; // Masukkan objek Player ke sini
    
    [Header("Pengaturan Posisi UI")]
    public Vector3 offset = new Vector3(0f, 2f, 0f);

    private bool isPlayerNear = false;

    void Start()
    {
        if (interactUI != null) 
        {
            interactUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerNear)
        {
            interactUI.transform.position = playerTransform.position + offset;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            interactUI.SetActive(true); // Munculkan teks UI
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            interactUI.SetActive(false); // Sembunyikan teks UI
        }
    }
}