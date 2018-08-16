using System.Collections;
using System.Collections.Generic;
using Parabox.CSG;
using ProBuilder.Core;
using ProBuilder.MeshOperations;
using UnityEngine;

/// <summary>
/// Class for ProBuilder utility functions
/// </summary>
public static class PBUtility
{

	/// <summary>
	/// A Vector value.
	/// </summary>
	public enum VectorValue
	{
		x,
		y,
		z
	}

	/// <summary>
	/// Struct representing a single cutout of a pb_Object. 
	/// </summary>
	public struct Cutout
	{
		public pb_Object obj;
		public Vector3 location;

		public Cutout(pb_Object obj, Vector3 location)
		{
			this.obj = obj;
			this.location = location;
		}

		public Cutout(pb_Object obj)
		{
			this.obj = obj;
			this.location = obj.transform.position;
		}
	}

	/// <summary>
	/// Cuts out the cutout from the given object at the given position (relative to the object).
	/// </summary>
	public static void CutOutObject(pb_Object o, Cutout c)
	{
		o.ToMesh();
		o.Refresh();

		Vector3 oldPostion = o.transform.position;
		Quaternion oldRotation = o.transform.rotation;
		o.transform.position = Vector3.zero;
		o.transform.rotation = Quaternion.identity;

		GameObject tempObject = new GameObject();
		MeshFilter mf = tempObject.AddComponent<MeshFilter>();
		mf.mesh = new CSG_Model(o.gameObject).ToMesh();

		c.obj.transform.position = c.location;
		mf.mesh = CSG.Subtract(mf.gameObject, c.obj.gameObject);

		pb_MeshImporter mI = new pb_MeshImporter(o);
		mI.Import(tempObject, pb_MeshImporter.Settings.Default);
		o.transform.position = oldPostion;
		o.transform.rotation = oldRotation;
		Object.DestroyImmediate(tempObject);
	}

	/// <summary>
	/// Cuts out the cutouts from the given object at the given position (relative to the object).
	/// </summary>
	public static void CutOutObjects(pb_Object o, List<Cutout> cuts)
	{
		o.ToMesh();
		o.Refresh();

		Vector3 oldPostion = o.transform.position;
		Quaternion oldRotation = o.transform.rotation;
		o.transform.position = Vector3.zero;
		o.transform.rotation = Quaternion.identity;

		GameObject tempObject = new GameObject();
		MeshFilter mf = tempObject.AddComponent<MeshFilter>();
		mf.mesh = new CSG_Model(o.gameObject).ToMesh();

		foreach (Cutout c in cuts)
		{
			c.obj.transform.position = c.location;
			mf.mesh = CSG.Subtract(mf.gameObject, c.obj.gameObject);
		}

		pb_MeshImporter mI = new pb_MeshImporter(o);
		mI.Import(tempObject, pb_MeshImporter.Settings.Default);
		o.transform.position = oldPostion;
		o.transform.rotation = oldRotation;
		Object.DestroyImmediate(tempObject);
	}

	/// <summary>
	/// Merges faces of the same side.
	/// </summary>
	/// <param name="o">pb_object to merge faces</param>
	/// <param name="side">Coordinate distance to side being used.</param>
	/// <param name="v">Vector value of the coordinate that indicates side</param>
	/// <param name="useRotation">Whether or not to merge all sides with same vector value v</param>
	/// <returns></returns>
	public static int MergeFacesOfSameSide(pb_Object o, float side, VectorValue v, bool useRotation = false)
	{
		List<pb_Face> mergeableFaces = new List<pb_Face>();

		foreach (pb_Face f in o.faces)
		{
			bool sameSide = true;
			if (useRotation)
			{
				switch (v)
				{
					case VectorValue.z:
						side = o.vertices[f.indices[0]].z;
						break;
					case VectorValue.y:
						side = o.vertices[f.indices[0]].y;
						break;
					case VectorValue.x:
					default:
						side = o.vertices[f.indices[0]].x;
						break;

				}
			}
			float delta = 0.01f;
			foreach (int index in f.indices)
			{
				switch (v)
				{
					case VectorValue.z:
						sameSide = sameSide && Mathf.Abs(o.vertices[index].z - side) < delta;
						break;
					case VectorValue.y:
						sameSide = sameSide && Mathf.Abs(o.vertices[index].y - side) < delta;
						break;
					case VectorValue.x:
					default:
						sameSide = sameSide && Mathf.Abs(o.vertices[index].x - side) < delta;
						break;

				}
			}

			if (sameSide)
			{
				mergeableFaces.Add(f);
			}
		}

		if (mergeableFaces.Count > 1)
		{
			pb_Face[] faces = mergeableFaces.ToArray();
			o.MergeFaces(faces);
			return 1;
		}

		return 0;
	}

	/// <summary>
	/// Merges the remaining faces of the given pb_object.
	/// </summary>
	public static void MergeRemainingFaces(pb_Object o, int mergedFaces)
	{
		List<pb_Face> mergeableFaces = new List<pb_Face>();

		for (int index = 0; index < o.faceCount - mergedFaces; ++index)
		{
			mergeableFaces.Add(o.faces[index]);
		}

		pb_Face[] faces = mergeableFaces.ToArray();
		o.MergeFaces(faces);
	}

	/// <summary>
	/// Sets the UV tiling for the given face.
	/// </summary>
	/// <param name="f">F.</param>
	/// <param name="tiling">Tiling.</param>
	public static void SetUVTiling(pb_Face f, float tiling)
	{
		f.manualUV = false;
		f.uv.scale = Vector2.one * tiling;
	}
}
