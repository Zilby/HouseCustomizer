using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(HouseCustomizer))]
public class HouseCustomizerEditor : RevertablePrefabEditor
{
	static string presetName = "Default";
	bool showPresets = false;

	static HouseCustomizerEditor instance = null;

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		HouseCustomizer h = (HouseCustomizer)target;

		if (instance == null)
		{
			instance = this;
			h.Initialize();
		}

		GUILayout.Space(10);

		GUILayout.Label("Generation Tools", EditorStyles.boldLabel);

		if (GUILayout.Button("Generate House"))
		{
			h.customizableHouse.GenerateHouse();
		}

		GUILayout.Space(10);

		int selGridInt = -1;

		selGridInt = GUILayout.SelectionGrid(selGridInt, new string[6] { "Generate Foundation",
							   "Generate Ceilings", "Generate Exterior Walls", "Generate Interior Walls",
							   "Generate Staircases", "Generate Chimneys" }, 3);

		if (selGridInt == 4)
		{
			h.customizableHouse.GenerateComponent(1);
		}
		if (selGridInt != -1)
		{
			h.customizableHouse.GenerateComponent(selGridInt);
		}

		GUILayout.Space(10);

		if (GUILayout.Button("Clear House"))
		{
			h.customizableHouse.ClearHouse();
		}

		GUILayout.Space(10);

		GUILayout.Label("Presets", EditorStyles.boldLabel);

		string path = "HousePresets";

		showPresets = EditorGUILayout.Foldout(showPresets, "Current Names");

		if (showPresets)
		{
			if (Selection.activeTransform)
			{
				selGridInt = -1;
				string[] fileNames = SerializeUtility<FullHouse>.GetFiles(path);
				selGridInt = GUILayout.SelectionGrid(selGridInt, fileNames, 4);

				if (selGridInt != -1)
				{
					presetName = fileNames[selGridInt];
				}
			}
		}

		if (!Selection.activeTransform)
		{
			showPresets = false;
		}

		GUILayout.Label("Preset Name", EditorStyles.label);
		presetName = GUILayout.TextField(presetName);

		selGridInt = -1;

		selGridInt = GUILayout.SelectionGrid(selGridInt, new string[2] { "Load", "Save" }, 2);

		if (selGridInt == 0)
		{
			h.customizableHouse = SerializeUtility<FullHouse>.Load(presetName, path);
			h.customizableHouse.customizer = h;
			h.customizableHouse.GenerateHouse();
		}
		if (selGridInt == 1)
		{
			SerializeUtility<FullHouse>.Write(h.customizableHouse, presetName, path);
		}

		DrawDefaults();

		DrawURL();
	}

	protected override string URL
	{
		get { return "https://docs.google.com/document/d/1YFaUcm7FRd0sdZC-sFwe0OI4lEtmxeUNuiFnu40mMXk/edit?usp=sharing"; }
	}
}

