using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrow : Poolable
{
    public int dmg;
    public float lifeTime = 0.1f;

    private GameMaster gm;

    private void Start()
    {
        gm = GameMaster.Instance;
    }

    private void OnEnable()
    {
        lifeTime += Time.time;
    }

    private void Update()
    {
        if (Time.time > lifeTime)
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
                if (other.CompareTag("Player"))
                {
                    other.GetComponent<Player>().TakeDamage(dmg);
                    gm.Shake(0.25f, 0.1f);
                }
                ReturnToPool();
                AudioMaster.Instance.Play("ArrowImpact");
            }

        }
    }
}
