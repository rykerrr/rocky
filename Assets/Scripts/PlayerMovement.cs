using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb = null;
    public Joystick movJoystick;
    public int dashDmg;
    public float speed = 5f;
    public float dashSpeed;
    public float dashDelay;
    public float dashTime;
    public float normSpd;

    private float movement;
    private float movX;
    private float movY;

    private float dashTimer;
    private float dashCooldown;
    private float timer;

    private bool dashing = false;
    private bool isSlowed = false;

    private Player plrScr;

    void Awake()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (!plrScr)
        {
            plrScr = GetComponent<Player>();
        }
        normSpd = speed;
    }

    private void Update()
    {
        if (Time.time > dashTimer && dashing)
        {
            dashing = false;
            dashCooldown = dashDelay + Time.time;
        }

        if (Time.time > timer && isSlowed)
        {
            speed = normSpd;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dash();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dashing)
        {
            if (GameMaster.isTesting)
            {
                movX = Input.GetAxisRaw("Horizontal");
                movY = Input.GetAxisRaw("Vertical");
            }
            else
            {
                movX = movJoystick.Horizontal;
                movY = movJoystick.Vertical;
            }

            rb.velocity = new Vector2(movX * speed, movY * speed);
        }
    }

    private void Dash()
    {
        if (Time.time > dashCooldown && !dashing)
        {
            dashing = true;

            rb.velocity = transform.up * dashSpeed;

            dashTimer = dashTime + Time.time;
        }
    }

    public void Slow(float slowAmn, float slowLength = 1f)
    {
        timer = slowLength + Time.time;
        speed *= slowAmn;
        isSlowed = true;
    }

    public void Slow(float slowAmn)
    {
        if (slowAmn == 1f)
        {
            speed = normSpd;
            return;
        }
        speed *= slowAmn;
        isSlowed = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dashing)
        {
            if (!collision.collider.isTrigger)
            {
                if (collision.collider.GetComponent<Enemy>())
                {
                    Enemy en = collision.collider.GetComponent<Enemy>();
                    en.TakeDamage(dashDmg);
                    plrScr.plrStats.GainExp(dashDmg);
                }

                dashing = false;
                Slow(0.4f, 0.5f);
                AudioMaster.Instance.Play("DashSlam");
                dashCooldown = dashDelay + Time.time;
            }
        }
    }

    private IEnumerator AddActionToGamemaster()
    {
        while(GameMaster.Instance == null)
        {
            Debug.Log("GameMaster.Instance is null");
            yield return new WaitForSecondsRealtime(1f);
        }

        while(GameMaster.Instance.hud == null)
        {
            Debug.Log("GameMaster.Instance.hud is null");
            yield return new WaitForSecondsRealtime(1f);
        }

        GameMaster.Instance.hud.DashEvent = Dash;
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

        GameMaster.Instance.hud.DashEvent = Dash;
    }

    private void OnDisable()
    {
        if (GameMaster.Instance)
        {
            GameMaster.Instance.hud.DashEvent = delegate { };
        }
    }
}
