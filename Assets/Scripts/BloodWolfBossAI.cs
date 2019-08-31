using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class BloodWolfBossAI : MonoBehaviour
{
    private enum States { ATTACKING, FINDING, IDLE }
    [SerializeField] private States curState;

    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform target;
    [SerializeField] private float rotPhaseDuration;
    [SerializeField] private float attackPhaseDuration;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float targetMinRange = 3f;
    [SerializeField] private float playerCheckDelay = 3f;
    [SerializeField] private float attackDelay;
    [SerializeField] private float distanceBeforeSpeedUp;
    [SerializeField] private float speedWhenSpedUp;

    public float speed = 5f;
    public int dmg;

    private Vector2 targPos;
    private Vector2 difference;

    private Collider2D[] isPlayerClose;

    private float timeLeftToCheck;
    private float angleZ;
    private float normSpd;
    private float normRotSpd;
    private float attackTimer;

    private GameMaster gm;

    private bool isAvailable = false;

    private void Start()
    {
        targPos = transform.position;
        if (!gm && FindObjectOfType<GameMaster>())
        {
            gm = FindObjectOfType<GameMaster>();
        }

        normSpd = speed;
        normRotSpd = rotationSpeed;
        gm.GetComponent<DarknessEffect>().DarkenScreen();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeLeftToCheck)
        {
            CheckForPlayer();
            timeLeftToCheck = Time.time + playerCheckDelay;
        }
    }

    private void FixedUpdate()
    {
        if (target)
        {
            targPos = target.position;
            if (Vector2.Distance(transform.position, targPos) < distanceBeforeSpeedUp && curState == States.FINDING)
            {
                StartCoroutine(SpeedUp());
            }
            else if (curState == States.ATTACKING && isAvailable)
            {
                StartCoroutine(AngleTowardsPlayer());
            }
        }
        else
        {
            StartCoroutine(AngleTowardsPlayer());
        }

        Rotate();
        Move();
    }

    private IEnumerator SpeedUp()
    {
        curState = States.ATTACKING;
        isAvailable = false;
        speed = speedWhenSpedUp;
        rotationSpeed = 0.01f;
        yield return new WaitForSeconds(attackPhaseDuration);
        isAvailable = true;
    }

    private IEnumerator AngleTowardsPlayer()
    {
        isAvailable = true;
        speed = normSpd;
        rotationSpeed = normRotSpd;
        curState = States.IDLE;
        yield return new WaitForSeconds(rotPhaseDuration);
        curState = States.FINDING;
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

    private void Move()
    {
        transform.Translate(Vector2.up * speed * -1);
    }

    private void OnDisable()
    {
        if (gm)
        {
            gm.GetComponent<DarknessEffect>().ReturnScreenToNormalLight();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetMinRange);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time > attackTimer && other.GetComponent<Player>())
        {
            attackTimer = attackDelay + Time.time;
            Player plrScr;
            plrScr = other.GetComponent<Player>();
            plrScr.TakeDamage(dmg);
            gm.Shake(0.14f, 0.1f);
        }
    }
}
#pragma warning restore 0649