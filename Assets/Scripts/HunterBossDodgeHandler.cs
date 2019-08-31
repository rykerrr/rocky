using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class HunterBossDodgeHandler : MonoBehaviour
{
    [SerializeField] private HunterBossAI hBossAI;
    [SerializeField] private float dodgeDelay;

    private float dodgeTimer;
    private float dodgeDuration;

    private void Start()
    {
        dodgeDuration = GetComponentInParent<HunterBossAI>().dodgeTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.time > dodgeTimer && GetComponentInParent<HunterBossAI>().canDodge)
        {
            if (collision.CompareTag("PlayerProjectiles") || collision.CompareTag("Player"))
            {
                int randDir = Random.Range(0, 2);
                StartCoroutine(hBossAI.Dodge(randDir, dodgeDuration));
                dodgeTimer = dodgeDelay + Time.time;
            }
        }
    }
}
#pragma warning restore 0649