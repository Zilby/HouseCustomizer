using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Adds a documentation button to a script's editor. 
/// </summary>
public abstract class DocumentationEditor : Editor {

    protected abstract string URL { get; }

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
        DrawURL();
	}

    /// <summary>
    /// Draws the documentation url button. 
    /// </summary>
    protected void DrawURL() 
    {
        GUILayout.Space(10);

        GUILayout.Label("Documentation", EditorStyles.boldLabel);

        if (GUILayout.Button("See Documentation"))
        {
            Application.OpenURL(URL);
        }
    }
}

