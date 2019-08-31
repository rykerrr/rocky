using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyGreatsword : Poolable
{
    [HideInInspector] public int dmg;
    [HideInInspector] public GameObject lastEnemyHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") && collision.gameObject != lastEnemyHit)
        {
            Enemy en = collision.GetComponent<Enemy>();
            en.TakeDamage(dmg);
            lastEnemyHit = collision.gameObject;
        }
    }

    private void OnDisable()
    {
        ReturnToPool();
    }
}
