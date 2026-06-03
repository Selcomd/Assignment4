using UnityEngine;

public class KeycardDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform checkpoint;
    [SerializeField] private int nextLevelKeycardAmount = 3;

    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    private GameObject player;
    private bool playerNearby = false;

    private void Update()
    {
        if (!playerNearby || player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= interactDistance && Input.GetKeyDown(interactKey))
        {
            if (KeycardManager.instance.HasAllKeycards)
            {
                TeleportPlayer();
            }
            else
            {
                Debug.Log("You need all keycards before you can proceed.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            player = other.gameObject;
            playerNearby = true;
            Debug.Log("Press E to use door.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerNearby = false;
            player = null;
        }
    }

    private void TeleportPlayer()
    {
        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        player.transform.position = checkpoint.position;
        player.transform.rotation = checkpoint.rotation;

        if (controller != null)
        {
            controller.enabled = true;
        }

        KeycardManager.instance.StartLevelKeycards(nextLevelKeycardAmount);

        Debug.Log("Access granted. Player teleported. Keycards reset.");
    }
}