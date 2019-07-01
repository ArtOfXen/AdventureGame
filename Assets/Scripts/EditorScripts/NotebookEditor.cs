using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NotebookScript))]
public class NotebookEditor : Editor
{
    private SerializedProperty notesProperty;
    private bool[] showNotebookEntries = new bool[NotebookScript.MAXIMUM_NUMBER_OF_NOTES];

    private const string notebookPropNotesName = "notes";

    private void OnEnable()
    {
        notesProperty = serializedObject.FindProperty(notebookPropNotesName);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < NotebookScript.MAXIMUM_NUMBER_OF_NOTES; i++)
        {
            notebookEntryGUI(i);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void notebookEntryGUI(int index)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        showNotebookEntries[index] = EditorGUILayout.Foldout(showNotebookEntries[index], "Notebook Entry " + index);

        if (showNotebookEntries[index])
        {
            EditorGUILayout.PropertyField(notesProperty.GetArrayElementAtIndex(index));
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }
}
