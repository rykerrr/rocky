using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatSwitch : MonoBehaviour
{
    private Transform plr;
    private SpriteRenderer sr;
    private PlayerMovement plrMov;

    private void Start()
    {
        if (!plr && FindObjectOfType<Player>())
        {
            plr = FindObjectOfType<Player>().transform;
        }
        if (!sr && plr)
        {
            sr = plr.GetComponent<SpriteRenderer>();
        }
        if (!plrMov && plr)
        {
            plrMov = plr.GetComponent<PlayerMovement>();
        }
    }

    public void ChangeHat(Sprite hat, int dashLength, int dashRegen, int dashDmg, float spd)
    {
        if (!plr && FindObjectOfType<Player>())
        {
            plr = FindObjectOfType<Player>().transform;
        }
        if (!sr && plr)
        {
            sr = plr.GetComponent<SpriteRenderer>();
        }
        if (!plrMov && plr)
        {
            plrMov = plr.GetComponent<PlayerMovement>();
        }

        sr.sprite = hat;
        if (dashLength != 0)
        {
            plrMov.dashSpeed = dashLength;
        }
        if (dashDmg != 0)
        {
            plrMov.dashDmg = (int)dashDmg;
        }
        if (dashRegen != 0)
        {
            plrMov.dashDelay = dashRegen;
        }
        if (spd != 0)
        {
            plrMov.speed = spd;
            plrMov.normSpd = spd;
        }
    }

}
