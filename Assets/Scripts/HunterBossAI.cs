using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class HunterBossAI : MonoBehaviour
{
    private enum AttackingStates { LOCKED_ON, DODGING, IDLE, SWORD_ATTACK, BOW_ATTACK, SKILL_1, SKILL_2, SKILL_3 }
    [SerializeField] private AttackingStates curState;

    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform target;
    [SerializeField] private Transform targetToFireAt;
    [SerializeField] private Transform objectContainer;
    [SerializeField] private Transform firePos;
    [SerializeField] private Transform arrowPref;
    [SerializeField] private Transform circlePref;

    [SerializeField] private int dmg;
    [SerializeField] private int arrowDmg;
    [SerializeField] private int arrowSkillDmg;
    [SerializeField] private int amnOfAnims;
    [SerializeField] private int amnOfCircles;
    [SerializeField] private int amnOfSkills;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float arrowLifeTime;
    [SerializeField] private float arrowSpeed;
    [SerializeField] private float orbitSpeed;
    [SerializeField] private float attackDelay;
    [SerializeField] private float skillDelay;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float targetMinRange;
    [SerializeField] private float maxRetreatRange;
    [SerializeField] private float playerCheckDelay;

    public float orbitDistance;

    public float orbitLockOnDuration;
    public float orbitLockOnDelay;
    public float dodgeSpeed;
    public float dodgeTime;

    public bool canDodge = true;

    private bool dodging;
    private bool swinging;
    private bool firing;

    private float orbitLockonDelayTimer;
    private float orbitLockonDurationTimer;
    private float timeLeftToCheck;
    private float attackTimer;
    private float skillTimer;

    private float angleZ;
    private float timeToChangeDirDelay = 1f;
    private float timeToChangeDir;

    private int i;
    private int dir;

    private Vector2 difference;
    private Vector2 normVel;

    private Collider2D[] isPlayerClose;
    private Rigidbody2D rb;
    private Animator anim;

    private Enemy thisEn;
    private GameMaster gm;
    private Poolable arrowPool;

    // Start is called before the first frame update
    void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!thisEn) thisEn = GetComponent<Enemy>();
        if (!anim) anim = GetComponent<Animator>();
        if (!gm) gm = FindObjectOfType<GameMaster>();
        arrowPool = arrowPref.GetComponent<Poolable>();
        curState = AttackingStates.IDLE;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!target)
        {
            curState = AttackingStates.IDLE;
        }

        if (Time.time > timeLeftToCheck && ((curState != AttackingStates.SKILL_1) || (curState != AttackingStates.SKILL_2)))
        {
            CheckForPlayer();
            timeLeftToCheck = Time.time + playerCheckDelay;
            if (target && curState == AttackingStates.IDLE)
            {
                if (thisEn.enStats.Health < thisEn.enStats.RetMaxHP() / 3)
                {
                    curState = AttackingStates.BOW_ATTACK;
                }
                else
                {
                    curState = AttackingStates.SWORD_ATTACK;
                }
            }
        }

        if (curState == AttackingStates.LOCKED_ON)
        {
            if (Time.time > orbitLockonDurationTimer)
            {
                if (thisEn.enStats.Health < thisEn.enStats.RetMaxHP() / 3)
                {
                    curState = AttackingStates.BOW_ATTACK;
                }
                else
                {
                    curState = AttackingStates.SWORD_ATTACK;
                }

                orbitLockonDelayTimer = Time.time + orbitLockOnDelay;
            }
        }

        if (target)
        {
            if (Vector2.Distance(target.position, transform.position) <= orbitDistance)
            {
                if ((curState == AttackingStates.SWORD_ATTACK || curState == AttackingStates.BOW_ATTACK) && Time.time > orbitLockonDelayTimer)
                {
                    int rng = Random.Range(0, 2);
                    if (rng == 0)
                    {
                        dir = 1;
                    }
                    else
                    {
                        dir = -1;
                    }
                    curState = AttackingStates.LOCKED_ON;
                    orbitLockonDurationTimer = Time.time + orbitLockOnDuration;
                }
            }

            if (curState == AttackingStates.SWORD_ATTACK)
            {
                if (Time.time > attackTimer)
                {
                    if (Vector2.Distance(target.position, transform.position) <= targetMinRange / 2 && anim.GetInteger("swingingNow") == 0)
                    {
                        i = Random.Range(1, amnOfAnims);
                        attackTimer = attackDelay + Time.time;
                        anim.SetInteger("swingingNow", i);
                    }
                }
            }
            else if (curState == AttackingStates.LOCKED_ON || curState == AttackingStates.BOW_ATTACK)
            {
                if (Time.time > attackTimer && anim.GetInteger("swingingNow") == 0)
                {
                    StartCoroutine(Fire(0.4f, arrowDmg, arrowSpeed));
                }

                if (Time.time > skillTimer && amnOfCircles > 0)
                {
                    int rng = Random.Range(0, amnOfSkills);
                    if (rng == 0)
                    {
                        StartCoroutine(Skill1RapidFire());
                        curState = AttackingStates.SKILL_1;
                    }
                    else if (rng == 1)
                    {
                        StartCoroutine(Skill2PredictionFire());
                        curState = AttackingStates.SKILL_2;
                    }
                    else if (rng == 2)
                    {
                        StartCoroutine(Skill3CenterRandomFire());
                        curState = AttackingStates.SKILL_3;
                    }
                }
            }

        }


    }

    private void FixedUpdate()
    {
        if (!dodging)
        {
            if (curState == AttackingStates.LOCKED_ON)
            {
                if (Time.time > timeToChangeDir)
                {
                    if (target && !firing)
                    {
                        if (Vector2.Distance(transform.position, target.position) < maxRetreatRange)
                        {
                            rb.velocity = (transform.up * (moveSpeed / 2)) + (transform.right * moveSpeed * dir);
                        }
                        else if (Vector2.Distance(transform.position, target.position) > orbitDistance)
                        {
                            rb.velocity = (transform.right * moveSpeed * dir) + (transform.up * -1 * moveSpeed);
                        }
                        else
                        {
                            rb.velocity = transform.right * moveSpeed * dir;
                        }

                        timeToChangeDir = Time.time + timeToChangeDirDelay;
                    }

                }

            }

            if (curState == AttackingStates.SWORD_ATTACK)
            {
                rb.velocity = transform.up * moveSpeed * -1;
            }

            if (curState == AttackingStates.BOW_ATTACK)
            {
                if (Vector2.Distance(transform.position, target.position) < maxRetreatRange)
                {
                    rb.velocity = transform.up * (moveSpeed / 2);
                }
            }

            if (curState == AttackingStates.SKILL_1 || curState == AttackingStates.SKILL_2 || curState == AttackingStates.SKILL_3)
            {
                rb.velocity = Vector2.zero;
            }

            if (curState == AttackingStates.IDLE)
            {
                rb.velocity = Vector2.zero;
            }
        }

        if (targetToFireAt)
        {
            Rotate();
        }

    }

    private void CheckForPlayer()
    {
        isPlayerClose = Physics2D.OverlapCircleAll(transform.position, targetMinRange, whatIsPlayer);
        bool targetYesNo = false;
        for (int i = 0; i < isPlayerClose.Length; i++)
        {
            if (isPlayerClose[i].CompareTag("Player"))
            {
                targetYesNo = true;
                target = isPlayerClose[i].GetComponent<Transform>();
            }
        }
        if (!targetYesNo)
        {
            target = null;
        }

        if (target)
        {
            targetToFireAt = target;
        }
    }

    private void Rotate(/*float rotSpd*/)
    {
        difference = transform.position - targetToFireAt.position;
        difference.Normalize();
        angleZ = Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, angleZ * -1), rotSpd);
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angleZ * -1);
    }


    public IEnumerator Dodge(int direction, float duration)
    {
        if (canDodge)
        {
            yield return new WaitForSeconds(0.1f);

            if (direction == 0)
            {
                rb.velocity = transform.right * dodgeSpeed;
                dodging = true;
            }
            else if (direction == 1)
            {
                rb.velocity = transform.right * dodgeSpeed * -1;
                dodging = true;
            }
            else if (direction == 5)
            {
                rb.velocity = transform.up * dodgeSpeed;
                dodging = true;
            }

            yield return new WaitForSeconds(duration);
            rb.velocity = Vector2.zero;
            dodging = false;
        }
    }

    private IEnumerator Fire(float waitTime, int _dmg, float _arrowSpeed)
    {
        firing = true;
        normVel = rb.velocity;
        rb.velocity = Vector2.zero;

        attackTimer = Time.time + 9999f;
        anim.SetBool("firingNow", true);
        yield return new WaitForSeconds(waitTime);
        GameObject arrowClone = Poolable.Get(() => Poolable.CreateObj(arrowPref.gameObject), arrowPool.NameOfKey);
        arrowClone.transform.position = firePos.position;
        arrowClone.transform.rotation = transform.rotation;

        AudioMaster.Instance.Play("ArrowFire");
        arrowClone.GetComponent<Rigidbody2D>().velocity = _arrowSpeed * transform.up * -1f;
        EnemyArrow enArrow = arrowClone.GetComponent<EnemyArrow>();
        enArrow.dmg = _dmg;
        enArrow.lifeTime = arrowLifeTime + Time.time;
        if (objectContainer) arrowClone.transform.parent = objectContainer;
        anim.SetBool("firingNow", false);
        attackTimer = attackDelay + Time.time;

        rb.velocity = normVel;
        firing = false;
    }

    private IEnumerator Skill1RapidFire()
    {
        StartCoroutine(Dodge(5, dodgeTime / 2));
        skillTimer = Time.time + 9999f;
        canDodge = false;
        yield return new WaitForSeconds(0.5f);

        Transform[] circlesToFireAt = new Transform[amnOfCircles];
        Vector2 posToSpawn;

        for (int i = 0; i < amnOfCircles; i++)
        {
            posToSpawn = new Vector2(target.position.x + Random.Range(-6.5f, 6.5f), target.position.y + Random.Range(-2.5f, 2.5f));
            circlesToFireAt[i] = Instantiate(circlePref, posToSpawn, Quaternion.identity) as Transform;
            targetToFireAt = circlesToFireAt[i];
            StartCoroutine(Fire(0f, arrowSkillDmg, arrowSpeed * 0.8f));
            Destroy(circlesToFireAt[i].gameObject, 0.25f);
            yield return new WaitForSeconds(0.08f);
            // circlesToFireAt[i].parent = objectContainer;
        }

        targetToFireAt = target;
        skillTimer = Time.time + skillDelay;
        curState = AttackingStates.LOCKED_ON;
        canDodge = true;
        CheckForPlayer();
    }

    private IEnumerator Skill2PredictionFire()
    {
        StartCoroutine(Dodge(5, dodgeTime / 3));
        skillTimer = Time.time + 9999f;
        canDodge = false;
        yield return new WaitForSeconds(0.5f);

        Transform[] circlesToFireAt = new Transform[amnOfCircles];
        Vector2 posToSpawn;

        for (int i = 0; i < amnOfCircles; i++)
        {
            posToSpawn = target.position + (new Vector3(target.GetComponent<Rigidbody2D>().velocity.x, target.GetComponent<Rigidbody2D>().velocity.y, 0f) * Time.deltaTime) * i * 2.7f;
            circlesToFireAt[i] = Instantiate(circlePref, posToSpawn, Quaternion.identity) as Transform;
            targetToFireAt = circlesToFireAt[i];
            StartCoroutine(Fire(0f, arrowSkillDmg, arrowSpeed * 1.1f));
            Destroy(circlesToFireAt[i].gameObject, 0.25f);
            yield return new WaitForSeconds(0.1f);
            // circlesToFireAt[i].parent = objectContainer;
        }

        targetToFireAt = target;
        skillTimer = Time.time + skillDelay;
        curState = AttackingStates.LOCKED_ON;
        canDodge = true;
        CheckForPlayer();
    }

    private IEnumerator Skill3CenterRandomFire()
    {
        skillTimer = Time.time + 9999f;
        canDodge = false;
        yield return new WaitForSeconds(0.5f);
        float x;
        float y;

        Transform[] circlesToFireAt = new Transform[amnOfCircles * 3];
        Vector2 posToSpawn;

        for (int i = 0; i < amnOfCircles * 3; i++)
        {
            x = Random.Range(-10f, 10f);
            y = Random.Range(-10f, 10f);

            if (x < 0)
            {
                Mathf.Clamp(x, -5f, -3f);
            }
            else
            {
                Mathf.Clamp(x, 3f, 5f);
            }

            if (y < 0)
            {
                Mathf.Clamp(y, -5f, -3f);
            }
            else
            {
                Mathf.Clamp(y, 3f, 5f);
            }

            posToSpawn = new Vector2(transform.position.x + x, transform.position.y + y);
            circlesToFireAt[i] = Instantiate(circlePref, posToSpawn, Quaternion.identity) as Transform;
            targetToFireAt = circlesToFireAt[i];
            StartCoroutine(Fire(0f, arrowSkillDmg, arrowSpeed * 0.9f));
            Destroy(circlesToFireAt[i].gameObject, 0.25f);
            yield return new WaitForSeconds(0.14f);
            // circlesToFireAt[i].parent = objectContainer;
        }

        targetToFireAt = target;
        skillTimer = Time.time + skillDelay;
        curState = AttackingStates.LOCKED_ON;
        canDodge = true;
        CheckForPlayer();
    }

    private void AnimationStop()
    {
        attackTimer = attackDelay + Time.time;
        anim.SetInteger("swingingNow", 0);
        swinging = false;
    }

    private void StopDamage()
    {
        swinging = false;
    }

    private void EnableDamage()
    {
        swinging = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (swinging)
        {
            if (other.CompareTag("Player"))
            {
                swinging = false;
                Player plrScr;
                plrScr = other.GetComponent<Player>();
                plrScr.TakeDamage(dmg);
                gm.Shake(0.17f, 0.2f);
            }

            attackTimer = attackDelay + Time.time;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, orbitDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, targetMinRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxRetreatRange);
    }


    private void OnDisable()
    {
        StatManager.currentCoins += 400;
    }
}
#pragma warning restore 0649