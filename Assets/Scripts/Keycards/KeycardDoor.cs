using UnityEngine;

public class KeycardDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform checkpoint;
    [SerializeField] private int requiredKeycards = 3;
    [SerializeField] private bool isFirstFloor = false;
    [SerializeField] private bool hasNextFloor = true;
    [SerializeField] private int nextFloorKeycardCount = 3;

    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    [Header("Door Audio")]
    [SerializeField] private AudioClip accessGrantedSound;
    [SerializeField] private AudioClip accessDeniedSound;
    [SerializeField] [Range(0f, 1f)] private float volume = 1f;

    private GameObject player;
    private bool playerNearby = false;

    private void Start()
    {
        if (isFirstFloor) KeycardManager.instance.StartLevelKeycards(requiredKeycards);
    }

    private void Update()
    {
        if (!playerNearby || player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= interactDistance && Input.GetKeyDown(interactKey))
        {
            if (KeycardManager.instance.CollectedKeycards >= requiredKeycards)
            {
                PlaySound(accessGrantedSound);
                TeleportPlayer();
            }
            else
            {
                PlaySound(accessDeniedSound);
                int remaining = requiredKeycards - KeycardManager.instance.CollectedKeycards;
                Debug.Log($"You need {remaining} more keycard(s) to proceed.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            player = other.gameObject;
            playerNearby = true;
            Debug.Log($"Press E to use door. ({KeycardManager.instance.CollectedKeycards}/{requiredKeycards} keycards)");
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

        if (controller != null) controller.enabled = false;

        player.transform.position = checkpoint.position;
        player.transform.rotation = checkpoint.rotation;

        if (controller != null) controller.enabled = true;

        if (hasNextFloor) KeycardManager.instance.StartLevelKeycards(nextFloorKeycardCount);

        Debug.Log("Access granted. Player teleported. Keycards reset.");
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null) AudioSource.PlayClipAtPoint(clip, transform.position, volume);
    }
}