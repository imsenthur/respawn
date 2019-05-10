using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMotor : MonoBehaviour
{
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
    }

    private void FixedUpdate()
    {
        rgbd.velocity = moveVel;

    }

    private void setWeaponRange()
    {
        hair.transform.localPosition = new Vector3(weaponRange, 0, 0);
    }

}
