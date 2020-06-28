using System.Collections.Generic;
using UnityEngine;

public interface IOverlapManager<T> where T : Overlapable
{
    List<T> GetAllOverlaps(T toCheck);

    List<T> GetAllOverlaps(Rect toCheck);

    void Add(T toAdd);

    void AddAll(List<T> toAdd);

    void AddAll(params T[] toAdd);

    void Remove(T toRemove);
}
