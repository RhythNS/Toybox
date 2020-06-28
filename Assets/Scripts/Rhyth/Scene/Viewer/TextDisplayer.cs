using UnityEngine;
using TMPro;
namespace Modularity.Scene
{
    public class TextDisplayer : MonoBehaviour
    {
        public Viewer viewer;
        private TMP_Text textDisplay, characterNameDisplay;
        public RectTransform rectTransform;

        private string toDisplay;
        private int atCharacter;

        private readonly float TIME_FOR_ONE_CHARACTER = 0.03f;
        private float timer;
        private bool allDisplayed;

        private void Start()
        {
            textDisplay = transform.Find("Text").GetComponent<TMP_Text>();
            characterNameDisplay = transform.Find("Name").GetComponent<TMP_Text>();
            allDisplayed = true;
            rectTransform = (RectTransform)transform;
            // get sound
        }

        public void SetText(string text, string speakingCharacter)
        {
            allDisplayed = false;
            timer = TIME_FOR_ONE_CHARACTER;
            atCharacter = 0;
            toDisplay = text;
            characterNameDisplay.text = speakingCharacter;
            textDisplay.text = string.Empty;
        }

        private void Update()
        {
            if (!allDisplayed)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    timer = TIME_FOR_ONE_CHARACTER;
                    if (++atCharacter > toDisplay.Length)
                    {
                        allDisplayed = true;
                    }
                    else
                    {
                        textDisplay.text = toDisplay.Substring(0, atCharacter);
                    }
                }
            }
        }

        public void OnClick()
        {
            if (allDisplayed)
            {
                viewer.RequestSkip();
                // sound play
            }
            else
            {
                allDisplayed = true;
                textDisplay.text = toDisplay;
            }
        }
    }
}