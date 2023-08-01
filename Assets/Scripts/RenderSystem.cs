/*
 * Render.cs
 * ---------
 * Made by Lucas Jeong
 * Contains rendering related class.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

/// <summary>
/// line mesh rendering using LineRenderer
/// </summary>
public class LineMesh
{
	public GameObject   Obj      { get; private set; }
	public LineRenderer Renderer { get; set; }

	/// <summary>
	/// main constructor of LineMesh
	/// </summary>
	/// <param name="name">LineMesh GameObject name</param>
	/// <param name="parent">parent GameObject</param>
	/// <param name="pos1">start position</param>
	/// <param name="pos2">end position</param>
	/// <param name="width">line width</param>
	/// <param name="matPath">material path</param>
	public LineMesh(string name,  GameObject parent, Vector3 pos1, Vector3 pos2,
	                float  width, string     matPath)
	{
		Obj = new GameObject(name)
		{
			transform =
			{
				parent = parent.transform
			}
		};
		Renderer               = Obj.AddComponent<LineRenderer>();
		Renderer.positionCount = 2;
		Renderer.SetPosition(0, pos1);
		Renderer.SetPosition(1, pos2);
		Renderer.startWidth    = width;
		Renderer.endWidth      = width;
		Renderer.useWorldSpace = true;
		Renderer.startColor    = Color.white;
		Renderer.endColor      = Color.white;
		Renderer.material      = new Material(Resources.Load<Material>(matPath));
	}
}

/// <summary>
/// cube mesh rendering class using UnityEngine
/// </summary>
public class CubeMesh
{
	public  GameObject   Obj       { get; private set; }
	private MeshFilter   MFilter   { get; set; }
	public  MeshRenderer MRenderer { get; private set; }
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
	/// <param name="matPath">material data path in Resources/</param>
	/// <param name="bTexture">using texture option</param>
	/// <param name="texture">Texture2D object</param>
	public CubeMesh(string name,    int  x,        int       y, int z, float unit, bool inverse,
	                string matPath, bool bTexture, Texture2D texture)
	{
		Obj       = new GameObject(name);
		MFilter   = Obj.AddComponent<MeshFilter>();
		MRenderer = Obj.AddComponent<MeshRenderer>();

		InitializeVertices(x, y, z, unit);
		InitializeTriangles(inverse);
		InitializeUVs();

		if (bTexture)
		{
			CreateMesh(matPath, texture);
		}
		else
		{
			CreateMesh(matPath);
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
/// prefab mesh load class
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

/// <summary>
/// particle load class
/// </summary>
public class ParticleRender
{
	public GameObject   Obj      { get; set; }
	public VisualEffect Renderer { get; set; }

	public ParticleRender(string particlePath, Vector3 pos, GameObject parent, Quaternion rotation)
	{
		Obj                  = Object.Instantiate(Resources.Load<GameObject>(particlePath), pos, rotation);
		Obj.transform.parent = parent.transform;
		Renderer             = Obj.GetComponentInChildren<VisualEffect>();
	}
}

public class RenderSystem : MonoBehaviour
{
	private static          GameObject       blockObj;
	private static          GameObject       shadowObj;
	private static          GameObject       gridObj;
	private static          GameObject       effectObj;
	private                 List<PrefabMesh> blockMeshList;
	private                 List<PrefabMesh> shadowMeshList;
	private                 List<PrefabMesh> gridMeshList;
	private                 List<LineMesh>   lineMeshList;
	public                  Vector3          startOffset;
	private                 float            lineGlowPower;
	private static readonly int              power = Shader.PropertyToID("_Power");

	public RenderSystem()
	{
		Init();
	}

	private void Init()
	{
		gridObj   = GameObject.Find("Grid");
		blockObj  = GameObject.Find("Blocks");
		shadowObj = GameObject.Find("Shadow");
		effectObj = GameObject.Find("Effect");

		startOffset = new Vector3(-GameManager.Grid.SizeX / 2f + GameManager.BlockSize / 2,
		                          GameManager.Grid.SizeY  / 2f - GameManager.BlockSize / 2,
		                          -GameManager.Grid.SizeZ / 2f + GameManager.BlockSize / 2);

		blockMeshList  = new List<PrefabMesh>();
		shadowMeshList = new List<PrefabMesh>();
		gridMeshList   = new List<PrefabMesh>();
		lineMeshList   = new List<LineMesh>();
		RenderLine();
		lineGlowPower = lineMeshList[0].Renderer.material.GetFloat(power);

		RenderCurrentBlock();
		RenderShadowBlock();

		if (GameManager.TestGrid) RenderGrid();
	}

	private void RenderLine()
	{
		const float width = 0.05f;

		LineMesh mesh01 = new("Line", gridObj,
		                      new Vector3(-GameManager.Grid.SizeX / 2f, -GameManager.Grid.SizeY / 2f,
		                                  GameManager.Grid.SizeZ  / 2f),
		                      new Vector3(GameManager.Grid.SizeX / 2f, -GameManager.Grid.SizeY / 2f,
		                                  GameManager.Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh01);

		LineMesh mesh02 = new("Line", gridObj,
		                      new Vector3(-GameManager.Grid.SizeX / 2f, GameManager.Grid.SizeY / 2f,
		                                  GameManager.Grid.SizeZ  / 2f),
		                      new Vector3(GameManager.Grid.SizeX / 2f, GameManager.Grid.SizeY / 2f,
		                                  GameManager.Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh02);

		LineMesh mesh03 = new("Line", gridObj,
		                      new Vector3(-GameManager.Grid.SizeX / 2f, -GameManager.Grid.SizeY / 2f,
		                                  GameManager.Grid.SizeZ  / 2f),
		                      new Vector3(-GameManager.Grid.SizeX / 2f, GameManager.Grid.SizeY / 2f,
		                                  GameManager.Grid.SizeZ  / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh03);

		LineMesh mesh04 = new("Line", gridObj,
		                      new Vector3(GameManager.Grid.SizeX / 2f, -GameManager.Grid.SizeY / 2f,
		                                  GameManager.Grid.SizeZ / 2f),
		                      new Vector3(GameManager.Grid.SizeX / 2f, GameManager.Grid.SizeY / 2f,
		                                  GameManager.Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh04);

		LineMesh mesh05 = new("Line", gridObj,
		                      new Vector3(-GameManager.Grid.SizeX / 2f, -GameManager.Grid.SizeY / 2f,
		                                  -GameManager.Grid.SizeZ / 2f),
		                      new Vector3(GameManager.Grid.SizeX  / 2f, -GameManager.Grid.SizeY / 2f,
		                                  -GameManager.Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh05);

		LineMesh mesh06 = new("Line", gridObj,
		                      new Vector3(-GameManager.Grid.SizeX / 2f, GameManager.Grid.SizeY / 2f,
		                                  -GameManager.Grid.SizeZ / 2f),
		                      new Vector3(GameManager.Grid.SizeX  / 2f, GameManager.Grid.SizeY / 2f,
		                                  -GameManager.Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh06);

		LineMesh mesh07 = new("Line", gridObj,
		                      new Vector3(-GameManager.Grid.SizeX / 2f, -GameManager.Grid.SizeY / 2f,
		                                  -GameManager.Grid.SizeZ / 2f),
		                      new Vector3(-GameManager.Grid.SizeX / 2f, GameManager.Grid.SizeY / 2f,
		                                  -GameManager.Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh07);

		LineMesh mesh08 = new("Line", gridObj,
		                      new Vector3(GameManager.Grid.SizeX  / 2f, -GameManager.Grid.SizeY / 2f,
		                                  -GameManager.Grid.SizeZ / 2f),
		                      new Vector3(GameManager.Grid.SizeX  / 2f, GameManager.Grid.SizeY / 2f,
		                                  -GameManager.Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh08);

		LineMesh mesh09 = new("Line", gridObj,
		                      new Vector3(-GameManager.Grid.SizeX / 2f, -GameManager.Grid.SizeY / 2f,
		                                  -GameManager.Grid.SizeZ / 2f),
		                      new Vector3(-GameManager.Grid.SizeX / 2f, -GameManager.Grid.SizeY / 2f,
		                                  GameManager.Grid.SizeZ  / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh09);

		LineMesh mesh10 = new("Line", gridObj,
		                      new Vector3(-GameManager.Grid.SizeX / 2f, GameManager.Grid.SizeY / 2f,
		                                  -GameManager.Grid.SizeZ / 2f),
		                      new Vector3(-GameManager.Grid.SizeX / 2f, GameManager.Grid.SizeY / 2f,
		                                  GameManager.Grid.SizeZ  / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh10);

		LineMesh mesh11 = new("Line", gridObj,
		                      new Vector3(GameManager.Grid.SizeX  / 2f, -GameManager.Grid.SizeY / 2f,
		                                  -GameManager.Grid.SizeZ / 2f),
		                      new Vector3(GameManager.Grid.SizeX / 2f, -GameManager.Grid.SizeY / 2f,
		                                  GameManager.Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh11);

		LineMesh mesh12 = new("Line", gridObj,
		                      new Vector3(GameManager.Grid.SizeX  / 2f, GameManager.Grid.SizeY / 2f,
		                                  -GameManager.Grid.SizeZ / 2f),
		                      new Vector3(GameManager.Grid.SizeX / 2f, GameManager.Grid.SizeY / 2f,
		                                  GameManager.Grid.SizeZ / 2f),
		                      width, "Materials/Line");
		lineMeshList.Add(mesh12);
	}

	private void RenderCurrentBlock()
	{
		ClearCurrentBlock();

		foreach (Coord coord in GameManager.CurrentBlock.TilePositions())
		{
			Vector3 offset = new(coord.X, -coord.Y, coord.Z);
			PrefabMesh mesh = new("Prefabs/Mesh_Block", startOffset + offset,
			                      Block.MatPath[GameManager.CurrentBlock.GetId()], coord, ShadowCastingMode.Off);

			blockMeshList.Add(mesh);
			mesh.Obj.transform.parent = blockObj.transform;
		}
	}

	private void RenderShadowBlock()
	{
		ClearShadowBlock();
		GameManager.ShadowBlock = GameManager.CurrentBlock.CopyBlock();

		do
		{
			GameManager.CurrentBlock.Move(Coord.Down);
		} while (GameManager.BlockFits(shadowBlock));

		shadowBlock.Move(Coord.Up);

		foreach (Coord coord in shadowBlock.TilePositions())
		{
			Vector3 offset = new(coord.X, -coord.Y, coord.Z);
			PrefabMesh mesh = new("Prefabs/Mesh_Block", startOffset + offset,
			                      Block.MatPath[0], coord, ShadowCastingMode.Off);

			shadowMeshList.Add(mesh);
			mesh.Obj.transform.parent = shadowObj.transform;
		}
	}

	private void RenderGrid()
	{
		ClearGrid();

		for (int i = 0; i < GameManager.GameManager.Grid.SizeY; ++i)
		{
			for (int j = 0; j < GameManager.GameManager.Grid.SizeX; ++j)
			{
				for (int k = 0; k < GameManager.GameManager.Grid.SizeZ; ++k)
				{
					if (Grid[j, i, k] != 0)
					{
						Vector3 offset = new(j, -i, k);
						PrefabMesh mesh = new("Prefabs/Mesh_Block", startOffset + offset, Block.MatPath[^1],
						                      new Coord(j, i, k), ShadowCastingMode.Off);
						mesh.Renderer.material.SetFloat(gradientColor,
						                                (float)i / (GameManager.GameManager.Grid.SizeY - 1));

						gridMeshList.Add(mesh);
						mesh.Obj.transform.parent = GameManager.GridObj.transform;
					}
				}
			}
		}
	}

	private void RefreshCurrentBlock()
	{
		RenderCurrentBlock();
		RenderShadowBlock();
	}

	private void ClearCurrentBlock()
	{
		foreach (PrefabMesh mesh in blockMeshList)
		{
			Destroy(mesh.Obj);
		}

		blockMeshList.Clear();
	}

	private void ClearShadowBlock()
	{
		foreach (PrefabMesh mesh in shadowMeshList)
		{
			Destroy(mesh.Obj);
		}

		shadowMeshList.Clear();
	}

	private void ClearGrid()
	{
		foreach (PrefabMesh mesh in gridMeshList)
		{
			Destroy(mesh.Obj);
		}

		gridMeshList.Clear();
	}
}