using ProBuilder.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A fully equipped customizable house. 
/// </summary>
[System.Serializable]
public class FullHouse : CustomizableHouse
{
	#region SerializableFields

	/// <summary>
	/// Contains all of the dimensions. 
	/// </summary>
	public Dimensions dimensions = new Dimensions();

	/// <summary>
	/// Contains all of the textures. 
	/// </summary>
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
	/// All of the house extensions.
	/// </summary>
	public List<HouseExtension> extensions = new List<HouseExtension>();

	/// <summary>
	/// Contains all of the advanced settings. 
	/// </summary>
	public AdvancedOptions advancedOptions = new AdvancedOptions();

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

	public override List<HouseExtension> EX
	{
		get { return extensions; }
		set { extensions = value; }
	}

	public override AdvancedOptions AO
	{
		get { return advancedOptions; }
		set { advancedOptions = value; }
	}

	#endregion

	/// <summary>
	/// Adds the cutouts for the extensions.
	/// </summary>
	protected override void AddExtensionCutouts(int i, ref List<pb_Object> cutouts, List<PBUtility.Cutout> cuts, Transform wall, float wallLength)
	{
		foreach (HouseExtension e in EX)
		{
			if ((int)e.wall == i)
			{
				int reverse;
				switch (i)
				{
					case 3:
						reverse = -1;
						break;
					case 2:
						reverse = 1;
						break;
					case 1:
						reverse = -1;
						break;
					case 0:
					default:
						reverse = 1;
						break;
				}
				pb_Object cutout = pb_ShapeGenerator.CubeGenerator(new Vector3(e.extent.WallLength(e.wall) - ((AO.foundationSeparation + AO.wallThickness)* 2f), 
																			   Mathf.Min(e.extent.Dim.height, Dim.height) - AO.ceilingThickness, AO.wallThickness + NUDGE));
				cutout.transform.SetParent(transform);
				cutout.transform.localPosition = new Vector3((WallLength(e.wall) + e.extent.WallLength(e.wall) - (AO.foundationSeparation * 4f)) * e.position / 2f * reverse, 
															 (Mathf.Min(e.extent.Dim.height, Dim.height) - (AO.ceilingThickness + Dim.height)) / 2f, 0);

				cutouts.Add(cutout);
				cuts.Add(new PBUtility.Cutout(cutout));
			}
		}
	}

	/// <summary>
	/// Creates the extensions.
	/// </summary>
	protected override void CreateExtensions()
	{
		SetUpExtensions();
		foreach (HouseExtension e in EX)
		{
			SetUpExtension(e);
			e.extent.GenerateHouse();
		}
	}

	/// <summary>
	/// Remakes the extensions for component generation.
	/// </summary>
	protected override void RemakeExtensions()
	{
		if (extensionsObj == null)
		{
			SetUpExtensions();
			foreach (HouseExtension e in EX)
			{
				SetUpExtension(e);
			}
		}
	}
}
