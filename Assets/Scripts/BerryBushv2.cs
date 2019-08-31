using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class BerryBushv2 : MonoBehaviour
{
    [SerializeField] private Transform berryPref;
    [SerializeField] private LayerMask whatIsBerry;

    [SerializeField] private float rad;

    public Transform[] currentBerries = new Transform[20];
    public int berryCount;

    private Collider2D[] berryInspect = new Collider2D[20];
    private ResourceGiverScript berryBushScript;
    private Vector2 pos;

    private int i;
    void Start()
    {
        berryCount = 0;
        if (!berryBushScript)
        {
            berryBushScript = GetComponent<ResourceGiverScript>();
        }
        SpawnBerries();
    }

    public void SpawnBerries()
    {
        for (i = 0; i < berryBushScript.maxResource; i++)
        {
            SpawnBerry();
        }
    }

    private void SpawnBerry()
    {
        pos = new Vector2(Random.Range(transform.position.x - transform.localScale.x, transform.position.x + transform.localScale.x), Random.Range(transform.position.y - transform.localScale.y, transform.position.y + transform.localScale.y));
        if (CheckForBerries() == false)
        {
            return;
        }
        Quaternion rot = Quaternion.Euler(0f, 0f, Random.Range(-360f, 360f));
        Transform berClone = Instantiate(berryPref, pos, rot) as Transform;
        berClone.parent = transform;
        currentBerries[i] = berClone;
        berryCount++;
    }

    private bool CheckForBerries()
    {
        berryInspect = Physics2D.OverlapCircleAll(pos, rad, whatIsBerry);

        if (berryInspect.Length > 0)
        {
            SpawnBerry();
            return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, rad);
    }

}
#pragma warning restore 0649