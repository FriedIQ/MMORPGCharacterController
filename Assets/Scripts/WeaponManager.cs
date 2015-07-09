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

    public List<GameObject> weaponList = new List<GameObject>();
    public WeaponController activeWeapon;
    int weaponNumber = 0;

    public WeaponType weaponType;


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
        activeWeapon = weaponList[weaponNumber].GetComponent<WeaponController>();
        // update weapon specific IK here.
        activeWeapon.equiped = true;
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
    }
}
