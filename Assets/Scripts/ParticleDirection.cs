using UnityEngine;
using System.Collections;

public class ParticleDirection : MonoBehaviour 
{
    public Transform particleSpawnPoint;

	// Use this for initialization
	void Start () 
    {

	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.position = particleSpawnPoint.TransformPoint(Vector3.zero);
        transform.forward = particleSpawnPoint.TransformDirection(Vector3.forward);
        transform.rotation = particleSpawnPoint.rotation;
	}
}
