using UnityEngine;

namespace Modularity.Scene
{
    public class Say : SimpleAction
    {
        public string name, text;

        public Say(string name, string text) : base(true)
        {
            this.name = name;
            this.text = text;
        }

        public override void Start(Viewer viewer)
        {
            Actor foundActor = null;
            foreach (Actor actor in viewer.ActiveActors)
            {
                if (actor.ActorInfo.Name == name)
                {
                    foundActor = actor;
                    break;
                }
            }
            if (foundActor == null)
            {
                //Debug.LogWarning("Actor " + name + " not in scene!");
            }
            else
            {
                // TODO highlight character
            }

            viewer.Text.SetText(text, name);
        }

        public static Action Parse(string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            string name = Parser.ParseWord(input, lineStart, stringStart, out parsedToLine, out stringStart);
            if (parsedToLine != lineStart)
                throw new ParserException(lineStart, "Say needs a character name and a string for the text!");

            if (ActorDirectory.Instance.GetActor(name) == null)
                throw new ParserException(lineStart, "Actor " + name + " not found!");

            string text = Parser.ParseParagraph(input, lineStart, stringStart, out parsedToLine, out parsedToStringIndex);

            return new Say(name, text);
        }
    }
}
