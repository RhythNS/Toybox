using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modularity.Scene
{
    public abstract class SceneGetter
    {
        public static Scene GetScene(string path)
        {
            TextAsset textFile = Resources.Load("Dialogue/" + path) as TextAsset;

            if (textFile == null)
            {
                throw new System.ArgumentException("path not found: " + path);
            }

            return Parser.Parse(textFile.text);
        }

    }
}