using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

#pragma warning disable 0649
public class MapSpawner : MonoBehaviour
{
    [SerializeField] private int maxObjectsPerGroup = 5;
    [SerializeField] private float distanceFromResource = 5f;
    [SerializeField] private float WallWidth = 3f;
    [SerializeField] private float xMax = 3f;
    [SerializeField] private float xMin = 3f;
    [SerializeField] private float yMax = 3f;
    [SerializeField] private float yMin = 3f;
    [SerializeField] private Transform objectContainer = null;
    [SerializeField] private Transform[] bush = null;
    [SerializeField] private Transform[] trees = null;
    [SerializeField] private Transform[] rocks = null;
    [SerializeField] private Transform wall = null;

    [SerializeField] private LayerMask whatIsBlocked;

    private int treeAmn;
    private int rockAmn;
    private int bushAmn;
    private Vector2 pos;

    private int i = 0;

    private Collider2D[] otherObjects = new Collider2D[20];
    private Transform[,] objects;
    private bool[] wallsExist = new bool[4];
    private float[] wallsPos = new float[4];
    float size;

    // Start is called before the first frame update
    void Start()
    {
        objects = new Transform[3, maxObjectsPerGroup];
        wallsPos[0] = xMax + WallWidth; // top wall
        wallsPos[1] = xMin - WallWidth; // bot wall
        wallsPos[2] = yMax + WallWidth; // right wall
        wallsPos[3] = yMin - WallWidth; // left wall
        size = (Mathf.Abs(xMin) + Mathf.Abs(xMax)) * 2 - WallWidth;
    }

    // Update is called once per frame
    void Update()
    {
        if (i != wallsExist.Length && wallsExist[i] == false)
        {
            if (i < 2)
            {
                SpawnTopBotWall();
            }
            if (i >= 2)
            {
                SpawnLeftRightWall();
            }
            i++;
        }
        if (bushAmn < maxObjectsPerGroup)
        {
            SpawnBush();

        }
        if (treeAmn < maxObjectsPerGroup)
        {
            SpawnTree();
        }
        if (rockAmn < maxObjectsPerGroup)
        {
            SpawnRock();
        }

    }

    private void SpawnTopBotWall()
    {

        Transform wallClone = Instantiate(wall, new Vector3(0, wallsPos[i], -1), transform.rotation) as Transform;
        wallClone.localScale = new Vector2(size, WallWidth); // resize the wall according to which side it's put on
        wallsExist[i] = true;
        wallClone.parent = objectContainer;
    }

    private void SpawnLeftRightWall()
    {
        Transform wallClone = Instantiate(wall, new Vector3(wallsPos[i], 0, -1), transform.rotation) as Transform;
        wallClone.localScale = new Vector2(WallWidth, size); // resize the wall according to which side it's put on
        wallsExist[i] = true;
        wallClone.parent = objectContainer;
    }

    private void SpawnBush()
    {
        pos = new Vector2(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));
        if (CheckForNearbyResource(SpawnBush) == false)
        {
            return;
        }
        else
        {
            Transform bushClone = Instantiate(bush[UnityEngine.Random.Range(0, bush.Length)], pos, Quaternion.identity) as Transform;
            bushClone.parent = objectContainer;
            bushAmn++;
        }
    }

    private void SpawnTree()
    {
        pos = new Vector2(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));
        if (CheckForNearbyResource(SpawnTree) == false)
        {
            return;
        }
        else
        {
            Transform treeClone = Instantiate(trees[UnityEngine.Random.Range(0, trees.Length)], pos, Quaternion.identity) as Transform;
            treeClone.parent = objectContainer;
            treeAmn++;
        }
    }

    private void SpawnRock()
    {
        pos = new Vector2(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));
        if (CheckForNearbyResource(SpawnRock) == false)
        {
            return;
        }
        else
        {
            int rng = UnityEngine.Random.Range(0, rocks.Length);
            if(rng == 3)
            {
                int rng1 = UnityEngine.Random.Range(0, 241);
                if(rng1 != UnityEngine.Random.Range(10, 30))
                {
                    return;
                }
            }
            Transform rockClone = Instantiate(rocks[rng], pos, Quaternion.identity) as Transform;
            rockClone.parent = objectContainer;
            rockAmn++;
        }

    }

    private bool CheckForNearbyResource(Action callback) // return false if there is resource nearby
    {
        otherObjects = Physics2D.OverlapCircleAll(pos, distanceFromResource, whatIsBlocked);

        if(otherObjects.Length > 0)
        {
            callback();
            return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, distanceFromResource);
    }

}
#pragma warning restore 0649