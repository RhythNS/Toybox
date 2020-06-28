using System.Collections;
using UnityEngine;

namespace Modularity.Scene
{
    public class Wait : ActionAdapter
    {
        private float waitTime;
        private bool finished;

        public Wait(float waitTime)
        {
            this.waitTime = waitTime;
            finished = false;
        }

        public override void Start(Viewer viewer)
        {
            viewer.StartCoroutine(GetEnumerator());
        }

        private IEnumerator GetEnumerator()
        {
            yield return new WaitForSeconds(waitTime);
            finished = true;
        }

        public override bool Update() => finished;


        public static Action Parse(string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            string waitTimeString = Parser.ParseWord(input, lineStart, stringStart, out parsedToLine, out parsedToStringIndex);
            if (!int.TryParse(waitTimeString, out int waitTimeInMiliSeconds))
                throw new ParserException(lineStart, "Could not cast " + waitTimeString + " to a string!");

            float waitTime = (float)waitTimeInMiliSeconds / 1000f;
            return new Wait(waitTime);
        }
    }
}