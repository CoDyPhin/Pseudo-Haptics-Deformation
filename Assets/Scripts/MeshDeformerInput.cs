using UnityEngine;

public class MeshDeformerInput : MonoBehaviour {
	
	public float force = 10f;
	public float forceOffset = 0.1f;
	MeshDeformer deformer;
	GameObject cilinder1;
	GameObject cilinder2;
	
	void Start (){
		// create two cilinders in front of the camera to simulate two fingers
		cilinder1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		cilinder1.transform.position = new Vector3(1.95f, 0.8f, 0f);
		cilinder1.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		cilinder1.transform.rotation = Camera.main.transform.rotation;
		cilinder2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		cilinder2.transform.position = cilinder1.transform.position + new Vector3(0.1f, 0, 0);
		cilinder2.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		cilinder2.transform.rotation = Camera.main.transform.rotation;
		// add rigidbody and colliders to the cilinders
		cilinder1.AddComponent<Rigidbody>();
		cilinder1.GetComponent<Rigidbody>().useGravity = false;
		cilinder2.AddComponent<Rigidbody>();
		cilinder2.GetComponent<Rigidbody>().useGravity = false;
		// freeze rotation
		cilinder1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		cilinder2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}

	void Update () {
		if (Input.GetMouseButton(0)) {
			HandleInput();
		}
		// add forces to move the cilinders using the arrow keys
		if (Input.GetKey(KeyCode.UpArrow)) {
			cilinder1.GetComponent<Rigidbody>().AddForce(new Vector3(0, 1, 0));
			cilinder2.GetComponent<Rigidbody>().AddForce(new Vector3(0, 1, 0));
		}
		else{
			cilinder1.GetComponent<Rigidbody>().velocity = Vector3.zero;
			cilinder2.GetComponent<Rigidbody>().velocity = Vector3.zero;

		}
		if (Input.GetKey(KeyCode.DownArrow)) {
			cilinder1.GetComponent<Rigidbody>().AddForce(new Vector3(0, -1, 0));
			cilinder2.GetComponent<Rigidbody>().AddForce(new Vector3(0, -1, 0));
		}
		else{
			cilinder1.GetComponent<Rigidbody>().velocity = Vector3.zero;
			cilinder2.GetComponent<Rigidbody>().velocity = Vector3.zero;

		}
		if (Input.GetKey(KeyCode.LeftArrow)) {
			cilinder1.GetComponent<Rigidbody>().AddForce(new Vector3(-1, 0, 0));
			cilinder2.GetComponent<Rigidbody>().AddForce(new Vector3(-1, 0, 0));

		}
		else{
			cilinder1.GetComponent<Rigidbody>().velocity = Vector3.zero;
			cilinder2.GetComponent<Rigidbody>().velocity = Vector3.zero;

		}
		if (Input.GetKey(KeyCode.RightArrow)) {
			cilinder1.GetComponent<Rigidbody>().AddForce(new Vector3(1, 0, 0));
			cilinder2.GetComponent<Rigidbody>().AddForce(new Vector3(-1, 0, 0));
		}
		else{
			cilinder1.GetComponent<Rigidbody>().velocity = Vector3.zero;
			cilinder2.GetComponent<Rigidbody>().velocity = Vector3.zero;

		}
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			deformer = hit.collider.GetComponent<MeshDeformer>();
			if (deformer) {
				Vector3 point = hit.point;
				point += hit.normal * forceOffset;
				//Debug.Log("RayCast point: " + point);
				deformer.AddDeformingForce(point, force);
			}
		}
	}
}