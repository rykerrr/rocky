using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class KopeshFlurryAttack : Ability
{
    public int dmg;

    [SerializeField] private float duration;
    [SerializeField] private float skillDelay;
    [SerializeField] private float flurrySwingDelay = 0.3f;

    private float skillTimer;

    private Animator anim;
    private GameMaster gm;
    private PlrHUD plrHD;
    private bool isAttacking;

    private float flurrySwingTimer;

    private void Start()
    {
        if (!anim) anim = transform.GetComponent<Animator>();
        if (!gm && FindObjectOfType<GameMaster>()) gm = FindObjectOfType<GameMaster>();
        if (!plrHD) plrHD = PlrHUD.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (skillTimer > Time.time)
        {
            plrHD.ChangeSkillCooldown(skillTimer - Time.time, skillDelay);
        }
    }

    public override void SkillCall()
    {
        if (!anim.GetBool("swingingNow") && !anim.GetBool("flurryAttack") && Time.time > skillTimer)
        {
            StartCoroutine(FlurryAttack());
        }
    }

    IEnumerator FlurryAttack()
    {
        plrHD.ChangeSkillCooldown(skillDelay, skillDelay);
        anim.SetBool("flurryAttack", true);
        isAttacking = true;

        yield return new WaitForSeconds(duration);
        anim.SetBool("flurryAttack", false);
        isAttacking = false;

        skillTimer = skillDelay + Time.time;
        plrHD.ChangeSkillCooldown(skillTimer - Time.time, skillDelay);
        if (!plrHD) plrHD = PlrHUD.Instance;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (isAttacking && Time.time > flurrySwingTimer)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy en = collider.GetComponent<Enemy>();
                en.TakeDamage(dmg);
                gm.Shake(0.1f, 0.1f);
                flurrySwingTimer = Time.time + flurrySwingDelay;
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