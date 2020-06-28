using System;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveOverlapManager<T> : Overlapable, IOverlapManager<T> where T : Overlapable
{
    private bool isLast;
    private List<T> overlapables;
    private RecursiveOverlapManager<T>[] helpers;
    private Rect rect;

    public RecursiveOverlapManager(Rect rect, int layersDown)
    {
        this.rect = rect;
        isLast = layersDown <= 0;
        if (isLast)
            overlapables = new List<T>();
        else
        {
            layersDown--;
            helpers = new RecursiveOverlapManager<T>[4];
            rect.width *= 0.5f;
            rect.height *= 0.5f;
            helpers[0] = new RecursiveOverlapManager<T>(rect, layersDown);
            rect.y += rect.height;
            helpers[1] = new RecursiveOverlapManager<T>(rect, layersDown);
            rect.x += rect.width;
            helpers[2] = new RecursiveOverlapManager<T>(rect, layersDown);
            rect.position = new Vector2(this.rect.x + rect.width, this.rect.y);
            helpers[3] = new RecursiveOverlapManager<T>(rect, layersDown);
        }
    }
    public override Rect GetRect() => rect;

    public List<T> GetAllOverlaps(T toCheck) => InnerGetOverlaps(new List<T>(), toCheck.GetRect());

    public List<T> GetAllOverlaps(Rect toCheck) => InnerGetOverlaps(new List<T>(), toCheck);

    private List<T> InnerGetOverlaps(List<T> list, Rect toCheck)
    {
        if (isLast)
        {
            for (int i = 0; i < overlapables.Count; i++)
            {
                if (overlapables[i].Overlaps(toCheck) && !list.Contains(overlapables[i]))
                    list.Add(overlapables[i]);
            }
        }
        else // not last
        {
            for (int i = 0; i < helpers.Length; i++)
            {
                if (helpers[i].Overlaps(toCheck))
                    list = helpers[i].InnerGetOverlaps(list, toCheck);
            }
        }
        return list;
    }

    public List<T> GetAllOverlapsForWorldEditor()
    {
        List<T> list = InnerGetAllOverlaps(new List<T>());

        // Remove duplicate
        for (int i = list.Count - 1; i > 0; i--)
            for (int j = i - 1; j >= 0; j--)
                if (list[i] == list[j])
                {
                    list.RemoveAt(i);
                    break;
                }

        return list;
    }

    private List<T> InnerGetAllOverlaps(List<T> list)
    {
        if (!isLast)
        {
            Array.ForEach(helpers, x => x.InnerGetAllOverlaps(list));
        }
        else // not last
        {
            for (int i = 0; i < overlapables.Count - 1; i++)
                for (int j = i + 1; j < overlapables.Count; j++)
                    if (overlapables[i].Overlaps(overlapables[j]))
                    {
                        list.Add(overlapables[i]);
                        list.Add(overlapables[j]);
                    }
        }
        return list;
    }

    public void Add(T toAdd)
    {
        if (isLast)
            if (overlapables.Contains(toAdd))
                Debug.LogWarning("OverlapHelper: Overlapable already in list! (" + toAdd + ")");
            else
                overlapables.Add(toAdd);
        else
        {
            for (int i = 0; i < helpers.Length; i++)
            {
                if (helpers[i].Overlaps(toAdd))
                    helpers[i].Add(toAdd);
            }
        }
    }

    public void AddAll(params T[] toAdd) => Array.ForEach(toAdd, x => Add(x));

    public void AddAll(List<T> toAdd) => toAdd.ForEach(x => Add(x));

    public void Remove(T toRemove)
    {
        if (isLast)
            overlapables.Remove(toRemove);
        else
        {
            for (int i = 0; i < helpers.Length; i++)
            {
                if (helpers[i].Overlaps(toRemove))
                    helpers[i].Remove(toRemove);
            }
        }
    }

}
