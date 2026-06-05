using UnityEngine;
using System;

public class LevelResetManager : MonoBehaviour
{
    [Serializable]
    public class FloorConfig
    {
        public string guardTag;
        public Transform respawnPoint;
        public int keycardCount;
    }

    [Header("Floor Configurations")]
    [SerializeField] private FloorConfig[] floors;

    [Header("Player")]
    [SerializeField] private Transform player;

    private bool resetTriggered = false;
    private FloorConfig activeFloor = null;

    public void TriggerReset(string guardTag)
    {
        if (resetTriggered) return;
        resetTriggered = true;

        activeFloor = Array.Find(floors, f => f.guardTag == guardTag);

        if (activeFloor == null) Debug.LogWarning($"LevelResetManager: No floor config found for tag '{guardTag}'!");
    }

    public void ExecuteReset()
    {
        if (activeFloor == null)
        {
            Debug.LogWarning("LevelResetManager: No active floor set, skipping reset.");
            resetTriggered = false;
            return;
        }

        if (HUDController.instance != null) HUDController.instance.AddCaught();

        if (player != null && activeFloor.respawnPoint != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            
            if (controller != null) controller.enabled = false;
            
            player.position = activeFloor.respawnPoint.position;
            player.rotation = activeFloor.respawnPoint.rotation;
            
            if (controller != null) controller.enabled = true;
        }

        KeycardPickup[] keycards = FindObjectsByType<KeycardPickup>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (KeycardPickup keycard in keycards) keycard.gameObject.SetActive(true);

        GuardAI[] guards = FindObjectsByType<GuardAI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (GuardAI guard in guards) guard.ResetGuard();

        FirstPersonPlayerController playerController = player.GetComponent<FirstPersonPlayerController>();
        if (playerController != null) playerController.RestoreSmokeScreen();

        KeycardManager.instance.StartLevelKeycards(activeFloor.keycardCount);

        activeFloor = null;
        resetTriggered = false;

        Debug.Log("Level reset complete.");
    }
}