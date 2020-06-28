using System;
using System.Collections.Generic;

namespace Modularity.Scene
{
    public abstract class Parser
    {
        public static Scene Parse(string input)
        {
            string[] inputArray = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            return Parse(inputArray);
        }

        public static Scene Parse(string[] input)
        {
            int startingIndex = 0, openParenthesis = 0;
            List<Node> nodes = new List<Node>();
            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] == "{")
                {
                    openParenthesis++;
                }
                else if (input[i] == "}")
                {
                    openParenthesis--;
                    if (openParenthesis == 0)
                    {
                        nodes.Add(GetNode(input, startingIndex, i - 1));
                        i += 2;
                        startingIndex = i;
                    }
                    else if (openParenthesis < 0)
                    {
                        throw new ParserException(i, "Parenthesis \"}\" closed when no open \"{\" one was found.");
                    }
                }
            }

            if (openParenthesis > 0)
                throw new ParserException(input.Length - 1, "Parenthesis \"{\" opened when no closing \"}\" one was found.");

            return new Scene(nodes);
        }

        public static Node GetNode(string[] input, int lineStart, int lineEnd)
        {
            if (!input[lineStart].StartsWith("node "))
                throw new ParserException(lineStart, "Node decleration does not start with \"node\": " + input[0]);
            if (!int.TryParse(input[lineStart].Substring(5), out int nodeNumber))
                throw new ParserException(lineStart, "Could not parse node number: " + input[0]);

            List<Action> actions = new List<Action>();
            for (int i = lineStart + 2; i < lineEnd + 1; i++)
            {
                actions.Add(GetAction(input, i, 0, out int parsedToLine, out _));
                i = parsedToLine - 1;
            }

            return new Node(nodeNumber, actions);
        }

        public static Action GetAction(string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            string syntax = ParseWord(input, lineStart, stringStart, out lineStart, out stringStart);

            Action action = null;
            switch (syntax)
            {
                case "enter":
                    action = Enter.Parse(true, input, lineStart, stringStart, out lineStart, out stringStart);
                    break;
                case "say":
                    action = Say.Parse(input, lineStart, stringStart, out lineStart, out stringStart);
                    break;
                case "leave":
                    action = Enter.Parse(false, input, lineStart, stringStart, out lineStart, out stringStart);
                    break;
                case "emote":
                    action = Emote.Parse(input, lineStart, stringStart, out lineStart, out stringStart);
                    break;
                case "image":
                    // action = Image.Parse(input, lineStart, stringStart, out lineStart, out stringStart);
                    break;
                case "effect":
                    // action = Effect.Parse(input, lineStart, stringStart, out lineStart, out stringStart);
                    break;
                case "inventory":
                    // action = Inventory.Parse(input, lineStart, stringStart, out lineStart, out stringStart);
                    break;
                case "wait":
                    action = Wait.Parse(input, lineStart, stringStart, out lineStart, out stringStart);
                    break;
                case "goto":
                    action = Goto.Parse(input, lineStart, stringStart, out lineStart, out stringStart);
                    break;
                case "if":
                    action = If.Parse(input, lineStart, stringStart, out lineStart, out stringStart);
                    break;
                case "subnode":
                    action = SubNode.Parse(input, lineStart, stringStart, out lineStart, out stringStart);
                    break;
                case "decision":
                    action = Decision.Parse(input, lineStart, stringStart, out lineStart, out stringStart);
                    break;
                case "exit":
                    action = new Exit();
                    break;
                default:
                    throw new ParserException(lineStart, "Syntax " + syntax + " unknown!");
            }

            if (stringStart != 0)
            {
                string connector = ParseWord(input, lineStart, stringStart, out lineStart, out stringStart);
                switch (connector)
                {
                    case "and":
                        action = And.Parse(action, input, lineStart, stringStart, out lineStart, out stringStart);
                        break;
                    default:
                        throw new ParserException(lineStart, "Connector " + connector + " unknown!");
                }
            }

            parsedToLine = lineStart;
            parsedToStringIndex = stringStart;

            return action;
        }

        public static int GetCorrespondingBracket(string[] input, int lineStart)
        {
            int openBrackets = 1;
            for (int i = lineStart; i < input.Length - 1; i++)
            {
                switch (input[i][0])
                {
                    case '{':
                        openBrackets++;
                        break;
                    case '}':
                        openBrackets--;
                        if (openBrackets == 0)
                            return i;
                        break;
                }
            }
            throw new ParserException(lineStart, "No } found for opened bracket!");
        }

        public static string ParseWord(string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            int indexOfNextSpace = input[lineStart].IndexOf(' ', stringStart);
            if (indexOfNextSpace == -1)
            {
                parsedToLine = lineStart + 1;
                parsedToStringIndex = 0;
                return input[lineStart].Substring(stringStart);
            }
            else
            {
                parsedToLine = lineStart;
                parsedToStringIndex = indexOfNextSpace + 1;
                return input[lineStart].Substring(stringStart, indexOfNextSpace - stringStart);
            }
        }

        public static string ParseParagraph(string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            if (input[lineStart][stringStart] != '%')
                throw new ParserException(lineStart, "Expected a Percent (%)!");
            for (int i = stringStart + 1; i < input[lineStart].Length; i++)
            {
                if (input[lineStart][i] == '%')
                {
                    if (input[lineStart].Length == i + 1)
                    {
                        parsedToLine = lineStart + 1;
                        parsedToStringIndex = 0;
                    }
                    else
                    {
                        parsedToLine = lineStart;
                        parsedToStringIndex = i + 2;
                    }
                    return input[lineStart].Substring(stringStart + 1, i - stringStart - 1);
                }
            }
            throw new ParserException(lineStart, "Percent (%) opened but not closed!");
        }

        public static Value ParseValue(string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            string syntax = ParseWord(input, lineStart, stringStart, out lineStart, out stringStart);
            switch (syntax)
            {
                case "inventory":
                    throw new NotImplementedException();
                default:
                    if (int.TryParse(syntax, out int value))
                    {
                        parsedToLine = lineStart;
                        parsedToStringIndex = stringStart;
                        return new AbsoluteValue(value);
                    }
                    throw new ParserException(lineStart, "Syntax " + syntax + " not found for parsing a value!");
            }
        }

        public static Condition ParseCondition(string[] input, int lineStart, int stringStart, out int parsedToLine, out int parsedToStringIndex)
        {
            Condition condition1, condition2;

            Value value1 = ParseValue(input, lineStart, stringStart, out lineStart, out stringStart);
            string operation = ParseWord(input, lineStart, stringStart, out lineStart, out stringStart);
            Value value2 = ParseValue(input, lineStart, stringStart, out parsedToLine, out parsedToStringIndex);

            switch (operation)
            {
                case "<":
                case ">":
                case "=":
                    condition1 = MathCondition.Parse(value1, value2, operation);
                    break;
                default:
                    throw new ParserException(lineStart, "Could not find " + operation + " for condition!");
            }

            string connector = ParseWord(input, parsedToLine, parsedToStringIndex, out int newLine, out int newString);
            if (newLine != lineStart)
                return condition1;

            switch (connector)
            {
                case "and":
                    condition2 = ParseCondition(input, newLine, newString, out parsedToLine, out parsedToStringIndex);
                    return new AndCondition(condition1, condition2);
                case "or":
                    condition2 = ParseCondition(input, newLine, newString, out parsedToLine, out parsedToStringIndex);
                    return new OrCondition(condition1, condition2);
                default:
                    return condition1;
            }
        }
    }
}