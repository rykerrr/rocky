using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class ResourceGatheringScript : MonoBehaviour
{
    [SerializeField] private Player plrScript;
    [SerializeField] private Weapon wepScript;
    public int resourceGain;
    // Start is called before the first frame update
    void Start()
    {
        if (!plrScript && FindObjectOfType<Player>())
        {
            plrScript = FindObjectOfType<Player>();
        }
        if (!wepScript)
        {
            wepScript = GetComponent<Weapon>();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (wepScript.swinging)
        {
            if (other.CompareTag("Resource")) // Stone Wood Food
            {
                ResourceGiverScript resScript = other.GetComponent<ResourceGiverScript>();
                if (resScript.typeOfResource == "Stone")
                {
                    Player.stoneResource += resScript.TakeResource(resourceGain);
                }
                if (resScript.typeOfResource == "Wood")
                {
                    Player.woodResource += resScript.TakeResource(resourceGain);
                }
                if (resScript.typeOfResource == "Food")
                {
                    Player.foodResource += resScript.TakeResource(resourceGain);
                }
            }
        }
    }
}
#pragma warning restore 0649