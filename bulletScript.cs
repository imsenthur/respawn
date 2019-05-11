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
    public Transform explo;
    public bool hit = false;
    // Update is called once per frame

    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * bulletSpeed);
        cPos = new Vector2(transform.position.x, transform.position.y) - iniPos;

        if(cPos.magnitude > endPos.magnitude + 0.25f)
        {
            Transform exlposion = Instantiate(explo, (endPos + iniPos), Quaternion.identity) as Transform;
            float size = Random.Range(0.8f, 1.0f);
            exlposion.localScale = new Vector3(size, size, 1);

            Destroy(exlposion.gameObject, 0.2f);

            Destroy(gameObject);
        }
        
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
