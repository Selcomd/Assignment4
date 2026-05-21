using UnityEngine;

public class WinPickup : MonoBehaviour
{
    [Header("Win Curtain")]
    public WinCurtainController winCurtain;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    private Transform player;
    private bool hasBeenCollected = false;

    private void Start()
    {
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }
    }

    private void Update()
    {
        if (hasBeenCollected || player == null)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= interactDistance && Input.GetKeyDown(interactKey))
        {
            CollectWinPickup();
        }
    }

    private void CollectWinPickup()
    {
        hasBeenCollected = true;

        if (winCurtain != null)
        {
            winCurtain.StartWinCurtain();
        }

        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}