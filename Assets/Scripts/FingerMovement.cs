using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerMovement : MonoBehaviour
{
    [SerializeField] private GameObject[] fingerTips;
    private Vector3[] prevTipsPosition = new Vector3[5];
    private float[] forcePerFinger = {0f, 0f, 0f, 0f, 0f};
    [SerializeField] private Vector3[] attemptedPositions = new Vector3[5];
    // Start is called before the first frame update
    void Start()
    {
        
        for (int i = 0; i < 5; i++)
        {
            fingerTips[i].transform.position = attemptedPositions[i];
            prevTipsPosition[i] = fingerTips[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        for (int i = 0; i < 5; i++)
        {  
            prevTipsPosition[i] = fingerTips[i].transform.position;
            Vector3 movementDirection = attemptedPositions[i] - prevTipsPosition[i];
            Collider bunnyCol = GameObject.Find("stanford-bunny").GetComponent<Collider>();
            if(bunnyCol.bounds.Contains(attemptedPositions[i])){
                Ray ray = new Ray(prevTipsPosition[i], movementDirection);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit)){
                    // add an offset to the hit point to make the finger tip stop before getting inside the mesh
                    hit.point += 0.00001f*hit.normal;
                    fingerTips[i].transform.position = hit.point;
                    if(forcePerFinger[i] <= 10f) forcePerFinger[i] += 0.5f*movementDirection.magnitude;
                    MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
                    if(deformer != null){
                        //Debug.Log("force: " + forcePerFinger[i]);
                        //Debug.Log("hit point: " + hit.point);
                        deformer.AddDeformingForce(hit.point, forcePerFinger[i]);
                    }
                }
            }
            else{
                fingerTips[i].transform.position = attemptedPositions[i];
                forcePerFinger[i] = 0f;
            }
        }
    }
}
