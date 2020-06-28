namespace Modularity.Scene
{
    public class If : Action
    {
        private Condition condition;
        private Action action;
        private bool shouldExecute;

        public If(Condition condition, Action action)
        {
            this.condition = condition;
            this.action = action;
        }

        public override void RequestSkip()
        {
            action.RequestSkip();
        }

        public override void Start(Viewer viewer)
        {
            if (!condition.IsFullfilled(viewer))
                shouldExecute = false;
            else
            {
                action.Start(viewer);
                shouldExecute = true;
            }
        }

        public override bool Update() => !shouldExecute || (shouldExecute && action.Update());


        public static Action Parse(string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            Condition condition = Parser.ParseCondition(input, lineStart, stringStart, out parsedToLine, out stringStart);
            if (parsedToLine != lineStart)
                throw new ParserException(lineStart, "If needs an connector like \"And\" or \"Or\" or \"then\" followed by an action!");

            string connector = Parser.ParseWord(input, parsedToLine, stringStart, out parsedToLine, out stringStart);
            if (!connector.Equals("then", System.StringComparison.OrdinalIgnoreCase))
                throw new ParserException(parsedToLine, "Expected \"then\" after condition of if.");

            Action action = Parser.GetAction(input, parsedToLine, stringStart, out parsedToLine, out parsedToStringIndex);
            return new If(condition, action);
        }

        public override void Reset()
        {
            action.Reset();
        }
    }
}