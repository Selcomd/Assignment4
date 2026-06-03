using UnityEngine;

public class KeycardPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    private Transform player;

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactDistance && Input.GetKeyDown(interactKey))
        {
            PickUpKeycard();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            player = other.transform;
            Debug.Log("Press E to pick up keycard.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            player = null;
        }
    }

    private void PickUpKeycard()
    {
        KeycardManager.instance.CollectKeycard();

        Debug.Log("Keycard picked up.");

        Destroy(gameObject);
    }
}