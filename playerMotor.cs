using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMotor : MonoBehaviour
{
    public float fireRate = 0.1f;
    public float damage = 10;
    private float timeToSpawnEffect = 0;
    public float effectSpawnRate = 10;
    public LayerMask tohit;

    float timeToFire = 0;
    public Transform firepoint;
    public Transform bulletTrail;
    public Transform muzzleFlash;

    public float movementThresh = 0.7f; //override
    public float aimThresh = 0.1f; //override
    public float speed = 1; //override
    private Rigidbody2D rgbd;
    private Vector2 moveVel;
    private Animator animator;
    public GameObject crosshair;
    private float prevAngle=0;

    public bool lockRot = false;

    public GameObject sprite;
    public GameObject hair;
    public float weaponRange = 2; //override

    private Vector2 lscale;
    // Start is called before the first frame update
    void Start()
    {
        rgbd = this.gameObject.GetComponent<Rigidbody2D>();
        animator = sprite.gameObject.GetComponent<Animator>();
        lscale = sprite.transform.localScale;
        setWeaponRange();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("lockRot"))
        {
            lockRot = !lockRot;
        }

        Vector2 moveInp = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if(moveInp.magnitude >= aimThresh && !lockRot)
        {
            float angle = Mathf.Atan2(moveInp.y, moveInp.x) * Mathf.Rad2Deg;

            if (Mathf.Abs(angle) > 90)
            {
                lscale.Set(-1, 1);
                
            }
            else if (Mathf.Abs(angle) < 90)
            {
                lscale.Set(1, 1);
            }

            if (moveVel != Vector2.zero)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }

            crosshair.transform.rotation = Quaternion.Euler(0, 0, angle);
            prevAngle = angle;
        }

        sprite.transform.localScale = lscale;

        if (moveInp.magnitude > movementThresh)
        {
            moveVel = moveInp.normalized * speed;
        }
        else
        {
            moveVel = Vector2.zero;
        }

        if (fireRate == 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                shoot();
            }
        }
        else
        {
            if (Input.GetButton("Fire1") && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / fireRate;
                shoot();
            }

        }
    }

    private void FixedUpdate()
    {
        rgbd.velocity = moveVel;

    }

    private void setWeaponRange()
    {
        hair.transform.localPosition = new Vector3(weaponRange, 0, 0);
    }

    void shoot()
    {
        Vector2 firePos = new Vector2(firepoint.position.x, firepoint.position.y);
        Vector2 cPos = new Vector2(hair.transform.position.x, hair.transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(firePos, cPos - firePos, 100, tohit);
        if(Time.time > timeToSpawnEffect)
        {
            bulletEffect(hit);
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
    }

    void bulletEffect(RaycastHit2D hit)
    {
        Transform bullet = Instantiate(bulletTrail, firepoint.position, firepoint.rotation) as Transform;
        if (hit.collider != null)
        {
            //Debug.Log(hit.point);
            bullet.gameObject.GetComponent<bulletScript>().setEndPos(hit.point, firepoint.position, true);
        }
        else
        {
            bullet.gameObject.GetComponent<bulletScript>().setEndPos(Vector2.zero, firepoint.position, false);
        }
        Transform muzzleInstance = Instantiate(muzzleFlash, firepoint.position, firepoint.rotation) as Transform;
        muzzleInstance.parent = firepoint;
        float size = Random.Range(0.6f, 1.0f);
        muzzleInstance.localScale = new Vector3(size, size, 1);

        Destroy(muzzleInstance.gameObject, 0.02f);


    }

}
