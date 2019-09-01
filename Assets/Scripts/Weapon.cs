using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Weapon : MonoBehaviour
{
    public int dmg = 5;
    public int dmgBuf;

    [SerializeField] private float swingDelay = 0.1f;
    [SerializeField] private int expPerHit = 1;
    [SerializeField] private int numOfAnims = 3;
    [SerializeField] private float dmgPerStrPoint = 1;

    public bool swinging;

    [SerializeField] private float swingTime;
    [SerializeField] private float rotSpd;
    [SerializeField] private float shakeAmn;

    [SerializeField] private Animator animContr;
    [SerializeField] private Player plr;

    private Transform throwPos;
    private Transform handPos;

    GameMaster gm;
    Transform lastEnemy;

    Quaternion startRot;
    Vector3 mousePos;
    Vector3 difference;
    float temp;
    float timeToSwing;
    int swingNum;

    private void Start()
    {
        GameMaster.Instance.hud.AttackEvent += Swing;
        swingNum = 0;
        if (transform.GetComponent<Animator>()) animContr = transform.GetComponent<Animator>();

        if (transform.parent)
        {
            if (!plr && transform.parent.GetComponent<Player>())
            {
                if (transform.parent)
                {
                    if (transform.parent.GetComponent<Player>())
                    {
                        plr = transform.parent.GetComponent<Player>();
                        if (plr)
                        {
                            handPos = plr.transform.Find("HandPos").transform;
                        }
                    }
                }
            }
        }

        gm = GameMaster.Instance;
        startRot = transform.rotation;
    }

    void Update()
    {
        if (GameMaster.isTesting)
        {
            if (Input.GetMouseButton(0) && !PlrHUD.MouseOverUI())
            {
                Swing();
            }
        }

    }

    private void Swing()
    {
        if (animContr)
        {
            if (!animContr.GetBool("swingingNow") && Time.time > timeToSwing)
            {
                animContr.SetBool("swingingNow", true);
                animContr.SetInteger("swingNum", swingNum);

                if (swingNum == numOfAnims - 1) swingNum = 0;
                else swingNum++;
            }
        }
    }

    private void EnableDamage(float dmgPercent)
    {
        AudioMaster.Instance.Play("WeaponSwing");
        float _temp = dmg * dmgPercent + (Player.Stats.strengthStat * dmgPerStrPoint);
        dmgBuf = Mathf.RoundToInt(_temp);
        swinging = true;
    }

    void AnimationStop()
    {
        timeToSwing = Time.time + swingDelay;
        animContr.SetBool("swingingNow", false);
        swinging = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (swinging && this.enabled)
        {
            if (!other.isTrigger)
            {
                if (other.CompareTag("Enemy") && other.GetComponent<Enemy>())
                {
                    Enemy en = other.GetComponent<Enemy>();
                    en.TakeDamage(dmgBuf);
                    plr.plrStats.GainExp(expPerHit);
                    swinging = false;
                    gm.Shake(shakeAmn, 0.13f);
                }

                if (other.CompareTag("Resource") && other.GetComponent<ResourceGiverScript>())
                {
                    plr.plrStats.GainExp(expPerHit);
                    swinging = false;
                    gm.Shake(shakeAmn / 2, 0.1f);
                }

                swingTime = swingDelay + Time.time;
            }

        }
    }

    private IEnumerator AddActionToGamemaster()
    {
        while (GameMaster.Instance == null)
        {
            Debug.Log("GameMaster.Instance is null");
            yield return new WaitForSecondsRealtime(1f);
        }

        while (GameMaster.Instance.hud == null)
        {
            Debug.Log("GameMaster.Instance.hud is null");
            yield return new WaitForSecondsRealtime(1f);
        }

        GameMaster.Instance.hud.AttackEvent = Swing;
    }

    private void OnEnable()
    {
        if (GameMaster.Instance == null)
        {
            StartCoroutine(AddActionToGamemaster());
            return;
        }
        else
        {
            if (GameMaster.Instance.hud == null)
            {
                StartCoroutine(AddActionToGamemaster());
                return;
            }
        }

        GameMaster.Instance.hud.AttackEvent = Swing;
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