using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays messages. The messages disappear after a while
/// </summary>
public class LogPanel : MonoBehaviour
{
    public static LogPanel Instance { get; private set; }

    public bool upIsOldest;
    public float displayingLength = 2f; // How long to display a message in seconds

    private TMP_Text text;
    private List<string> displaying;
    private List<float> timers; // Contains the timer for string in displaying
    private bool deactivated;

    private void Awake()
    {
        Instance = this;
        displaying = new List<string>();
        timers = new List<float>();
        text = GetComponent<TMP_Text>();
        gameObject.SetActive(false);
        deactivated = false;
    }

    public void Set(string text)
    {
        if (deactivated || text == null || text.Length == 0)
            return;

        gameObject.SetActive(true);
        if (displaying.Count > 5) // If there are more than 5 present, delete the oldest one
        {
            int removeAt = upIsOldest ? 0 : displaying.Count - 1;
            displaying.RemoveAt(removeAt);
            timers.RemoveAt(removeAt);
        }

        int insertAt = upIsOldest ? displaying.Count : 0;
        displaying.Insert(insertAt, text);
        timers.Insert(insertAt, displayingLength);

        UpdateText();
    }

    private void Update()
    {
        if (displaying.Count > 0)
        {
            bool update = false;
            if (upIsOldest)
            {
                // Iterate through timers
                for (int i = timers.Count - 1; i >= 0; i--)
                {
                    // Count timers down
                    timers[i] -= Time.deltaTime;
                    if (timers[i] < 0) // if they are less then 0 then remove them
                    {
                        timers.RemoveAt(i);
                        displaying.RemoveAt(i);
                        update = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < timers.Count; i++)
                {
                    timers[i] -= Time.deltaTime;
                    if (timers[i] < 0)
                    {
                        timers.RemoveAt(i);
                        displaying.RemoveAt(i);

                        i--;
                        if (i < 0)
                            i = 0;

                        update = true;
                    }
                }
            }
            // If something changed update the text
            if (update)
            {
                UpdateText();
            }
        }
    }

    /// <summary>
    /// Updates the text object
    /// </summary>
    private void UpdateText()
    {
        // If there are no strings then deactivate the object
        if (displaying.Count == 0)
        {
            gameObject.SetActive(false);
            text.text = string.Empty;
        }
        else
        {
            // Display strings in order
            string displayText = displaying[0];
            for (int i = 1; i < displaying.Count; i++)
            {
                displayText += "\n" + displaying[i];
            }
            text.text = displayText;
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        deactivated = true;
    }

}
