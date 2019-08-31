using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class ResourceGiverScript : MonoBehaviour
{
    [SerializeField] private float timeToRegen;
    [SerializeField] private int resLoss;

    [HideInInspector] public float timer;

    public int maxResource;
    public int resourceLeftToGive;
    public string typeOfResource;
    public bool regening = false;

    private SpriteRenderer sr;
    private BerryBushv2 bushGFXScript;

    private void Start()
    {
        resourceLeftToGive = maxResource;
        if (!sr) sr = GetComponent<SpriteRenderer>();
        if (!bushGFXScript) bushGFXScript = GetComponent<BerryBushv2>();
    }

    private void Update()
    {
        if(Time.time > timer && regening)
        {
            timer = Time.time + timeToRegen;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            resourceLeftToGive = maxResource;
            bushGFXScript.SpawnBerries();
            regening = false;
        }
    }

    private void DestroyBerry()
    {
        Destroy(bushGFXScript.currentBerries[bushGFXScript.berryCount - 1].gameObject);
        bushGFXScript.berryCount--;
        return;
    }

    public int TakeResource(int res)
    {
        if (!regening)
        {
            if (GetComponent<BerryBushv2>())
            {
                DestroyBerry();
            }

            if (resourceLeftToGive > resLoss)
            {
                resourceLeftToGive -= resLoss;
                return res * resLoss;
            }
            else
            {
                timer = Time.time + timeToRegen;
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.6f);
                regening = true;
                return resourceLeftToGive;
            }

        }
        else return 0;
    }
}
#pragma warning restore 0649