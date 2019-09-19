using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class BKHSlam : Ability
{
    public int dmg;

    [SerializeField] private Transform crackPref;

    [SerializeField] private float slamDelay;
    [SerializeField] private float timeBeforeSlam;
    [SerializeField] private float slamRadius;

    [SerializeField] private LayerMask whatToHit;
    [SerializeField] private GameMaster gm;

    private PlrHUD plrHD;
    private Animator anim;
    private Collider2D[] slamColliders;

    private float slamTimer;

    void Start()
    {
        if (!anim) anim = transform.GetComponent<Animator>();
        if (!gm) gm = FindObjectOfType<GameMaster>();
        if (!plrHD) plrHD = PlrHUD.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (slamTimer > Time.time && !anim.GetBool("groundSlam"))
        {
            if (!plrHD) plrHD = PlrHUD.Instance;
            plrHD.ChangeSkillCooldown(slamTimer - Time.time, slamDelay);
        }
    }

    public override void SkillCall()
    {
        if (!anim.GetBool("swingingNow") && !anim.GetBool("groundSlam") && Time.time > slamTimer)
        {
            StartCoroutine(GroundSlam());
        }

    }

    IEnumerator GroundSlam()
    {
        plrHD.ChangeSkillCooldown(slamDelay, slamDelay);
        anim.SetBool("groundSlam", true);
        yield return new WaitForSeconds(timeBeforeSlam);
        gm.Shake(2.3f, 0.12f);

        Instantiate(crackPref, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
        slamColliders = Physics2D.OverlapCircleAll(transform.position, slamRadius, whatToHit);
        for (int i = 0; i < slamColliders.Length; i++)
        {
            if (slamColliders[i].CompareTag("Enemy"))
            {
                Enemy en = slamColliders[i].GetComponent<Enemy>();
                en.TakeDamage(dmg);
            }
        }
        yield return new WaitForSeconds(3f - timeBeforeSlam);
        anim.SetBool("groundSlam", false);
        slamTimer = slamDelay + Time.time;
        plrHD.ChangeSkillCooldown(slamTimer - Time.time, slamDelay);
        if (!plrHD) plrHD = PlrHUD.Instance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slamRadius);
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