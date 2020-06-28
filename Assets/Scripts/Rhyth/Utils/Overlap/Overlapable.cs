using UnityEngine;

public abstract class Overlapable
{
    public abstract Rect GetRect();

    public bool Overlaps(Overlapable other) => GetRect().Overlaps(other.GetRect());

    public bool Overlaps(Rect other) => GetRect().Overlaps(other);

}
