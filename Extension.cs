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
	public HouseExtension hExt;

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
		switch (i)
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
		if (w == hExt.wall)
		{
			pb_Object cutout = pb_ShapeGenerator.CubeGenerator(new Vector3(f.WallLength(w) - ((f.AO.foundationSeparation + f.AO.wallThickness) * 2f),
																		   Mathf.Min(f.Dim.height, Dim.height) - f.AO.ceilingThickness, AO.wallThickness + NUDGE));
			cutout.transform.SetParent(transform);
			cutout.transform.localPosition = new Vector3((f.WallLength(w) + WallLength(w) - (f.AO.foundationSeparation * 4f)) * hExt.position / 2f * reverse,
														 (Mathf.Min(f.Dim.height, Dim.height) - (f.AO.ceilingThickness + Dim.height)) / 2f, 0);

			cutouts.Add(cutout);
			cuts.Add(new PBUtility.Cutout(cutout));
		}
	}

	/// <summary>
	/// Extends the roofs to match extensions.
	/// </summary>
	protected override void ExtendRoofs(List<pb_Object> roofs)
	{
		if (hExt.extendRoof && hExt.Extendable)
		{
			int[] triangles1 = new int[] { 1, 3 };
			int[] triangles2 = new int[] { 5, 7 };
			int[] triangles3 = new int[] { 0, 2 };
			int[] triangles4 = new int[] { 9, 11 };

			float ratio;
			float distance;
			float fixDist;

			switch (hExt.wall)
			{
				case Wall.front:
				case Wall.back:
					ratio = Dim.frontGableHeight / customizer.customizableHouse.Dim.sideGableHeight;
					distance = ((customizer.customizableHouse.Dim.depth) * ratio / 2f) - 
						((AO.roofOverhangFront / 2f) + AO.foundationSeparation + AO.wallThickness) +
						(AO.roofHeight * (1 - ratio));
					fixDist = AO.foundationSeparation + AO.wallThickness;
					break;
				case Wall.right:
				case Wall.left:
				default:
					ratio = Dim.sideGableHeight / customizer.customizableHouse.Dim.frontGableHeight;
					distance = (customizer.customizableHouse.Dim.width * ratio / 2f) -
						((AO.roofOverhangFront / 2f) + AO.foundationSeparation) +
						(AO.roofHeight * (1 - ratio));
					fixDist = AO.foundationSeparation;
					break;
			}

			switch (hExt.wall)
			{
				case Wall.front:
				case Wall.right:
					roofs[0].TranslateVertices(triangles1, new Vector3(0, 0, distance));
					roofs[0].TranslateVertices(triangles3, new Vector3(0, 0, -fixDist));
					roofs[1].TranslateVertices(triangles2, new Vector3(0, 0, -distance));
					roofs[1].TranslateVertices(triangles4, new Vector3(0, 0, fixDist));
					break;
				case Wall.back:
				case Wall.left:
					roofs[0].TranslateVertices(triangles2, new Vector3(0, 0, -distance));
					roofs[0].TranslateVertices(triangles4, new Vector3(0, 0, fixDist));
					roofs[1].TranslateVertices(triangles1, new Vector3(0, 0, distance));
					roofs[1].TranslateVertices(triangles3, new Vector3(0, 0, -fixDist));
					break;
			}
		}
	}

	protected override void CreateExtensions() { }

	protected override void RemakeExtensions() { }
}
