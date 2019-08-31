using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class BowmanAI : MonoBehaviour
{
    public Transform objectContainer;

    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private LayerMask whatCanBeHit;
    [SerializeField] private Transform target;
    [SerializeField] private Transform arrowPref;
    [SerializeField] private Transform firePos;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float targetMinRange = 3f;
    [SerializeField] private float playerCheckDelay = 3f;
    [SerializeField] private float attackRange;
    [SerializeField] private float arrowLifeTime;
    [SerializeField] private float attackDelay;
    [SerializeField] private float arrowSpeed;

    [SerializeField] private int dmg;

    private Vector2 targPos;
    private Vector2 difference;

    private Collider2D[] isPlayerClose;

    private float timeLeftToCheck;
    private float angleZ;
    private float attackTimer;

    private GameMaster gm;
    private Rigidbody2D rb;
    private Animator anim;
    private Poolable arrowPool;

    private void Start()
    {
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
        if (!firePos)
        {
            firePos = transform.Find("FirePoint").transform;
        }

        arrowPool = arrowPref.GetComponent<Poolable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timeLeftToCheck)
        {
            CheckForPlayer();
            timeLeftToCheck = Time.time + playerCheckDelay;
        }

        if (target)
        {
            if (Vector2.Distance(target.position, transform.position) <= attackRange)
            {
                if(Time.time > attackTimer)
                {
                    Fire();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if(target)
        {
            targPos = target.position;
            Rotate();
            if (Vector2.Distance(target.position, transform.position) > attackRange)
            {
                rb.velocity = transform.up * speed * -1;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else if (!target)
        {
            rb.velocity = Vector2.zero;
            transform.rotation = transform.rotation;
        }
    }

    public void Initialize(GameMaster _gm)
    {
        gm = _gm;
    }

    private void Fire()
    {
        GameObject arrowClone = Poolable.Get(() => Poolable.CreateObj(arrowPref.gameObject), arrowPool.NameOfKey);
        arrowClone.transform.position = firePos.position;
        arrowClone.transform.rotation = transform.rotation;
        EnemyArrow enArrow = arrowClone.GetComponent<EnemyArrow>();

        arrowClone.GetComponent<Rigidbody2D>().velocity = arrowSpeed * transform.up * -1f;
        enArrow.lifeTime = arrowLifeTime + Time.time;
        enArrow.dmg = dmg;

        AudioMaster.Instance.Play("ArrowFire");
        if (objectContainer) arrowClone.transform.parent = objectContainer;
        attackTimer = attackDelay + Time.time;
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
}
#pragma warning restore 0649