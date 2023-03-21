using UnityEngine;
using System.Collections;

public class CarDamage : MonoBehaviour 
{
	public float maxMoveDelta = 50.0f; // maximum distance one vertice moves per explosion (in meters)
	public float maxCollisionStrength = 50.0f;
	public float YforceDamp = 0.1f; // 0.0 - 1.0
	public float demolutionRange = 0.5f;
	public float impactDirManipulator = 0.0f;
	public MeshFilter meshfilters;
	
	private float sqrDemRange;
	private MeshCollider meshCollider;

	public void Start()
	{
        //maxMoveDelta = Mathf.Clamp01(maxMoveDelta);
        meshCollider = GetComponent<MeshCollider>();
        sqrDemRange = demolutionRange*demolutionRange;
	}
	
	public void OnCollisionEnter( Collision collision ) 
	{
		Vector3 colRelVel = collision.relativeVelocity;
		colRelVel.y *= YforceDamp;
		
		Vector3 colPointToMe = transform.position - collision.contacts[0].point;

		// Dot = angle to collision point, frontal = highest damage, strip = lowest damage
		float colStrength = colRelVel.magnitude * Vector3.Dot(collision.contacts[0].normal, colPointToMe.normalized);

		OnMeshForce( collision.contacts[0].point, Mathf.Clamp01(colStrength/maxCollisionStrength) );
		//Debug.DrawLine(collision.contacts[0].point, transform.position, Color.red);
		//Debug.Break();
	}

	// if called by SendMessage(), we only have 1 param
	public void OnMeshForce( Vector4 originPosAndForce )
	{
        OnMeshForce( (Vector3)originPosAndForce, originPosAndForce.w );
	}

	public void OnMeshForce( Vector3 originPos, float force )
	{
        // force should be between 0.0 and 1.0
        force = Mathf.Clamp01(force);


        Vector3 [] verts = meshfilters.mesh.vertices;

		for (int i=0;i<verts.Length;i++)
		{
			Vector3 scaledVert = Vector3.Scale( verts[i], transform.localScale );
			Vector3 vertWorldPos = meshfilters.transform.position + (meshfilters.transform.rotation * scaledVert);
			Vector3 originToMeDir = vertWorldPos - originPos;
			Vector3 flatVertToCenterDir = transform.position - vertWorldPos;
			flatVertToCenterDir.y = 0.0f;

			// 0.5 - 1 => 45 to 0  / current vertice is nearer to exploPos than center of bounds
			if( originToMeDir.sqrMagnitude < sqrDemRange ) //dot > 0.8f )
			{
				float dist = Mathf.Clamp01(originToMeDir.sqrMagnitude/sqrDemRange);
				float moveDelta = force * (1.0f-dist) * maxMoveDelta;

				Vector3 moveDir = Vector3.Slerp(originToMeDir, flatVertToCenterDir, impactDirManipulator).normalized * moveDelta;

				verts[i] += Quaternion.Inverse(transform.rotation)*moveDir;


				//Debug.DrawRay(vertWorldPos, moveDir, Color.red);
				//Debug.DrawLine(vertWorldPos, transform.position, Color.green);
				///Debug.Break();
			}
		}
		meshfilters.mesh.SetVertices(verts);
		meshfilters.mesh.RecalculateBounds();
		//GetComponent<MeshCollider>().sharedMesh = null;
		meshCollider.sharedMesh = meshfilters.mesh;
	}
}
