using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    public float bulletSpeed = 0.1f;
    public Vector2 endPos;
    private Vector2 iniPos;
    private Vector2 cPos;
    public float maxBulletDrop = 20;
    public bool hit = false;
    // Update is called once per frame

    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * bulletSpeed);
        cPos = new Vector2(transform.position.x, transform.position.y) - iniPos;

        if(cPos.magnitude > endPos.magnitude + 0.5f)
        Destroy(gameObject);
    }

    public void setEndPos(Vector2 end, Vector2 start, bool ifHit)
    {
        iniPos = start;
        hit = ifHit;

        if (ifHit)
        {
            endPos = end - iniPos;
        }
        else
        {
            endPos = iniPos + Vector2.right * maxBulletDrop;
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
