using System.Collections;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
	private       ParticleRender rotationParticle;
	private const string         vfxRotation = "Prefabs/VFX_Rotation";
	private const string         vfxDrop     = "Prefabs/VFX_Drop";
	
	private static readonly int alpha         = Shader.PropertyToID("_Alpha");
	private static readonly int clear         = Shader.PropertyToID("_Clear");
	private static readonly int emission      = Shader.PropertyToID("_Emission");
	private static readonly int over          = Shader.PropertyToID("_GameOver");
	private static readonly int smoothness    = Shader.PropertyToID("_Smoothness");
	private static readonly int color         = Shader.PropertyToID("_Color");
	private static readonly int speed         = Shader.PropertyToID("_Speed");
	private static readonly int gradientColor = Shader.PropertyToID("_GradientColor");

	public EffectSystem()
	{
		
	}

	private void Init()
	{
		rotationParticle = null;
	}
	
	private void DropEffect()
	{
		int yMax = currentBlock.Tile.Select(coord => coord.Y).Prepend(-1).Max();

		foreach (Coord coord in currentBlock.Tile)
		{
			if (coord.Y != yMax) continue;

			Vector3 offset = startOffset + currentBlock.Pos.ToVector() + coord.ToVector() +
			                 new Vector3(0f, -blockSize / 2f, 0f);
			ParticleRender ptc = new(vfxDrop, offset, Quaternion.identity);
			Destroy(ptc.Obj, 3f);
		}
	}

	private IEnumerator CameraFOVEffect()
	{
		const float target      = 120f;
		float       originFOV   = mainCamera.fieldOfView;
		float       originSpeed = Grid.Mesh.MRenderer.material.GetFloat(speed);

		isPause = true;
		Grid.Mesh.MRenderer.material.SetFloat(speed, 10f);

		while (mainCamera.fieldOfView < target - 1f)
		{
			mainCamera.fieldOfView =  Mathf.Lerp(mainCamera.fieldOfView, target, 0.1f);
			audioSourceBGM.pitch   -= 0.02f;

			yield return new WaitForSeconds(0.01f);
		}

		while (mainCamera.fieldOfView > originFOV + 1f)
		{
			mainCamera.fieldOfView =  Mathf.Lerp(mainCamera.fieldOfView, originFOV, 0.2f);
			audioSourceBGM.pitch   += 0.02f;

			yield return new WaitForSeconds(0.01f);
		}

		isPause = false;

		mainCamera.fieldOfView = originFOV;
		Grid.Mesh.MRenderer.material.SetFloat(speed, originSpeed);
		audioSourceBGM.pitch = 1f;
	}

	private IEnumerator GridEffect()
	{
		const float   alphaUnit = 0.01f;
		float         alphaSet  = Grid.Mesh.MRenderer.material.GetFloat(alpha) + alphaUnit;
		Vector3       targetLoc = Grid.Mesh.Obj.transform.position             - Vector3.up    * 5f;
		float         glowSet   = lineGlowPower                                + lineGlowPower * 0.01f;
		const float   range     = 0.15f;
		List<Vector3> listRd    = new();

		for (int i = 0; i < 24; ++i)
		{
			listRd.Add(new Vector3(Random.Range(-range, range),
			                       Random.Range(-range, range),
			                       Random.Range(-range, range)));
		}

		while ((Grid.Mesh.Obj.transform.position - targetLoc).magnitude > 0.001f)
		{
			alphaSet -= 0.01f;
			glowSet  -= lineGlowPower * 0.01f;

			Grid.Mesh.Obj.transform.position = Vector3.Lerp(Grid.Mesh.Obj.transform.position, targetLoc, 0.02f);
			Grid.Mesh.MRenderer.material.SetFloat(alpha, Mathf.Max(alphaSet, 0f));

			for (int i = 0; i < lineMeshList.Count; ++i)
			{
				lineMeshList[i].Renderer.material.SetFloat(power, glowSet);
				lineMeshList[i].Renderer.material.SetFloat(alpha, alphaSet);
				lineMeshList[i].Renderer.SetPosition(0, lineMeshList[i].Renderer.GetPosition(0) +
				                                        listRd[i * 2]);
				lineMeshList[i].Renderer.SetPosition(1, lineMeshList[i].Renderer.GetPosition(1) +
				                                        listRd[i * 2 + 1]);
			}

			yield return new WaitForSeconds(0.02f);
		}

		Destroy(Grid.Mesh.Obj);

		foreach (LineMesh mesh in lineMeshList)
		{
			Destroy(mesh.Obj);
		}

		lineMeshList.Clear();
	}

	private IEnumerator GameOverEffect()
	{
		ClearCurrentBlock();
		ClearShadowBlock();

		const float explosionForce  = 200f;
		float       explosionRadius = Grid.SizeY;
		const float torque          = 50f;

		foreach (PrefabMesh mesh in gridMeshList)
		{
			Rigidbody rb = mesh.Obj.AddComponent<Rigidbody>();

			rb.AddExplosionForce(explosionForce,
			                     new Vector3(0f, startOffset.y - mesh.Pos.Y - blockSize / 2f, 0f),
			                     explosionRadius);

			Vector3 rdVec = new(Random.Range(-torque, torque), Random.Range(-torque, torque),
			                    Random.Range(-torque, torque));
			rb.AddTorque(rdVec);
			rb.angularDrag = Random.Range(0.5f, 2f);

			mesh.Renderer.material.SetFloat(over,       1f);
			mesh.Renderer.material.SetFloat(smoothness, 0f);
		}

		StartCoroutine(GridEffect());

		float alphaSet = 1.01f;

		while (alphaSet > 0f)
		{
			alphaSet -= 0.01f;

			foreach (PrefabMesh mesh in gridMeshList)
			{
				mesh.Renderer.material.SetFloat(alpha, alphaSet);
			}

			yield return new WaitForSeconds(0.02f);
		}

		foreach (PrefabMesh mesh in gridMeshList)
		{
			Destroy(mesh.Obj);
		}

		gridMeshList.Clear();
	}

	private IEnumerator ClearEffect(List<int> cleared)
	{
		List<PrefabMesh> clearMeshList   = new();
		const float      explosionForce  = 900f;
		float            explosionRadius = Grid.SizeX + Grid.SizeZ;
		const float      explosionUp     = 5f;
		const float      torque          = 100f;

		foreach (int height in cleared)
		{
			for (int i = 0; i < Grid.SizeX; ++i)
			{
				for (int j = 0; j < Grid.SizeZ; ++j)
				{
					Vector3 offset = new(i, -height, j);
					PrefabMesh mesh = new("Prefabs/Mesh_Block", startOffset + offset, Block.MatPath[^1],
					                      new Coord(i, height, j), ShadowCastingMode.Off);
					mesh.Renderer.material.SetFloat(clear,    1f);
					mesh.Renderer.material.SetFloat(color,    Random.Range(0f, 1f));
					mesh.Renderer.material.SetFloat(emission, 10f);

					Rigidbody rb = mesh.Obj.AddComponent<Rigidbody>();


					rb.AddForce(Physics.gravity * 40f, ForceMode.Acceleration);
					rb.AddForce(new Vector3(0f, Random.Range(-explosionUp, explosionUp), 0f),
					            ForceMode.Impulse);
					rb.AddExplosionForce(explosionForce,
					                     new Vector3(0f, startOffset.y - height, 0f),
					                     explosionRadius);

					Vector3 rdVec = new(Random.Range(-torque, torque), Random.Range(-torque, torque),
					                    Random.Range(-torque, torque));
					rb.AddTorque(rdVec);
					rb.angularDrag = Random.Range(0.5f, 2f);

					clearMeshList.Add(mesh);
					mesh.Obj.transform.parent = EffectObj.transform;
				}
			}
		}

		float alphaSet    = 1.02f;
		float emissionSet = 2.1f;

		while (alphaSet > 0)
		{
			alphaSet    -= 0.02f;
			emissionSet =  emissionSet > 0 ? emissionSet - 0.1f : 0f;

			foreach (PrefabMesh mesh in clearMeshList)
			{
				mesh.Obj.transform.localScale *= 1.02f;
				mesh.Renderer.material.SetFloat(alpha,    alphaSet);
				mesh.Renderer.material.SetFloat(emission, emissionSet);
			}

			yield return new WaitForSeconds(0.02f);
		}

		foreach (PrefabMesh mesh in clearMeshList)
		{
			Destroy(mesh.Obj);
		}
	}
}