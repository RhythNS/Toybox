using UnityEngine;

public abstract class TransformUtil
{
    public static Transform GetTransformFromPath(Transform root, params string[] path)
    {
        if (root == null || path == null || path.Length == 0)
            return null;

        foreach (string str in path)
        {
            bool found = false;
            for (int i = 0; i < root.childCount; i++)
            {
                if (root.GetChild(i).name == str)
                {
                    root = root.GetChild(i);
                    found = true;
                    break;
                }
            }

            if (!found)
                return null;
        }
        return root;
    }

    public static void ChangeLayerRecursivly(Transform root, int layer)
    {
        root.gameObject.layer = layer;

        for (int i = 0; i < root.childCount; i++)
        {
            ChangeLayerRecursivly(root.GetChild(i), layer);
        }
    }

}