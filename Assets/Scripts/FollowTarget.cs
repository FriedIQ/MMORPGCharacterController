using UnityEngine;

public abstract class FollowTarget : MonoBehaviour 
{
    [SerializeField]
    public Transform target;

    [SerializeField]
    public bool autoTargetPlayer = true;

    public Transform Target
    { 
        get { return this.target; } 
    }

	virtual protected void Start () 
    {
        if (autoTargetPlayer)
        {
            FindTargetPlayer();
        }
	}
	
	void FixedUpdate () 
    {
	    if(autoTargetPlayer && (target == null || !target.gameObject.activeSelf))
        {
            FindTargetPlayer();
        }

        var rigidBody = target.GetComponent<Rigidbody>();
        if (target != null && (rigidBody != null && !rigidBody.isKinematic))
        {
            Follow(Time.deltaTime);
        }
	}

    protected abstract void Follow(float deltaTime);

    public void FindTargetPlayer()
    {
        if(target == null)
        {
            var targetObject = GameObject.FindGameObjectWithTag("Player");

            if(targetObject)
            {
                SetTarget(targetObject.transform);
            }
        }
    }

    public virtual void SetTarget(Transform newTargetTransform)
    {

    }
}
