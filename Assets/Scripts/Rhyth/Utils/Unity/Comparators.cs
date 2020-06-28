using UnityEngine;

public abstract class Comparators
{
    public static int Vector2IntXThenY(Vector2Int first, Vector2Int second)
    {
        // If they are the same return 0
        if (first == second)
            return 0;

        // if the x value is the same then compare the y value
        if (first.x == second.x)
            return first.y > second.y ? 1 : -1; // check the y value

        // X is not the same check the x value
        return first.x > second.x ? 1 : -1;
    }

}
