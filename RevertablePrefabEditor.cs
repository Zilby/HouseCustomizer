using UnityEditor;
using UnityEngine;

/// <summary>
/// Adds a button to revert to defaults in the editor. 
/// </summary>
public abstract class RevertablePrefabEditor : DocumentationEditor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DrawDefaults();

        DrawURL();
    }

    protected void DrawDefaults() {
        GUILayout.Space(10);

        GUILayout.Label("Default Values", EditorStyles.boldLabel);
        if (GUILayout.Button("Revert to Defaults"))
        {
            GameObject go = ((Component)target).gameObject;
            PrefabUtility.ReconnectToLastPrefab(go);
            PrefabUtility.RevertPrefabInstance(go);
        }
    }
}