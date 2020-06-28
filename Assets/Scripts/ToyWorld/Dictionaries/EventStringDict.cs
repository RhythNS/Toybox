using UnityEngine;

[System.Serializable]
public enum ToyEvent
{
    NotEnoughResources, CollBoxTooFar, ToolNotFound
}

[System.Serializable]
public struct EventForString
{
    public ToyEvent toyEvent;
    public string eventString;
}

/// <summary>
/// Dict for strings for given ToyEvents
/// </summary>
public class EventStringDict : MonoBehaviour
{
    [SerializeField] private EventForString[] eventForStrings;
    [SerializeField] private string errorString;

    public static EventStringDict Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public string GetString(ToyEvent toyEvent)
    {
        for (int i = 0; i < eventForStrings.Length; i++)
        {
            if (eventForStrings[i].toyEvent == toyEvent)
                return eventForStrings[i].eventString;
        }
        Debug.LogError("Umimplemented case " + toyEvent);
        return errorString;
    }
}
