using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour 
{
    public bool equiped;
    public WeaponManager.WeaponType weaponType;
    public WeaponManager.RestPosition restPosition;

    public int maximumAmmo;
    public int maximumClipAmmo = 30;
    public int currentAmmo;
    public bool canBurstFire;

    public GameObject handPosition;
    public GameObject bulletPrefab;
    GameObject bulletSpawnObject;
    public Transform bulletSpawnPoint;
    ParticleSystem particalSystem;

    WeaponManager parentControl;

    bool fireBullet;
    AudioSource audioSource;
    Animator animator;

    [Header("Positions")]
    public bool hasOwner;
    public Vector3 equipPosition;
    public Vector3 equipRotation;
    public Vector3 unequipPostion;
    public Vector3 unequipRotation;
    // debug scale
    Vector3 scale;

	// Use this for initialization
	void Start () 
    {
        currentAmmo = maximumClipAmmo;
        bulletSpawnObject = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
        bulletSpawnObject.AddComponent<ParticleDirection>();
        bulletSpawnObject.GetComponent<ParticleDirection>().particleSpawnPoint = bulletSpawnPoint;

        particalSystem = bulletSpawnObject.GetComponent<ParticleSystem>();

        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        scale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.localScale = scale;

        // if this weapon is currently equiped
        if(equiped)
        {
            transform.parent = transform.GetComponentInParent<WeaponManager>().transform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand).Find("WeaponNode");
            transform.localPosition = equipPosition;
            transform.localRotation = Quaternion.Euler(equipRotation);

            if(fireBullet)
            {
                if(currentAmmo > 0)
                {
                    currentAmmo--;
                    particalSystem.Emit(1);
                    audioSource.Play();
                    // animator.SetTrigger("Fire");
                    fireBullet = false;
                }
                else
                {
                    if (maximumAmmo >= maximumClipAmmo)
                    {
                        currentAmmo = maximumClipAmmo;
                        currentAmmo -= maximumClipAmmo;
                    }
                    else
                    {
                        currentAmmo = maximumClipAmmo - (maximumClipAmmo - maximumAmmo);
                    }

                    Debug.Log("Reload");
                }
            }
        }
        else
        {
            if(hasOwner)
            {
                switch(restPosition)
                {
                    case WeaponManager.RestPosition.RightHip:
                        transform.parent = transform.GetComponentInParent<WeaponManager>().transform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Hips); //.Find("RightHipRest");
                        break;
                    case WeaponManager.RestPosition.Waist:
                        break;
                    case WeaponManager.RestPosition.Back:
                        transform.parent = transform.GetComponentInParent<WeaponManager>().transform.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Spine); //.Find("BackRest");
                        break;
                }

                transform.localPosition = unequipPostion;
                transform.localRotation = Quaternion.Euler(unequipRotation);
            }
        }
	}

    public void Fire()
    {
        fireBullet = true;
    }
}
