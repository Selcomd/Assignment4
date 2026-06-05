using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinCurtainController : MonoBehaviour
{
    [Header("Curtain Panels")]
    public RectTransform topCurtain;
    public RectTransform bottomCurtain;

    [Header("Win Text")]
    public GameObject winText;

    [Header("Stats Display")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text caughtText;
    [SerializeField] private float statsDelay = 1f;

    [Header("Settings")]
    public float closeSpeed = 900f;
    public float restartDelay = 5f;
    public float overscanPixels = 100f;

    private bool hasPlayed = false;

    private void Awake()
    {
        if (winText != null) winText.SetActive(false);
        if (timeText != null) timeText.gameObject.SetActive(false);
        if (caughtText != null) caughtText.gameObject.SetActive(false);
    }

    public void StartWinCurtain()
    {
        if (hasPlayed) return;

        hasPlayed = true;
        gameObject.SetActive(true);

        if (HUDController.instance != null) HUDController.instance.StopTimer();

        SetupCurtains();
        StartCoroutine(CloseCurtain());
    }

    private void SetupCurtains()
    {
        if (topCurtain == null || bottomCurtain == null) return;

        topCurtain.anchorMin = new Vector2(0f, 0.5f);
        topCurtain.anchorMax = new Vector2(1f, 1f);
        topCurtain.pivot = new Vector2(0.5f, 0.5f);
        topCurtain.offsetMin = new Vector2(-overscanPixels, -overscanPixels);
        topCurtain.offsetMax = new Vector2(overscanPixels, overscanPixels);

        bottomCurtain.anchorMin = new Vector2(0f, 0f);
        bottomCurtain.anchorMax = new Vector2(1f, 0.5f);
        bottomCurtain.pivot = new Vector2(0.5f, 0.5f);
        bottomCurtain.offsetMin = new Vector2(-overscanPixels, -overscanPixels);
        bottomCurtain.offsetMax = new Vector2(overscanPixels, overscanPixels);

        topCurtain.anchoredPosition = new Vector2(0f, topCurtain.rect.height + overscanPixels);
        bottomCurtain.anchoredPosition = new Vector2(0f, -(bottomCurtain.rect.height + overscanPixels));
    }

    private IEnumerator CloseCurtain()
    {
        if (winText != null) winText.SetActive(false);

        Vector2 closedPosition = Vector2.zero;

        while (Vector2.Distance(topCurtain.anchoredPosition, closedPosition) > 1f || Vector2.Distance(bottomCurtain.anchoredPosition, closedPosition) > 1f)
        {
            topCurtain.anchoredPosition = Vector2.MoveTowards(topCurtain.anchoredPosition, closedPosition, closeSpeed * Time.deltaTime);
            bottomCurtain.anchoredPosition = Vector2.MoveTowards(bottomCurtain.anchoredPosition, closedPosition, closeSpeed * Time.deltaTime);
            yield return null;
        }

        topCurtain.anchoredPosition = Vector2.zero;
        bottomCurtain.anchoredPosition = Vector2.zero;

        if (winText != null) winText.SetActive(true);

        yield return new WaitForSeconds(statsDelay);

        if (HUDController.instance != null)
        {
            float elapsed = HUDController.instance.ElapsedTime;
            int minutes = (int)(elapsed / 60f);
            int seconds = (int)(elapsed % 60f);

            if (timeText != null)
            {
                timeText.gameObject.SetActive(true);
                timeText.text = $"Time: {minutes:00}:{seconds:00}";
            }

            if (caughtText != null)
            {
                caughtText.gameObject.SetActive(true);
                caughtText.text = $"Times Caught: {HUDController.instance.CaughtCount}";
            }
        }

        yield return new WaitForSeconds(restartDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}