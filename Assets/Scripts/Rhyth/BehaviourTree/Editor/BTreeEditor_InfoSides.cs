﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rhyth.BTree
{
    public partial class BTreeEditor : EditorWindow
    {
        /// <summary>
        /// Draws the left side of the window. This side is for displaying debug window values, what the root node is
        /// and handles the adding, selction and removing of Values.
        /// </summary>
        private void DrawTreeInfo()
        {
            if (debug)
            {
                Vector2 changedOffset = offset / zoomLevel;
                Vector2 newOffset = EditorGUILayout.Vector2Field("Position:", changedOffset);
                if (changedOffset != newOffset)
                    offset = newOffset * zoomLevel;

                zoomLevel = Mathf.Clamp(EditorGUILayout.FloatField("Zoomlevel", zoomLevel), MIN_ZOOM_LEVEL, MAX_ZOOM_LEVEL);
            }

            lockTreeView = EditorGUILayout.Toggle("Lock Window to current Tree", lockTreeView);

            if (inPlayMode == false)
            {
                // Handle root node
                List<BNode> parentLessNodes = GetParentlessNodes(); // only nodes without parents can become root
                SerializedProperty rootNode = tree.FindProperty("root");
                BNode previousRoot = rootNode.objectReferenceValue as BNode;

                string[] choices = new string[parentLessNodes.Count + 1];
                choices[0] = "Null"; // If the root node should be set to nothing

                // fill all the choices with names of the nodes
                int index = 0;
                for (int i = 0; i < parentLessNodes.Count; i++)
                {
                    choices[i + 1] = parentLessNodes[i].name;
                    if (parentLessNodes[i] == previousRoot)
                        index = i + 1;
                }

                // Update the new root node
                int newIndex = EditorGUILayout.Popup("root", index, choices);
                if (newIndex != index)
                {
                    if (newIndex == 0)
                        rootNode.objectReferenceValue = null;
                    else
                        rootNode.objectReferenceValue = parentLessNodes[newIndex - 1];
                }
                this.rootNode = newIndex == 0 ? null : parentLessNodes[newIndex - 1];
                // ---Handle root node

                tree.ApplyModifiedProperties();

                // Add the Button for adding a new value
                if (GUILayout.Button("Add Value"))
                {
                    GenericMenu valueMenu = new GenericMenu();
                    for (int i = 0; i < valueTypes.Length; i++)
                    {
                        Type type = valueTypes[i];
                        string name = valueTypes[i].Name;
                        if (name.EndsWith("value", StringComparison.OrdinalIgnoreCase))
                            name = name.Substring(0, name.Length - 5);

                        valueMenu.AddItem(new GUIContent(name), false, () =>
                        {
                            Value newNode = (Value)CreateInstance(type);
                            newNode.name = "New " + type.Name;
                            AssetDatabase.AddObjectToAsset(newNode, tree.targetObject);
                            selectedObject = newNode;
                            AssetDatabase.SaveAssets();
                        });
                    }
                    valueMenu.ShowAsContext();
                }
            }

            // Display all values in a list
            for (int i = 0; i < allValues.Length; i++)
            {
                if (GUILayout.Button(allValues[i].name))
                {
                    selectedObject = allValues[i];
                }
            }
        }

        /// <summary>
        /// Properties that are ignored if debug is set to false.
        /// </summary>
        private static readonly string[] DEBUG_PROPERTIES = { "children", "boundsInEditor" };

        /// <summary>
        /// Properties that are ignored when triying to find every SerializedProperty.
        /// </summary>
        private static readonly string[] IGNORE_PROPERTIES = { "m_Script", "breakPointEnabled" };

        /// <summary>
        /// This draws the left side of the window. This side shows information about the currently selected Object
        /// which can be edited.
        /// </summary>
        private void DrawInspector()
        {
            if (selectedObject != null)
            {
                // Get the SerializedObject and draw every property
                SerializedObject serObject = new SerializedObject(selectedObject);
                SerializedProperty prop = serObject.GetIterator();
                if (prop.NextVisible(true))
                {
                    bool appendNormalProperties = true;
                    EditorGUILayout.LabelField(serObject.targetObject.GetType().Name);
                    serObject.targetObject.name = EditorGUILayout.TextField("Name", serObject.targetObject.name);

                    if (selectedObject is BNode)
                    {
                        SerializedProperty breakPointProperty = serObject.FindProperty("breakPointEnabled");
                        breakPointProperty.boolValue = EditorGUILayout.Toggle("Break Point Enabled", breakPointProperty.boolValue);

                        // If it has a customEditor, draw this before drawing the properties
                        if (typeForCustomEditor.TryGetValue(serObject.targetObject.GetType(), out CustomBNodeEditor editor))
                        {
                            editor.DrawInspector(serObject);
                            appendNormalProperties = editor.AppendNormalEditor;
                        }
                    }

                    if (appendNormalProperties == true)
                    {
                        do
                        {
                            for (int i = 0; i < IGNORE_PROPERTIES.Length; i++)
                                if (prop.name == IGNORE_PROPERTIES[i])
                                    goto SkipProperty;

                            for (int i = 0; i < DEBUG_PROPERTIES.Length && debug == false; i++)
                                if (prop.name == DEBUG_PROPERTIES[i])
                                    goto SkipProperty;

                            EditorGUILayout.PropertyField(serObject.FindProperty(prop.name), true);

                        SkipProperty:;
                        }
                        while (prop.NextVisible(false));
                    }
                }
                if (inPlayMode == false) // We should not be able to change anything in play mode
                    serObject.ApplyModifiedProperties();
            }

            // Draw the delete button
            if (inPlayMode == false)
            {
                bool changed = false;
                if (selectedObject is Value)
                {
                    if (GUILayout.Button("Delete"))
                    {
                        AssetDatabase.RemoveObjectFromAsset(selectedObject);
                        AssetDatabase.SaveAssets();
                        selectedObject = null;
                    }
                }
                else if (selectedObject is BNode)
                {
                    if (GUILayout.Button("Delete"))
                    {
                        RemoveNode(selectedObject as BNode);
                    }
                }
                if (changed)
                {
                    tree.ApplyModifiedProperties();
                    tree.Update();
                }
            }
        }
    }
}