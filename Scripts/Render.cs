/*
 * Render.cs
 * ---------
 * Made by Lucas Jeong
 * Contains rendering related class.
 */

using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// cube mesh rendering class using UnityEngine
/// </summary>
public class CubeMesh
{
	public  GameObject   Obj       { get; private set; }
	private MeshFilter   MFilter   { get; set; }
	private MeshRenderer MRenderer { get; set; }
	private Vector3[]    Vertices  { get; set; }
	private Vector2[]    UVs       { get; set; }
	private int[]        Triangles { get; set; }

	/// <summary>
	/// main constructor of CubeMesh
	/// </summary>
	/// <param name="name">cube mesh GameObject name</param>
	/// <param name="x">width</param>
	/// <param name="y">height</param>
	/// <param name="z">depth</param>
	/// <param name="unit">default unit of size</param>
	/// <param name="inverse">flipping faces option</param>
	/// <param name="materialPath">material data path in Resources/</param>
	/// <param name="bTexture">using texture option</param>
	/// <param name="texture">Texture2D object</param>
	public CubeMesh(string name,         int  x,        int       y, int z, float unit, bool inverse,
	                string materialPath, bool bTexture, Texture2D texture)
	{
		Obj       = new GameObject(name);
		MFilter   = Obj.AddComponent<MeshFilter>();
		MRenderer = Obj.AddComponent<MeshRenderer>();

		InitializeVertices(x, y, z, unit);
		InitializeTriangles(inverse);
		InitializeUVs();

		if (bTexture)
		{
			CreateMesh(materialPath, texture);
		}
		else
		{
			CreateMesh(materialPath);
		}
	}

	private void InitializeVertices(int x, int y, int z, float unit)
	{
		Vertices = new[]
		{
			new Vector3(-x / 2f, y / 2f, -z / 2f) * unit,
			new Vector3(x  / 2f, y / 2f, -z / 2f) * unit,
			new Vector3(x  / 2f, y / 2f, z  / 2f) * unit,
			new Vector3(-x / 2f, y / 2f, z  / 2f) * unit,

			new Vector3(-x / 2f, y  / 2f, z / 2f) * unit,
			new Vector3(x  / 2f, y  / 2f, z / 2f) * unit,
			new Vector3(x  / 2f, -y / 2f, z / 2f) * unit,
			new Vector3(-x / 2f, -y / 2f, z / 2f) * unit,

			new Vector3(-x / 2f, -y / 2f, z  / 2f) * unit,
			new Vector3(x  / 2f, -y / 2f, z  / 2f) * unit,
			new Vector3(x  / 2f, -y / 2f, -z / 2f) * unit,
			new Vector3(-x / 2f, -y / 2f, -z / 2f) * unit,

			new Vector3(-x / 2f, -y / 2f, -z / 2f) * unit,
			new Vector3(x  / 2f, -y / 2f, -z / 2f) * unit,
			new Vector3(x  / 2f, y  / 2f, -z / 2f) * unit,
			new Vector3(-x / 2f, y  / 2f, -z / 2f) * unit,

			new Vector3(x / 2f, y  / 2f, z  / 2f) * unit,
			new Vector3(x / 2f, y  / 2f, -z / 2f) * unit,
			new Vector3(x / 2f, -y / 2f, -z / 2f) * unit,
			new Vector3(x / 2f, -y / 2f, z  / 2f) * unit,

			new Vector3(-x / 2f, y  / 2f, -z / 2f) * unit,
			new Vector3(-x / 2f, y  / 2f, z  / 2f) * unit,
			new Vector3(-x / 2f, -y / 2f, z  / 2f) * unit,
			new Vector3(-x / 2f, -y / 2f, -z / 2f) * unit
		};
	}

	private void InitializeTriangles(bool inverse)
	{
		if (inverse)
		{
			Triangles = new[]
			{
				0, 1, 2,
				0, 2, 3,
				4, 5, 6,
				4, 6, 7,
				8, 9, 10,
				8, 10, 11,
				12, 13, 14,
				12, 14, 15,
				16, 17, 18,
				16, 18, 19,
				20, 21, 22,
				20, 22, 23
			};
		}

		else
		{
			Triangles = new[]
			{
				0, 2, 1,
				0, 3, 2,
				4, 6, 5,
				4, 7, 6,
				8, 10, 9,
				8, 11, 10,
				12, 14, 13,
				12, 15, 14,
				16, 18, 17,
				16, 19, 18,
				20, 22, 21,
				20, 23, 22
			};
		}
	}

	private void InitializeUVs()
	{
		UVs = new[]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f),

			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f),

			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f),

			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f),

			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f),

			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f)
		};
	}

	private void CreateMesh(string materialPath, Texture2D texture)
	{
		MFilter.mesh = new Mesh()
		{
			vertices  = Vertices,
			triangles = Triangles,
			uv        = UVs
		};

		MRenderer.material = new Material(Resources.Load<Material>(materialPath))
		{
			mainTexture = texture
		};

		MFilter.mesh.RecalculateNormals();
	}

	private void CreateMesh(string materialPath)
	{
		MFilter.mesh = new Mesh()
		{
			vertices  = Vertices,
			triangles = Triangles,
			uv        = UVs
		};

		MRenderer.material = new Material(Resources.Load<Material>(materialPath));

		MFilter.mesh.RecalculateNormals();
	}
}

/// <summary>
/// prefab mesh rendering class
/// </summary>
public class PrefabMesh
{
	public          GameObject Obj { get; set; }
	public readonly Coord      Pos;
	public          Renderer   Renderer;

	public PrefabMesh(string meshPath, Vector3 pos, string matPath, Coord coord, ShadowCastingMode shadowMode)
	{
		Obj                        = Object.Instantiate(Resources.Load<GameObject>(meshPath), pos, Quaternion.identity);
		Renderer                   = Obj.GetComponent<Renderer>();
		Renderer.shadowCastingMode = shadowMode;
		Renderer.sharedMaterial    = Resources.Load<Material>(matPath);
		Pos                        = coord;
	}
}