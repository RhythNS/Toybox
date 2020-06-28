using UnityEngine;

public abstract class GameobjectUtil
{
    public static void DestroyInEditMode(GameObject gameObject)
    {
        if (Application.isEditor == false)
        {
            Debug.LogError("Game not in edit mode!");
            return;
        }

        DestroyRecursiv(gameObject);
    }

    private static void DestroyRecursiv(GameObject gameObject)
    {
        for (int i = gameObject.transform.childCount; i > 0; --i)
            DestroyRecursiv(gameObject.transform.GetChild(0).gameObject);
        
        Object.DestroyImmediate(gameObject);
    }

}
