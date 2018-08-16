using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using ProBuilder.Core;
using ProBuilder.MeshOperations;

/// <summary>
/// Controls houses' appearance, materials and meshes. 
/// </summary>
[System.Serializable]
public abstract class CustomizableHouse
{
	#region Enums

	public enum Wall
	{
		front = 0,
		back = 1,
		right = 2,
		left = 3,
	}

	public enum WindowType
	{
		none = -1,
		sixteenPaneAlpha = 0,
		sixteenPaneBlack = 1,
		twentyFourPaneAlpha = 2,
		twentyFourPaneBlack = 3,
	}

	public enum FrameType
	{
		// In case there are sprites that could be used as frames, 
		// keep any values that aren't sprites negative so that we can 
		// make the sprite values indexes of a list. 
		none = -100,
		outline = -99,
		outlineWithInner = -98,
		topAndBottom = -97,
		archAndBottom = -96,
	}

	public enum ShutterType
	{
		none = -1,
		panel = 0,
		board = 1,
		slats = 2,
	}

	public enum GableWindowType
	{
		none = -1,
		SemiCircleAlpha = 0,
		SemiCircleBlack = 1,
		RoundAlpha = 2,
		RoundBlack = 3,
	}

	public enum PorchRailingType
	{
		none = -1,
		railing1 = 0,
		railing2 = 1,
	}

	public enum StairRailingType
	{
		none = -1,
		railing1 = 0,
	}

	public enum AwningType
	{
		none = -1,
		single = 0,
		small = 1,
		large = 2,
	}

	public enum BalconetType
	{
		none = -1,
		single = 0,
		small = 1,
		fancy = 2,
	}

	#endregion

	#region SerializableTypes

	[System.Serializable]
	public class Dimensions : ISerializationCallbackReceiver
	{
		/// <summary>
		/// The width of the house. 
		/// </summary>
		[Range(1.0f, 50.0f)]
		public float width = 16f;

		/// <summary>
		/// The depth of the house. 
		/// </summary>
		[Range(1.0f, 50.0f)]
		public float depth = 16f;

		/// <summary>
		/// The height of the house. 
		/// </summary>
		[Range(1.0f, 50.0f)]
		public float height = 16f;

		/// <summary>
		/// The height of the front gable.
		/// </summary>
		[ConditionalHide("FrontGableActive", min: 0f, max: 30f)]
		public float frontGableHeight = 5.0f;

		/// <summary>
		/// The height of the side gable.
		/// </summary>
		[ConditionalHide("SideGableActive", min: 0f, max: 30f)]
		public float sideGableHeight = 0.0f;

		/// <summary>
		/// The number of floors of this house
		/// </summary>
		[Range(1, 5)]
		public int floors = 1;

		/// <summary>
		/// Gets the height of a floor.
		/// </summary>
		public float FloorHeight
		{
			get { return height / floors; }
		}

		public bool FrontGableActive
		{
			get { return sideGableHeight == 0f; }
		}

		public bool SideGableActive
		{
			get { return frontGableHeight == 0f; }
		}

		[HideInInspector]
		public bool initialized = false;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!initialized)
			{
				initialized = true;
				width = 16f;
				depth = 16f;
				height = 16f;
				frontGableHeight = 5.0f;
				sideGableHeight = 0.0f;
				floors = 1;
			}
		}
	}

	[System.Serializable]
	public class Textures
	{
		/// <summary>
		/// Struct containing all of the outer textures. 
		/// </summary>
		public OuterTextures outerTextures = new OuterTextures();

		/// <summary>
		/// Struct containing all of the inner textures. 
		/// </summary>
		public InnerTextures innerTextures = new InnerTextures();

		/// <summary>
		/// Struct containing all of the colors.
		/// </summary>
		public Colors colors = new Colors();

		/// <summary>
		/// Struct containing all of the tilings.
		/// </summary>
		public Tilings tilings = new Tilings();
	}

	[System.Serializable]
	public class OuterDetails : ISerializationCallbackReceiver
	{
		/// <summary>
		/// Whether or not there are roof edges. 
		/// </summary>
		public bool roofEdges = false;

		/// <summary>
		/// Whether or not there are wall edges.
		/// </summary>
		public bool wallEdges = false;

		/// <summary>
		/// Whether or not there are foundation edges. 
		/// </summary>
		public bool foundationEdges = false;

		/// <summary>
		/// The type of windows on this house. 
		/// </summary>
		public WindowType windowType = WindowType.sixteenPaneAlpha;

		/// <summary>
		/// The type of window frame on this house.
		/// </summary>
		public FrameType windowFrame = FrameType.outlineWithInner;

		/// <summary>
		/// Determines whether shutters are enabled. 
		/// </summary>
		/// <value><c>true</c> if show shutter; otherwise, <c>false</c>.</value>
		public bool ShowShutter
		{
			get
			{
				switch (windowFrame)
				{
					case FrameType.outline:
					case FrameType.outlineWithInner:
						return false;
					case FrameType.topAndBottom:
					case FrameType.archAndBottom:
					case FrameType.none:
					default:
						return true;
				}
			}
		}

		/// <summary>
		/// The type of window shutters on this house.
		/// </summary>
		[ConditionalHide("ShowShutter")]
		public ShutterType windowShutter = ShutterType.none;

		/// <summary>
		/// The type of gable windows on this house. 
		/// </summary>
		public GableWindowType gableWindowType = GableWindowType.SemiCircleAlpha;

		/// <summary>
		/// The type of the railing.
		/// </summary>
		public PorchRailingType porchRailingType = PorchRailingType.railing2;

		/// <summary>
		/// This house's windows. 
		/// </summary>
		public List<Window> windows;

		/// <summary>
		/// This house's gable windows. 
		/// </summary>
		public List<GableWindow> gableWindows;

		/// <summary>
		/// This house's store windows. 
		/// </summary>
		public List<StoreWindow> storeWindows;

		/// <summary>
		/// This house's outer doors. 
		/// </summary>
		public List<OuterDoor> outerDoors;

		/// <summary>
		/// This house's chimneys. 
		/// </summary>
		public List<Chimney> chimneys;

		/// <summary>
		/// This house's porches. 
		/// </summary>
		public List<Porch> porches;

		/// <summary>
		/// This house's dormers.
		/// </summary>
		public List<Dormer> dormers;

		[HideInInspector]
		public bool initialized = false;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!initialized)
			{
				initialized = true;
				roofEdges = false;
				wallEdges = false;
				foundationEdges = false;
				windowType = WindowType.sixteenPaneAlpha;
				windowFrame = FrameType.outlineWithInner;
				windowShutter = ShutterType.none;
				gableWindowType = GableWindowType.SemiCircleAlpha;
				porchRailingType = PorchRailingType.railing2;
			}
		}
	}

	[System.Serializable]
	public class InnerDetails : ISerializationCallbackReceiver
	{
		/// <summary>
		/// Whether or not the ceiling has rafters. 
		/// </summary>
		public bool rafters = false;

		/// <summary>
		/// The type of the railing.
		/// </summary>
		public StairRailingType stairRailingType = StairRailingType.railing1;

		/// <summary>
		/// This house's inner walls. 
		/// </summary>
		public List<InnerWall> innerWalls;

		/// <summary>
		/// This house's stairs. 
		/// </summary>
		public List<StairCase> innerStairs;

		[HideInInspector]
		public bool initialized = false;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!initialized)
			{
				initialized = true;
				rafters = false;
				stairRailingType = StairRailingType.railing1;
			}
		}
	}

	[System.Serializable]
	public class AdvancedOptions
	{
		[Header("Foundation")]

		/// <summary>
		/// How far the foundation is from the outer walls. 
		/// </summary>
		public float foundationSeparation = 0.25f;

		/// <summary>
		/// The height of the foundation. 
		/// </summary>
		public float foundationHeight = 0.5f;

		/// <summary>
		/// How thick the walls are. 
		/// </summary>
		public float wallThickness = 0.25f;

		/// <summary>
		/// How deep an indent in a wall is. 
		/// </summary>
		public float indentThickness = 0.25f;

		[Header("Ceilings")]

		/// <summary>
		/// How thick the ceilings are. 
		/// </summary>
		public float ceilingThickness = 0.5f;

		/// <summary>
		/// How thick the rafters are. 
		/// </summary>
		public float rafterThickness = 0.3f;

		/// <summary>
		/// How wide the rafters are. 
		/// </summary>
		public float rafterWidth = 1f;


		/// <summary>
		/// How thick the supporting rafters are. 
		/// </summary>
		public float sideRafterThickness = 0.5f;

		/// <summary>
		/// How wide the supporting rafters are. 
		/// </summary>
		public float sideRafterWidth = 0.4f;

		[Header("Roofs")]

		/// <summary>
		/// The height of the roof.
		/// </summary>
		public float roofHeight = 0.5f;

		/// <summary>
		/// How much the front of the roof hangs over the house. 
		/// </summary>
		public float roofOverhangFront = 0.4f;

		/// <summary>
		/// How much the side of the roof hangs over the house. 
		/// </summary>
		public float roofOverhangSide = 0.5f;

		/// <summary>
		/// The width of the roof corners.
		/// </summary>
		public float roofCornerWidth = 1f;

		/// <summary>
		/// The dormer dimensions.
		/// </summary>
		public Vector2 dormerDimensions = new Vector2(3, 3);

		/// <summary>
		/// The height of the dormer gable.
		/// </summary>
		public float dormerGableHeight = 1f;

		/// <summary>
		/// The height of the dormer roof.
		/// </summary>
		public float dormerRoofHeight = 0.25f;

		/// <summary>
		/// How much the front of the roof hangs over the dormers. 
		/// </summary>
		public float dormerOverhangFront = 0.25f;

		/// <summary>
		/// How much the side of the roof hangs over the dormers. 
		/// </summary>
		public float dormerOverhangSide = 0.25f;

		/// <summary>
		/// The y offset of the dormer windows. 
		/// </summary>
		public float dormerWindowYOffset = 0.2f;

		/// <summary>
		/// Attempts to fix the front dormer texture alignment to the sides. 
		/// May distort the front face of the dormer. 
		/// </summary>
		public bool dormerAlignmentFix = false;

		/// <summary>
		/// YOffset for the dormerAlignment
		/// </summary>
		[ConditionalHide("dormerAlignmentFix", true, true)]
		public float dormerAlignmentOffsetY = 0f;

		[Header("Wall Edges")]

		/// <summary>
		/// The depth of the house edges;
		/// </summary>
		public float edgeDepth = 0.1f;

		/// <summary>
		/// The width of the wall edges.
		/// </summary>
		public float edgeWidth = 0.3f;


		[Header("Windows & Doors")]

		/// <summary>
		/// The dimensions of a door. 
		/// </summary>
		public Vector3 doorDimensions = new Vector3(1.5f, 3.0f, 1.0f);

		/// <summary>
		/// The dimensions of a window. 
		/// </summary>
		public Vector3 windowDimensions = new Vector3(1.5f, 2f, 1.0f);

		/// <summary>
		/// The height of the windows relative to their floor. 
		/// </summary>
		public float windowHeight = 1.1f;

		/// <summary>
		/// The diameter of the gable windows
		/// </summary>
		public float gableWindowDiameter = 2f;

		/// <summary>
		/// The number of sides per gable window
		/// </summary>
		public int gableWindowSides = 20;

		/// <summary>
		/// The dimensions of a store window. 
		/// </summary>
		public Vector3 storeWindowDimensions = new Vector3(3.2f, 3.2f, 1.0f);

		/// <summary>
		/// The height of the store windows relative to their floor. 
		/// </summary>
		public float storeWindowHeight = 0.4f;

		/// <summary>
		/// The width of a frame.
		/// </summary>
		public float frameWidth = 0.25f;

		/// <summary>
		/// The depth of a frame.
		/// </summary>
		public float frameDepth = 0.125f;

		/// <summary>
		/// The angle of the top of a top and bottom frame.
		/// </summary>
		public float topFrameAngle = 0.08f;

		/// <summary>
		/// The width of the shutters, relative to the windows. 
		/// </summary>
		public float shutterWidth = 0.5f;

		/// <summary>
		/// The y offset of the awnings. 
		/// </summary>
		public float awningYOffset = 0.15f;

		/// <summary>
		/// The length of the awnings. 
		/// </summary>
		public float awningLength = 1.5f;

		/// <summary>
		/// The y offset of the balconets. 
		/// </summary>
		public float balconetYOffset = -0.05f;

		[Header("Chimneys")]

		/// <summary>
		/// The height of the chimney top.
		/// </summary>
		public float chimneyTopHeight = 0.5f;

		/// <summary>
		/// The width of the chimney top.
		/// </summary>
		public float chimneyTopWidth = 0.5f;

		[Header("Stairs")]

		/// <summary>
		/// The max length of stairs.
		/// </summary>
		public float maxStairLength = 20f;

		/// <summary>
		/// The max width of stairs.
		/// </summary>
		public float maxStairWidth = 10f;

		/// <summary>
		/// The railing thickness.
		/// </summary>
		public float railingThickness = 0.1f;

		/// <summary>
		/// The width of the railings.
		/// </summary>
		public float railingWidth = 0.25f;

		[Header("Porches")]

		/// <summary>
		/// The height of the porch wood.
		/// </summary>
		public float porchWoodHeight = 0.1f;

		/// <summary>
		/// The porch wood overhang.
		/// </summary>
		public float porchWoodOverhang = 0.05f;

		/// <summary>
		/// The width of the porch stair guards.
		/// </summary>
		public float porchStairGuardWidth = 0.3f;

		/// <summary>
		/// How far inset the columns are from the side edge of the porch. 
		/// </summary>
		public float porchColumnXInset = 1f;

		/// <summary>
		/// How far inset the columns are from the front edge of the porch. 
		/// </summary>
		public float porchColumnZInset = 0.6f;

		/// <summary>
		/// The porch column frequency.
		/// </summary>
		public float porchColumnFrequency = 3f;

		/// <summary>
		/// The porch roof height.
		/// </summary>
		public float porchRoofHeight = 0.3f;

		/// <summary>
		/// The porch roof angle.
		/// </summary>
		public float porchRoofAngle = 1.5f;

		/// <summary>
		/// The height of the porch railing.
		/// </summary>
		public float porchRailingHeight = 0.8f;
	}

	[System.Serializable]
	public struct HouseExtension
	{
		/// <summary>
		/// The house extension.
		/// </summary>
		public Extension extent;
		/// <summary>
		/// The wall this extension is placed on.
		/// </summary>
		public Wall wall;
		/// <summary>
		/// The position of the extension.
		/// </summary>
		[Range(-1, 1)]
		public float position;
	}

	[System.Serializable]
	public struct OuterTextures
	{
		/// <summary>
		/// The material of the side of the house. 
		/// </summary>
		public Material sidingMaterial;

		/// <summary>
		/// The material of the roof of the house. 
		/// </summary>
		public Material roofMaterial;

		/// <summary>
		/// The material of the outer trim of the house. 
		/// </summary>
		public Material outerTrimMaterial;

		/// <summary>
		/// The material of the foundation of the house. 
		/// </summary>
		public Material foundationMaterial;

		/// <summary>
		/// The material of the outer steps of the house. 
		/// </summary>
		public Material outerWood;

		/// <summary>
		/// The chimney material.
		/// </summary>
		public Material chimneyMaterial;

		/// <summary>
		/// The awning material.
		/// </summary>
		public Material awningMaterial;
	}

	[System.Serializable]
	public struct InnerTextures
	{
		/// <summary>
		/// The material of the inner wall of the house. 
		/// </summary>
		public Material innerWallMaterial;

		/// <summary>
		/// The material of the floor of the house. 
		/// </summary>
		public Material floorMaterial;

		/// <summary>
		/// The material of the ceiling of the house. 
		/// </summary>
		public Material ceilingMaterial;

		/// <summary>
		/// The material of the inner trim of the house. 
		/// </summary>
		public Material innerTrimMaterial;

		/// <summary>
		/// The material of the inner wood (rafters, etc) of the house. 
		/// </summary>
		public Material innerWoodMaterial;

		/// <summary>
		/// The inner stairs material.
		/// </summary>
		public Material innerStairsMaterial;

		/// <summary>
		/// The baluster material.
		/// </summary>
		public Material balusterMaterial;

		/// <summary>
		/// The railing material.
		/// </summary>
		public Material railingMaterial;
	}

	[System.Serializable]
	public struct Colors
	{
		/// <summary>
		/// The color of the porch railing.
		/// </summary>
		public Color porchRailingColor;

		/// <summary>
		/// The color of the gable windows.
		/// </summary>
		public Color gableWindowColor;

		/// <summary>
		/// The color of the shutters.
		/// </summary>
		public Color shutterColor;
	}

	[System.Serializable]
	public struct Tilings
	{
		[Header("Outer Tilings")]

		/// <summary>
		/// The tiling of the side of the house. 
		/// </summary>
		[Range(-100, 100)]
		public float sidingTiling;

		/// <summary>
		/// The tiling of the roof of the house. 
		/// </summary>
		[Range(-100, 100)]
		public float roofTiling;

		/// <summary>
		/// The tiling of the outer trim of the house. 
		/// </summary>
		[Range(-100, 100)]
		public float outerTrimTiling;

		/// <summary>
		/// The tiling of the foundation of the house. 
		/// </summary>
		[Range(-100, 100)]
		public float foundationTiling;

		/// <summary>
		/// The tiling of the outer steps of the house. 
		/// </summary>
		[Range(-100, 100)]
		public float outerWood;

		/// <summary>
		/// The chimney tiling.
		/// </summary>
		[Range(-100, 100)]
		public float chimneyTiling;

		/// <summary>
		/// The awning tiling.
		/// </summary>
		[Range(-100, 100)]
		public float awningTiling;

		[Header("Inner Tilings")]

		/// <summary>
		/// The tiling of the inner wall of the house. 
		/// </summary>
		[Range(-100, 100)]
		public float innerWallTiling;

		/// <summary>
		/// The tiling of the floor of the house. 
		/// </summary>
		[Range(-100, 100)]
		public float floorTiling;

		/// <summary>
		/// The tiling of the ceiling of the house. 
		/// </summary>
		[Range(-100, 100)]
		public float ceilingTiling;

		/// <summary>
		/// The tiling of the inner trim of the house. 
		/// </summary>
		[Range(-100, 100)]
		public float innerTrimTiling;

		/// <summary>
		/// The tiling of the inner wood (rafters, etc) of the house. 
		/// </summary>
		[Range(-100, 100)]
		public float innerWoodTiling;

		/// <summary>
		/// The inner stairs tiling.
		/// </summary>
		[Range(-100, 100)]
		public float innerStairsTiling;

		/// <summary>
		/// The baluster tiling.
		/// </summary>
		[Range(-100, 100)]
		public float balusterTiling;

		/// <summary>
		/// The railing tiling.
		/// </summary>
		[Range(-100, 100)]
		public float railingTiling;
	}

	[System.Serializable]
	public class Window : ISerializationCallbackReceiver
	{
		public Wall wall;
		[Range(1, 5)]
		public int floor = 1;

		[Range(-1, 1)]
		public List<float> placements;

		/// <summary>
		/// The type of the awning.
		/// </summary>
		public AwningType awningType = AwningType.none;

		public bool ShowAwningPlacement
		{
			get
			{
				switch (awningType)
				{
					case AwningType.small:
					case AwningType.large:
						return true;
					case AwningType.none:
					case AwningType.single:
					default:
						return false;
				}
			}
		}

		[ConditionalHide("ShowAwningPlacement", true, false, -1, 1)]
		public float awningPlacement;

		/// <summary>
		/// The type of the balconet.
		/// </summary>
		public BalconetType balconetType = BalconetType.none;

		public bool ShowBalconetPlacement
		{
			get
			{
				switch (balconetType)
				{
					case BalconetType.small:
						return true;
					case BalconetType.none:
					case BalconetType.single:
					case BalconetType.fancy:
					default:
						return false;
				}
			}
		}

		[ConditionalHide("ShowBalconetPlacement", true, false, -1, 1)]
		public float balconetPlacement;

		public bool overrideDetails = false;

		/// <summary>
		/// The type of these windows.
		/// </summary>
		[ConditionalHide("overrideDetails", true)]
		public WindowType typeOverride = WindowType.none;

		/// <summary>
		/// The type of these windows' frames.
		/// </summary>
		[ConditionalHide("overrideDetails", true)]
		public FrameType frameOverride = FrameType.none;

		/// <summary>
		/// Determines whether shutters are enabled. 
		/// </summary>
		/// <value><c>true</c> if show shutter; otherwise, <c>false</c>.</value>
		public bool ShowShutter
		{
			get
			{
				switch (frameOverride)
				{
					case FrameType.topAndBottom:
					case FrameType.archAndBottom:
						return overrideDetails;
					case FrameType.outline:
					case FrameType.outlineWithInner:
					case FrameType.none:
					default:
						return false;
				}
			}
		}

		/// <summary>
		/// The type of these windows' shutters
		/// </summary>
		[ConditionalHide("ShowShutter", true)]
		public ShutterType shutterOverride = ShutterType.none;

		[HideInInspector]
		public bool initialized = false;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!initialized)
			{
				initialized = true;
				floor = 1;
				placements = new List<float> { 0 };
				awningType = AwningType.none;
				balconetType = BalconetType.none;
				overrideDetails = false;
				typeOverride = WindowType.none;
				frameOverride = FrameType.none;
				shutterOverride = ShutterType.none;
			}
		}
	}

	[System.Serializable]
	public class GableWindow : ISerializationCallbackReceiver
	{
		public Wall wall;

		[Range(-1.0f, 1.0f)]
		public float placementx;

		[Range(-1.0f, 1.0f)]
		public float placementy;

		public bool overrideDetails = false;

		/// <summary>
		/// The type of this window.
		/// </summary>
		[ConditionalHide("overrideDetails", true)]
		public GableWindowType typeOverride = GableWindowType.none;

		/// <summary>
		/// The type of this window's frames.
		/// </summary>
		[ConditionalHide("overrideDetails", true)]
		public FrameType frameOverride = FrameType.none;

		[HideInInspector]
		public bool initialized = false;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!initialized)
			{
				initialized = true;
				placementx = 0;
				placementy = 0;
				overrideDetails = false;
				typeOverride = GableWindowType.none;
				frameOverride = FrameType.none;
			}
		}
	}

	[System.Serializable]
	public class OuterDoor : ISerializationCallbackReceiver
	{
		public Wall wall;
		[Range(-1.0f, 1.0f)]
		public float placement;

		public bool steps = true;

		/// <summary>
		/// The type of the awning.
		/// </summary>
		public AwningType awningType = AwningType.none;

		public bool ShowAwningPlacement
		{
			get
			{
				switch (awningType)
				{
					case AwningType.small:
					case AwningType.large:
						return true;
					case AwningType.none:
					case AwningType.single:
					default:
						return false;
				}
			}
		}

		[ConditionalHide("ShowAwningPlacement", true, false, -1, 1)]
		public float awningPlacement;

		[HideInInspector]
		public bool initialized = false;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!initialized)
			{
				initialized = true;
				placement = 0;
				awningType = AwningType.none;
				awningPlacement = 0;
			}
		}
	}

	[System.Serializable]
	public class StoreWindow : ISerializationCallbackReceiver
	{
		public Wall wall;

		[Range(-1, 1)]
		public List<float> placements;

		[ConditionalHide("canIndent", true)]
		public bool indent = true;

		/// <summary>
		/// The type of the awning.
		/// </summary>
		public AwningType awningType = AwningType.none;

		public bool ShowAwningPlacement
		{
			get
			{
				switch (awningType)
				{
					case AwningType.small:
					case AwningType.large:
						return true;
					case AwningType.none:
					case AwningType.single:
					default:
						return false;
				}
			}
		}

		[ConditionalHide("ShowAwningPlacement", true, false, -1, 1)]
		public float awningPlacement;

		[HideInInspector]
		public bool initialized = false;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!initialized)
			{
				initialized = true;
				placements = new List<float> { 0 };
				indent = true;
				awningType = AwningType.none;
				awningPlacement = 0;
			}
		}
	}

	[System.Serializable]
	public class Chimney : ISerializationCallbackReceiver
	{
		[Header("Dimensions")]

		/// <summary>
		/// Where the chimney is placed relative to the x axis. 
		/// </summary>
		[Range(-1.0f, 1.0f)]
		public float placementX;

		/// <summary>
		/// Where the chimney is placed relative to the y axis. 
		/// </summary>
		[Range(-1.0f, 1.0f)]
		public float placementY;

		/// <summary>
		/// The width of the chimney. 
		/// </summary>
		[Range(1.0f, 10.0f)]
		public float width = 3f;

		/// <summary>
		/// The depth of the chimney. 
		/// </summary>
		[Range(1.0f, 10.0f)]
		public float depth = 3f;

		/// <summary>
		/// The height of the chimney. 
		/// </summary>
		[Range(5.0f, 40.0f)]
		public float height = 20f;

		[Header("Extrudes")]

		/// <summary>
		/// The front extrude of the chimney. 
		/// </summary>
		[Range(0.0f, 2.0f)]
		public float frontExtrude = 0f;

		/// <summary>
		/// The left extrude of the chimney. 
		/// </summary>
		[Range(0.0f, 2.0f)]
		public float leftExtrude = 0f;

		/// <summary>
		/// The right extrude of the chimney. 
		/// </summary>
		[Range(0.0f, 2.0f)]
		public float rightExtrude = 0f;

		/// <summary>
		/// The back extrude of the chimney. 
		/// </summary>
		[Range(0.0f, 2.0f)]
		public float backExtrude = 0f;

		/// <summary>
		/// The extrude height of the chimney. 
		/// </summary>
		[Range(0f, 1.0f)]
		public float extrudeHeight = 0.1f;

		/// <summary>
		/// The extrude angle of the chimney. 
		/// </summary>
		[Range(0f, 1.0f)]
		public float extrudeAngle = 0.8f;

		[HideInInspector]
		public bool initialized = false;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!initialized)
			{
				initialized = true;
				placementX = 0;
				placementY = 0;
				width = 3f;
				depth = 3f;
				height = 20f;
				frontExtrude = 0f;
				leftExtrude = 0f;
				rightExtrude = 0f;
				backExtrude = 0f;
				extrudeHeight = 0.1f;
				extrudeAngle = 0.8f;
			}
		}
	}

	[System.Serializable]
	public class Porch : ISerializationCallbackReceiver
	{
		public Wall wall;

		[Range(-1.0f, 1.0f)]
		public float placement = 0;

		[Range(0.1f, 1.0f)]
		public float length = 0.8f;

		[Range(1, 20)]
		public float width = 4f;

		[ConditionalHide("leftCorner", false, true)]
		public bool leftStairs;

		[ConditionalHide("rightCorner", false, true)]
		public bool rightStairs;

		public bool middleStairs;

		[ConditionalHide("middleStairs", true, false, -1.0f, 1.0f)]
		public float middleStairPlacement = 0;

		[ConditionalHide("middleStairs", true, false, 0.01f, 1.0f)]
		public float middleStairWidth = 0.1f;

		[ConditionalHide("LeftCornerEnabled", true, false)]
		public bool leftCorner;

		[ConditionalHide("RightCornerEnabled", true, false)]
		public bool rightCorner;

		public bool LeftCornerEnabled
		{
			get
			{
				return placement == -1f || length == 1f;
			}
		}

		public bool RightCornerEnabled
		{
			get
			{
				return placement == 1f || length == 1f;
			}
		}

		[HideInInspector]
		public bool initialized = false;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!initialized)
			{
				initialized = true;
				placement = 0;
				length = 0.8f;
				width = 4f;
				leftStairs = false;
				rightStairs = false;
				middleStairs = false;
				middleStairPlacement = 0;
				middleStairWidth = 0.1f;
				leftCorner = false;
				rightCorner = false;
			}
		}
	}

	[System.Serializable]
	public class InnerWall : ISerializationCallbackReceiver
	{

		public bool rotate = false;

		[Range(1, 5)]
		public int floor = 1;

		[Range(0f, 1f)]
		public float width = 1f;

		/// <summary>
		/// Where the wall is placed relative to the x axis. 
		/// </summary>
		[Range(-1.0f, 1.0f)]
		public float placementX;

		/// <summary>
		/// Where the wall is placed relative to the y axis. 
		/// </summary>
		[Range(-1.0f, 1.0f)]
		public float placementY;

		[Range(-1.0f, 1.0f)]
		public List<float> doorPlacements;

		[HideInInspector]
		public bool initialized = false;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!initialized)
			{
				initialized = true;
				rotate = false;
				floor = 1;
				width = 1f;
				placementX = 0;
				placementY = 0;
				doorPlacements = new List<float> { 0 };
			}
		}
	}

	[System.Serializable]
	public class StairCase : ISerializationCallbackReceiver
	{
		/// <summary>
		/// The rotation of the staircase.
		/// </summary>
		[Range(0, 3)]
		public int rotation = 0;

		/// <summary>
		/// The floor the staircase is located on.
		/// </summary>
		[Range(1, 5)]
		public int floor = 1;

		/// <summary>
		/// The width of the staircase. 
		/// </summary>
		[Range(0f, 1f)]
		public float width = 0.5f;

		/// <summary>
		/// The length of the staircase.
		/// </summary>
		[Range(0f, 1f)]
		public float length = 0.5f;

		/// <summary>
		/// Where the staircase is placed relative to the x axis. 
		/// </summary>
		[Range(-1.0f, 1.0f)]
		public float placementX;

		/// <summary>
		/// Where the staircase is placed relative to the y axis. 
		/// </summary>
		[Range(-1.0f, 1.0f)]
		public float placementY;

		/// <summary>
		/// The length of the cutout above the staircase.
		/// </summary>
		[Range(0f, 1f)]
		public float cutoutLength = 1f;

		/// <summary>
		/// Disables the left railing.
		/// </summary>
		public bool disableLeftRailing = false;

		/// <summary>
		/// Disables the right railing.
		/// </summary>
		public bool disableRightRailing = false;

		/// <summary>
		/// Disables the left guard railing.
		/// </summary>
		public bool disableLeftGuardRailing = false;

		/// <summary>
		/// Disables the right guard railing.
		/// </summary>
		public bool disableRightGuardRailing = false;

		/// <summary>
		/// Disables the front guard railing.
		/// </summary>
		public bool disableFrontGuardRailing = false;

		[HideInInspector]
		public bool initialized = false;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!initialized)
			{
				initialized = true;
				rotation = 0;
				floor = 1;
				width = 0.5f;
				length = 0.5f;
				placementX = 0;
				placementY = 0;
				cutoutLength = 1f;
				disableLeftRailing = false;
				disableRightRailing = false;
				disableLeftGuardRailing = false;
				disableRightGuardRailing = false;
				disableFrontGuardRailing = false;
			}
		}
	}

	[System.Serializable]
	public class Dormer : ISerializationCallbackReceiver
	{
		/// <summary>
		/// Which roof the dormers are placed on.
		/// </summary>
		public bool alternateRoof = true;

		/// <summary>
		/// Where the dormers are placed relative to the x axis. 
		/// </summary>
		[Range(-1.0f, 1.0f)]
		public List<float> placements;

		/// <summary>
		/// Where the dormers are placed relative to the y axis. 
		/// </summary>
		[Range(-1.0f, 1.0f)]
		public float placementY;

		public bool overrideWindowDetails = false;

		/// <summary>
		/// The type of these dormer's windows.
		/// </summary>
		[ConditionalHide("overrideWindowDetails", true)]
		public WindowType typeOverride = WindowType.none;

		/// <summary>
		/// The type of these dormer's windows' frames.
		/// </summary>
		[ConditionalHide("overrideWindowDetails", true)]
		public FrameType frameOverride = FrameType.none;

		[HideInInspector]
		public bool initialized = false;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!initialized)
			{
				initialized = true;
				alternateRoof = true;
				placements = new List<float> { 0 };
				placementY = 0;
				overrideWindowDetails = false;
				typeOverride = WindowType.none;
				frameOverride = FrameType.none;
			}
		}
	}

	#endregion

	#region SerializableFields

	/// <summary>
	/// Contains all of the dimensions. 
	/// </summary>
	public virtual Dimensions Dim { get; set; }

	/// <summary>
	/// Contains all of the textures. 
	/// </summary>
	public virtual Textures Tex { get; set; }

	/// <summary>
	/// Contains all of the outer details. 
	/// </summary>
	public virtual OuterDetails OD { get; set; }

	/// <summary>
	/// Contains all of the inner details. 
	/// </summary>
	public virtual InnerDetails ID { get; set; }

	/// <summary>
	/// All of the house extensions.
	/// </summary>
	public virtual List<HouseExtension> EX { get; set; }

	/// <summary>
	/// Contains all of the advanced settings. 
	/// </summary>
	public virtual AdvancedOptions AO { get; set; }

	#endregion

	#region Getters&Setters
	public bool canIndent
	{
		get { return AO.wallThickness > AO.indentThickness; }
	}
	#endregion

	#region Variables

	[NonSerialized]
	public HouseCustomizer customizer;

	[NonSerialized]
	public bool initialized = false;

	#region Constants

	/// <summary>
	/// Used to avoid z fighting/layering. 
	/// </summary>
	protected virtual float NUDGE
	{
		get { return 0.001f; }
	}

	/// <summary>
	/// The baluster offset.
	/// </summary>
	private const float BALUSTER_OFFSET = 0.08f;

	/// <summary>
	/// The baluster height.
	/// Height of the baluster prefab, can't be found with the current probuilder API
	/// </summary>
	private const float BALUSTER_HEIGHT = 0.75f;

	/// <summary>
	/// The porch column y offset.
	/// </summary>
	private const float PORCH_COLUMN_OFFSETY = 0.3f;

	/// <summary>
	/// The porch column height.
	/// Height of the porch column prefab, can't be found with the current probuilder API
	/// </summary>
	private const float PORCH_COLUMN_HEIGHT = 4.105f;

	/// <summary>
	/// The porch column radius.
	/// Radius of the porch column prefab, can't be found with the current probuilder API
	/// </summary>
	private const float PORCH_COLUMN_RADIUS = 0.25f;

	/// <summary>
	/// The y offset of an awning. 
	/// </summary>
	private const float AWNING_YOFFSET = 0.85f;

	/// <summary>
	/// The z offset of an awning. 
	/// </summary>
	private const float AWNING_ZOFFSET = -0.6f;

	/// <summary>
	/// The placement range of a small awning. 
	/// </summary>
	private const float AWNING_SMALL_PLACEMENTRANGE = 1.97f;

	/// <summary>
	/// The placement range of a large awning. 
	/// </summary>
	private const float AWNING_LARGE_PLACEMENTRANGE = 3f;

	/// <summary>
	/// The y offset of a balconet. 
	/// </summary>
	private const float BALCONET_YOFFSET = -0.73f;

	/// <summary>
	/// The y offset of a fancy balconet. 
	/// </summary>
	private const float BALCONET_FANCY_YOFFSET = -0.55f;

	/// <summary>
	/// The z offset of a balconet. 
	/// </summary>
	private const float BALCONET_ZOFFSET = -0.24f;

	/// <summary>
	/// The placement range of a balconet. 
	/// </summary>
	private const float BALCONET_SMALL_PLACEMENTRANGE = 1.6f;

	#endregion

	/// <summary>
	/// Gets the gameobject transform.
	/// </summary>
	/// <value>The transform.</value>
	protected virtual Transform transform
	{
		get { return customizer.transform; }
	}

	/// <summary>
	/// How wide the house is on the inside. 
	/// </summary>
	private float INNER_WIDTH
	{
		get
		{
			return Dim.width - ((AO.foundationSeparation + AO.wallThickness) * 2f);
		}
	}

	/// <summary>
	/// How deep the house is on the inside. 
	/// </summary>
	private float INNER_DEPTH
	{
		get
		{
			return Dim.depth - ((AO.foundationSeparation + AO.wallThickness) * 2f);
		}
	}

	/// <summary>
	/// Gets the component generation actions.
	/// </summary>
	private List<Action> ComponentGenerations
	{
		get
		{
			return new List<Action> {
				CreateFoundation,
				CreateCeilings,
				CreateExteriorWalls,
				CreateInnerWalls,
				CreateStaircases,
				CreateChimneys,
			};
		}
	}


	/// <summary>
	/// The current inner textures.
	/// </summary>
	private InnerTextures currentInner;

	/// <summary>
	/// The current outer textures.
	/// </summary>
	private OuterTextures currentOuter;

	/// <summary>
	/// The current colors.
	/// </summary>
	private Colors currentColors;

	/// <summary>
	/// The current tilings.
	/// </summary>
	private Tilings currentTilings;

	#region ObjectLists

	/// <summary>
	/// The foundation. 
	/// </summary>
	private List<pb_Object> foundation;

	/// <summary>
	/// The bottom floors. 
	/// </summary>
	private List<pb_Object> bottomFloors;

	/// <summary>
	/// All of the ceilings
	/// </summary>
	private List<pb_Object> ceilings;

	/// <summary>
	/// The beams of the house. 
	/// </summary>
	private List<pb_Object> beams;

	/// <summary>
	/// All of the exterior walls. 
	/// </summary>
	private List<pb_Object> exteriorWalls;

	/// <summary>
	/// All of the interior walls. 
	/// </summary>
	private List<pb_Object> interiorWalls;

	/// <summary>
	/// All of the staircases. 
	/// </summary>
	private List<pb_Object> stairCases;

	/// <summary>
	/// All of the balusters.
	/// </summary>
	private List<pb_Object> balusters;

	/// <summary>
	/// All of the railings.
	/// </summary>
	private List<pb_Object> stairRailings;

	/// <summary>
	/// The roofs of the house. 
	/// </summary>
	private List<pb_Object> roofs;

	/// <summary>
	/// The outer frames of the house. 
	/// </summary>
	private List<pb_Object> outerFrames;

	/// <summary>
	/// The inner frames of outer frames of the house. 
	/// </summary>
	private List<pb_Object> outerInnerFrames;

	/// <summary>
	/// The inner frames of the house. 
	/// </summary>
	private List<pb_Object> innerFrames;

	/// <summary>
	/// The outer wood parts of the house. 
	/// </summary>
	private List<pb_Object> outerWood;

	/// <summary>
	/// The chimneys.
	/// </summary>
	private List<pb_Object> chimneyObjs;

	/// <summary>
	/// Various parts of the outer trim.
	/// </summary>
	private List<pb_Object> outerTrim;

	/// <summary>
	/// Various parts of the outer tiling.
	/// </summary>
	private List<pb_Object> exteriorSiding;

	/// <summary>
	/// The awnings.
	/// </summary>
	private List<pb_Object> awnings;

	/// <summary>
	/// All pro builder objects.
	/// </summary>
	private List<pb_Object> allObjs;

	/// <summary>
	/// The gable window sprites.
	/// </summary>
	private List<SpriteRenderer> gableWindowSprites;

	/// <summary>
	/// The window shutter sprites.
	/// </summary>
	private List<SpriteRenderer> shutterSprites;

	/// <summary>
	/// The porch railings.
	/// </summary>
	private List<SpriteRenderer> porchRailings;

	#endregion

	#region HouseObjects
	/// <summary>
	/// The foundation object.
	/// </summary>
	protected GameObject foundationObj;

	/// <summary>
	/// The ceilings object.
	/// </summary>
	protected GameObject ceilingsObj;

	/// <summary>
	/// The exterior walls object.
	/// </summary>
	protected GameObject exteriorWallsObj;

	/// <summary>
	/// The roofs object.
	/// </summary>
	protected GameObject roofsObj;

	/// <summary>
	/// The inner walls object.
	/// </summary>
	protected GameObject innerWallsObj;

	/// <summary>
	/// The stair cases object.
	/// </summary>
	protected GameObject stairCasesObj;

	/// <summary>
	/// The chimneys object.
	/// </summary>
	protected GameObject chimneysObj;

	/// <summary>
	/// The extensions object. 
	/// </summary>
	protected GameObject extensionsObj;

	/// <summary>
	/// Gets the house objects.
	/// </summary>
	protected List<GameObject> HouseObjects
	{
		get
		{
			return new List<GameObject> {
				foundationObj, ceilingsObj, exteriorWallsObj, roofsObj,
				innerWallsObj, stairCasesObj, chimneysObj, extensionsObj,
			};
		}
	}

	#endregion

	#endregion

	#region Functions

	#region UtilityFunctions

	/// <summary>
	/// Gets and object to be used for hierarchy organization.
	/// </summary>
	/// <returns>The hierarchy object.</returns>
	/// <param name="n">The name of this object.</param>
	/// <param name="parent">The parent of this object.</param>
	private Transform GetHierarchyObject(string n, Transform parent = null, Vector3 position = new Vector3())
	{
		if (parent == null)
		{
			parent = transform;
		}
		Transform r = new GameObject(n).transform;
		r.parent = parent;
		r.localPosition = position;
		r.localRotation = Quaternion.identity;
		return r;
	}

	/// <summary>
	/// Creates a cube of the given characteristics, adds it to the given list and adds it to all objs. 
	/// </summary>
	/// <param name="cDimensions">The dimensions of the cube.</param>
	/// <param name="position">The local position of the cube.</param>
	/// <param name="parent">The parent of the cube.</param>
	/// <param name="n">The name of the cube.</param>
	/// <param name="list">The object list the cube is added to.</param>
	private pb_Object CreateCube(Vector3 cDimensions, Vector3 position, Transform parent, string n,
							ref List<pb_Object> list, Vector3 rotation = default(Vector3))
	{
		pb_Object cube = pb_ShapeGenerator.CubeGenerator(cDimensions);
		cube.transform.SetParent(parent);
		cube.transform.localPosition = position;
		cube.transform.localRotation = Quaternion.Euler(rotation);
		cube.gameObject.name = n;
		if (list != null)
		{
			list.Add(cube);
		}
		allObjs.Add(cube);
		return cube;
	}

	/// <summary>
	/// Creates a sprite renderer of the given characteristics, and adds it to the given list.
	/// </summary>
	/// <param name="sDimensions">The dimensions of the sprite renderer.</param>
	/// <param name="position">The local position of the sprite renderer.</param>
	/// <param name="parent">The parent of the sprite renderer.</param>
	/// <param name="n">The name of the sprite renderer.</param>
	/// <param name="list">The sprite renderer list the sprite renderer is added to.</param>
	/// <param name="sprite">The sprite to be used.</param>
	private SpriteRenderer CreateSprite(Vector2 sDimensions, Vector3 position, Transform parent, string n,
										ref List<SpriteRenderer> list, UnityEngine.Sprite sprite, Vector3 rotation = default(Vector3))
	{
		SpriteRenderer s = new GameObject(n).AddComponent<SpriteRenderer>();
		list.Add(s);
		s.sprite = sprite;
		s.transform.parent = parent;
		s.transform.localPosition = position;
		SpriteUtility.ResizeSpriteRendererToDimensions(sDimensions, s);
		s.transform.localRotation = Quaternion.Euler(rotation);
		return s;
	}

	/// <summary>
	/// Determines whether an object is facing to the side, and gives either width or depth. 
	/// </summary>
	private float FacingLength(Transform t)
	{
		return t.localRotation.eulerAngles.y % 180 == 0 ? Dim.width : Dim.depth;
	}

	/// <summary>
	/// Determines whether an object is facing to the side, and gives either width or depth. 
	/// </summary>
	private float FacingLength(Quaternion r)
	{
		return r.eulerAngles.y % 180 == 0 ? Dim.width : Dim.depth;
	}

	/// <summary>
	/// Returns the length of the given wall. 
	/// </summary>
	public float WallLength(Wall w)
	{
		switch (w)
		{
			case Wall.front:
			case Wall.back:
				return Dim.width;
			case Wall.right:
			case Wall.left:
			default:
				return Dim.depth;
		}
	}

	/// <summary>
	/// Returns the depth of the given wall.
	/// </summary>
	public float WallDepth(Wall w)
	{
		switch (w)
		{
			case Wall.front:
			case Wall.back:
				return Dim.depth / 2f;
			case Wall.right:
			case Wall.left:
			default:
				return Dim.width / 2f;
		}
	}

	/// <summary>
	/// Determines whether an object is facing to the side, and gives either front gable height or side gable height. 
	/// </summary>
	private float FacingGableHeight(Transform t)
	{
		return t.localRotation.eulerAngles.y % 180 == 0 ? Dim.frontGableHeight : Dim.sideGableHeight;
	}

	/// <summary>
	/// Determines whether an object is facing to the side, and gives front gable height or side gable height. 
	/// </summary>
	private float FacingGableHeight(Quaternion r)
	{
		return r.eulerAngles.y % 180 == 0 ? Dim.frontGableHeight : Dim.sideGableHeight;
	}

	/// <summary>
	/// Remakes the object with the given object.
	/// </summary>
	/// <param name="obj">The ref object to be remade.</param>
	/// <param name="remake">The object to be remade into</param>
	private void RemakeObject(ref GameObject obj, GameObject remake)
	{
		if (obj == null && transform.Find(remake.name) != null)
		{
			obj = transform.Find(remake.name).gameObject;
		}
		if (obj != null && obj != remake)
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}

		obj = remake;
	}

	#endregion

	#region HouseGeneration

	/// <summary>
	/// Generates the house from the given specifications. 
	/// </summary>
	public void GenerateHouse()
	{
		allObjs = new List<pb_Object>();
		initialized = true;

		// Things get weird when you use CSG code away from the origin. 
		Vector3 resetPosition = transform.position;
		Quaternion resetRotation = transform.rotation;
		transform.position = Vector3.zero;
		transform.eulerAngles = Vector3.zero;

		CreateFoundation();
		CreateCeilings();
		CreateExteriorWalls();
		CreateInnerWalls();
		CreateStaircases();
		CreateChimneys();
		CreateExtensions();

		// Assign the materials
		AssignEverything();

		// Reset our position. 
		transform.position = resetPosition;
		transform.rotation = resetRotation;
	}

	/// <summary>
	/// Generates a specific component of the house. 
	/// </summary>
	public void GenerateComponent(int i)
	{
		if (!initialized)
		{
			allObjs = new List<pb_Object>();
			initialized = true;
		}
		// Things get weird when you use CSG code away from the origin. 
		Vector3 resetPosition = transform.position;
		transform.position = Vector3.zero;

		ComponentGenerations[i]();
		RemakeExtensions();
		foreach (HouseExtension e in EX)
		{
			e.extent.GenerateComponent(i);
		}

		// Assign the materials
		AssignEverything();

		// Reset our position. 
		transform.position = resetPosition;
	}

	/// <summary>
	/// Clears the house of all objects. 
	/// </summary>
	public void ClearHouse()
	{
		foreach (GameObject g in HouseObjects)
		{
			if (g != null)
			{
				UnityEngine.Object.DestroyImmediate(g);
			}
		}
	}

	#region Foundation

	/// <summary>
	/// Creates the foundation.
	/// </summary>
	public void CreateFoundation()
	{
		foundation = new List<pb_Object>();
		bottomFloors = new List<pb_Object>();

		pb_Object f = CreateCube(new Vector3(Dim.width, AO.foundationHeight - NUDGE, Dim.depth), Vector3.zero,
								 transform, "Foundation", ref foundation);

		RemakeObject(ref foundationObj, f.gameObject);

		CreateCube(new Vector3(INNER_WIDTH + (AO.wallThickness * 2f), 0.01f, INNER_DEPTH + (AO.wallThickness * 2f)),
				   new Vector3(0f, (AO.foundationHeight / 2f) - 0.0049f, 0f),
				   f.transform, "Floor", ref bottomFloors);
	}

	#endregion

	#region Ceilings

	/// <summary>
	/// Creates the ceilings.
	/// </summary>
	public void CreateCeilings()
	{
		ceilings = new List<pb_Object>();
		beams = new List<pb_Object>();

		Transform ceil = GetHierarchyObject("Ceilings");

		RemakeObject(ref ceilingsObj, ceil.gameObject);

		for (int i = 1; i <= Dim.floors; ++i)
		{
			CreateCube(new Vector3(INNER_WIDTH + (AO.wallThickness * 2f) - NUDGE, AO.ceilingThickness, INNER_DEPTH + (AO.wallThickness * 2f) - NUDGE),
					   new Vector3(0f, (Dim.FloorHeight * i) + ((AO.foundationHeight - AO.ceilingThickness) / 2f) - NUDGE, 0f),
					   ceil, "Ceiling", ref ceilings);
		}

		// Create the ceiling beams

		if (ID.rafters)
		{
			CreateRafters();
		}
	}

	/// <summary>
	/// Creates the rafters.
	/// </summary>
	private void CreateRafters()
	{
		float distance = Dim.width > Dim.depth ? INNER_WIDTH : INNER_DEPTH;
		int numBeams = Mathf.FloorToInt(distance / 3) - 1;
		float beamDistance = distance / (numBeams + 1);

		foreach (pb_Object ceil in ceilings)
		{
			float currentDistance = beamDistance - (distance / 2f);

			float yOffsetM = -((AO.rafterThickness + AO.ceilingThickness) / 2f);

			// Create middle rafters
			for (int i = 0; i < numBeams; ++i)
			{
				if (Dim.width > Dim.depth)
				{
					CreateCube(new Vector3(AO.rafterWidth, AO.rafterThickness, INNER_DEPTH),
							   new Vector3(currentDistance, yOffsetM, 0f),
							   ceil.transform, "Beam", ref beams);
				}
				else
				{
					CreateCube(new Vector3(INNER_WIDTH, AO.rafterThickness, AO.rafterWidth),
							   new Vector3(0f, yOffsetM, currentDistance),
							   ceil.transform, "Beam", ref beams);
				}
				currentDistance += beamDistance;
			}

			float yOffsetC = -((AO.sideRafterThickness + AO.ceilingThickness) / 2f);

			// Create connecting rafters
			if (Dim.width > Dim.depth)
			{
				CreateCube(new Vector3(INNER_WIDTH, AO.sideRafterThickness, AO.sideRafterWidth),
						   new Vector3(0f, yOffsetC, (INNER_DEPTH - AO.sideRafterWidth) / 2f),
						   ceil.transform, "Beam", ref beams);
				CreateCube(new Vector3(INNER_WIDTH, AO.sideRafterThickness, AO.sideRafterWidth),
						   new Vector3(0f, yOffsetC, (INNER_DEPTH - AO.sideRafterWidth) / -2f),
						   ceil.transform, "Beam", ref beams);
			}
			else
			{
				CreateCube(new Vector3(AO.sideRafterWidth, AO.sideRafterThickness, INNER_DEPTH),
						   new Vector3((INNER_WIDTH - AO.sideRafterWidth) / 2f,
									   yOffsetC, 0f),
						   ceil.transform, "Beam", ref beams);
				CreateCube(new Vector3(AO.sideRafterWidth, AO.sideRafterThickness, INNER_DEPTH),
						   new Vector3((INNER_WIDTH - AO.sideRafterWidth) / -2f,
									   yOffsetC, 0f),
						   ceil.transform, "Beam", ref beams);
			}
		}
	}

	#endregion

	#region ExteriorWalls

	#region MainWalls

	/// <summary>
	/// Creates the walls of the house.
	/// </summary>
	public void CreateExteriorWalls()
	{
		exteriorWalls = new List<pb_Object>();
		roofs = new List<pb_Object>();
		outerTrim = new List<pb_Object>();
		outerFrames = new List<pb_Object>();
		outerWood = new List<pb_Object>();
		outerInnerFrames = new List<pb_Object>();
		exteriorSiding = new List<pb_Object>();
		awnings = new List<pb_Object>();
		gableWindowSprites = new List<SpriteRenderer>();
		shutterSprites = new List<SpriteRenderer>();

		// Create the four exterior walls

		Transform walls = GetHierarchyObject("Exterior Walls");

		RemakeObject(ref exteriorWallsObj, walls.gameObject);

		float yOffset = ((Dim.height + AO.foundationHeight) / 2.0f);

		CreateCube(new Vector3(Dim.width - (AO.foundationSeparation * 2), Dim.height, AO.wallThickness),
				   new Vector3(0, yOffset, (-Dim.depth / 2) + (AO.foundationSeparation + (AO.wallThickness / 2f))),
				   walls, "WallF", ref exteriorWalls);

		CreateCube(new Vector3(Dim.width - (AO.foundationSeparation * 2), Dim.height, AO.wallThickness),
				   new Vector3(0, yOffset, (Dim.depth / 2) - (AO.foundationSeparation + (AO.wallThickness / 2f))),
				   walls, "WallB", ref exteriorWalls, new Vector3(0, 180, 0));

		CreateCube(new Vector3(Dim.depth - ((AO.foundationSeparation + AO.wallThickness) * 2), Dim.height, AO.wallThickness),
				   new Vector3((Dim.width / 2) - (AO.foundationSeparation + (AO.wallThickness / 2f)), yOffset, 0),
				   walls, "WallR", ref exteriorWalls, new Vector3(0, 270, 0));

		CreateCube(new Vector3(Dim.depth - ((AO.foundationSeparation + AO.wallThickness) * 2), Dim.height, AO.wallThickness),
				   new Vector3((-Dim.width / 2) + (AO.foundationSeparation + (AO.wallThickness / 2f)), yOffset, 0),
				   walls, "WallL", ref exteriorWalls, new Vector3(0, 90, 0));

		CreateEdges(walls, yOffset);

		CreateGables();

		AddWallDetails();

		CreatePorches();
	}

	/// <summary>
	/// Creates the edges for the walls.
	/// </summary>
	/// <param name="parent">Parent.</param>
	/// <param name="yOffset">Y offset.</param>
	private void CreateEdges(Transform parent, float yOffset)
	{
		if (OD.wallEdges)
		{
			Transform edges = GetHierarchyObject("Edges", parent);

			float eWOffset = (Dim.width / 2) + AO.edgeDepth - (AO.foundationSeparation + (AO.edgeWidth / 2f));
			float eDOffset = (Dim.depth / 2) + AO.edgeDepth - (AO.foundationSeparation + (AO.edgeWidth / 2f));

			CreateCube(new Vector3(AO.edgeWidth, Dim.height, AO.edgeWidth),
					   new Vector3(eWOffset, yOffset, eDOffset), edges, "WallEdge", ref outerTrim);
			CreateCube(new Vector3(AO.edgeWidth, Dim.height, AO.edgeWidth),
					   new Vector3(-eWOffset, yOffset, eDOffset), edges, "WallEdge", ref outerTrim);
			CreateCube(new Vector3(AO.edgeWidth, Dim.height, AO.edgeWidth),
					   new Vector3(eWOffset, yOffset, -eDOffset), edges, "WallEdge", ref outerTrim);
			CreateCube(new Vector3(AO.edgeWidth, Dim.height, AO.edgeWidth),
					   new Vector3(-eWOffset, yOffset, -eDOffset), edges, "WallEdge", ref outerTrim);
		}

		if (OD.roofEdges)
		{
			foreach (pb_Object w in exteriorWalls)
			{
				CreateCube(new Vector3(FacingLength(w.transform) - (AO.foundationSeparation * 2f),
									   AO.edgeWidth, AO.edgeDepth),
						   new Vector3(0, (Dim.height - AO.edgeWidth) / 2f, -(AO.wallThickness + AO.edgeDepth) / 2f),
					   w.transform, "RoofEdge", ref outerTrim);
			}
		}

		if (OD.foundationEdges)
		{
			foreach (pb_Object w in exteriorWalls)
			{
				CreateCube(new Vector3(FacingLength(w.transform) - (AO.foundationSeparation * 2f),
									   AO.edgeWidth, AO.edgeDepth),
						   new Vector3(0, -(Dim.height - AO.edgeWidth) / 2f, -(AO.wallThickness + AO.edgeDepth) / 2f),
					   w.transform, "FoundationEdge", ref outerTrim);
			}
		}
	}

	#endregion

	#region Roofs

	/// <summary>
	/// Creates the main gables on the house. 
	/// </summary>
	private void CreateGables()
	{
		// 10 is front
		// 42 is back
		// 66 is middle

		int[] triangles = new int[3];
		triangles[0] = 10;
		triangles[1] = 66;
		triangles[2] = 42;

		Transform r = GetHierarchyObject("Roofs");

		RemakeObject(ref roofsObj, r.gameObject);

		// Add the front gable if height is greater than 0
		if (Dim.frontGableHeight > 0)
		{
			for (int i = 0; i < 2; ++i)
			{
				CreateGable(exteriorWalls[i], triangles, Dim.frontGableHeight);
			}

			transform.eulerAngles = new Vector3(0, 90, 0);
			CreateRoof(r, Dim.width, Dim.depth, 0f, Dim.frontGableHeight, "Left", "Right", false);
			transform.eulerAngles = Vector3.zero;
		}

		// Add the side gable if height is greater than 0

		if (Dim.sideGableHeight > 0)
		{
			for (int i = 2; i < 4; ++i)
			{
				CreateGable(exteriorWalls[i], triangles, Dim.sideGableHeight);
			}

			CreateRoof(r, Dim.depth, Dim.width, AO.wallThickness, Dim.sideGableHeight, "Front", "Back", true);
		}
	}

	/// <summary>
	/// Creates the roofs via the given parameters. 
	/// </summary>
	/// <param name="parent">The parent of the roof</param>
	/// <param name="rDepth">The depth of the roof</param>
	/// <param name="wTranslate">The width-wise translation of the roof.</param>
	/// <param name="gableHeight">The gable height.</param>
	/// <param name="r1Name">The name of roof1.</param>
	/// <param name="r2Name">The name of roof2.</param>
	/// <param name="rotate">Whether or not the roof is rotated. </param>
	private void CreateRoof(Transform parent, float rWidth, float rDepth, float wOffset,
							float gableHeight, string r1Name, string r2Name, bool rotate)
	{
		// The width of the cube used to construct the roofs. 
		float cubeWidth = 1f;

		float lengthOffset = (AO.foundationSeparation * 2f) - AO.roofOverhangFront;
		float distanceOffset = AO.foundationSeparation + (cubeWidth / 2) - AO.roofOverhangSide;
		float translateOffset = AO.foundationSeparation + cubeWidth - AO.roofOverhangSide;
		float wTranslate = (rWidth / 2) - (translateOffset + wOffset);
		float sideOffset = gableHeight * AO.roofOverhangSide / wTranslate;
		float yOffset = Dim.height + ((AO.foundationHeight + AO.roofHeight) / 2f) - sideOffset - 0.1f;

		float dist = (-rWidth / 2) + distanceOffset + wOffset;
		float wDist = rotate ? 0 : dist;
		float dDist = rotate ? dist : 0;

		// Corner thickness = translate * roofHeight / sqrt(gableHeight ^ 2 + translate ^ 2)
		float cornerThickness = AO.roofHeight * wTranslate /
										Mathf.Sqrt(Mathf.Pow(gableHeight, 2) +
												   Mathf.Pow(wTranslate, 2));
		float yCOffset = (AO.roofOverhangSide - wOffset) * gableHeight / wTranslate;
		float rCDistOffset1 = ((AO.roofCornerWidth - cubeWidth) / 2f) + AO.roofOverhangSide;
		float rCDistOffset2 = (AO.foundationSeparation * 2f) - (AO.roofOverhangFront / 2f);

		Vector3 rCDimensions = new Vector3(AO.roofCornerWidth, cornerThickness, AO.roofOverhangFront / 2f);
		float rCWDist = rCDistOffset1 - wOffset;
		float rCDDist = ((rDepth - rCDistOffset2) / 2f) - NUDGE;

		pb_Object r1 = CreateCube(new Vector3(cubeWidth, AO.roofHeight, rDepth - lengthOffset),
								  new Vector3(wDist, yOffset, dDist),
								  parent, "Roof" + r1Name, ref roofs, rotate ? new Vector3(0, -90, 0) : default(Vector3));

		pb_Object r2 = CreateCube(new Vector3(cubeWidth, AO.roofHeight, rDepth - lengthOffset),
								  new Vector3(-wDist, yOffset, -dDist),
								  parent, "Roof" + r2Name, ref roofs, rotate ? new Vector3(0, 90, 0) : new Vector3(0, 180, 0));

		List<pb_Object> rS = new List<pb_Object> { r1, r2 };

		foreach (pb_Object r in rS)
		{
			// rotate the uvs so that the material is shown correctly on roof
			r.faces[4].uv.rotation = 90;

			CreateCube(rCDimensions, new Vector3(rCWDist, yCOffset, rCDDist),
					   r.transform, "RoofCorner", ref outerTrim);

			CreateCube(rCDimensions, new Vector3(rCWDist, yCOffset, -rCDDist),
					   r.transform, "RoofCorner", ref outerTrim);

			int[] triangles = new int[3];

			r.faces[1].ToQuadOrTriangles(out triangles);

			r.TranslateVertices(triangles, new Vector3(wTranslate, gableHeight + sideOffset, 0f));

		}

		foreach (Dormer d in OD.dormers)
		{
			Vector3 rDimensions = new Vector3(rDepth - (AO.foundationSeparation * 2), gableHeight,
										  (rWidth / 2f) - AO.foundationSeparation);
			float zOffset = (cubeWidth / 2) - wOffset;
			CreateDormer(d, d.alternateRoof ? r2 : r1, rDimensions, zOffset, sideOffset, -wOffset);
		}
	}

	/// <summary>
	/// Creates a gable for the given pb_object (assumed to be a cube). 
	/// </summary>
	/// <param name="o">The object to be given a gable</param>
	/// <param name="triangles">The edge triangles to be used to create the gable.</param>
	/// <param name="gHeight">The height of the gable.</param>
	private void CreateGable(pb_Object o, int[] triangles, float gHeight, Action a = null)
	{
		o.Subdivide();
		o.TranslateVertices(triangles, new Vector3(0, gHeight, 0));

		if (a != null)
		{
			a();
		}

		pb_Face[] faces = new pb_Face[4];

		while (o.faces.Length > 6)
		{

			for (int i = 0; i < 4; ++i)
			{
				faces[i] = o.faces[i];
			}

			o.MergeFaces(faces);
		}
	}

	/// <summary>
	/// Creates the dormers for the given roof.
	/// </summary>
	/// <param name="d">The dormer reference.</param>
	/// <param name="roof">The roof object.</param>
	/// <param name="roofDimensions">The roof dimensions.</param>
	private void CreateDormer(Dormer d, pb_Object roof, Vector3 roofDimensions, float zOffset, float yOffset, float wOffset)
	{
		Vector3 dDimensions = new Vector3(AO.dormerDimensions.x,
										  AO.dormerDimensions.y,
										  AO.dormerDimensions.y * roofDimensions.z / roofDimensions.y);

		float gableLength = AO.dormerGableHeight * roofDimensions.z / roofDimensions.y;

		int[] triangles1 = new int[2] { 8, 9 }; // back bottom edge
		int[] triangles2 = new int[3] { 10, 66, 42 }; // gable edge
		int[] triangles3 = new int[1] { 42 }; // back triangle of gable edge
		int[] triangles4 = new int[16] { 74, 75, 66, 65, 11, 10, 1, 2, 91, 90, 82, 81, 43, 42, 34, 33 }; // mid edge loop of roof
		int[] triangles5 = new int[2] { 42, 43 }; // back middle edge of roof
		int[] triangles6 = new int[2] { 21, 37 }; // bottom corner triangles of roof
		int[] triangles7 = new int[2] { 22, 38 }; // mid-bottom corner triangles of roof

		pb_Object windowCutout = pb_ShapeGenerator.CubeGenerator(new Vector3(AO.windowDimensions.x, AO.windowDimensions.y, dDimensions.z / 4f));
		windowCutout.transform.SetParent(transform);

		foreach (float x in d.placements)
		{
			pb_Object dormer = CreateCube(dDimensions, new Vector3(((((roofDimensions.z - (dDimensions.z + gableLength)) * (d.placementY + 1f))
																	+ dDimensions.z) / 2.0f) + zOffset,
																	((((roofDimensions.y - (dDimensions.y + AO.dormerGableHeight)) * (d.placementY + 1f))
																	 + dDimensions.y + AO.roofHeight) / 2.0f) +
																	yOffset + (wOffset * roofDimensions.y / roofDimensions.z),
																	((roofDimensions.x - dDimensions.x) / 2.0f) * -x),
										  roof.transform, "Dormer", ref exteriorSiding, new Vector3(0f, -90f, 0f));

			dormer.TranslateVertices(triangles1, new Vector3(0, dDimensions.y - NUDGE, 0));

			Action a = delegate { dormer.TranslateVertices(triangles3, new Vector3(0, 0, -gableLength)); };
			CreateGable(dormer, triangles2, AO.dormerGableHeight, a);

			float dormerSideOffset = AO.dormerGableHeight * AO.dormerOverhangSide / (dDimensions.x / 2f);
			pb_Object dormerRoof = CreateCube(new Vector3(dDimensions.x + (AO.dormerOverhangSide * 2f),
														  AO.dormerRoofHeight, dDimensions.z + AO.dormerOverhangFront),
											  new Vector3(0f, (dDimensions.y / 2f) - dormerSideOffset, AO.dormerOverhangFront / 2f),
											  dormer.transform, "Dormer Roof", ref roofs);

			dormerRoof.faces[4].uv.rotation = 90;

			dormerRoof.Subdivide();
			dormerRoof.TranslateVertices(triangles4, new Vector3(0, dormerSideOffset + AO.dormerGableHeight, 0));
			dormerRoof.TranslateVertices(triangles5, new Vector3(0, 0, -gableLength));

			float roofInset = AO.dormerRoofHeight * roofDimensions.z / roofDimensions.y;

			dormerRoof.TranslateVertices(triangles6, new Vector3(0, 0, roofInset));
			dormerRoof.TranslateVertices(triangles7, new Vector3(0, 0, roofInset / 2f));


			pb_Face[] faces = new pb_Face[4];

			while (dormerRoof.faces.Length > 8)
			{

				for (int i = 0; i < 4; ++i)
				{
					faces[i] = dormerRoof.faces[i];
				}

				dormerRoof.MergeFaces(faces);
			}

			WindowType wT = !d.overrideWindowDetails ? OD.windowType : d.typeOverride;
			FrameType fT = !d.overrideWindowDetails ? OD.windowFrame : d.frameOverride;

			// Add dormer window
			switch (wT)
			{
				case WindowType.sixteenPaneBlack:
				case WindowType.twentyFourPaneBlack:
					CreateDormerWindowDetails(dormer.transform, dDimensions, wT, fT);
					break;
				case WindowType.sixteenPaneAlpha:
				case WindowType.twentyFourPaneAlpha:
					windowCutout.transform.rotation = Quaternion.identity;
					PBUtility.CutOutObject(dormer, new PBUtility.Cutout(windowCutout, new Vector3(0, AO.dormerWindowYOffset, (dDimensions.z * 3f / 8f) + 0.1f)));

					// Trying to cut through roof with CSG gives stack overflow error
					//windowCutout.transform.rotation = Quaternion.Euler(0, 90, 0);
					//PBUtility.CutOutObject(roof, new PBUtility.Cutout(windowCutout, dormer.transform.localPosition)); 

					int merged = 0;
					if (!AO.dormerAlignmentFix)
					{
						merged += PBUtility.MergeFacesOfSameSide(dormer, 0, PBUtility.VectorValue.y, true);
						merged += PBUtility.MergeFacesOfSameSide(dormer, dDimensions.x / 2f, PBUtility.VectorValue.x, false);
						merged += PBUtility.MergeFacesOfSameSide(dormer, -dDimensions.x / 2f, PBUtility.VectorValue.x, false);
						merged += PBUtility.MergeFacesOfSameSide(dormer, AO.windowDimensions.x / 2f, PBUtility.VectorValue.x, false);
						merged += PBUtility.MergeFacesOfSameSide(dormer, -AO.windowDimensions.x / 2f, PBUtility.VectorValue.x, false);
						merged += PBUtility.MergeFacesOfSameSide(dormer, dDimensions.z / 2f, PBUtility.VectorValue.z, false);
						dormer.faces[dormer.faceCount - 1].uv.offset = new Vector2(0f, AO.dormerAlignmentOffsetY);
					}
					else
					{
						PBUtility.MergeRemainingFaces(dormer, merged);
					}

					CreateDormerWindowDetails(dormer.transform, dDimensions, wT, fT);
					break;
				case WindowType.none:
				default:
					break;
			}

		}

		UnityEngine.Object.DestroyImmediate(windowCutout.gameObject);
	}

	/// <summary>
	/// Creates window details for the dormer window
	/// </summary>
	/// <param name="parent"></param>
	/// <param name="dDimensions"></param>
	/// <param name="wT"></param>
	private void CreateDormerWindowDetails(Transform parent, Vector3 dDimensions, WindowType wT, FrameType fT)
	{
		Transform w = GetHierarchyObject("Window", parent);
		w.transform.localPosition = new Vector3(0f, AO.dormerWindowYOffset, (dDimensions.z - AO.wallThickness) / 2f);
		w.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
		w.transform.localScale = new Vector3(AO.windowDimensions.x, AO.windowDimensions.y, 1f);


		GameObject window = GameObject.CreatePrimitive(PrimitiveType.Quad);
		window.name = "Panel";
		window.transform.parent = w;
		window.transform.localPosition = new Vector3(0, 0, (AO.wallThickness / -2f) - NUDGE);
		window.transform.localScale = Vector3.one;
		window.transform.localRotation = Quaternion.identity;
		window.GetComponent<MeshRenderer>().sharedMaterial = customizer.references.windowMaterials[(int)wT];

		CreateWindowFrame(fT, w);
	}

	#endregion

	#region WallDetails

	/// <summary>
	/// Adds wall details and cutouts to the walls. 
	/// </summary>
	private void AddWallDetails()
	{
		pb_Object doorCutout = pb_ShapeGenerator.CubeGenerator(AO.doorDimensions);
		doorCutout.transform.SetParent(transform);

		pb_Object windowCutout = pb_ShapeGenerator.CubeGenerator(AO.windowDimensions);
		windowCutout.transform.SetParent(transform);

		pb_Object gableWindowCutoutFull = pb_ShapeGenerator.ArchGenerator(360, AO.gableWindowDiameter,
																	AO.gableWindowDiameter, 1,
																	AO.gableWindowSides * 2,
																	false, true, true, true, true);
		gableWindowCutoutFull.transform.SetParent(transform);

		pb_Object gableWindowCutoutHalf = pb_ShapeGenerator.ArchGenerator(180, AO.gableWindowDiameter,
																	AO.gableWindowDiameter, 1,
																	AO.gableWindowSides,
																	false, true, true, true, true);
		gableWindowCutoutHalf.transform.SetParent(transform);

		pb_Object storeWindowCutout = pb_ShapeGenerator.CubeGenerator(AO.storeWindowDimensions);
		storeWindowCutout.transform.SetParent(transform);

		pb_Object storeWindowIndentCutout = pb_ShapeGenerator.CubeGenerator(new Vector3(AO.storeWindowDimensions.x,
			AO.storeWindowDimensions.y + AO.storeWindowHeight +
			(AO.frameWidth / 2f) + (AO.frameWidth / 4f), AO.indentThickness));
		storeWindowIndentCutout.transform.SetParent(transform);

		List<pb_Object> extensionCutouts = new List<pb_Object>();

		for (int i = 0; i < exteriorWalls.Count; ++i)
		{
			Transform wall = exteriorWalls[i].transform;

			Quaternion oldRotation = wall.rotation;
			wall.rotation = Quaternion.identity;
			float wallLength = FacingLength(oldRotation) - (oldRotation.eulerAngles.y % 180 == 0 ? 0 : AO.wallThickness * 2f);

			List<PBUtility.Cutout> cuts = new List<PBUtility.Cutout>();

			AddOuterDoors(i, doorCutout, cuts, wall, wallLength);
			AddWindows(i, windowCutout, cuts, wall, wallLength);
			AddGableWindows(i, gableWindowCutoutFull, gableWindowCutoutHalf, cuts, wall, wallLength, FacingGableHeight(oldRotation));
			AddStoreWindows(i, storeWindowCutout, storeWindowIndentCutout, cuts, wall, wallLength);
			AddExtensionCutouts(i, ref extensionCutouts, cuts, wall, wallLength);

			PBUtility.CutOutObjects(exteriorWalls[i], cuts);

			wall.rotation = oldRotation;

			int mergedFaces = 0;

			mergedFaces += PBUtility.MergeFacesOfSameSide(exteriorWalls[i], AO.wallThickness / -2f, PBUtility.VectorValue.z);
			mergedFaces += PBUtility.MergeFacesOfSameSide(exteriorWalls[i], AO.wallThickness / 2f, PBUtility.VectorValue.z);

			// merge edges of walls

			if (oldRotation.eulerAngles.y % 180 == 0)
			{
				mergedFaces += PBUtility.MergeFacesOfSameSide(exteriorWalls[i], (Dim.width / 2f) - AO.foundationSeparation, PBUtility.VectorValue.x);
				mergedFaces += PBUtility.MergeFacesOfSameSide(exteriorWalls[i], -((Dim.width / 2f) - AO.foundationSeparation), PBUtility.VectorValue.x);
			}
			else
			{
				mergedFaces += PBUtility.MergeFacesOfSameSide(exteriorWalls[i], (Dim.depth / 2f) -
															  AO.foundationSeparation - AO.wallThickness, PBUtility.VectorValue.x);
				mergedFaces += PBUtility.MergeFacesOfSameSide(exteriorWalls[i], -((Dim.depth / 2f) - AO.foundationSeparation
																				  - AO.wallThickness), PBUtility.VectorValue.x);
			}

			if (mergedFaces > 0)
			{
				PBUtility.MergeRemainingFaces(exteriorWalls[i], mergedFaces);
			}
			else
			{
				pb_Face temp = exteriorWalls[i].faces[0];
				exteriorWalls[i].faces[0] = exteriorWalls[i].faces[1];
				exteriorWalls[i].faces[1] = temp;
			}
		}

		UnityEngine.Object.DestroyImmediate(doorCutout.gameObject);
		UnityEngine.Object.DestroyImmediate(windowCutout.gameObject);
		UnityEngine.Object.DestroyImmediate(gableWindowCutoutFull.gameObject);
		UnityEngine.Object.DestroyImmediate(gableWindowCutoutHalf.gameObject);
		UnityEngine.Object.DestroyImmediate(storeWindowCutout.gameObject);
		UnityEngine.Object.DestroyImmediate(storeWindowIndentCutout.gameObject);
		foreach (pb_Object o in extensionCutouts)
		{
			UnityEngine.Object.DestroyImmediate(o.gameObject);
		}
	}

	/// <summary>
	/// Adds the doors to the given wall. 
	/// </summary>
	private void AddOuterDoors(int i, pb_Object cutout, List<PBUtility.Cutout> cuts, Transform wall, float wallLength)
	{
		float doorArea = wallLength - ((AO.foundationSeparation * 2f) +
									   AO.doorDimensions.x + AO.frameWidth);

		foreach (OuterDoor d in OD.outerDoors)
		{
			if ((int)d.wall == i)
			{
				cutout.transform.localPosition = new Vector3((doorArea / 2.0f) * d.placement,
																 (AO.doorDimensions.y / 2f) - (Dim.height / 2f), 0f);
				cuts.Add(new PBUtility.Cutout(cutout));

				CreateOuterDoorDetails(cutout.transform.localPosition, wall, d);

				// Cut hole in foundation edges 
				if (OD.foundationEdges)
				{
					PBUtility.CutOutObject(wall.Find("FoundationEdge").GetComponent<pb_Object>(),
										   new PBUtility.Cutout(cutout, new Vector3(cutout.transform.localPosition.x, 0f, 0f)));
				}
			}
		}
	}

	/// <summary>
	/// Adds the windows to the given wall. 
	/// </summary>
	private void AddWindows(int i, pb_Object cutout, List<PBUtility.Cutout> cuts, Transform wall, float wallLength)
	{
		foreach (Window w in OD.windows)
		{
			if ((int)w.wall == i)
			{
				WindowType wT = !w.overrideDetails ? OD.windowType : w.typeOverride;
				FrameType fT = !w.overrideDetails ? OD.windowFrame : w.frameOverride;

				float windowWidth = 0.1f;

				switch (fT)
				{
					case FrameType.outline:
					case FrameType.outlineWithInner:
						windowWidth += AO.frameWidth;
						break;
					case FrameType.topAndBottom:
					case FrameType.archAndBottom:
					case FrameType.none:
					default:
						break;
				}

				if ((!w.overrideDetails && OD.ShowShutter) ||
					w.ShowShutter)
				{
					switch (w.ShowShutter ? w.shutterOverride : OD.windowShutter)
					{
						case ShutterType.board:
						case ShutterType.panel:
						case ShutterType.slats:
							windowWidth += AO.shutterWidth * (AO.windowDimensions.x + 0.1f) * 2f;
							break;
						case ShutterType.none:
						default:
							break;
					}
				}

				float windowArea = wallLength - ((AO.foundationSeparation * 2f) + AO.windowDimensions.x + windowWidth);

				foreach (float placement in w.placements)
				{
					cutout.transform.localPosition =
									new Vector3((windowArea / 2.0f) * placement,
												AO.windowHeight + ((w.floor - 1) * Dim.FloorHeight) +
												((AO.windowDimensions.y + AO.frameWidth - Dim.height) / 2f), 0f);

					switch (wT)
					{
						case WindowType.sixteenPaneBlack:
						case WindowType.twentyFourPaneBlack:
							CreateWindowDetails(cutout.transform.localPosition, wall, w);
							break;
						case WindowType.sixteenPaneAlpha:
						case WindowType.twentyFourPaneAlpha:
							cuts.Add(new PBUtility.Cutout(cutout));
							CreateWindowDetails(cutout.transform.localPosition, wall, w);
							break;
						case WindowType.none:
						default:
							break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Adds the gable windows to the given wall. 
	/// </summary>
	private void AddGableWindows(int i, pb_Object cutoutFull, pb_Object cutoutHalf, List<PBUtility.Cutout> cuts, Transform wall, float wallLength, float height)
	{
		foreach (GableWindow w in OD.gableWindows)
		{
			if ((int)w.wall == i && height > 0)
			{
				GableWindowType wT = w.typeOverride == GableWindowType.none ? OD.gableWindowType : w.typeOverride;

				pb_Object cutout = null;

				switch (wT)
				{
					case GableWindowType.RoundAlpha:
					case GableWindowType.RoundBlack:
						cutout = cutoutFull;
						break;
					case GableWindowType.SemiCircleAlpha:
					case GableWindowType.SemiCircleBlack:
					case GableWindowType.none:
					default:
						cutout = cutoutHalf;
						break;
				}
				cutout.transform.SetParent(transform);

				cutout.transform.localPosition =
						  new Vector3((((wallLength - AO.gableWindowDiameter)
										/ 2.0f) - AO.foundationSeparation) * w.placementx,
									  ((Dim.height + height + (height * w.placementy)) / 2f)
									  - AO.gableWindowDiameter, -0.5f);

				switch (wT)
				{
					case GableWindowType.RoundBlack:
					case GableWindowType.SemiCircleBlack:
						CreateGableWindowDetails(cutout.transform.localPosition, wall, w);
						break;
					case GableWindowType.RoundAlpha:
					case GableWindowType.SemiCircleAlpha:
						cuts.Add(new PBUtility.Cutout(cutout));
						CreateGableWindowDetails(cutout.transform.localPosition, wall, w);
						break;
					case GableWindowType.none:
					default:
						break;
				}
			}
		}
	}

	/// <summary>
	/// Adds the store windows to the given wall. 
	/// </summary>
	private void AddStoreWindows(int i, pb_Object cutout, pb_Object indentCutout, List<PBUtility.Cutout> cuts, Transform wall, float wallLength)
	{
		foreach (StoreWindow w in OD.storeWindows)
		{
			if ((int)w.wall == i)
			{
				float windowWidth = 0.1f;

				float windowArea = wallLength - ((AO.foundationSeparation * 2f) + AO.storeWindowDimensions.x + windowWidth);

				foreach (float placement in w.placements)
				{
					cutout.transform.localPosition =
									new Vector3((windowArea / 2.0f) * placement,
												AO.storeWindowHeight +
												((AO.storeWindowDimensions.y + AO.frameWidth - Dim.height) / 2f), 0f);
					if (w.indent && canIndent)
					{
						indentCutout.transform.localPosition = cutout.transform.localPosition -
							new Vector3(0, (AO.storeWindowHeight + (AO.frameWidth / 2f) - (AO.frameWidth / 4f)) / 2f,
							((AO.wallThickness - AO.indentThickness) / 2f) + NUDGE);
						cuts.Add(new PBUtility.Cutout(indentCutout));
						cutout.transform.localPosition += new Vector3(0, 0, AO.indentThickness);
					}
					cuts.Add(new PBUtility.Cutout(cutout));
					CreateStoreWindowDetails(cutout.transform.localPosition, wall, w);
				}
			}
		}
	}

	/// <summary>
	/// Adds the cutouts for the extensions.
	/// </summary>
	protected abstract void AddExtensionCutouts(int i, ref List<pb_Object> cutouts, List<PBUtility.Cutout> cuts, Transform wall, float wallLength);

	/// <summary>
	/// Creates a window texture based on the current window type. 
	/// </summary>
	/// <param name="location">The location of the window.</param>
	/// <param name="wall">Which wall the window is on.</param>
	private void CreateWindowDetails(Vector3 location, Transform parent, Window wd)
	{
		WindowType wT = !wd.overrideDetails ? OD.windowType : wd.typeOverride;
		FrameType fT = !wd.overrideDetails ? OD.windowFrame : wd.frameOverride;
		ShutterType sT = !wd.overrideDetails ? OD.windowShutter : wd.shutterOverride;

		Transform w = GetHierarchyObject("Window", parent, location);
		w.localScale = AO.windowDimensions;

		GameObject front = GameObject.CreatePrimitive(PrimitiveType.Quad);
		front.name = "Front";
		front.transform.parent = w.transform;
		front.transform.localPosition = new Vector3(0f, 0f, -(AO.wallThickness / 2f) - NUDGE);
		front.transform.localScale = Vector3.one;
		front.transform.localRotation = Quaternion.identity;
		front.GetComponent<MeshRenderer>().sharedMaterial = customizer.references.windowMaterials[(int)wT];

		GameObject back = UnityEngine.Object.Instantiate(front, w, false);
		back.name = "Back";
		back.transform.localPosition = new Vector3(0f, 0f, (AO.wallThickness / 2f) + 0.01f);
		back.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

		CreateWindowFrame(fT, w);

		if ((!wd.overrideDetails && OD.ShowShutter) ||
			wd.ShowShutter)
		{
			switch (sT)
			{
				case ShutterType.board:
				case ShutterType.panel:
				case ShutterType.slats:
					float shutterOffsetX = ((1 + AO.shutterWidth) / 2f);
					float shutterOffsetZ = -(AO.wallThickness / 2f) - NUDGE;

					CreateSprite(new Vector2(AO.windowDimensions.x * AO.shutterWidth, AO.windowDimensions.y),
								 new Vector3(-shutterOffsetX, 0f, shutterOffsetZ),
								 w, "LeftShutter", ref shutterSprites, customizer.references.shutterSprites[(int)sT]).flipX = true;
					CreateSprite(new Vector2(AO.windowDimensions.x * AO.shutterWidth, AO.windowDimensions.y),
								 new Vector3(shutterOffsetX, 0f, shutterOffsetZ),
								 w, "RightShutter", ref shutterSprites, customizer.references.shutterSprites[(int)sT]);

					break;
				case ShutterType.none:
				default:
					break;
			}
		}

		CreateAwning(wd.awningType, w, wd.awningPlacement);

		switch (wd.balconetType)
		{
			case BalconetType.single:
				CreateBalconet(wd.balconetType, w, BALCONET_YOFFSET);
				break;
			case BalconetType.fancy:
				CreateBalconet(wd.balconetType, w, BALCONET_FANCY_YOFFSET);
				break;
			case BalconetType.small:
				CreateBalconet(wd.balconetType, w, BALCONET_YOFFSET,
					BALCONET_SMALL_PLACEMENTRANGE, wd.balconetPlacement);
				break;
			case BalconetType.none:
			default:
				break;
		}
	}

	/// <summary>
	/// Creates a gable window texture based on the current window type. 
	/// </summary>
	/// <param name="location">The location of the window.</param>
	/// <param name="wall">Which wall the window is on.</param>
	private void CreateGableWindowDetails(Vector3 location, Transform parent, GableWindow gw)
	{
		GableWindowType wT = !gw.overrideDetails ? OD.gableWindowType : gw.typeOverride;
		FrameType fT = !gw.overrideDetails ? OD.windowFrame : gw.frameOverride;

		Transform w;
		float scaleY = AO.gableWindowDiameter;
		location.z += 0.5f;

		switch (wT)
		{
			case GableWindowType.RoundAlpha:
			case GableWindowType.RoundBlack:
				scaleY *= 2f;
				break;
			case GableWindowType.SemiCircleAlpha:
			case GableWindowType.SemiCircleBlack:
			case GableWindowType.none:
			default:
				location.y += AO.gableWindowDiameter / 2f;
				break;
		}

		w = GetHierarchyObject("GableWindow", parent, location);

		switch (fT)
		{
			case FrameType.outline:
			case FrameType.outlineWithInner:
			case FrameType.topAndBottom:
			case FrameType.archAndBottom:
				pb_Object gableFrame;
				switch (wT)
				{
					case GableWindowType.RoundAlpha:
					case GableWindowType.RoundBlack:
						gableFrame =
							pb_ShapeGenerator.ArchGenerator(360, AO.gableWindowDiameter + (AO.frameWidth / 2f),
															AO.frameWidth, (AO.wallThickness / 2) + AO.frameDepth,
															AO.gableWindowSides * 2,
															true, true, true, true, true);
						gableFrame.transform.parent = w;
						gableFrame.transform.localRotation = Quaternion.identity;
						gableFrame.transform.localPosition = new Vector3(0, 0, -((AO.wallThickness / 2) + AO.frameDepth));
						break;
					case GableWindowType.SemiCircleAlpha:
					case GableWindowType.SemiCircleBlack:
					case GableWindowType.none:
					default:
						gableFrame =
							pb_ShapeGenerator.ArchGenerator(180, AO.gableWindowDiameter + (AO.frameWidth / 2f),
															AO.frameWidth, (AO.wallThickness / 2) + AO.frameDepth,
															AO.gableWindowSides,
															true, true, true, true, true);
						gableFrame.transform.parent = w;
						gableFrame.transform.localRotation = Quaternion.identity;
						gableFrame.transform.localPosition = new Vector3(0, -AO.gableWindowDiameter / 2f,
																		 -((AO.wallThickness / 2) + AO.frameDepth));

						CreateCube(new Vector3((AO.gableWindowDiameter * 2f) + AO.frameWidth, AO.frameWidth,
											   (AO.wallThickness / 2) + AO.frameDepth),
								   new Vector3(0, -AO.gableWindowDiameter / 2f,
											   -((AO.wallThickness / 2) + AO.frameDepth) / 2f),
								   w, "BottomFrame", ref outerFrames);
						break;
				}
				outerFrames.Add(gableFrame);
				allObjs.Add(gableFrame);
				break;
			case FrameType.none:
			default:
				break;
		}

		CreateSprite(new Vector2(AO.gableWindowDiameter * 2, scaleY) * 1.05f,
					 new Vector3(0f, 0f, -(AO.wallThickness / 2f) - NUDGE),
					 w.transform, "WindowSprite", ref gableWindowSprites, customizer.references.gableWindowSprites[(int)wT]);
	}

	/// <summary>
	/// Creates the store window details.
	/// </summary>
	/// <param name="location">The location of the window.</param>
	/// <param name="wall">Which wall the window is on.</param>
	private void CreateStoreWindowDetails(Vector3 location, Transform parent, StoreWindow wd)
	{
		Transform sw = GetHierarchyObject("StoreWindow", parent, location);
		sw.localScale = AO.storeWindowDimensions;

		Vector3 fDimensions = new Vector3(AO.storeWindowDimensions.x,
										  (AO.frameWidth / 2f) + NUDGE,
										  (AO.wallThickness / 2f) + (AO.frameDepth / 2f));
		Transform frame = GetHierarchyObject("Frame", sw, new Vector3(0f, 0f, fDimensions.z / -2f));
		float offsetY = AO.storeWindowDimensions.y + (AO.frameWidth / 2f);
		pb_Object bot = CreateCube(fDimensions, new Vector3(0f, offsetY / -2f, NUDGE), frame, "FrameBottom", ref outerFrames);
		int[] triangles = new int[2] { 8, 9 };
		bot.TranslateVertices(triangles, new Vector3(0, 0, AO.frameDepth / 4f));

		pb_Object top = UnityEngine.Object.Instantiate<pb_Object>(bot, frame);
		top.name = "FrameTop";
		top.transform.localPosition += new Vector3(0, AO.storeWindowDimensions.y, 0);
		triangles = new int[4] { 20, 21, 22, 23 };
		top.TranslateVertices(triangles, new Vector3(0, -AO.frameWidth, 0));
		outerFrames.Add(top);
		allObjs.Add(top);

		CreateCube(new Vector3(fDimensions.x, fDimensions.y / 2f, fDimensions.z + AO.frameDepth / 2f),
				   new Vector3(0, (fDimensions.y / 2f) + (fDimensions.y / 4f), -AO.frameDepth / 4f), top.transform, "TopOutcrop", ref outerFrames);

		CreateCube(new Vector3(AO.frameWidth, AO.storeWindowDimensions.y, fDimensions.z - (AO.frameDepth / 2f)),
				   new Vector3(-(AO.storeWindowDimensions.x - AO.frameWidth) / 2f, 0f, AO.frameDepth / 4f),
				   frame, "FrameLeft", ref outerFrames);

		CreateCube(new Vector3(AO.frameWidth, AO.storeWindowDimensions.y, fDimensions.z - (AO.frameDepth / 2f)),
				   new Vector3((AO.storeWindowDimensions.x - AO.frameWidth) / 2f, 0f, AO.frameDepth / 4f),
				   frame, "FrameRight", ref outerFrames);

		CreateCube(new Vector3(AO.frameWidth / 4f, AO.storeWindowDimensions.y, AO.frameWidth / 4f),
				   new Vector3(-(AO.storeWindowDimensions.x - AO.frameWidth) / 6f, 0f, AO.frameDepth / 4f),
				   frame, "FrameSlat", ref outerFrames);

		CreateCube(new Vector3(AO.frameWidth / 4f, AO.storeWindowDimensions.y, AO.frameWidth / 4f),
				   new Vector3((AO.storeWindowDimensions.x - AO.frameWidth) / 6f, 0f, AO.frameDepth / 4f),
				   frame, "FrameSlat", ref outerFrames);

		CreateCube(new Vector3(AO.storeWindowDimensions.x, AO.frameWidth / 4f, AO.frameWidth / 4f),
				   new Vector3(0f, (-(AO.storeWindowDimensions.y - AO.frameWidth) / 6f) - (AO.frameWidth / 2f), AO.frameDepth / 4f),
				   frame, "FrameSlat", ref outerFrames);

		CreateCube(new Vector3(AO.storeWindowDimensions.x, AO.frameWidth / 4f, AO.frameWidth / 4f),
				   new Vector3(0f, ((AO.storeWindowDimensions.y - AO.frameWidth) / 6f) - (AO.frameWidth / 2f), AO.frameDepth / 4f),
				   frame, "FrameSlat", ref outerFrames);

		CreateAwning(wd.awningType, sw, wd.awningPlacement, true);
	}

	/// <summary>
	/// Creates the outer door details.
	/// </summary>
	private void CreateOuterDoorDetails(Vector3 location, Transform parent, OuterDoor d)
	{
		Transform door = GetHierarchyObject("Door", parent, location);

		if (d.steps)
		{
			Vector3 stairDimensions = new Vector3(AO.doorDimensions.x + AO.frameWidth,
												  AO.foundationHeight, 1.5f * AO.foundationHeight);
			pb_Object stairs = pb_ShapeGenerator.StairGenerator(stairDimensions, Mathf.RoundToInt(AO.foundationHeight / 0.25f), true);
			stairs.gameObject.name = "Steps";
			stairs.transform.SetParent(door);
			stairs.transform.localPosition = new Vector3(-stairDimensions.x / 2f, -stairDimensions.y - (AO.doorDimensions.y / 2f),
														 -((AO.wallThickness / 2f) + stairDimensions.z + AO.foundationSeparation));
			outerWood.Add(stairs);
			allObjs.Add(stairs);
		}

		CreateFrame(new Vector3(AO.doorDimensions.x, AO.doorDimensions.y,
								(AO.wallThickness / 2) + AO.frameDepth),
					Vector3.zero, door, "Frame", false, true);

		CreateAwning(d.awningType, door, d.awningPlacement, true);
	}

	/// <summary>
	/// Creates a frame.
	/// </summary>
	/// <param name="fDimensions">The dimensions of the frame.</param>
	/// <param name="position">The position of the frame.</param>
	/// <param name="parent">The parent of the frame.</param>
	/// <param name="n">The name of the frame.</param>
	/// <param name="bottom">Whether or not the frame has a bottom.</param>
	private Transform CreateFrame(Vector3 fDimensions, Vector3 position, Transform parent, string n,
								  bool bottom, bool hasInner, bool isInner = false)
	{
		Transform frame = GetHierarchyObject(n, parent, position);

		List<pb_Object> frames = isInner ? innerFrames : outerFrames;

		CreateFrame(frame, bottom, fDimensions, ref frames, -2.0f, "Outer");

		if (hasInner)
		{
			CreateFrame(frame, bottom, fDimensions, ref outerInnerFrames, 2.0f, "Inner");
		}
		return frame;
	}

	/// <summary>
	/// Creates the frame.
	/// </summary>
	/// <param name="parent">The frame's parent</param>
	/// <param name="bottom">Whether or not it has a bottom</param>
	/// <param name="fDimensions">The dimensions of the frame.</param>
	/// <param name="frames">The object list this frame belongs to.</param>
	/// <param name="divider">The divider that determines this frame's depth.</param>
	/// <param name="n">The name of the frame.</param>
	private void CreateFrame(Transform parent, bool bottom, Vector3 fDimensions,
							 ref List<pb_Object> frames, float divider, string n)
	{
		CreateCube(new Vector3(fDimensions.x + AO.frameWidth, AO.frameWidth, fDimensions.z),
				   new Vector3(0f, fDimensions.y / 2f, fDimensions.z / divider), parent, "Top" + n, ref frames);
		CreateCube(new Vector3(AO.frameWidth, fDimensions.y, fDimensions.z - NUDGE),
				   new Vector3(fDimensions.x / 2f, 0f, fDimensions.z / divider), parent, "Right" + n, ref frames);
		CreateCube(new Vector3(AO.frameWidth, fDimensions.y, fDimensions.z - NUDGE),
				   new Vector3(fDimensions.x / -2f, 0f, fDimensions.z / divider), parent, "Left" + n, ref frames);
		if (bottom)
		{
			CreateCube(new Vector3(fDimensions.x + AO.frameWidth, AO.frameWidth, fDimensions.z),
					   new Vector3(0f, fDimensions.y / -2f, fDimensions.z / divider), parent, "Bottom" + n, ref frames);
		}
	}

	/// <summary>
	/// Creates a frame for the given window transform
	/// </summary>
	/// <param name="fT"></param>
	/// <param name="w"></param>
	private void CreateWindowFrame(FrameType fT, Transform w)
	{
		switch (fT)
		{
			case FrameType.outline:
			case FrameType.outlineWithInner:
				CreateFrame(new Vector3(AO.windowDimensions.x + 0.01f, AO.windowDimensions.y,
									(AO.wallThickness / 2) + AO.frameDepth),
							Vector3.zero, w, "Frame", true, fT == FrameType.outlineWithInner);
				break;
			case FrameType.topAndBottom:
			case FrameType.archAndBottom:
				Vector3 fDimensions = new Vector3(AO.windowDimensions.x, AO.frameWidth + NUDGE, (AO.wallThickness / 2f) + AO.frameDepth);
				Transform frame = GetHierarchyObject("Frame", w, new Vector3(0f, 0f, fDimensions.z / -2f));
				float offsetY = AO.windowDimensions.y + AO.frameWidth;
				pb_Object bot = CreateCube(fDimensions, new Vector3(0f, offsetY / -2f, 0f), frame, "FrameBottom", ref outerFrames);
				int[] triangles = new int[2] { 8, 9 };
				bot.TranslateVertices(triangles, new Vector3(0, 0, AO.frameDepth / 2f));

				if (fT == FrameType.topAndBottom)
				{
					pb_Object top = UnityEngine.Object.Instantiate<pb_Object>(bot, frame);
					top.name = "FrameTop";
					top.transform.localPosition += new Vector3(0, offsetY, 0);
					outerFrames.Add(top);
					allObjs.Add(top);

					triangles = new int[2] { 6, 7 };
					top.TranslateVertices(triangles, new Vector3(AO.topFrameAngle, 0, 0));
					triangles = new int[2] { 14, 15 };
					top.TranslateVertices(triangles, new Vector3(-AO.topFrameAngle, 0, 0));
				}
				else if (fT == FrameType.archAndBottom)
				{
					pb_Object top = pb_ShapeGenerator.ArchGenerator(60, AO.windowDimensions.x + (AO.topFrameAngle * 2f),
																	AO.frameWidth, fDimensions.z, 10,
																	true, true, true, true, true);
					top.transform.parent = frame;
					top.transform.localPosition = new Vector3(0, (offsetY / 2f) - (AO.windowDimensions.x + (AO.topFrameAngle * 2f))
															  + AO.frameWidth, -fDimensions.z / 2f);
					top.transform.localEulerAngles = new Vector3(0, 0, 60f);
					outerFrames.Add(top);
					allObjs.Add(top);
				}
				break;

			case FrameType.none:
			default:
				break;
		}
	}

	/// <summary>
	/// Creates an awning. 
	/// </summary>
	/// <param name="a">The awning type.</param>
	/// <param name="parent">parent of this awning</param>
	private void CreateAwning(AwningType a, Transform parent, float placement, bool useFake = false)
	{
		Transform w = parent;
		if (useFake)
		{
			w = GetHierarchyObject("FakeWindow", parent.parent, new Vector3(parent.transform.localPosition.x, AO.windowHeight +
																		   ((AO.windowDimensions.y + AO.frameWidth - Dim.height) / 2f),
																		   0f));
			w.localScale = AO.windowDimensions;
		}

		float placementRange = 0f;
		switch (a)
		{
			case AwningType.small:
				placementRange = AWNING_SMALL_PLACEMENTRANGE;
				break;
			case AwningType.large:
				placementRange = AWNING_LARGE_PLACEMENTRANGE;
				break;
			case AwningType.single:
				placement = 0f;
				break;
			case AwningType.none:
			default:
				break;
		}

		if (a != AwningType.none)
		{
			pb_Object awning = UnityEngine.Object.Instantiate(customizer.references.awnings[(int)a], w).GetComponent<pb_Object>();
			awning.transform.localPosition = new Vector3(placement * placementRange,
														 AWNING_YOFFSET + AO.awningYOffset,
														 (AWNING_ZOFFSET * AO.awningLength) - (AO.wallThickness / 2f));
			awning.transform.localScale = new Vector3(1f, 1f, AO.awningLength);
			awnings.Add(awning);
			allObjs.Add(awning);
		}

		if (useFake)
		{
			if (w.childCount > 0)
			{
				w.GetChild(0).parent = parent;
			}
			UnityEngine.Object.DestroyImmediate(w.gameObject);
		}
	}

	/// <summary>
	/// Creates an balconet. 
	/// </summary>
	/// <param name="b">The balconet type.</param>
	/// <param name="parent">parent of this balconet</param>
	/// <param name="yOffset">the y offset of this balconet</param>
	/// <param name="placementRange">the placement range of this balconet</param>
	/// <param name="placement">the placement of this balconet</param>
	private void CreateBalconet(BalconetType b, Transform parent, float yOffset, float placementRange = 0f, float placement = 0f)
	{
		pb_Object balconet = UnityEngine.Object.Instantiate(customizer.references.balconets[(int)b], parent).GetComponent<pb_Object>();
		balconet.transform.localPosition = new Vector3(placement * placementRange,
													   yOffset + AO.balconetYOffset,
													   (BALCONET_ZOFFSET * AO.windowDimensions.x) - (AO.wallThickness / 2f));
		balconet.transform.localScale = new Vector3(1f, 1f, Mathf.Min(AO.windowDimensions.x, AO.windowDimensions.y));
		outerTrim.Add(balconet);
		allObjs.Add(balconet);
	}

	#endregion

	#region Porches

	/// <summary>
	/// Creates the porches.
	/// </summary>
	private void CreatePorches()
	{
		porchRailings = new List<SpriteRenderer>();

		foreach (Porch p in OD.porches)
		{
			Transform parent = exteriorWalls[(int)p.wall].transform;
			float length = FacingLength(parent) - (2 * AO.foundationSeparation);
			float pLength = length * p.length;

			Quaternion oldRotation = parent.rotation;
			parent.rotation = Quaternion.identity;

			Transform porch = CreateCube(new Vector3(pLength, AO.foundationHeight - AO.porchWoodHeight, p.width),
										 new Vector3((length - pLength) * (p.placement / 2f),
													 -(Dim.height + AO.foundationHeight + AO.porchWoodHeight) / 2f,
													 -(((AO.wallThickness + p.width) / 2f) + AO.foundationSeparation)),
										 parent, "Porch", ref foundation).transform;

			CreateCube(new Vector3(pLength + (AO.porchWoodOverhang * 2f), AO.porchWoodHeight,
								   p.width + AO.porchWoodOverhang + AO.foundationSeparation),
					   new Vector3(0f, (AO.foundationHeight / 2f) + NUDGE,
								   (AO.foundationSeparation - AO.porchWoodOverhang) / 2f),
					   porch, "PorchWood", ref outerWood);

			// Create porch stairs
			Transform temp = GetHierarchyObject("Temp", porch);
			float middleStairPlacement = ((pLength - (p.middleStairWidth * pLength)) * p.middleStairPlacement) / 2f;

			if (p.middleStairs)
			{
				CreatePorchStairs(temp, porch, new Vector3(middleStairPlacement, 0f, -p.width / 2f),
								  p.middleStairWidth * pLength, "Middle", 0f);
			}
			if (p.leftStairs && !(p.LeftCornerEnabled && p.leftCorner))
			{
				CreatePorchStairs(temp, porch, new Vector3(-pLength / 2f, 0f, AO.porchStairGuardWidth / 2f),
								  p.width - AO.porchStairGuardWidth, "Left", 90f);
			}
			if (p.rightStairs && !(p.RightCornerEnabled && p.rightCorner))
			{
				CreatePorchStairs(temp, porch, new Vector3(pLength / 2f, 0f, AO.porchStairGuardWidth / 2f),
								  p.width - AO.porchStairGuardWidth, "Right", -90f);
			}

			UnityEngine.Object.DestroyImmediate(temp.gameObject);

			CreatePorchRoof(porch, p, pLength, middleStairPlacement);

			if (p.RightCornerEnabled && p.rightCorner)
			{
				CreatePorchCorner(porch, p, pLength, 1);
			}
			if (p.LeftCornerEnabled && p.leftCorner)
			{
				CreatePorchCorner(porch, p, pLength, -1);
			}

			parent.rotation = oldRotation;
		}
	}

	/// <summary>
	/// Creates the porch stairs.
	/// </summary>
	/// <param name="temp">Temp transform for orienting stairs.</param>
	/// <param name="porch">The porch transform.</param>
	/// <param name="position">The position of the stairs.</param>
	/// <param name="stairWidth">The stair width.</param>
	/// <param name="n">The name of the stairs.</param>
	/// <param name="rotation">The rotation of the stairs.</param>
	private void CreatePorchStairs(Transform temp, Transform porch, Vector3 position, float stairWidth, string n, float rotation)
	{
		temp.localPosition = position;
		Vector3 stairDimensions = new Vector3(stairWidth, AO.foundationHeight - AO.porchWoodHeight,
											  1.5f * AO.foundationHeight);

		int stairCount = Mathf.RoundToInt(AO.foundationHeight / 0.25f);
		pb_Object stairs = pb_ShapeGenerator.StairGenerator(stairDimensions, stairCount, true);
		float stepWidth = stairDimensions.z / stairCount;
		stairs.gameObject.name = n + "Steps";
		stairs.transform.SetParent(temp);
		stairs.transform.localPosition = new Vector3(-stairDimensions.x / 2f, -stairDimensions.y / 2f, -stairDimensions.z);

		for (int i = 0; i < stairCount; ++i)
		{
			CreateCube(new Vector3(stairWidth, AO.porchWoodHeight,
								   stepWidth + AO.porchWoodOverhang),
					   new Vector3(stairWidth / 2f, ((stairDimensions.y / stairCount) * (i + 1)) + (AO.porchWoodHeight / 2f) - NUDGE,
								   (stepWidth * (i + 0.5f)) - (AO.porchWoodOverhang / 2f)),
								   stairs.transform, "StepWood", ref outerWood);
		}

		if (rotation <= 0)
		{
			CreatePorchStairGuard(stairDimensions, stairCount, temp, stairs.transform, true);
		}

		if (rotation >= 0)
		{
			CreatePorchStairGuard(stairDimensions, stairCount, temp, stairs.transform, false);
		}

		temp.transform.localEulerAngles = new Vector3(0f, rotation, 0f);
		stairs.transform.SetParent(porch);
		temp.transform.localRotation = Quaternion.identity;

		foundation.Add(stairs);
		allObjs.Add(stairs);
	}

	/// <summary>
	/// Creates the stair guard.
	/// </summary>
	/// <param name="stairDimensions">Stair dimensions.</param>
	/// <param name="stairCount">Stair count.</param>
	/// <param name="temp">The temp reference transform.</param>
	/// <param name="stairs">Stairs.</param>
	/// <param name="left">Whether it's the left or the right stair guard</param>
	private void CreatePorchStairGuard(Vector3 stairDimensions, int stairCount, Transform temp, Transform stairs, bool left)
	{
		float stepWidth = stairDimensions.z / stairCount;
		Vector3 guardDimensions = new Vector3(AO.porchStairGuardWidth, stairDimensions.y, (stepWidth + AO.porchWoodOverhang) * 2f);

		int[] triangles = new int[9] { 41, 42, 25, 38, 34, 22, 37, 33, 21 };

		pb_Object guard = CreateCube(guardDimensions, new Vector3((left ? -1 : 1) * (guardDimensions.x + stairDimensions.x) / 2f,
																			 0f, -guardDimensions.z / 2f),
									 temp, (left ? "Left" : "Right") + "StairGuard", ref foundation);

		pb_Object guardWood = CreateCube(new Vector3(guardDimensions.x + AO.porchWoodOverhang,
													 AO.porchWoodHeight, guardDimensions.z),
										 new Vector3((left ? -1 : 1) * (AO.porchWoodOverhang / 2f),
													 (guardDimensions.y + AO.porchWoodHeight) / 2f, 0),
										 guard.transform, "StairGuardWood", ref outerWood);

		guard.transform.parent = stairs;

		guard.Subdivide();
		guardWood.Subdivide();
		guard.TranslateVertices(triangles, new Vector3(0f, 0f, -stairDimensions.z + guardDimensions.z));
		guardWood.TranslateVertices(triangles, new Vector3(0f, -stairDimensions.y + (stairDimensions.y / stairCount),
														   -stairDimensions.z + guardDimensions.z));

		triangles = new int[3] { 38, 34, 22 };

		guard.TranslateVertices(triangles, new Vector3(0f, (-stairDimensions.y / 2f) + (stairDimensions.y / stairCount), 0f));

		triangles = new int[3] { 41, 42, 25 };

		guard.TranslateVertices(triangles, new Vector3(0f, -stairDimensions.y + (stairDimensions.y / stairCount), 0f));
	}

	/// <summary>
	/// Creates the porch roof.
	/// </summary>
	/// <param name="porch">Porch transform.</param>
	/// <param name="p">Porch object.</param>
	/// <param name="pLength">Porch length.</param>
	/// <param name="middleStairPlacement">Middle stair placement.</param>
	private void CreatePorchRoof(Transform porch, Porch p, float pLength, float middleStairPlacement)
	{
		// Create porch columns
		Transform columns = GetHierarchyObject("Columns", porch, new Vector3(-PORCH_COLUMN_RADIUS, PORCH_COLUMN_OFFSETY,
																			 -(p.width / 2f) + AO.porchColumnZInset));

		Transform railings = GetHierarchyObject("Railings", porch, new Vector3(0f, (AO.foundationHeight + AO.porchRailingHeight) / 2f,
																			   -(p.width / 2f) + AO.porchColumnZInset - PORCH_COLUMN_RADIUS));
		Vector3 adjust = new Vector3(0f, 0, 0);
		float distAdjust = 0f;
		float cornerDist = p.width + AO.porchColumnXInset - AO.porchColumnZInset + PORCH_COLUMN_RADIUS + AO.foundationSeparation;
		if (p.RightCornerEnabled && p.rightCorner)
		{
			adjust.x += cornerDist / 2f;
			distAdjust += cornerDist;
		}
		if (p.LeftCornerEnabled && p.leftCorner)
		{
			adjust.x -= cornerDist / 2f;
			distAdjust += cornerDist;
		}
		columns.localPosition += adjust;
		railings.localPosition += adjust;
		middleStairPlacement -= adjust.x;

		float distance = pLength - (AO.porchColumnXInset * 2f) + distAdjust;
		int numColumns = Mathf.FloorToInt(distance / AO.porchColumnFrequency) + 1;
		float columnDistance = distance / (numColumns - 1);

		bool stopped = false;

		for (int i = 0; i < numColumns; ++i)
		{
			if (!(p.middleStairs && middleStairPlacement > ((i - 0.3f) * columnDistance) - (distance / 2f)
				  && middleStairPlacement < ((i + 0.3f) * columnDistance) - (distance / 2f)))
			{
				pb_Object column = UnityEngine.Object.Instantiate(customizer.references.porchColumn, columns).GetComponent<pb_Object>();
				column.transform.localPosition = new Vector3((i * columnDistance) - (distance / 2f), 0f, 0f);
				outerTrim.Add(column);
				allObjs.Add(column);
				if (!stopped && i != 0 && !(p.middleStairs && middleStairPlacement > ((i - 1) * columnDistance) - (distance / 2f)
											&& middleStairPlacement < (i * columnDistance) - (distance / 2f)))
				{
					// create railing
					switch (OD.porchRailingType)
					{
						case PorchRailingType.railing1:
						case PorchRailingType.railing2:
							CreateSprite(new Vector2(columnDistance - (PORCH_COLUMN_RADIUS * 2f), AO.porchRailingHeight),
										 new Vector3(((i - 0.5f) * columnDistance) - (distance / 2f), 0f, 0f),
										 railings, "Railing", ref porchRailings, customizer.references.railingSprites[(int)OD.porchRailingType]);
							break;
						case PorchRailingType.none:
						default:
							if (railings)
							{
								UnityEngine.Object.DestroyImmediate(railings.gameObject);
							};
							break;
					}
				}
				else
				{
					stopped = false;
				}
			}
			else
			{
				stopped = true;
			}
		}

		pb_Object pRoof = CreateCube(new Vector3(pLength, AO.porchRoofHeight, p.width + AO.foundationSeparation),
									 new Vector3(0, ((AO.porchRoofHeight + AO.foundationHeight) / 2f) + PORCH_COLUMN_HEIGHT, AO.foundationSeparation / 2f),
									porch, "PorchRoof", ref roofs);
		CreateCube(new Vector3(pLength, AO.porchRoofHeight, AO.porchColumnZInset),
				   new Vector3(0, 0, -((p.width + AO.foundationSeparation) - (AO.porchColumnZInset)) / 2f),
				   pRoof.transform, "PorchRoofEdge", ref outerTrim);

		int[] triangles = pRoof.faces[0].indices;
		pRoof.TranslateVertices(triangles, new Vector3(0, AO.porchRoofAngle, 0));
	}

	/// <summary>
	/// Creates the porch corner.
	/// </summary>
	/// <param name="porch">Porch transform.</param>
	/// <param name="p">Porch object.</param>
	/// <param name="pLength">Porch length.</param>
	/// <param name="inverse">Inverse.</param>
	private void CreatePorchCorner(Transform porch, Porch p, float pLength, int inverse)
	{
		float cWidth = p.width + AO.foundationSeparation;

		Transform pCorner = CreateCube(new Vector3(cWidth, AO.foundationHeight - AO.porchWoodHeight, cWidth),
									   new Vector3((pLength + cWidth) / 2f * inverse, 0f, AO.foundationSeparation / 2f),
											   porch, "PorchCorner", ref foundation).transform;
		CreateCube(new Vector3(cWidth, AO.porchWoodHeight, cWidth),
				   new Vector3(AO.porchWoodOverhang * inverse,
							   (AO.foundationHeight / 2f) + NUDGE,
							   -AO.porchWoodOverhang),
				   pCorner, "PorchWood", ref outerWood);

		Transform railings = GetHierarchyObject("Railing", pCorner,
												new Vector3(((cWidth / 2f) - AO.porchColumnZInset + PORCH_COLUMN_RADIUS) * inverse,
															(AO.foundationHeight + AO.porchRailingHeight) / 2f,
															(AO.porchColumnZInset + AO.porchColumnXInset - PORCH_COLUMN_RADIUS) / 2f));
		switch (OD.porchRailingType)
		{
			case PorchRailingType.railing1:
			case PorchRailingType.railing2:
				CreateSprite(new Vector2(cWidth + AO.porchColumnXInset - AO.porchColumnZInset -
										 PORCH_COLUMN_RADIUS, AO.porchRailingHeight),
							 Vector3.zero, railings, "Railing", ref porchRailings,
							 customizer.references.railingSprites[(int)OD.porchRailingType], new Vector3(0f, 90f, 0f));
				break;
			case PorchRailingType.none:
			default:
				if (railings)
				{
					UnityEngine.Object.DestroyImmediate(railings.gameObject);
				};
				break;
		}

		Vector3 roofDimensions = new Vector3(cWidth, AO.porchRoofHeight, cWidth);
		Vector3 roofPosition = new Vector3(0, ((AO.porchRoofHeight + AO.foundationHeight) / 2f) +
										   PORCH_COLUMN_HEIGHT, 0f);

		pb_Object pRoof1 = CreateCube(roofDimensions, roofPosition, pCorner, "PorchRoof", ref roofs);
		pb_Object pRoof2 = CreateCube(roofDimensions, roofPosition, pCorner, "PorchRoof", ref roofs, new Vector3(0, -90f * inverse, 0));


		Vector3 pEdgeDimensions = new Vector3(cWidth, AO.porchRoofHeight, AO.porchColumnZInset);
		Vector3 pEdgePosition = new Vector3(0, 0, -((cWidth) - (AO.porchColumnZInset)) / 2f);

		CreateCube(pEdgeDimensions, pEdgePosition, pRoof1.transform, "PorchRoofEdge", ref outerTrim);
		CreateCube(pEdgeDimensions, pEdgePosition, pRoof2.transform, "PorchRoofEdge", ref outerTrim);

		int[] triangles = new int[4] { 0, 1, 2, 3 };
		pRoof1.TranslateVertices(triangles, new Vector3(0, AO.porchRoofAngle, 0));
		pRoof2.TranslateVertices(triangles, new Vector3(0, AO.porchRoofAngle, 0));
		triangles = inverse == 1 ? new int[2] { 1, 3 } : new int[2] { 0, 2 };
		pRoof1.TranslateVertices(triangles, new Vector3(-(cWidth - NUDGE) * inverse, 0, 0));
		triangles = inverse == 1 ? new int[2] { 0, 2 } : new int[2] { 1, 3 };
		pRoof2.TranslateVertices(triangles, new Vector3((cWidth - NUDGE) * inverse, 0, 0));
	}

	#endregion

	#endregion

	#region InteriorWalls

	/// <summary>
	/// Creates the inner walls.
	/// </summary>
	public void CreateInnerWalls()
	{
		interiorWalls = new List<pb_Object>();
		innerFrames = new List<pb_Object>();

		Transform t = GetHierarchyObject("Inner Walls");

		RemakeObject(ref innerWallsObj, t.gameObject);

		foreach (InnerWall w in ID.innerWalls)
		{
			pb_Object wall;
			pb_Object doorCutout = pb_ShapeGenerator.CubeGenerator(AO.doorDimensions);

			float wHeight = Dim.FloorHeight - AO.ceilingThickness;
			float yOffset = ((Dim.FloorHeight * (w.floor - 1)) +
							 ((AO.foundationHeight + Dim.FloorHeight -
							   AO.ceilingThickness) / 2.0f));

			if (!w.rotate)
			{
				wall = CreateCube(new Vector3(INNER_WIDTH * w.width, wHeight, AO.wallThickness),
								  new Vector3((INNER_WIDTH - (INNER_WIDTH * w.width)) * w.placementX / 2f, yOffset,
											  (INNER_DEPTH + AO.wallThickness - NUDGE) * w.placementY / 2f),
								  t, "InnerWall", ref interiorWalls);
			}
			else
			{
				wall = CreateCube(new Vector3(AO.wallThickness, wHeight, INNER_DEPTH * w.width),
								  new Vector3((INNER_WIDTH + AO.wallThickness - NUDGE) * w.placementX / 2f, yOffset,
											  (INNER_DEPTH - (INNER_DEPTH * w.width)) * w.placementY / 2f),
								  t, "InnerWall", ref interiorWalls);
			}

			List<PBUtility.Cutout> cuts = new List<PBUtility.Cutout>();

			foreach (float p in w.doorPlacements)
			{
				if (!w.rotate)
				{
					doorCutout.transform.position =
						new Vector3(((INNER_WIDTH * w.width) - AO.doorDimensions.x) * p / 2f,
									(Dim.FloorHeight - AO.ceilingThickness - AO.doorDimensions.y) / -2f,
									0f);
				}
				else
				{
					doorCutout.transform.eulerAngles = new Vector3(0, 90f, 0);
					doorCutout.transform.position =
								  new Vector3(0f, (Dim.FloorHeight - AO.ceilingThickness - AO.doorDimensions.y) / -2f,
											  ((INNER_DEPTH * w.width) - AO.doorDimensions.x) * p / 2f);
				}

				cuts.Add(new PBUtility.Cutout(doorCutout));
				CreateFrame(new Vector3(AO.doorDimensions.x, AO.doorDimensions.y,
								(AO.wallThickness / 2) + AO.frameDepth),
						doorCutout.transform.localPosition, wall.transform, "Frame", false, true, true)
				.eulerAngles = new Vector3(0, w.rotate ? 90f : 0f, 0);
			}

			PBUtility.CutOutObjects(wall, cuts);

			PBUtility.MergeFacesOfSameSide(wall, AO.wallThickness / -2f, PBUtility.VectorValue.z);
			PBUtility.MergeFacesOfSameSide(wall, AO.wallThickness / 2f, PBUtility.VectorValue.z);
			PBUtility.MergeFacesOfSameSide(wall, AO.wallThickness / -2f, PBUtility.VectorValue.x);
			PBUtility.MergeFacesOfSameSide(wall, AO.wallThickness / 2f, PBUtility.VectorValue.x);

			UnityEngine.Object.DestroyImmediate(doorCutout.gameObject);
		}
	}

	#endregion

	#region Staircases

	/// <summary>
	/// Creates the staircases for the house.
	/// </summary>
	public void CreateStaircases()
	{
		stairCases = new List<pb_Object>();
		balusters = new List<pb_Object>();
		stairRailings = new List<pb_Object>();

		Transform t = GetHierarchyObject("Staircases");

		RemakeObject(ref stairCasesObj, t.gameObject);

		foreach (StairCase s in ID.innerStairs)
		{
			float yOffset = ((Dim.FloorHeight * (s.floor - 1)) +
							 ((AO.foundationHeight) / 2.0f));
			Vector3 stairDimensions = new Vector3(AO.maxStairWidth * s.width,
												  Dim.FloorHeight, AO.maxStairLength * s.length);

			int numStairs = Mathf.RoundToInt(stairDimensions.y / 0.25f);

			// Use a temp transform to center the stairs for rotation (stair mesh's center is bottom left corner)
			Transform temp = GetHierarchyObject("temp", t,
												new Vector3(((INNER_WIDTH - (s.rotation % 2 == 0 ?
												stairDimensions.x : stairDimensions.z)) * s.placementX) / 2f, yOffset,
															((INNER_DEPTH - (s.rotation % 2 == 0 ?
												stairDimensions.z : stairDimensions.x)) * s.placementY) / 2f));

			pb_Object stairs = pb_ShapeGenerator.StairGenerator(stairDimensions, numStairs, true);
			stairs.gameObject.name = "StairCase";
			stairs.transform.SetParent(temp);
			stairs.transform.localPosition = new Vector3(-stairDimensions.x / 2f, 0f,
														 -stairDimensions.z / 2f);
			temp.localRotation = Quaternion.Euler(new Vector3(0f, 90f * s.rotation, 0f));
			stairs.transform.SetParent(t);

			stairCases.Add(stairs);
			allObjs.Add(stairs);

			// Add railings

			switch (ID.stairRailingType)
			{
				case StairRailingType.railing1:
					if (!s.disableLeftRailing)
					{
						AddStairRailing(numStairs, stairDimensions, stairs.transform, true);
					}
					if (!s.disableRightRailing)
					{
						AddStairRailing(numStairs, stairDimensions, stairs.transform, false);
					}
					if (!s.disableLeftGuardRailing)
					{
						AddCutoutRailing(numStairs, stairDimensions, stairs.transform, s, true);
					}
					if (!s.disableRightGuardRailing)
					{
						AddCutoutRailing(numStairs, stairDimensions, stairs.transform, s, false);
					}
					if (!s.disableFrontGuardRailing)
					{
						AddFrontCutoutRailing(numStairs, stairDimensions, stairs.transform, s);
					}
					break;
				case StairRailingType.none:
				default:
					break;

			}

			// Cut out the hole in the ceiling

			stairDimensions.z *= s.cutoutLength;

			pb_Object cutout = pb_ShapeGenerator.CubeGenerator(stairDimensions * (1 + NUDGE));
			cutout.transform.parent = temp;
			cutout.transform.position = new Vector3(temp.position.x, 0f, temp.position.z);
			cutout.transform.localPosition = cutout.transform.localPosition +
				new Vector3(0, 0, (-stairDimensions.z + (stairDimensions.z / s.cutoutLength)) / 2f);
			cutout.transform.localRotation = Quaternion.identity;

			pb_Object ceil = ceilings[s.floor - 1];

			PBUtility.CutOutObject(ceil, new PBUtility.Cutout(cutout));

			PBUtility.MergeFacesOfSameSide(ceil, AO.ceilingThickness / 2f, PBUtility.VectorValue.y);
			PBUtility.MergeFacesOfSameSide(ceil, -AO.ceilingThickness / 2f, PBUtility.VectorValue.y);

			pb_Face tempFace = ceil.faces[4];
			ceil.faces[4] = ceil.faces[ceil.faceCount - 2];
			ceil.faces[ceil.faceCount - 2] = tempFace;

			// Destroy rafters intersecting staircase cutout. 

			if (ID.rafters)
			{
				cutout.transform.position += ceil.transform.position;
				Bounds cutBound = cutout.GetComponent<MeshRenderer>().bounds;

				for (int i = 0; i < ceil.transform.childCount - 2; i++)
				{
					if (cutBound.Intersects(ceil.transform.GetChild(i).GetComponent<MeshRenderer>().bounds))
					{
						UnityEngine.Object.DestroyImmediate(ceil.transform.GetChild(i).gameObject);
						i--;
					}
				}
			}

			UnityEngine.Object.DestroyImmediate(cutout.gameObject);
			UnityEngine.Object.DestroyImmediate(temp.gameObject);
		}
	}

	/// <summary>
	/// Adds the railings to the given staircase.
	/// </summary>
	/// <param name="numStairs">The number of stairs.</param>
	/// <param name="stairDimensions">The stair dimensions.</param>
	/// <param name="stairs">The stair transform.</param>
	/// <param name="left">Whether or not this is the left or right railing</param>
	private void AddStairRailing(int numStairs, Vector3 stairDimensions, Transform stairs, bool left)
	{

		Transform railing = GetHierarchyObject(left ? "Left Railing" : "Right Railing", stairs);
		Transform bals = GetHierarchyObject("Balusters", railing);

		float inset = left ? AO.railingWidth / 2f : stairDimensions.x - (AO.railingWidth / 2f);

		for (int i = 0; i < numStairs; ++i)
		{
			CreateBaluster(bals, new Vector3(inset, (stairDimensions.y * (i + 1) / numStairs),
											 (stairDimensions.z * (i + 0.5f) / numStairs) - BALUSTER_OFFSET));
		}

		CreateBaluster(bals, new Vector3(inset, stairDimensions.y,
										 (stairDimensions.z * (numStairs + 0.5f) / numStairs) - BALUSTER_OFFSET));



		CreateCube(new Vector3(AO.railingWidth, AO.railingThickness, stairDimensions.z / numStairs),
				   new Vector3(inset, (stairDimensions.y / numStairs) + (AO.railingThickness / 2f) + BALUSTER_HEIGHT, 0f),
				   railing, "Railing Bottom", ref stairRailings);

		CreateCube(new Vector3(AO.railingWidth, AO.railingThickness, stairDimensions.z / numStairs),
				   new Vector3(inset, stairDimensions.y + (AO.railingThickness / 2f) + BALUSTER_HEIGHT, stairDimensions.z),
				   railing, "Railing Top", ref stairRailings);

		pb_Object rail = CreateCube(new Vector3(AO.railingWidth, AO.railingThickness,
												(stairDimensions.z / numStairs) * (numStairs - 1)),
									new Vector3(inset, (stairDimensions.y / numStairs) +
												(AO.railingThickness / 2f) + BALUSTER_HEIGHT,
												(stairDimensions.z / 2f)),
				  railing, "Railing Middle", ref stairRailings);

		int[] triangles = new int[3];

		rail.faces[0].ToQuadOrTriangles(out triangles);
		rail.TranslateVertices(triangles, new Vector3(0f, stairDimensions.y - (stairDimensions.y / numStairs), 0f));
	}

	/// <summary>
	/// Adds the cutout railings to the given staircase.
	/// </summary>
	/// <param name="numStairs">The number of stairs.</param>
	/// <param name="stairDimensions">The stair dimensions.</param>
	/// <param name="stairs">The stair transform.</param>
	/// <param name="s">The staircase</param>
	/// <param name="left">Whether or not this is the left or right railing</param>
	private void AddCutoutRailing(int numStairs, Vector3 stairDimensions, Transform stairs, StairCase s, bool left)
	{
		float inset = left ? -AO.railingWidth / 2f : stairDimensions.x + (AO.railingWidth / 2f);
		float stairWidth = stairDimensions.z / numStairs;
		float numBals = Mathf.RoundToInt(((stairDimensions.z * s.cutoutLength) + stairWidth) / 0.4f);
		Transform railing = GetHierarchyObject("Left Guard Railing", stairs.transform);
		Transform bals = GetHierarchyObject("Balusters", railing, new Vector3(inset, stairDimensions.y,
																			  (stairDimensions.z * (numStairs + 0.5f) / numStairs) - BALUSTER_OFFSET));
		for (int i = 0; i < numBals; ++i)
		{
			CreateBaluster(bals, new Vector3(0f, 0f, ((-stairDimensions.z * s.cutoutLength) -
													  (s.disableFrontGuardRailing ? 0f : AO.railingWidth)) * i /
													 (s.disableFrontGuardRailing ? numBals - 1 : numBals)));
		}

		CreateCube(new Vector3(AO.railingWidth, AO.railingThickness, (stairDimensions.z * s.cutoutLength) + (stairWidth / 2f)),
				   new Vector3(inset, stairDimensions.y + (AO.railingThickness / 2f) + BALUSTER_HEIGHT,
							   stairDimensions.z - (((stairDimensions.z * s.cutoutLength) - (stairWidth / 2f)) / 2f)),
			   railing, "Railing", ref stairRailings);
	}

	/// <summary>
	/// Adds the front cutout railings to the given staircase.
	/// </summary>
	/// <param name="numStairs">The number of stairs.</param>
	/// <param name="stairDimensions">The stair dimensions.</param>
	/// <param name="stairs">The stair transform.</param>
	/// <param name="s">The staircase</param>
	private void AddFrontCutoutRailing(int numStairs, Vector3 stairDimensions, Transform stairs, StairCase s)
	{
		Transform railing = GetHierarchyObject("Front Guard Railing", stairs);
		float railWidth = stairDimensions.x + (s.disableLeftGuardRailing ? 0 : AO.railingWidth)
										 + (s.disableRightGuardRailing ? 0 : AO.railingWidth);
		float railXOffset = (stairDimensions.x - (s.disableLeftGuardRailing ? 0 : AO.railingWidth)
							 + (s.disableRightGuardRailing ? 0 : AO.railingWidth)) / 2f;
		float railZOffset = stairDimensions.z - (stairDimensions.z * s.cutoutLength) - (AO.railingWidth / 2f);

		CreateCube(new Vector3(railWidth, AO.railingThickness, AO.railingWidth),
				   new Vector3(railXOffset, stairDimensions.y + (AO.railingThickness / 2f) + BALUSTER_HEIGHT, railZOffset),
				   railing, "Railing", ref stairRailings);

		float stairWidth = stairDimensions.z / numStairs;
		float numBals = Mathf.RoundToInt(railWidth / 0.4f);
		Transform bals = GetHierarchyObject("Balusters", railing,
											new Vector3(s.disableLeftGuardRailing ? AO.railingWidth / 2f : -AO.railingWidth / 2f,
														stairDimensions.y, railZOffset));
		for (int i = 0; i < numBals; ++i)
		{
			CreateBaluster(bals, new Vector3((stairDimensions.x + (s.disableLeftGuardRailing ? -AO.railingWidth : 0f)
											 + (s.disableRightGuardRailing ? 0f : AO.railingWidth)) * i / (numBals - 1), 0f, 0f));
		}
	}

	/// <summary>
	/// Creates an individual baluster. 
	/// </summary>
	private void CreateBaluster(Transform bals, Vector3 position)
	{
		GameObject bal = UnityEngine.Object.Instantiate(customizer.references.baluster, Vector3.zero, Quaternion.identity, bals);
		bal.transform.localPosition = position;

		pb_Object o = bal.GetComponentInChildren<pb_Object>();
		balusters.Add(o);
		allObjs.Add(o);
	}

	#endregion

	#region Chimneys

	/// <summary>
	/// Creates the chimneys.
	/// </summary>
	public void CreateChimneys()
	{
		chimneyObjs = new List<pb_Object>();

		Transform ch = GetHierarchyObject("Chimneys");

		RemakeObject(ref chimneysObj, ch.gameObject);

		List<pb_Object> holes = new List<pb_Object>();

		foreach (Chimney c in OD.chimneys)
		{
			pb_Object chimBase = CreateCube(new Vector3(c.width, c.height + AO.foundationHeight, c.depth),
										new Vector3((((Dim.width + c.width) / 2f) - AO.foundationSeparation) * c.placementX,
													(c.height / 2f) + NUDGE,
										(((Dim.depth + c.depth) / 2f) - AO.foundationSeparation) * c.placementY),
										ch, "Chimney", ref chimneyObjs);

			pb_Object top = CreateCube(new Vector3(c.width + AO.chimneyTopWidth, AO.chimneyTopHeight,
												   c.depth + AO.chimneyTopWidth),
									   new Vector3(0f, ((c.height + AO.chimneyTopHeight + AO.foundationHeight) / 2f), 0f),
									   chimBase.transform, "ChimneyTop", ref chimneyObjs);

			CreateCube(new Vector3(c.width, NUDGE, c.depth),
					   new Vector3(0f, (AO.chimneyTopHeight / 2f) + NUDGE, 0f),
					   top.transform, "ChimneyHole", ref holes);

			pb_Object extrude = CreateCube(new Vector3((c.width - NUDGE) + c.leftExtrude + c.rightExtrude,
													   (c.extrudeHeight * c.height) + AO.foundationHeight,
													   (c.depth - NUDGE) + c.frontExtrude + c.backExtrude),
										   new Vector3((c.rightExtrude - c.leftExtrude) / 2f,
													   (((c.extrudeHeight * c.height) - c.height) / 2f),
													   (c.backExtrude - c.frontExtrude) / 2f),
										   chimBase.transform, "ChimneyExtrude", ref chimneyObjs);

			extrude.Subdivide();

			// front edge
			int[] triangles = new int[3] { 25, 41, 42 };

			extrude.TranslateVertices(triangles, new Vector3(0, 0f, c.frontExtrude));

			// right edge
			triangles[1] = 26;
			triangles[2] = 29;

			extrude.TranslateVertices(triangles, new Vector3(-c.rightExtrude, 0f, 0f));

			// back edge
			triangles[0] = 12;
			triangles[1] = 13;

			extrude.TranslateVertices(triangles, new Vector3(0f, 0f, -c.backExtrude));

			// left edge
			triangles[0] = 41;
			triangles[2] = 58;

			extrude.TranslateVertices(triangles, new Vector3(c.leftExtrude, 0f, 0f));

			// All vertices to be raised/lowered
			triangles = new int[8] { 22, 34, 38, 6, 23, 2, 3, 50 };

			extrude.TranslateVertices(triangles, new Vector3(0f, (c.extrudeAngle - 0.5f) * c.extrudeHeight * c.height, 0f));
		}

		AssignMaterialToPbObjects(customizer.references.blackMat, holes);
	}

	#endregion

	#region Extensions

	/// <summary>
	/// Creates the extensions.
	/// </summary>
	protected abstract void CreateExtensions();

	/// <summary>
	/// Remakes the extensions for component generation.
	/// </summary>
	protected abstract void RemakeExtensions();

	/// <summary>
	/// Sets up main extensions object.
	/// </summary>
	protected void SetUpExtensions()
	{
		Transform extensions = GetHierarchyObject("Extensions");
		RemakeObject(ref extensionsObj, extensions.gameObject);
	}

	/// <summary>
	/// Sets up extensions to generate components.
	/// </summary>
	protected void SetUpExtension(HouseExtension e)
	{
		e.extent.customizer = customizer;
		float pos = (WallLength(e.wall) + e.extent.WallLength(e.wall) - (AO.foundationSeparation * 4f)) * e.position / 2f;
		float depth = WallDepth(e.wall) + e.extent.WallDepth(e.wall) - (AO.foundationSeparation * 2f);
		Vector3 position = Vector3.zero;
		switch (e.wall)
		{
			case Wall.front:
				position.x += pos;
				position.z -= (depth - AO.wallThickness + NUDGE);
				break;
			case Wall.back:
				position.x += pos;
				position.z += (depth - AO.wallThickness + NUDGE);
				break;
			case Wall.right:
				position.z += pos;
				position.x += depth;
				break;
			case Wall.left:
				position.z += pos;
				position.x -= depth;
				break;
			default:
				break;
		}
		e.extent.exTransform = GetHierarchyObject("Extension", extensionsObj.transform, position);
		e.extent.exWall = e.wall;
		e.extent.exPosition = e.position;
		e.extent.AO = AO;
	}

	#endregion

	#endregion

	#region MaterialsFunctions

	/// <summary>
	/// Assigns materials to all of the available pb_Objects. 
	/// </summary>
	private void AssignMaterials()
	{
		AssignMaterialToPbObjects(Tex.outerTextures.foundationMaterial, foundation);
		AssignMaterialToPbObjects(Tex.innerTextures.floorMaterial, bottomFloors);
		AssignMaterialToPbObjects(Tex.outerTextures.sidingMaterial, exteriorWalls, 1, Tex.innerTextures.innerWallMaterial);
		AssignMaterialToPbObjects(Tex.innerTextures.innerWallMaterial, interiorWalls);
		AssignMaterialToPbObjects(Tex.innerTextures.ceilingMaterial, ceilings, 4, Tex.innerTextures.floorMaterial);
		AssignMaterialToPbObjects(Tex.outerTextures.outerTrimMaterial, roofs, 4, Tex.outerTextures.roofMaterial);
		AssignMaterialToPbObjects(Tex.innerTextures.innerWoodMaterial, beams);
		AssignMaterialToPbObjects(Tex.innerTextures.innerStairsMaterial, stairCases);
		AssignMaterialToPbObjects(Tex.innerTextures.balusterMaterial, balusters);
		AssignMaterialToPbObjects(Tex.innerTextures.railingMaterial, stairRailings);
		AssignMaterialToPbObjects(Tex.outerTextures.outerTrimMaterial, outerFrames);
		AssignMaterialToPbObjects(Tex.innerTextures.innerTrimMaterial, outerInnerFrames);
		AssignMaterialToPbObjects(Tex.innerTextures.innerTrimMaterial, innerFrames);
		AssignMaterialToPbObjects(Tex.outerTextures.outerWood, outerWood);
		AssignMaterialToPbObjects(Tex.outerTextures.chimneyMaterial, chimneyObjs);
		AssignMaterialToPbObjects(Tex.outerTextures.outerTrimMaterial, outerTrim);
		AssignMaterialToPbObjects(Tex.outerTextures.sidingMaterial, exteriorSiding);
		AssignMaterialToPbObjects(Tex.outerTextures.awningMaterial, awnings);

		currentInner = Tex.innerTextures;
		currentOuter = Tex.outerTextures;
	}

	/// <summary>
	/// Assigns colors to all of the available sprites. 
	/// </summary>
	private void AssignColors()
	{
		AssignColorToSprites(Tex.colors.porchRailingColor, porchRailings);
		AssignColorToSprites(Tex.colors.gableWindowColor, gableWindowSprites);
		AssignColorToSprites(Tex.colors.shutterColor, shutterSprites);

		currentColors = Tex.colors;
	}

	/// <summary>
	/// Assigns tilings to all of the available pb_Objects. 
	/// </summary>
	private void AssignTilings()
	{
		AssignTilingToPbObjects(foundation, Tex.tilings.foundationTiling);
		AssignTilingToPbObjects(bottomFloors, Tex.tilings.floorTiling);
		AssignTilingToPbObjects(exteriorWalls, Tex.tilings.sidingTiling, 1, Tex.tilings.innerWallTiling);
		AssignTilingToPbObjects(interiorWalls, Tex.tilings.innerWallTiling);
		AssignTilingToPbObjects(ceilings, Tex.tilings.ceilingTiling, 4, Tex.tilings.floorTiling);
		AssignTilingToPbObjects(roofs, Tex.tilings.outerTrimTiling, 4, Tex.tilings.roofTiling);
		AssignTilingToPbObjects(beams, Tex.tilings.innerWoodTiling);
		AssignTilingToPbObjects(stairCases, Tex.tilings.innerStairsTiling);
		AssignTilingToPbObjects(balusters, Tex.tilings.balusterTiling);
		AssignTilingToPbObjects(stairRailings, Tex.tilings.railingTiling);
		AssignTilingToPbObjects(outerFrames, Tex.tilings.outerTrimTiling);
		AssignTilingToPbObjects(outerInnerFrames, Tex.tilings.innerTrimTiling);
		AssignTilingToPbObjects(innerFrames, Tex.tilings.innerTrimTiling);
		AssignTilingToPbObjects(outerWood, Tex.tilings.outerWood);
		AssignTilingToPbObjects(chimneyObjs, Tex.tilings.chimneyTiling);
		AssignTilingToPbObjects(outerTrim, Tex.tilings.outerTrimTiling);
		AssignTilingToPbObjects(exteriorSiding, Tex.tilings.sidingTiling);
		AssignTilingToPbObjects(awnings, Tex.tilings.awningTiling);

		currentTilings = Tex.tilings;
	}

	/// <summary>
	/// Assigns the material to pb objects.
	/// </summary>
	private void AssignMaterialToPbObjects(Material m, List<pb_Object> objects, int secondFace = -1, Material m2 = null)
	{
		foreach (pb_Object o in objects)
		{
			if (o != null)
			{
				o.SetFaceMaterial(o.faces, m);
				if (secondFace >= 0)
				{
					pb_Face[] single = new pb_Face[1];
					single[0] = o.faces[secondFace];
					o.SetFaceMaterial(single, m2);
				}
			}

		}
	}

	/// <summary>
	/// Assigns the color to the sprites.
	/// </summary>
	private void AssignColorToSprites(Color c, List<SpriteRenderer> rends)
	{
		foreach (SpriteRenderer r in rends)
		{
			if (r != null)
			{
				r.color = c;
			}
		}
	}

	/// <summary>
	/// Assigns the tiling to pb objects.
	/// </summary>
	private void AssignTilingToPbObjects(List<pb_Object> objects, float currentTiling, int secondFace = -1, float currentTiling2 = -1)
	{
		foreach (pb_Object o in objects)
		{
			for (int i = 0; i < o.faces.Length; ++i)
			{
				if (i == secondFace)
				{
					PBUtility.SetUVTiling(o.faces[i], Mathf.Pow(100 / (currentTiling2 + NUDGE + 100), 10));
				}
				else
				{
					PBUtility.SetUVTiling(o.faces[i], Mathf.Pow(100 / (currentTiling + NUDGE + 100), 10));
				}
			}
		}
	}

	/// <summary>
	/// Applies changes to materials. 
	/// </summary>
	public void ApplyMaterialChanges()
	{
		if (!currentInner.Equals(Tex.innerTextures) || !currentOuter.Equals(Tex.outerTextures))
		{
			AssignMaterials();
			RefreshObjects();
			foreach (HouseExtension e in EX)
			{
				e.extent.Tex = Tex;
				e.extent.ApplyMaterialChanges();
			}
		}
	}

	/// <summary>
	/// Applies changes to colors. 
	/// </summary>
	public void ApplyColorChanges()
	{
		if (!currentColors.Equals(Tex.colors))
		{
			AssignColors();
			foreach (HouseExtension e in EX)
			{
				e.extent.Tex = Tex;
				e.extent.ApplyColorChanges();
			}
		}
	}

	/// <summary>
	/// Applies changes to tilings. 
	/// </summary>
	public void ApplyTilingChanges()
	{
		if (!currentTilings.Equals(Tex.tilings))
		{
			AssignTilings();
			RefreshObjects();
			foreach (HouseExtension e in EX)
			{
				e.extent.Tex = Tex;
				e.extent.ApplyTilingChanges();
			}
		}
	}

	/// <summary>
	/// Refreshs all of the objects.
	/// </summary>
	private void RefreshObjects()
	{
		foreach (pb_Object o in allObjs)
		{
			if (o != null)
			{
				o.ToMesh();
				o.Refresh();
			}
		}
	}

	/// <summary>
	/// Assigns everything related to house textures. 
	/// </summary>
	public void AssignEverything()
	{
		AssignMaterials();
		AssignColors();
		AssignTilings();
		RefreshObjects();
		foreach (HouseExtension e in EX)
		{
			e.extent.Tex = Tex;
			e.extent.AssignEverything();
		}
	}
	#endregion

	#endregion
}
