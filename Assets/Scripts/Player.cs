using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{

    [SerializeField] private float movSpeed;
    private float axisX;
    private float axisZ;
    private Vector3 direction = Vector3.zero;

    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashLockTime;
    [SerializeField] private float dashCooldown;
    private float dashTime;
    private float dashCdTime;
    private bool dashing = false;
    private bool canDash = true;

    [SerializeField] private Collider attackArea;
    [SerializeField] private GameObject attackAreaVis;

    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float shotLockTime;
    [SerializeField] private float shotCooldown;
    private float shotTime;
    private float shotCdTime;
    private bool shooting = false;
    private bool canShoot = true;

    [SerializeField] private Material cooldownMaterial;
    private Material gunMaterial;
    private Material playerMaterial;
    private Material swordMaterial;

    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject gun;
    private MeshRenderer swordMesh;
    private MeshRenderer gunMesh;
    private CharacterController self;


    void Start()
    {
        self = GetComponent<CharacterController>();
        swordMesh = sword.GetComponent<MeshRenderer>();
        gunMesh = gun.GetComponent<MeshRenderer>();
        swordMaterial = swordMesh.material;
        gunMaterial = gunMesh.material;
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            Durations();
            Cooldowns();
            GetInputs();
            Move();
        }
    }

    private void GetInputs()
    {

        if (!dashing && !shooting)
        {
            axisX = Input.GetAxisRaw("Horizontal");
            axisZ = Input.GetAxisRaw("Vertical");
            direction = new Vector3(axisX, 0, axisZ);
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (!dashing && canDash && !shooting)
            {
                if (axisX != 0 || axisZ != 0)
                {
                    Dash();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (!shooting && canShoot && !dashing)
            {
                if (axisX != 0 || axisZ != 0)
                {
                    Shoot();
                }
            }
        }

    }

    private void Cooldowns()
    {
        if (!canDash)
        {
            if (Time.time > dashCdTime)
            {
                canDash = true;
                swordMesh.material = swordMaterial;
            }
        }

        if (!canShoot)
        {
            if (Time.time > shotCdTime)
            {
                canShoot = true;
                gunMesh.material = gunMaterial;
            }
        }
    }

    private void Durations()
    {
        if (Time.time > dashTime && dashing)
        {
            dashing = false;
            Cmd_EndSlash();
            attackAreaVis.SetActive(false);
        }

        if (Time.time > shotTime && shooting)
        {
            shooting = false;
        }
    }

    private void Dash()
    {
        dashing = true;
        canDash = false;
        direction *= dashSpeed;
        swordMesh.material = cooldownMaterial;
        Cmd_Slash();
        attackAreaVis.SetActive(true);
        dashTime = Time.time + dashLockTime;
        dashCdTime = Time.time + dashCooldown;         
    }

    [Command] private void Cmd_Slash()
    {
        attackArea.enabled = true;
    }

    [Command]
    private void Cmd_EndSlash()
    {
        attackArea.enabled = false;
    }


    private void Shoot()
    {
        shooting = true;
        canShoot = false;
        Cmd_SpawnBullet(direction);
        direction.x = 0;
        direction.z = 0;
        gunMesh.material = cooldownMaterial;
        shotTime = Time.time + shotLockTime;
        shotCdTime = Time.time + shotCooldown;
    }

    [Command] private void Cmd_SpawnBullet(Vector3 bulletDir)
    {
        GameObject newBullet = Instantiate(bullet, bulletSpawn.position, new Quaternion()); 
        newBullet.GetComponent<Bullet>().Direction = bulletDir;
        NetworkServer.SpawnWithClientAuthority(newBullet, gameObject);
    }

    [Command] public void Cmd_Die()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" && dashing && other.transform != gameObject.transform)
        {    
            other.GetComponent<Player>().Cmd_Die();
        }
    }

    private void Move()
    {       
        self.Move(direction * movSpeed * Time.deltaTime);

        if (axisX != 0 || axisZ != 0)
        {
            if (!shooting)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }        
        }
    }
}
