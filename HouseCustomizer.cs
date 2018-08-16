using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using ProBuilder.Core;
using ProBuilder.MeshOperations;

/// <summary>
/// Controls the customizable house component. 
/// </summary>
[ExecuteInEditMode]
public class HouseCustomizer : MonoBehaviour
{
	[System.Serializable]
	public struct References
	{
		/// <summary>
		/// All black material. 
		/// </summary>
		public Material blackMat;

		/// <summary>
		/// The window materials.
		/// </summary>
		public List<Material> windowMaterials;

		/// <summary>
		/// The gable window sprites.
		/// </summary>
		public List<UnityEngine.Sprite> gableWindowSprites;

		/// <summary>
		/// The shutter sprites.
		/// </summary>
		public List<UnityEngine.Sprite> shutterSprites;

		/// <summary>
		/// The railing sprites.
		/// </summary>
		public List<UnityEngine.Sprite> railingSprites;

		/// <summary>
		/// The baluster prefab.
		/// </summary>
		public GameObject baluster;

		/// <summary>
		/// The porch column prefab.
		/// </summary>
		public GameObject porchColumn;

		/// <summary>
		/// All of the awning prefabs. 
		/// </summary>
		public List<GameObject> awnings;

		/// <summary>
		/// All of the balconet prefabs. 
		/// </summary>
		public List<GameObject> balconets;
	}

	/// <summary>
	/// The customizable house.
	/// </summary>
	public FullHouse customizableHouse;

	/// <summary>
	/// Contains all of the references. 
	/// </summary>
	public References references;

	#region Getters&Setters
	public bool canIndent
	{
		get { return customizableHouse.AO.wallThickness > customizableHouse.AO.indentThickness; }
	}
	#endregion

	public void Awake()
	{
		Initialize();
	}

	public void Reset()
	{
		Initialize();
	}

	public void Update()
	{
		if (!Application.isPlaying && customizableHouse != null && customizableHouse.initialized)
		{
			customizableHouse.ApplyMaterialChanges();
			customizableHouse.ApplyColorChanges();
			customizableHouse.ApplyTilingChanges();
		}
	}

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	public void Initialize()
	{
		customizableHouse.customizer = this;
	}
}
