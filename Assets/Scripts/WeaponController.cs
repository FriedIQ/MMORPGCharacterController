using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour 
{
    public bool equiped;
    public WeaponManager.WeaponType weaponType;

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
    bool hasOwner;
    public Vector3 equipPosition;
    public Vector3 equipRotation;
    public Vector3 unequipPostion;
    public Vector3 unequipRotation;
    // debug scale
    Vector3 scale;

    public enum RestPosition
    {
        RightHip,
        Waist
    }

	// Use this for initialization
	void Start () 
    {
        currentAmmo = maximumAmmo;
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
	}

    public void Fire()
    {
        fireBullet = true;
    }
}
