using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{

    public GameObject target;
    public float verticalOffset;
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    public Vector2 focusAreaSize;

    FocusArea focusArea;
    public Camera mapCamera;
    private Vector3 pos;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    private float halfCamWidth;
    private float halfCamHeight;
    public Texture2D map;

    bool lookAheadStopped;

    void Start()
    {
        halfCamHeight = 2f * mapCamera.orthographicSize / 2;
        halfCamWidth = halfCamHeight * mapCamera.aspect;
        //modded
        target = GameObject.FindGameObjectWithTag("Player");


        if (target != null)
        {
            focusArea = new FocusArea(target.GetComponent<BoxCollider2D>().bounds, focusAreaSize);
        }
    }

    private void Update()
    {
        if (target == null)
        {
            //modded
            target = GameObject.FindGameObjectWithTag("Player");
            focusArea = new FocusArea(target.GetComponent<BoxCollider2D>().bounds, focusAreaSize);
        }
    }

    void LateUpdate()
    {
        if (target != null)
            focusArea.Update(target.GetComponent<BoxCollider2D>().bounds);

        Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

        if (focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            if (Mathf.Sign(Input.GetAxis("Horizontal")) == Mathf.Sign(focusArea.velocity.x) && Input.GetAxis("Horizontal") != 0)
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDstX;
            }
            else
            {
                if (!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                }
            }
        }


        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        focusPosition += Vector2.right * currentLookAheadX;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;

        clampCam();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.centre, focusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 centre;
        public Vector2 velocity;
        float left, right;
        float top, bottom;


        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }

        void clampCam()
    {
        pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, halfCamWidth, (map.width - halfCamWidth - 1));
        pos.y = Mathf.Clamp(pos.y, halfCamHeight, (map.height - halfCamHeight - 1));
        transform.position = pos;
    }

}