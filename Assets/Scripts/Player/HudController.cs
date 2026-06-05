using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController instance;

    [Header("Timer")]
    [SerializeField] private TMP_Text timerText;

    [Header("Caught Counter")]
    [SerializeField] private TMP_Text caughtText;

    [Header("Keycards")]
    [SerializeField] private TMP_Text keycardText;

    [Header("Smoke Ability Indicator")]
    [SerializeField] private Image smokeIndicatorImage;
    [SerializeField] private TMP_Text smokeIndicatorText;
    [SerializeField] private Color smokeAvailableColor = Color.green;
    [SerializeField] private Color smokeUsedColor = Color.gray;
    [SerializeField] private string smokeAvailableLabel = "SMOKE [R]";
    [SerializeField] private string smokeUsedLabel = "SMOKE USED";

    private float timer = 0f;
    private int caughtCount = 0;
    private bool running = true;

    private FirstPersonPlayerController playerController;

    public float ElapsedTime => timer;
    public int CaughtCount => caughtCount;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerController = FindFirstObjectByType<FirstPersonPlayerController>();
        UpdateUI();
    }

    private void Update()
    {
        if (running) timer += Time.deltaTime;

        UpdateUI();
    }

    public void AddCaught()
    {
        caughtCount++;
    }

    public void StopTimer()
    {
        running = false;
    }

    private void UpdateUI()
    {
        if (timerText != null)
        {
            int minutes = (int)(timer / 60f);
            int seconds = (int)(timer % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        if (caughtText != null) caughtText.text = $"Times Caught: {caughtCount}";
        if (keycardText != null && KeycardManager.instance != null) keycardText.text = $"Keycards: {KeycardManager.instance.CollectedKeycards} / {KeycardManager.instance.RequiredKeycards}";

        if (playerController != null)
        {
            bool available = playerController.SmokeAvailable;

            if (smokeIndicatorImage != null) smokeIndicatorImage.color = available ? smokeAvailableColor : smokeUsedColor;
            if (smokeIndicatorText != null) smokeIndicatorText.text = available ? smokeAvailableLabel : smokeUsedLabel;
        }
    }
}