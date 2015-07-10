using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour 
{    
    public enum WeaponType
    {
        Pistol,
        Rifle
    }

    public enum RestPosition
    {
        RightHip,
        Waist,
        Back
    }

    Animator animator;

    // IK Setup
    private float IKWeight;

    public List<GameObject> weaponList = new List<GameObject>();
    public WeaponController activeWeapon;
    int weaponNumber = 0;

    public WeaponType weaponType;

    public bool aiming;

	// Use this for initialization
	void Start () 
    {
        animator = GetComponent<Animator>();

        if(weaponList.Count > 0)
        {
            activeWeapon = weaponList[weaponNumber].GetComponent<WeaponController>();

            // Setup weapon specific IK here.

            activeWeapon.equiped = true;

            foreach(GameObject gameObject in weaponList)
            {
                gameObject.GetComponent<WeaponController>().hasOwner = true;
            }
        }
        else
        {
            Debug.Log("No weapons in the weapon list.");
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        // update weapon specific IK here.
	    IKWeight = Mathf.MoveTowards(IKWeight, (aiming) ? 1.0f : 0.0f, Time.deltaTime);
    }

    void OnAnimatorIK()
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, IKWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, IKWeight);

        Vector3 pos = activeWeapon.handPosition.transform.TransformPoint(Vector3.zero);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, activeWeapon.handPosition.transform.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, activeWeapon.handPosition.transform.rotation);
    }

    public void FireActiveWeapon()
    {
        if(activeWeapon != null)
        {
            activeWeapon.Fire();
        }
    }

    public void ChangeWeapon(bool ascending)
    {
        if(weaponList.Count > 0)
        {
            activeWeapon.equiped = false;
            if(ascending)
            {
                if(weaponNumber < weaponList.Count - 1)
                {
                    weaponNumber++;
                }
                else
                {
                    weaponNumber = 0;
                }
            }
            else
            {
                if(weaponNumber > 0)
                {
                    weaponNumber--;
                }
                else
                {
                    weaponNumber = weaponList.Count - 1;
                }
            }
        }

        activeWeapon = weaponList[weaponNumber].GetComponent<WeaponController>();
        activeWeapon.equiped = true;
    }
}
