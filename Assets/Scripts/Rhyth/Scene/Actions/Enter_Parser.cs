using System;

namespace Modularity.Scene
{
    public partial class Enter : Action
    {
        public enum EnterDirection
        {
            Up, Down, Left, Right, Instant
        }

        public static Action Parse(bool entering, string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            // expected: "name direction"
            string actorName = Parser.ParseWord(input, lineStart, stringStart, out parsedToLine, out stringStart);
            if (parsedToLine != lineStart)
                throw new ParserException(lineStart, "Enter needs a character name and direction!");

            Actor actor = ActorDirectory.Instance.GetActor(actorName);
            if (actor == null)
                throw new ParserException(lineStart, "Name " + actorName + " not found!");

            string directionString = Parser.ParseWord(input, lineStart, stringStart, out parsedToLine, out parsedToStringIndex);

            if (!Enum.TryParse(directionString, true, out EnterDirection direction))
                throw new ParserException(lineStart, "Direction " + directionString + " not found!");

            return new Enter(entering, actor, direction);
        }
    }
}