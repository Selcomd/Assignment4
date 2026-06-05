using System.Collections;
using UnityEngine;

public class LossCurtainController : MonoBehaviour
{
    [Header("Curtain Panels")]
    public RectTransform topCurtain;
    public RectTransform bottomCurtain;

    [Header("Loss Text")]
    public GameObject lossText;

    [Header("Settings")]
    public float closeSpeed = 900f;
    public float openSpeed = 600f;
    public float restartDelay = 3f;
    public float overscanPixels = 100f;

    private bool hasPlayed = false;

    private void Awake()
    {
        if (lossText != null)
            lossText.SetActive(false);
    }

    public void StartLossCurtain()
    {
        if (hasPlayed) return;

        hasPlayed = true;
        gameObject.SetActive(true);

        SetupCurtains();
        StartCoroutine(CurtainSequence());
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

    private IEnumerator CurtainSequence()
    {
        if (lossText != null) lossText.SetActive(false);

        Vector2 closedPosition = Vector2.zero;

        while (Vector2.Distance(topCurtain.anchoredPosition, closedPosition) > 1f || Vector2.Distance(bottomCurtain.anchoredPosition, closedPosition) > 1f)
        {
            topCurtain.anchoredPosition = Vector2.MoveTowards(topCurtain.anchoredPosition, closedPosition, closeSpeed * Time.deltaTime);
            bottomCurtain.anchoredPosition = Vector2.MoveTowards(bottomCurtain.anchoredPosition, closedPosition, closeSpeed * Time.deltaTime);
            yield return null;
        }

        topCurtain.anchoredPosition = Vector2.zero;
        bottomCurtain.anchoredPosition = Vector2.zero;

        LevelResetManager resetManager = FindFirstObjectByType<LevelResetManager>();
        if (resetManager != null) resetManager.ExecuteReset();

        if (lossText != null) lossText.SetActive(true);

        yield return new WaitForSeconds(restartDelay);

        if (lossText != null) lossText.SetActive(false);

        Vector2 topOpen = new Vector2(0f, topCurtain.rect.height + overscanPixels);
        Vector2 bottomOpen = new Vector2(0f, -(bottomCurtain.rect.height + overscanPixels));

        while (Vector2.Distance(topCurtain.anchoredPosition, topOpen) > 1f || Vector2.Distance(bottomCurtain.anchoredPosition, bottomOpen) > 1f)
        {
            topCurtain.anchoredPosition = Vector2.MoveTowards(topCurtain.anchoredPosition, topOpen, openSpeed * Time.deltaTime);
            bottomCurtain.anchoredPosition = Vector2.MoveTowards(bottomCurtain.anchoredPosition, bottomOpen, openSpeed * Time.deltaTime);
            yield return null;
        }

        hasPlayed = false;
        gameObject.SetActive(false);
    }
}