using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Arrow : Poolable
{
    public int dmg;
    public int pierceCount = 0;
    public float lifeTime = 0f;
    private GameObject lastEnemyHit;

    private void OnEnable()
    {
        lifeTime += Time.time;
    }
    
    private void Update()
    {
        if(Time.time > lifeTime)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enabled)
        {
            if (!other.isTrigger)
            {
                if (other.CompareTag("Enemy") && other.gameObject != lastEnemyHit)
                {
                    lastEnemyHit = other.gameObject;
                    Enemy en = other.GetComponent<Enemy>();
                    en.TakeDamage(dmg);
                }

                if(pierceCount <= 0)
                {
                    ReturnToPool();
                }

                AudioMaster.Instance.Play("ArrowImpact");

                pierceCount--;
            }

        }
    }
}
#pragma warning restore 0649