using UnityEngine;
using System.Collections;

public class Deformation : MonoBehaviour 
{
    public MeshFilter meshFilter;
    public float deformationStrength = 0.1f;
    public int numVerticesToDeform = 5;

    private Mesh mesh;
    private Vector3[] originalVertices;

    void Start() {
        mesh = meshFilter.mesh;
        originalVertices = mesh.vertices;
    }

    void OnCollisionEnter(Collision collision) {
        // Get the contact point and normal
        ContactPoint contact = collision.contacts[0];
		// get the relative position of point of the collision
		Vector3 point = contact.point - transform.position;
        Vector3 normal = contact.normal;

        // Calculate the deformation amount
        float deformationAmount = deformationStrength * collision.relativeVelocity.magnitude;

        // Deform the mesh
        DeformMeshAtPoint(point, normal, deformationAmount);
    }

    void OnCollisionExit(Collision collision) {
        // Remove deformation from the mesh
        //RemoveDeformation();
    }

    void DeformMeshAtPoint(Vector3 point, Vector3 normal, float deformationAmount) {
		Debug.Log(point);

        // Find the closest vertices to the contact point
        int[] closestVertices = FindClosestVertices(point, numVerticesToDeform);

        // Deform the vertices along the normal
        Vector3 deformation = normal * deformationAmount;
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < closestVertices.Length; i++) {
            int vertexIndex = closestVertices[i];
            vertices[vertexIndex] = originalVertices[vertexIndex] + deformation;
        }
        mesh.SetVertices(vertices);

        // Mark the mesh as dirty
        //mesh.MarkDynamic();

        // Recalculate the mesh normals and bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Update the mesh filter to trigger a redraw
        meshFilter.mesh = mesh;
    }

    void RemoveDeformation() {
        // Reset all vertices to their original positions
        mesh.SetVertices(originalVertices);

        // Mark the mesh as dirty
        //mesh.MarkDynamic();

        // Recalculate the mesh normals and bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Update the mesh filter to trigger a redraw
        meshFilter.mesh = mesh;
    }

    int[] FindClosestVertices(Vector3 point, int numVertices) {
        // Find the closest vertices to the given point
        int[] closestVertices = new int[numVertices];
        float[] closestDistances = new float[numVertices];
        for (int i = 0; i < numVertices; i++) {
            closestDistances[i] = Mathf.Infinity;
        }
        for (int i = 0; i < mesh.vertexCount; i++) {
            float distance = Vector3.Distance(mesh.vertices[i], point);
            for (int j = 0; j < numVertices; j++) {
                if (distance < closestDistances[j]) {
                    for (int k = numVertices - 1; k > j; k--) {
                        closestDistances[k] = closestDistances[k - 1];
                        closestVertices[k] = closestVertices[k - 1];
                    }
                    closestDistances[j] = distance;
                    closestVertices[j] = i;
                    break;
                }
            }
        }
		Debug.Log(originalVertices[closestVertices[0]]);
        return closestVertices;
    }
}
