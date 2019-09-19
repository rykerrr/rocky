using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class BowHandler : MonoBehaviour
{
    public int dmg = 5;

    [SerializeField] private int dmgBoostPerDexPoint = 2;
    [SerializeField] private int arrowCost = 3;
    [SerializeField] private float minDelayAmn = 0.3f;
    [SerializeField] private float baseAttackDelay = 3f;
    [SerializeField] private float delayBoostPerDexPoint = 0.03f;
    [SerializeField] private float attackDelay;
    [SerializeField] private float arrowLifeTime;
    [SerializeField] private LayerMask whatToNotMiss;
    [SerializeField] private Transform arrow;
    [SerializeField] private Transform firePos = null;
    [SerializeField] private Player plr;
    [SerializeField] private Animator anim;

    private float attackTimer;
    private RaycastHit2D hit;

    private int dmgWithoutWood;
    private int dmgBuf;

    private Poolable arrowPool;

    private void Awake()
    {
        arrowPool = arrow.GetComponent<Poolable>();
    }

    void Start()
    {
        GameMaster.Instance.hud.AttackEvent += Effects;

        if (!anim && GetComponent<Animator>())
        {
            anim = GetComponent<Animator>();
        }
        if (!plr && FindObjectOfType<Player>())
        {
            plr = FindObjectOfType<Player>();
        }
        if (!firePos && plr)
        {
            firePos = plr.transform.Find("fireTowards").GetComponent<Transform>();
        }

        dmgWithoutWood = Mathf.RoundToInt(dmg / 3);

        if (dmgWithoutWood <= 0)
        {
            dmgWithoutWood = 1;
        }

        CheckDelayWithStats();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < attackTimer)
        {
            if (anim.GetBool("isFiring") == false) anim.SetBool("isFiring", true);
        }

        if (Time.time > attackTimer && anim && anim.GetBool("isFiring") == true) anim.SetBool("isFiring", false);


    }

    private void CheckDelayWithStats()
    {
        if (attackDelay > minDelayAmn)
        {
            attackDelay = baseAttackDelay - (delayBoostPerDexPoint * Player.Stats.dexterityStat);
        }
        else return;
    }

    private void CheckDmgWithStats()
    {
        dmgBuf = dmg + (Player.Stats.dexterityStat * dmgBoostPerDexPoint);
    }

    private void Effects()
    {
        if ((Time.time > attackTimer))
        {
            attackTimer = Time.time + attackDelay;
            Vector3 arrowSpawnPos = new Vector3(firePos.position.x, firePos.position.y, -1);
            CheckDelayWithStats();
            CheckDmgWithStats();
            AudioMaster.Instance.Play("ArrowFire");

            if (anim && anim.GetBool("isFiring") == false) anim.SetBool("isFiring", true);
            GameObject bulClone = Poolable.Get(() => Poolable.CreateObj(arrow.gameObject), arrowPool.NameOfKey);
            bulClone.GetComponent<Arrow>().lifeTime = arrowLifeTime + Time.time;
            bulClone.transform.position = arrowSpawnPos;
            bulClone.transform.rotation = plr.transform.rotation;

            if (Player.woodResource > arrowCost)
            {
                bulClone.GetComponent<Arrow>().dmg = dmgBuf;
                Player.woodResource -= arrowCost;
            }
            else
            {
                bulClone.GetComponent<Arrow>().dmg = dmgWithoutWood;
            }

            bulClone.GetComponent<Rigidbody2D>().velocity = plr.transform.up * 40f;
        }
    }

    private IEnumerator AddActionToGamemaster()
    {
        while (GameMaster.Instance == null)
        {
            yield return new WaitForSecondsRealtime(1f);
        }

        while (GameMaster.Instance.hud == null)
        {
            yield return new WaitForSecondsRealtime(1f);
        }

        GameMaster.Instance.hud.AttackEvent = Effects;
    }

    private void OnEnable()
    {
        if (GameMaster.Instance == null)
        {
            StartCoroutine(AddActionToGamemaster());
            return;
        }
        else
        {
            if (GameMaster.Instance.hud == null)
            {
                StartCoroutine(AddActionToGamemaster());
                return;
            }
        }

        GameMaster.Instance.hud.AttackEvent = Effects;
    }

    private void OnDisable()
    {
        if (GameMaster.Instance)
        {
            GameMaster.Instance.hud.AttackEvent = delegate { };
        }
    }
}
#pragma warning restore 0649