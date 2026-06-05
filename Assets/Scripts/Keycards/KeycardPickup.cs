using UnityEngine;

public class KeycardPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    [Header("Pickup Effect")]
    [SerializeField] private ParticleSystem pickupEffect;

    [Header("Pickup Audio")]
    [SerializeField] private AudioClip pickupSound;

    private Transform player;

    private void OnEnable()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            float dist = Vector3.Distance(transform.position, playerObj.transform.position);
            if (dist <= interactDistance) player = playerObj.transform;
        }
    }

    private void OnDisable()
    {
        player = null;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactDistance && Input.GetKeyDown(interactKey)) PickUpKeycard();
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
        if (other.CompareTag(playerTag)) player = null;
    }

    private void PickUpKeycard()
    {
        KeycardManager.instance.CollectKeycard();

        if (pickupEffect != null)
        {
            ParticleSystem effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
            effect.gameObject.SetActive(true);
            effect.Play();
            Destroy(effect.gameObject, 2f);
        }

        if (pickupSound != null)
        {
            float volume = AudioSettingsManager.instance != null ? AudioSettingsManager.instance.GetVolume(AudioCategory.KeycardPickup) : 1f;

            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
        }

        Debug.Log("Keycard picked up.");
        gameObject.SetActive(false);
    }
}