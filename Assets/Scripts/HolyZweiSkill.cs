using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class HolyZweiSkill : Ability
{
    [SerializeField] private Transform plr;
    [SerializeField] private Transform holySwordPref;

    [SerializeField] private float speed;
    [SerializeField] private float throwTime;
    [SerializeField] private float throwDelay;
    [SerializeField] private float slowSpd;

    public int dmg;

    private bool throwing = false;
    private bool returning = false;
    private bool retHit = true;

    private Transform handPos;
    private GameObject holySword;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector3 retPoint;

    private Player plrScr;
    private Weapon wep;
    private PlrHUD plrHD;
    private Poolable holySwordPool;

    private float delayTimer;
    private float inAirTime;

    private int buff;

    private void Awake()
    {
        holySwordPool = holySwordPref.GetComponent<Poolable>();
    }

    private void Start()
    {
        if (!plr && FindObjectOfType<Player>()) plr = FindObjectOfType<Player>().transform;
        if (!handPos) handPos = plr.Find("HandPos").transform;
        if (!plrScr) plrScr = plr.GetComponent<Player>();
        if (!wep) wep = GetComponent<Weapon>();
        if (!anim) anim = GetComponent<Animator>();
        if (!plrHD) plrHD = PlrHUD.Instance;
    }

    private void Update()
    {
        if (delayTimer > Time.time)
        {
            if (!plrHD) plrHD = PlrHUD.Instance;
            plrHD.ChangeSkillCooldown(delayTimer - Time.time, throwDelay);
        }

        if (throwing && Time.time > inAirTime)
        {
            returning = true;
        }
    }

    public void SkillCall()
    {
        if (Time.time > delayTimer && !throwing && !returning && !anim.GetBool("swingingNow"))
        {
            StartCoroutine(startThrow());
        }
    }

    private IEnumerator startThrow()
    {
        plrHD.ChangeSkillCooldown(throwDelay, throwDelay);
        anim.SetBool("bladeThrow", true);
        buff = wep.dmg;
        wep.dmg = 0;
        yield return new WaitForSeconds(1.05f);
        retPoint = handPos.position;
        holySword = Poolable.Get(() => Poolable.CreateObj(holySwordPref.gameObject), holySwordPool.NameOfKey);
        holySword.transform.position = transform.position;
        holySword.transform.rotation = transform.rotation;
        holySword.GetComponent<HolyGreatsword>().dmg = dmg * 2;
        rb = holySword.GetComponent<Rigidbody2D>();
        Vector3 flyPath;
        flyPath = plr.up;
        rb.velocity = flyPath * speed * Time.deltaTime;
        inAirTime = throwTime + Time.time;
        throwing = true;
        yield return new WaitForSeconds(2.00f - 1.05f);
        anim.SetBool("bladeThrow", false);
        delayTimer = Time.time + throwDelay;
        if (!plrHD) plrHD = PlrHUD.Instance;
        plrHD.ChangeSkillCooldown(delayTimer - Time.time, throwDelay);
    }

    private void FullReturn()
    {
        holySword.SetActive(false);
        throwing = false;
        returning = false;
        retHit = true;
    }

    private void FixedUpdate()
    {
        if (holySword != null)
        {
            holySword.transform.Rotate(new Vector3(0f, 0f, 1f), 15f);
            wep.dmg = buff;
            if (returning)
            {
                if (retHit)
                {
                    holySword.GetComponent<HolyGreatsword>().lastEnemyHit = null;
                    retHit = false;
                }
                rb.velocity = (retPoint - holySword.transform.position).normalized * speed * Time.deltaTime * 1.8f;
                if (Vector2.Distance(holySword.transform.position, retPoint) < 2f)
                {
                    FullReturn();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (throwing)
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<Enemy>().TakeDamage(dmg);
            }
        }

    }

    private void OnEnable()
    {
        GameMaster.Instance.wepChange.skillCall += SkillCall;
    }

    private void OnDisable()
    {
        if (GameMaster.Instance)
        {
            GameMaster.Instance.wepChange.skillCall += SkillCall;
        }

        if (holySword != null)
        {
            holySword.SetActive(false);
        }
    }
}
#pragma warning restore 0649