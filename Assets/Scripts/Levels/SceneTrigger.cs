using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName;

    [Header("Player Detection")]
    [SerializeField] private string playerTag = "Player";

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag(playerTag))
        {
            hasTriggered = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}