using ProBuilder.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An extension of a customizable house. 
/// </summary>
[System.Serializable]
public class Extension : CustomizableHouse
{
	#region SerializableFields

	/// <summary>
	/// Contains all of the dimensions. 
	/// </summary>
	public Dimensions dimensions = new Dimensions();

	/// <summary>
	/// Contains all of the textures. 
	/// </summary>
	[System.NonSerialized]
	public Textures textures = new Textures();

	/// <summary>
	/// Contains all of the outer details. 
	/// </summary>
	public OuterDetails outerDetails = new OuterDetails();

	/// <summary>
	/// Contains all of the inner details. 
	/// </summary>
	public InnerDetails innerDetails = new InnerDetails();

	/// <summary>
	/// Contains all of the advanced settings. 
	/// </summary>
	[System.NonSerialized]
	public AdvancedOptions advancedOptions = new AdvancedOptions();

	[System.NonSerialized]
	public Transform exTransform;

	[System.NonSerialized]
	public Wall exWall;

	[System.NonSerialized]
	public float exPosition;

	public override Dimensions Dim
	{
		get { return dimensions; }
		set { dimensions = value; }
	}

	public override Textures Tex
	{
		get { return textures; }
		set { textures = value; }
	}

	public override OuterDetails OD
	{
		get { return outerDetails; }
		set { outerDetails = value; }
	}

	public override InnerDetails ID
	{
		get { return innerDetails; }
		set { innerDetails = value; }
	}

	public override AdvancedOptions AO
	{
		get { return advancedOptions; }
		set { advancedOptions = value; }
	}

	public override List<HouseExtension> EX
	{
		get { return new List<HouseExtension>(); }
		set { }
	}

	/// <summary>
	/// Gets the gameobject transform.
	/// </summary>
	/// <value>The transform.</value>
	protected override Transform transform
	{
		get { return exTransform; }
	}
			

	#endregion

	/// <summary>
	/// Used to avoid z fighting/layering. 
	/// </summary>
	protected override float NUDGE
	{
		get { return 0.002f; }
	}

	/// <summary>
	/// Adds the cutouts for the extensions.
	/// </summary>
	protected override void AddExtensionCutouts(int i, ref List<pb_Object> cutouts, List<PBUtility.Cutout> cuts, Transform wall, float wallLength)
	{
		FullHouse f = transform.GetComponentInParent<HouseCustomizer>().customizableHouse;
		Wall w;
		int reverse;
		switch(i)
		{
			case 3:
				w = Wall.right;
				reverse = 1;
				break;
			case 2:
				w = Wall.left;
				reverse = -1;
				break;
			case 1:
				w = Wall.front;
				reverse = 1;
				break;
			case 0:
			default:
				w = Wall.back;
				reverse = -1;
				break;
		}
		if (w == exWall)
		{
			pb_Object cutout = pb_ShapeGenerator.CubeGenerator(new Vector3(f.WallLength(w) - ((f.AO.foundationSeparation + f.AO.wallThickness) * 2f),
																		   Mathf.Min(f.Dim.height, Dim.height) - f.AO.ceilingThickness, AO.wallThickness + NUDGE));
			cutout.transform.SetParent(transform);
			cutout.transform.localPosition = new Vector3((f.WallLength(w) + WallLength(w) - (f.AO.foundationSeparation * 4f)) * exPosition / 2f * reverse,
														 (Mathf.Min(f.Dim.height, Dim.height) - (f.AO.ceilingThickness + Dim.height)) / 2f, 0);

			cutouts.Add(cutout);
			cuts.Add(new PBUtility.Cutout(cutout));
		}
	}

	protected override void CreateExtensions() { }

	protected override void RemakeExtensions() { }	
}
