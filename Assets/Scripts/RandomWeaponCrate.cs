using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWeaponCrate : MonoBehaviour
{
    private WeaponChange wepChange;
    private PlrHUD plrHud;


    void Start()
    {
        if(!wepChange) wepChange = WeaponChange.Instance;
        if(!plrHud) plrHud = PlrHUD.Instance;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!wepChange) wepChange = WeaponChange.Instance;
        if (!plrHud) plrHud = PlrHUD.Instance;

        if (collision.collider.CompareTag("Player") && wepChange)
        {
            int rng;
            if (wepChange.currentWeaponObject == null)
            {
                rng = Random.Range(0, 4);
            }
            else
            {
                rng = Random.Range(Mathf.Abs(wepChange.curWeaponNumInList - 1), wepChange.curWeaponNumInList + 3);
            }

            if (rng >= wepChange.weaponList.Length) rng = wepChange.weaponList.Length - 1;

            if(wepChange.weaponList[rng].GetComponent<Boomerang>() || wepChange.weaponList[rng].GetComponent<BowHandler>())
            {
                plrHud.OpenBox(rng, BowOrBoom(wepChange.weaponList[rng]), "N\\A", 1, wepChange.weaponList[rng].GetComponent<SpriteRenderer>().sprite);
            }
            else
            {
                plrHud.OpenBox(rng, Random.Range(wepChange.weaponList[rng].GetComponent<Weapon>().dmg / 3, wepChange.weaponList[rng].GetComponent<Weapon>().dmg * 2), wepChange.weaponList[rng].GetComponent<ResourceGatheringScript>().resourceGain, 0, wepChange.weaponList[rng].Find("Sprite").GetComponent<SpriteRenderer>().sprite);
            }
        }
        else
        {
            wepChange = FindObjectOfType<WeaponChange>();
            plrHud = FindObjectOfType<PlrHUD>();
        }
    }

    private int BowOrBoom(Transform obj)
    {
        if (obj.GetComponent<BowHandler>()) return Random.Range(obj.GetComponent<BowHandler>().dmg / 3, (obj.GetComponent<BowHandler>().dmg * 2));
        else return Random.Range(obj.GetComponent<Boomerang>().dmg / 3, (obj.GetComponent<Boomerang>().dmg * 2));
    }

}
