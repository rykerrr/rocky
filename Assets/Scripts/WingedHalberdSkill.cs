using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class WingedHalberdSkill : Ability
{
    [SerializeField] private Transform plr;
    [SerializeField] private Transform slicePref;
    [SerializeField] private Animator anim;

    [SerializeField] private float speed;
    [SerializeField] private float throwDelay;
    [SerializeField] private float upTime;
    [SerializeField] private float waitTimeBeforeSwing;

    [SerializeField] private int amtOfPierces;

    public int dmg;

    private PlrHUD plrHD;

    private float throwTimer;

    private Poolable slicePool;

    private void Awake()
    {
        slicePool = slicePref.GetComponent<Poolable>();
    }

    void Start()
    {
        if (!plr) plr = FindObjectOfType<Player>().transform;
        if (!anim) anim = transform.GetComponent<Animator>();
        if (!plrHD) plrHD = PlrHUD.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (throwTimer > Time.time)
        {
            if (!plrHD) plrHD = PlrHUD.Instance;
            plrHD.ChangeSkillCooldown(throwTimer - Time.time, throwDelay);
        }
    }

    public override void SkillCall()
    {
        if (!anim.GetBool("swingingNow") && !anim.GetBool("windSlice") && Time.time > throwTimer)
        {
            StartCoroutine(StartAnim());
        }
    }

    IEnumerator StartAnim()
    {
        plrHD.ChangeSkillCooldown(throwDelay, throwDelay);
        anim.SetBool("windSlice", true);
        yield return new WaitForSeconds(waitTimeBeforeSwing);
        GameObject windClone = Poolable.Get(() => Poolable.CreateObj(slicePref.gameObject), slicePool.NameOfKey);
        Arrow windCloneArr = windClone.GetComponent<Arrow>();
        windClone.transform.position = transform.position;
        windClone.transform.rotation = slicePref.rotation * plr.rotation;

        windClone.GetComponent<Rigidbody2D>().velocity = plr.up * speed;
        windCloneArr.lifeTime = upTime + Time.time;
        windCloneArr.dmg = dmg;
        windCloneArr.pierceCount = amtOfPierces;

        yield return new WaitForSeconds(2 - waitTimeBeforeSwing);
        anim.SetBool("windSlice", false);
        throwTimer = Time.time + throwDelay;
        if (!plrHD) plrHD = PlrHUD.Instance;
        plrHD.ChangeSkillCooldown(throwTimer - Time.time, throwDelay);
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