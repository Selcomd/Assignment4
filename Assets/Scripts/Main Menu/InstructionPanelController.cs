using UnityEngine;

public class InstructionPanelController : MonoBehaviour
{
    [SerializeField] private KeyCode closeKey = KeyCode.Escape;

    private void Start()
    {
        LockGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(closeKey))
        {
            UnlockGame();
            gameObject.SetActive(false);
        }
    }

    private void LockGame()
    {
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UnlockGame()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}