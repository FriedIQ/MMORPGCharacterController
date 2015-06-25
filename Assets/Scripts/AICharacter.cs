using UnityEngine;
using System.Collections;

public class AICharacter : MonoBehaviour
{
    NavMeshAgent agent;
    CharacterMovement characterMovement;

    public Transform target;
    float targetTolerance = 1;

    private Vector3 targetPosition;

	// Use this for initialization
	void Start () 
    {
	    agent = GetComponentInChildren<NavMeshAgent>();
	    characterMovement = GetComponent<CharacterMovement>();

    }
	
	// Update is called once per frame
	void Update () 
    {
	    if (target != null)
	    {
	        if ((target.position - targetPosition).magnitude > targetTolerance)
	        {
                Debug.Log(string.Format("Tracking Target ({0})", ((target.position - targetPosition).magnitude)));
	            targetPosition = target.position;
	            agent.SetDestination(targetPosition);
	        }

	        agent.transform.position = transform.position;
            characterMovement.Move(agent.desiredVelocity);
	    }
	    else
	    {
            characterMovement.Move(Vector3.zero);
	    }
	}
}
