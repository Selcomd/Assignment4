using UnityEngine;

public class KeycardManager : MonoBehaviour
{
    public static KeycardManager instance;

    private int requiredKeycards;
    private int collectedKeycards;

    public bool HasAllKeycards
    {
        get { return collectedKeycards >= requiredKeycards; }
    }

    public int CollectedKeycards
    {
        get { return collectedKeycards; }
    }

    public int RequiredKeycards
    {
        get { return requiredKeycards; }
    }

    private void Awake()
    {
        instance = this;
    }

    public void StartLevelKeycards(int amountNeeded)
    {
        requiredKeycards = amountNeeded;
        collectedKeycards = 0;

        Debug.Log("New level started. Need " + requiredKeycards + " keycards.");
    }

    public void CollectKeycard()
    {
        collectedKeycards++;

        Debug.Log("Keycards collected: " + collectedKeycards + " / " + requiredKeycards);
    }
}