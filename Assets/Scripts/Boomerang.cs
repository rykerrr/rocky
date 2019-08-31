using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Boomerang : MonoBehaviour
{
    public int dmg;

    [SerializeField] private Transform plr;

    [SerializeField] private float speed;
    [SerializeField] private float throwTime;
    [SerializeField] private float throwDelay;
    [SerializeField] private float slowSpd;

    [SerializeField] private int dmgPerDexPoint;

    [SerializeField] private bool throwing = false;
    [SerializeField] private bool returning = false;

    private Transform handPos;
    private Rigidbody2D rb;

    private Player plrScr;

    private float delayTimer;
    private float inAirTime;

    private int dmgBuf;

    private void Start()
    {
        if (!plr && FindObjectOfType<Player>()) plr = FindObjectOfType<Player>().transform;
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!handPos) handPos = plr.Find("HandPos").transform;
        if (!plrScr) plrScr = plr.GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && !PlrHUD.MouseOverUI())
        {
            Throw();
        }

        if (throwing && Time.time > inAirTime)
        {
            returning = true;
        }
    }

    private void FixedUpdate()
    {
        if (throwing)
        {
            transform.Rotate(new Vector3(0f, 0f, 1f), 20f);
            if (returning)
            {
                rb.velocity = (handPos.position - transform.position).normalized * speed * Time.deltaTime;
                if (Vector2.Distance(transform.position, handPos.position) < 0.7f)
                {
                    EndThrow();
                }
            }
        }
    }

    private void Throw()
    {
        if (!throwing && Time.time > delayTimer)
        {
            CheckDmgWithStats();
            plrScr.ChangeSpd(slowSpd);
            Vector3 flyPath;
            transform.parent = null;
            throwing = true;
            flyPath = plr.up;
            rb.velocity = flyPath * speed * Time.deltaTime;
            inAirTime = throwTime + Time.time;
        }
    }

    private void EndThrow()
    {
        plrScr.ChangeSpd(1f);
        delayTimer = throwDelay + Time.time;
        rb.velocity = Vector3.zero;
        transform.parent = plr;
        transform.position = handPos.position;
        transform.localEulerAngles = Vector3.zero;
        returning = false;
        throwing = false;
    }

    private void CheckDmgWithStats()
    {
        dmgBuf = dmg + (dmgPerDexPoint * Player.Stats.dexterityStat);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (throwing)
        {
            if (!collision.isTrigger)
            {
                if (collision.CompareTag("Enemy"))
                {
                    collision.GetComponent<Enemy>().TakeDamage(dmgBuf);
                }

                returning = true;
            }

        }
    }

    private void OnEnable()
    {
        GameMaster.Instance.hud.AttackEvent = Throw;
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