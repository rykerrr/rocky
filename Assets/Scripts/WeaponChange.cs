using System;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class WeaponChange : MonoBehaviour
{
    public Action skillCall = delegate { };
    public static WeaponChange Instance { get; private set; }

    public Transform[] weaponList;
    public int curWeaponNumInList;
    [SerializeField] private float switchDelay = 3f;
    [SerializeField] private string[] currentWeapons;
    public Transform[] weaponObjectsInInventory;
    public int[] weaponDamageInInventory;
    public Transform currentWeaponObject;
    [SerializeField] private string currentWeapon;
    public int currentWeaponId;
    [SerializeField] private Transform handPos;
    [SerializeField] private Transform plr;
    [SerializeField] private Image[] weaponImgs;

    private float timeToWait;
    public bool switching = false;
    private PlrHUD hud;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if (!plr && FindObjectOfType<Player>())
        {
            plr = FindObjectOfType<Player>().gameObject.GetComponent<Transform>();
        }
        if (!handPos)
        {
            handPos = plr.Find("HandPos");
        }
        if (!currentWeaponObject && plr.GetComponentInChildren<Weapon>() != null)
        {
            currentWeaponObject = plr.GetComponentInChildren<Weapon>().transform;
            currentWeaponId = 1;
            currentWeapon = currentWeaponObject.name;
            weaponObjectsInInventory[0] = currentWeaponObject;
            weaponImgs[0].sprite = currentWeaponObject.GetComponentInChildren<SpriteRenderer>().sprite;
            weaponImgs[0].color = new Color(weaponImgs[0].color.r, weaponImgs[0].color.g, weaponImgs[0].color.b, 1f);

        }
        hud = PlrHUD.Instance;
    }

    private void Update()
    {
        if (!switching && Time.time > timeToWait)
        {

            if (Input.GetKeyDown(KeyCode.Alpha1) && weaponObjectsInInventory[0] != null)
            {
                ChangeWeapon(weaponObjectsInInventory[0], 1, weaponDamageInInventory[0]);
                timeToWait = switchDelay + Time.time;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && weaponObjectsInInventory[1] != null)
            {
                ChangeWeapon(weaponObjectsInInventory[1], 2, weaponDamageInInventory[1]);
                timeToWait = switchDelay + Time.time;
            }
        }
    }

    private void FindPlayer()
    {
        if (!plr && FindObjectOfType<Player>())
        {
            plr = FindObjectOfType<Player>().gameObject.GetComponent<Transform>();
        }
        if (!handPos && plr)
        {
            handPos = plr.Find("HandPos");
        }
        if (plr)
        {
            if (!currentWeaponObject && plr.GetComponentInChildren<Weapon>() != null)
            {
                currentWeaponObject = plr.GetComponentInChildren<Weapon>().transform;
                currentWeaponId = 1;
                currentWeapon = currentWeaponObject.name;
                weaponObjectsInInventory[0] = currentWeaponObject;
                weaponImgs[0].sprite = currentWeaponObject.GetComponentInChildren<SpriteRenderer>().sprite;
                weaponImgs[0].color = new Color(weaponImgs[0].color.r, weaponImgs[0].color.g, weaponImgs[0].color.b, 1f);
            }
        }


    }

    public void WeaponChangeInven(Transform pref, int inventID, int dmg) // changes mostly "ui" part of the weapons, the inventory slot, skill slot, etc, and the back end in this script on which weapons
    { // are in the inventory
        if (!handPos || !plr)
        {
            FindPlayer();
        }

        if (inventID >= 0 && inventID <= 1)
        {
            weaponObjectsInInventory[inventID] = pref;

            if (pref.GetComponent<SpriteRenderer>())
            {
                weaponImgs[inventID].sprite = pref.GetComponent<SpriteRenderer>().sprite;
            }
            else
            {
                weaponImgs[inventID].sprite = pref.GetComponentInChildren<SpriteRenderer>().sprite;
            }

            if (inventID + 1 == currentWeaponId) // if the new weapon is the currently equipped weapon
            {
                ChangeWeapon(pref, inventID + 1, dmg); // + 1 because 0 and 1 are getting passed in, but currentWeaponId is 1 or 2
                currentWeaponId = inventID + 1; // here + look below in the color change, it uses the normal value
            }

            weaponImgs[inventID].color = new Color(weaponImgs[inventID].color.r, weaponImgs[inventID].color.g, weaponImgs[inventID].color.b, 1f);
        }
        else
        {
            return;
        }

        switching = false;
        ChangeWeapon(weaponObjectsInInventory[inventID], inventID, dmg);
        timeToWait = switchDelay + Time.time;

        if (!hud) hud = PlrHUD.Instance;

        hud.ChangeSkillCooldown(0, 1);
        weaponDamageInInventory[inventID] = dmg;
    }

    private void ChangeWeapon(Transform pref, int id, int dmg)
    {
        if (!handPos || !plr) // 
        {
            FindPlayer();
        }
        if (!hud)
        {
            hud = PlrHUD.Instance;
        }

        if (hud.timeLeft <= 0.5f)
        {
            if (plr && handPos)
            {
                Transform wepClone;

                if (currentWeaponObject)
                {
                    Destroy(currentWeaponObject.gameObject);
                }

                if (pref.GetComponent<BowHandler>() || pref.GetComponent<Boomerang>() || pref.name == "Khopesh")
                {
                    wepClone = Instantiate(pref, handPos.position, pref.rotation * plr.rotation) as Transform;
                }
                else
                {
                    wepClone = Instantiate(pref, handPos.position, pref.rotation) as Transform;
                }

                if (wepClone.GetComponent<Weapon>()) wepClone.GetComponent<Weapon>().dmg = dmg;
                else if (wepClone.GetComponent<Boomerang>()) wepClone.GetComponent<Boomerang>().dmg = dmg;
                else if (wepClone.GetComponent<BowHandler>()) wepClone.GetComponent<BowHandler>().dmg = dmg;

                currentWeaponObject = wepClone;
                wepClone.parent = plr;
                currentWeapon = pref.name;
                currentWeaponId = id;
                plr.GetComponent<Player>().ChangeSpd(1f);
            }

            if (!hud) hud = PlrHUD.Instance;

            hud.ChangeSkillCooldown(0, 1);
        }
    }

    public void ButtonChangeWeapon(int id)
    {
        if (Time.time > timeToWait)
        {
            if (weaponObjectsInInventory[id] != null)
            {
                if (weaponObjectsInInventory[id].GetComponent<BowHandler>())
                {
                    ChangeWeapon(weaponObjectsInInventory[1], 2, weaponObjectsInInventory[1].GetComponent<BowHandler>().dmg);
                }
                else if (weaponObjectsInInventory[id].GetComponent<Boomerang>())
                {
                    ChangeWeapon(weaponObjectsInInventory[1], 2, weaponObjectsInInventory[1].GetComponent<Boomerang>().dmg);
                }
                else
                {
                    ChangeWeapon(weaponObjectsInInventory[0], 1, weaponObjectsInInventory[0].GetComponent<Weapon>().dmg);
                }
                timeToWait = switchDelay + Time.time;

                if(currentWeaponObject.GetComponent<Ability>())
                {
                    skillCall = currentWeaponObject.GetComponent<Ability>().SkillCall;
                }
            }
        }

    }

    public void UseSkill()
    {
        skillCall();
    }


    public void EmptyField(int numID)
    {
        weaponImgs[numID].sprite = null;
        weaponImgs[numID].color = new Color(weaponImgs[numID].color.r, weaponImgs[numID].color.g, weaponImgs[numID].color.b, 0f);
        weaponObjectsInInventory[numID] = null;

        if (currentWeaponObject)
        {
            currentWeaponObject = null;
            currentWeapon = null;
        }

    }
}
#pragma warning restore 0649