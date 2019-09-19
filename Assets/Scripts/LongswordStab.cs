using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class LongswordStab : Ability
{
    public int dmg;

    [SerializeField] private float timeTilStab;
    [SerializeField] private float skillDelay;

    private Animator anim;
    private GameMaster gm;
    private PlrHUD plrHD;
    GameObject lastTarget;

    private bool attacking;
    private float attackTimer;

    void Start()
    {
        if (!anim) anim = transform.GetComponent<Animator>();
        if (!gm && FindObjectOfType<GameMaster>()) gm = FindObjectOfType<GameMaster>();
        if (!plrHD) plrHD = PlrHUD.Instance;
    }

    private void Update()
    {
        if (attackTimer > Time.time)
        {
            if (!plrHD) plrHD = PlrHUD.Instance;
            plrHD.ChangeSkillCooldown(attackTimer - Time.time, skillDelay);
        }

        if (Input.GetKey(KeyCode.Mouse1) && Time.time > attackTimer && !anim.GetBool("stabSkill") && !anim.GetBool("swingingNow") && !attacking)
        {
            StartCoroutine(Stab());
        }
    }

    public override void SkillCall()
    {
        if (Time.time > attackTimer && !anim.GetBool("stabSkill") && !anim.GetBool("swingingNow") && !attacking)
        {
            StartCoroutine(Stab());

        }
    }

    IEnumerator Stab()
    {
        plrHD.ChangeSkillCooldown(skillDelay, skillDelay);
        anim.SetBool("stabSkill", true);
        yield return new WaitForSeconds(timeTilStab);
        attacking = true;
        yield return new WaitForSeconds(1.3f - timeTilStab);
        anim.SetBool("stabSkill", false);
        attacking = false;
        attackTimer = Time.time + skillDelay;
        lastTarget = null;
        if (!plrHD) plrHD = PlrHUD.Instance;
        plrHD.ChangeSkillCooldown(attackTimer - Time.time, skillDelay);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attacking)
        {
            if (collision.CompareTag("Enemy") && collision.gameObject != lastTarget)
            {
                lastTarget = collision.gameObject;
                Enemy en = collision.transform.GetComponent<Enemy>();
                en.TakeDamage(dmg);
                gm.Shake(0.2f, 0.1f);
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
            GameMaster.Instance.wepChange.skillCall -= SkillCall;
        }
    }
}
#pragma warning restore 0649