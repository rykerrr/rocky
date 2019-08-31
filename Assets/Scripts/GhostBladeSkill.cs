using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class GhostBladeSkill : Ability
{
    public int dmg;

    [SerializeField] private float skillDelay;

    private Animator anim;
    private GameMaster gm;
    private PlrHUD plrHD;

    private bool attacking;
    private float attackTimer;

    void Start()
    {
        if (!anim) anim = transform.GetComponent<Animator>();
        if (!gm) gm = FindObjectOfType<GameMaster>();
        if (!plrHD) plrHD = PlrHUD.Instance;
    }

    private void Update()
    {
        if (attackTimer > Time.time)
        {
            if (!plrHD) plrHD = PlrHUD.Instance;
            plrHD.ChangeSkillCooldown(attackTimer - Time.time, skillDelay);
        }
    }

    public void SkillCall()
    {
        if (Time.time > attackTimer && !anim.GetBool("ghostSkill") && !anim.GetBool("swingingNow") && !attacking)
        {
            GhostStab();
        }

    }

    private void GhostStab()
    {
        if (!plrHD) plrHD = PlrHUD.Instance;
        plrHD.ChangeSkillCooldown(skillDelay, skillDelay);
        anim.SetBool("ghostSkill", true);
        attacking = true;
    }

    private void StopStab()
    {
        attacking = false;
        attackTimer = Time.time + skillDelay;
        anim.SetBool("ghostSkill", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attacking)
        {
            if (collision.CompareTag("Enemy"))
            {
                Enemy en = collision.transform.GetComponent<Enemy>();
                if (en.ReturnHealth() < en.ReturnMaxHealth() / 5)
                {
                    en.TakeDamage(en.ReturnHealth() + 5);
                    gm.Shake(0.35f, 0.1f);
                }
                else
                {
                    en.TakeDamage(dmg);
                    gm.Shake(0.2f, 0.1f);
                }
            }
            attacking = false;
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