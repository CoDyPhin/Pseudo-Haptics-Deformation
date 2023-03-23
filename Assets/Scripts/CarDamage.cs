using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarDamage : MonoBehaviour 
{
	public float maxMoveDelta = 50.0f; // maximum distance one vertice moves per explosion (in meters)
	public float maxCollisionStrength = 50.0f;
	//public float YforceDamp = 0.1f; // 0.0 - 1.0
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
		int contactCount = collision.contactCount;
		ContactPoint[] contactPoints = new ContactPoint[contactCount];
		collision.GetContacts(contactPoints);

		for (int i = 0; i < contactCount; i++)
		{
			Vector3 relColPoint = transform.position - contactPoints[i].point;
			float colStrength = colRelVel.magnitude * Vector3.Dot(contactPoints[i].normal, relColPoint.normalized);
			OnMeshForce(contactPoints[i].point, Mathf.Clamp01(colStrength*(contactCount-i/contactCount)/maxCollisionStrength));
		}

		//colRelVel.y *= YforceDamp;
		
		//Vector3 relColPoint = transform.position - collision.contacts[0].point;

		// Dot = angle to collision point, frontal = highest damage, strip = lowest damage
		//float colStrength = colRelVel.magnitude * Vector3.Dot(collision.contacts[0].normal, relColPoint.normalized);

		//OnMeshForce(collision.contacts[0].point, Mathf.Clamp01(colStrength/maxCollisionStrength));

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

		List<Vector3> verts = new List<Vector3>();
        meshfilters.mesh.GetVertices(verts);

		for (int i = 0; i < verts.Count; i++){
			Vector3 scaledVert = Vector3.Scale( verts[i], transform.localScale );
			Debug.Log(scaledVert);
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
