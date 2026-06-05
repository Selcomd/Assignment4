using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "CityScene";

    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject settingsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenHowToPlay()
    {
        mainMenuPanel.SetActive(false);
        howToPlayPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        howToPlayPanel.SetActive(false);
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}