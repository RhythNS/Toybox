using System.Collections.Generic;

public abstract class StringUtil
{

    public static int[] EveryIndexOf(string input, char value)
    {
        List<int> indexes = new List<int>();
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == value)
                indexes.Add(i);
        }
        return indexes.ToArray();
    }

}
