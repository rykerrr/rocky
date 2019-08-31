using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class BossAI : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform target;

    [SerializeField] private bool hasDash = true;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float targetMinRange = 3f;
    [SerializeField] private float playerCheckDelay = 3f;
    [SerializeField] private float dashMaxRange;
    [SerializeField] private float attackDelay;
    [SerializeField] private float dashDelay;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;

    [SerializeField] private int dmg;
    [SerializeField] private int dashDmg;
    [SerializeField] private int amnOfAnims = 3;

    private Vector2 targPos;
    private Vector2 difference;

    private Collider2D[] isPlayerClose;

    private float timeLeftToCheck;
    private float angleZ;
    [SerializeField] private int i;
    [SerializeField] private float attackTimer;

    private GameMaster gm;
    private Rigidbody2D rb;
    private Animator anim;

    private bool swinging;
    private bool dashing;

    private float dashTimer;
    private float dashCooldown;
    private float normSpd;

    private void Start()
    {
        normSpd = speed;
        targPos = transform.position;
        if (!gm && FindObjectOfType<GameMaster>())
        {
            gm = FindObjectOfType<GameMaster>();
        }
        if (!anim)
        {
            anim = GetComponent<Animator>();
        }
        if (!rb)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            if (Time.time > attackTimer)
            {
                if(hasDash && !dashing)
                {
                    if(Vector2.Distance(transform.position, target.position) < dashMaxRange && Time.time > dashCooldown && Vector2.Distance(transform.position, target.position) > 5f)
                    {
                        Dash(target);
                    }
                }

                if (Vector2.Distance(transform.position, target.position) < targetMinRange / 2 && anim.GetInteger("swingingNow") == 0)
                {
                    i = Random.Range(1, amnOfAnims);
                    attackTimer = attackDelay + Time.time;
                    anim.SetInteger("swingingNow", i);
                }
            }
            else if (dashing && hasDash)
            {
                if(Time.time > dashTimer)
                {
                    dashing = false;
                    dashCooldown = dashDelay + Time.time;
                }
            }
        }

        if (Time.time > timeLeftToCheck)
        {
            CheckForPlayer();
            timeLeftToCheck = Time.time + playerCheckDelay;
        }
    }

    private void FixedUpdate()
    {
        if (target && !dashing)
        {
            targPos = target.position;
            Rotate();
            rb.velocity = transform.up * speed * -1;
        }
        else if (!dashing)
        {
            rb.velocity = Vector2.zero;
            transform.rotation = transform.rotation;
        }
    }

    private void Dash(Transform targ)
    {
        normSpd = speed;
        Rotate();
        rb.velocity = (transform.position - target.position).normalized * dashSpeed * -1;
        rb.rotation = 0f;
        dashing = true;
        dashTimer = Time.time + dashDuration;
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

    }

    private void Rotate()
    {
        difference = transform.position - new Vector3(targPos.x, targPos.y);
        difference.Normalize();
        angleZ = Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, angleZ * -1), rotationSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetMinRange);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (swinging && !dashing)
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (dashing && hasDash)
        {
            if (collision.collider.CompareTag("Player"))
            {
                Player plrScr;
                plrScr = collision.collider.GetComponent<Player>();
                plrScr.TakeDamage(dashDmg);
                dashing = false;
                dashCooldown = dashDelay + Time.time;
                gm.Shake(1.2f, 0.1f);
            }
        }
    }
}
#pragma warning restore 0649