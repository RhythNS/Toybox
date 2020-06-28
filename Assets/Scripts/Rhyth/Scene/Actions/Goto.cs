namespace Modularity.Scene
{
    public class Goto : ActionAdapter
    {
        private int nodeNumber;

        public Goto(int nodeNumber)
        {
            this.nodeNumber = nodeNumber;
        }

        public override void Start(Viewer viewer)
        {
            viewer.JumpToNode = nodeNumber;
        }

        public static Action Parse(string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            string nodeDec = Parser.ParseWord(input, lineStart, stringStart, out lineStart, out stringStart);
            string numberString = Parser.ParseWord(input, lineStart, stringStart, out parsedToLine, out parsedToStringIndex);
            if (!int.TryParse(numberString, out int nodeNumber))
                throw new ParserException(lineStart, "Node number for Jump is not an integer! (" + numberString + ")");

            return new Goto(nodeNumber);
        }
    }
}