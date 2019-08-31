using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Enemy : Poolable
{
    [System.Serializable]
    public class Stats
    {
        public int maxHP = 100;
        private int hp;
        [SerializeField] private float expOnDeath = 100;

        public int Health
        {
            get { return hp; }
            set { hp = value; }
        }
        public int RetMaxHP()
        {
            return maxHP;
        }
        public int RetExpOnDeath()
        {
            return (int) expOnDeath;
        }

        public void Init()
        {
            Health = RetMaxHP();
            expOnDeath = WaveSpawner.EXPMultiplier;
        }
    }

    [SerializeField] private Transform deathParticles;
    [SerializeField] private Transform plr;
    [SerializeField] private EnemyUIHandler enUi;

    public Stats enStats = new Stats();

    private void OnEnable()
    {
        enStats.Init();
        if (!enUi) enUi = GetComponent<EnemyUIHandler>();
    }

    public void TakeDamage(int _dmg)
    {
        if (enStats.Health - _dmg > 0)
        {
            enStats.Health -= _dmg;
            enUi.LowerHp(ReturnMaxHealth(), ReturnHealth());
            AudioMaster.Instance.Play("EnemyHurt");
        }
        else
        {
            KillEnemy();
            AudioMaster.Instance.Play("EnemyDeath");
        }
    }

    public int ReturnHealth()
    {
        return enStats.Health;
    }

    public int ReturnMaxHealth()
    {
        return enStats.RetMaxHP();
    }

    public void SetMaxHealth(int _maxHP)
    {
        if (!enUi)
        {
            return;
        }
        enStats.maxHP = _maxHP;
        enStats.Init();
        enUi.LowerHp(ReturnMaxHealth(), ReturnHealth());
    }

    private void KillEnemy()
    {
        if (!plr) plr = FindObjectOfType<Player>().transform;
        Transform deathPartClone = Instantiate(deathParticles, transform.position, transform.rotation) as Transform;
        plr.GetComponent<Player>().plrStats.GainExp(enStats.RetExpOnDeath());
        Destroy(deathPartClone.gameObject, 0.5f);
        ReturnToPool();
    }

}
#pragma warning restore 0649