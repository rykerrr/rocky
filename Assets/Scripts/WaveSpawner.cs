using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class WaveSpawner : MonoBehaviour
{
    public enum States { WAITING, SPAWNING, BOSS, IDLE }
    [System.Serializable]
    public class Wave
    {
        public Transform[] enemyList;
        public Transform bossPref;
        public int waveSize;
        public float timeBetweenSpawn;
    }

    [SerializeField] private Transform pooledContainer;
    [SerializeField] private Animator waveStartAnim;

    public Transform PooledContainer => pooledContainer;

    public static float EXPMultiplier = 1f;
    public static float coinMultiplier = 1f;

    public bool bossAlive;
    public bool bossWave;
    public int wavesSurvived;
    public int curWave;
    public int numOfEn;
    public float difficultyMultiplier;
    public States curState;

    [SerializeField] private Transform objContainer;
    [SerializeField] private Transform boxPref;
    [SerializeField] private Transform bossSpawnLocation;
    public Transform curBox;
    [SerializeField] private Transform enContainer;
    [SerializeField] private Wave[] waves;
    [SerializeField] private Transform[] spawnList;
    [SerializeField] private int lastWave;
    [SerializeField] private float timeBetweenWaves;

    private float enemySpawnTimer;
    private float waveDelayTimer;

    [SerializeField] private int enemiesLeft;

    GameMaster gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameMaster>();
        difficultyMultiplier = 1f;
        curWave = 0;
        wavesSurvived = 0;
        curState = States.WAITING;
        bossAlive = false;
    }

    void Start()
    {
        SpawnBox();
    }

    void Update()
    {
        waveStartAnim.SetBool("fadeIn", false);

        if (curWave == lastWave && curState == States.IDLE)
        {
            curWave = 0;
            curState = States.WAITING;
            difficultyMultiplier += 0.50f;
            coinMultiplier += 0.4f;
            EXPMultiplier += 0.75f;
            waveDelayTimer = (timeBetweenWaves * 2f) + Time.time;
        }

        if ((curWave % 5 == 0 && curWave != 0) && enContainer.childCount == 0 && curState == States.WAITING && Time.time > waveDelayTimer)
        {
            bossWave = true;
            enemiesLeft = waves[curWave].waveSize;
            curState = States.BOSS;
            waveStartAnim.SetBool("fadeIn", true);
            enemySpawnTimer = Time.time + 3.5f;
        }

        if ((Time.time > waveDelayTimer && enContainer.childCount == 0) && ((curState == States.WAITING) || (curState == States.IDLE && curWave != lastWave)))
        {
            curState = States.SPAWNING;
            enemiesLeft = waves[curWave].waveSize;
            waveStartAnim.SetBool("fadeIn", true);
            enemySpawnTimer = Time.time + 3.5f;
        }

        if (curState == States.SPAWNING || curState == States.BOSS)
        {
            if (Time.time > enemySpawnTimer)
            {
                if (enemiesLeft <= 0 && enContainer.childCount == 0 && !bossWave)
                {
                    curState = States.WAITING;
                    curWave++;
                    waveDelayTimer = timeBetweenWaves + Time.time;
                    wavesSurvived++;
                    return;
                }
                else
                {
                    if (enemiesLeft > 0)
                    {
                        SpawnEnemy();
                        enemiesLeft--;
                    }

                    if (curState == States.BOSS)
                    {
                        SpawnBoss();
                        bossAlive = true;
                        curState = States.SPAWNING;
                    }
                    else if (enContainer.childCount == 0 && curState == States.SPAWNING && bossWave)
                    {
                        SpawnBox();
                        bossAlive = false;
                        bossWave = false;
                        curWave++;
                        waveDelayTimer = timeBetweenWaves * 2f + Time.time;
                        curState = States.IDLE;
                        wavesSurvived++;
                    }
                }
            }
        }

        numOfEn = enContainer.childCount;
    }

    private void SpawnEnemy()
    {
        int rng = Random.Range(0, waves[curWave].enemyList.Length);
        int randomSpawn = Random.Range(0, spawnList.Length);
        Vector2 spawnPos = new Vector2(Random.Range(spawnList[randomSpawn].position.x - 3, spawnList[randomSpawn].position.x + 3), Random.Range(spawnList[randomSpawn].position.y - 3, spawnList[randomSpawn].position.y + 3));
        GameObject enemy = Poolable.Get(() => Poolable.CreateObj(waves[curWave].enemyList[rng].gameObject), waves[curWave].enemyList[rng].GetComponent<Poolable>().NameOfKey);
        enemy.transform.rotation = Quaternion.identity;
        enemy.transform.position = spawnPos;

        #region Injection
        if (enemy.GetComponent<EnemyAI>())
        {
            enemy.GetComponent<EnemyAI>().Initialize(gm);
        }
        if (enemy.GetComponent<BowmanAI>())
        {
            enemy.GetComponent<BowmanAI>().objectContainer = objContainer;
            enemy.GetComponent<BowmanAI>().Initialize(gm);
        }
        if (enemy.GetComponent<SwordsmanAI>())
        {
            enemy.GetComponent<SwordsmanAI>().Initialize(gm);
        }
        #endregion

        enemy.transform.parent = enContainer;
        Enemy en = waves[curWave].enemyList[rng].GetComponent<Enemy>();
        int _hp = Mathf.RoundToInt((Random.Range((en.enStats.RetMaxHP() / 1.4f), (en.enStats.RetMaxHP() * 1.2f)) * difficultyMultiplier) * (0.3f * (GameMaster.weaponIdOnStart + 1)));
        en = enemy.GetComponent<Enemy>();
        en.SetMaxHealth(_hp);
        enemySpawnTimer = waves[curWave].timeBetweenSpawn + Time.time;
    }

    private void SpawnBoss()
    {
        Transform curBoss = Instantiate(waves[curWave].bossPref, bossSpawnLocation.position, Quaternion.identity) as Transform;
        Enemy boss = curBoss.GetComponent<Enemy>();
        int _hp = Mathf.RoundToInt(difficultyMultiplier * boss.enStats.Health * (0.2f * (GameMaster.weaponIdOnStart + 1)));
        boss.SetMaxHealth(_hp);
        curBoss.parent = enContainer;
    }

    private void SpawnBox()
    {
        if (curBox == null)
        {
            Transform box = Instantiate(boxPref, Vector2.zero, Quaternion.Euler(0f, 0f, Random.Range(0, 360))) as Transform;
            curBox = box;
            return;
        }

        curBox.gameObject.SetActive(true);
    }
}
#pragma warning restore 0649