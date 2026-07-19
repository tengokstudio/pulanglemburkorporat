using UnityEngine;

public class LemariInteract : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject interactUI; 

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