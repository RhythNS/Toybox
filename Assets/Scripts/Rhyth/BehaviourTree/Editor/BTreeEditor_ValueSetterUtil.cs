﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Rhyth.BTree
{
    public partial class BTreeEditor : EditorWindow
    {
        private void SetInEditModeReferences()
        {
            UnityEngine.Object[] values = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tree.targetObject));
            List<BNode> nodeList = new List<BNode>();
            List<Value> valueList = new List<Value>();
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is Value)
                {
                    valueList.Add(values[i] as Value);
                }
                else if (values[i] is BNode)
                {
                    nodeList.Add(values[i] as BNode);
                }
            }
            allNodes = nodeList.ToArray();
            allValues = valueList.ToArray();
        }

        public void SetInPlayModeReferences()
        {
            GameObject newSelection = Selection.activeGameObject;
            if (lockTreeView == false &&
                newSelection != null && newSelection.TryGetComponent(out Brain brain) &&
                (tree == null || brain != lastSelectedBrain))
            {
                lastSelectedBrain = brain;
                tree = new SerializedObject(brain.Tree);
                List<BNode> nodeList = new List<BNode>();
                GetChildren(brain.Tree.Root, nodeList);
                allNodes = nodeList.ToArray();
            }

            // These references are not needed since we cant edit the tree while in play mode but 
            // are required so no errors are thrown
            allValues = new Value[0];
            if (typeForCustomEditor == null)
                GetCustomEditors();
        }

        private void GetCustomEditors()
        {
            typeForCustomEditor = new Dictionary<Type, CustomBNodeEditor>();
            Type[] customEditors = GetDerivedTypes(typeof(CustomBNodeEditor));
            for (int i = 0; i < customEditors.Length; i++)
            {
                CustomBNodeEditor editor = (CustomBNodeEditor)Activator.CreateInstance(customEditors[i]);
                typeForCustomEditor.Add(editor.NodeType, editor);
            }
        }

        private void GetChildren(BNode root, List<BNode> allNodes)
        {
            BNode[] children = root.Children;
            for (int i = 0; i < children.Length; i++)
                GetChildren(children[i], allNodes);
            allNodes.Add(root);
        }

        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection
        public Type[] GetDerivedTypes(Type baseType)
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    try
                    {
                        if (type.IsAbstract == false && baseType.IsAssignableFrom(type))
                        {
                            types.Add(type);
                        }
                    }
                    catch (ReflectionTypeLoadException) { }
                }
            }
            return types.OrderBy(x => x.Name).ToArray();
        }

        private bool GetParentNode(BNode child, out BNode parent)
        {
            for (int i = 0; i < allNodes.Length; i++)
            {
                BNode[] children = allNodes[i].Children;
                for (int j = 0; j < children.Length; j++)
                {
                    if (children[j] == child)
                    {
                        parent = allNodes[i];
                        return true;
                    }
                }
            }
            parent = default;
            return false;
        }

        private List<BNode> GetParentlessNodes()
        {
            List<BNode> list = new List<BNode>(allNodes);
            for (int i = 0; i < allNodes.Length; i++)
            {
                BNode[] children = allNodes[i].Children;
                if (children != null)
                {
                    for (int j = 0; j < children.Length; j++)
                    {
                        list.Remove(children[j]);
                    }
                }
            }
            return list;
        }

        private bool GetChildIndex(BNode parent, BNode child, out int index)
        {
            BNode[] children = parent.Children;
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i] == child)
                {
                    index = i;
                    return true;
                }
            }
            index = default;
            return false;
        }

        private void RemoveConnection(BNode node)
        {
            if (GetParentNode(node, out BNode parent))
            {
                if (!GetChildIndex(parent, node, out int index))
                    Debug.LogError("Parent does not contain child. Something seems wrong!");
                else
                {
                    SerializableUtil.ArrayRemoveAtIndex(new SerializedObject(parent).FindProperty("children"), index);
                }
            }
        }

        private void RemoveNode(BNode deleteNode)
        {
            RemoveConnection(deleteNode);

            if (selectedObject == deleteNode)
                selectedObject = null;

            AssetDatabase.RemoveObjectFromAsset(deleteNode);
            AssetDatabase.SaveAssets();
        }

        private void AddToArray(BNode parent, BNode child, bool applyChanges)
        {
            SerializedObject parentObject = new SerializedObject(parent);
            SerializedProperty array = parentObject.FindProperty("children");
            array.arraySize++;
            array.GetArrayElementAtIndex(array.arraySize - 1).objectReferenceValue = child;
            if (applyChanges)
                parentObject.ApplyModifiedProperties();
        }

        private void SortChildren(BNode parent)
        {
            SerializedObject serializedParent = new SerializedObject(parent);
            BNode[] children = parent.Children;
            SerializedProperty array = serializedParent.FindProperty("children");

            for (int i = 0; i < children.Length - 1; i++)
            {
                int toChangeIndex = i;
                float lowestNumber = children[i].boundsInEditor.x;
                for (int j = i + 1; j < children.Length; j++)
                {
                    if (children[j].boundsInEditor.x < lowestNumber)
                    {
                        toChangeIndex = j;
                        lowestNumber = children[j].boundsInEditor.x;
                    }
                }

                if (toChangeIndex != i)
                {
                    SerializableUtil.ArraySwapElement(array, i, toChangeIndex, false);
                    BNode temp = children[i];
                    children[i] = children[toChangeIndex];
                    children[toChangeIndex] = temp;
                }
            }

            serializedParent.ApplyModifiedProperties();
        }

        private bool NodeIsChild(BNode parent, BNode toFind)
        {
            if (parent == toFind)
                return true;

            BNode[] children = parent.Children;

            if (children == null)
                return false;
            for (int i = 0; i < children.Length; i++)
            {
                if (NodeIsChild(children[i], toFind))
                    return true;
            }

            return false;
        }
    }
}