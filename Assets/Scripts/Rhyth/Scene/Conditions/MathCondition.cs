namespace Modularity.Scene
{
    public class MathCondition : Condition
    {
        public enum Operation
        {
            LessThen, MoreThen, Equals
        }

        private Value value1, value2;
        private Operation operation;

        public MathCondition(Value value1, Value value2, Operation operation)
        {
            this.value1 = value1;
            this.value2 = value2;
            this.operation = operation;
        }

        public override bool IsFullfilled(Viewer viewer)
        {
            switch (operation)
            {
                case Operation.LessThen:
                    return value1.GetValue(viewer) < value2.GetValue(viewer);
                case Operation.MoreThen:
                    return value1.GetValue(viewer) > value2.GetValue(viewer);
                case Operation.Equals:
                    return value1.GetValue(viewer) == value2.GetValue(viewer);
                default:
                    throw new System.Exception("Operation " + operation + " not implemented!");
            }
        }

        public static Condition Parse(Value value1, Value value2, string operation)
        {
            Operation? operationEnum = null;
            switch (operation)
            {
                case "<":
                    operationEnum = Operation.LessThen;
                    break;
                case ">":
                    operationEnum = Operation.MoreThen;
                    break;
                case "=":
                    operationEnum = Operation.Equals;
                    break;
                default:
                    throw new System.Exception("Could not parse " + operation);
            }
            return new MathCondition(value1, value2, operationEnum.Value);
        }
    }
}