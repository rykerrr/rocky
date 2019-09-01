using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

#pragma warning disable 0649
public class GameMaster : MonoBehaviour
{
    #region Singleton
    private static GameMaster instance;
    private static object objLock = new object();
    private static bool turnedOff = false;

    [SerializeField] private Joystick mvStick;
    [SerializeField] private Joystick rotStick;

    public static GameMaster Instance
    {
        get
        {
            if (turnedOff == true)
            {
                return null;
            }

            lock (objLock)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameMaster>();
                }

                return instance;
            }

        }
    }

    #endregion
    public static int startLives = 3;
    public static int weaponIdOnStart;
    public static int timeForAds;
    public static bool normalStart = false;

    [SerializeField] private Transform[] plrRespawnPoints;
    [SerializeField] private Transform plrPref;

    [SerializeField] private float respawnTime;

    [SerializeField] private int lives;

    private float shakeAmn;
    private float respawnTimer;

    private bool respawning = false;

    private Camera mainCam;
    [SerializeField] public PlrHUD hud;
    [SerializeField] public WeaponChange wepChange;
    [SerializeField] private CameraFollow camFoll;
    [SerializeField] private HatSwitch hatSwitch;
    [SerializeField] private WaveSpawner wavSpawner;
    [SerializeField] private Vector3 _ref = Vector3.zero;

    private object m_plrLock;

    public WaveSpawner WaveSpawnerReference => wavSpawner;
    int h;

    public static bool isTesting;

    private void Awake()
    {
        turnedOff = false;
        instance = this;

        if (!hud) hud = FindObjectOfType<PlrHUD>();
        if (!wepChange) wepChange = GetComponent<WeaponChange>();
        if (!mainCam) mainCam = Camera.main;
        if (!camFoll) camFoll = FindObjectOfType<CameraFollow>();
        if (!hatSwitch) hatSwitch = GetComponent<HatSwitch>();
        if (!wavSpawner) wavSpawner = GetComponent<WaveSpawner>();

        //#if UNITY_EDITOR
        //        isTesting = true;
        //#else
        //        isTesting = false;
        //#endif
    }

    void Start()
    {
        m_plrLock = new object();
        if (!normalStart)
        {
            StatManager.GetWeapon();
        }

        weaponIdOnStart = StatManager.currentWeapon;

        // Physics2D.IgnoreLayerCollision(8, 10);
        Physics2D.IgnoreLayerCollision(10, 10);
        Physics2D.IgnoreLayerCollision(10, 30);
        Physics2D.IgnoreLayerCollision(9, 9);
        Physics2D.IgnoreLayerCollision(9, 30);

        lives = startLives;

        if (weaponIdOnStart > 0)
        {
            if (wepChange.weaponList[weaponIdOnStart].GetComponent<BowHandler>())
            {
                hud.StartWeaponChange(weaponIdOnStart, wepChange.weaponList[weaponIdOnStart].GetComponent<BowHandler>().dmg, 1);
            }
            else if (wepChange.weaponList[weaponIdOnStart].GetComponent<Boomerang>())
            {
                hud.StartWeaponChange(weaponIdOnStart, wepChange.weaponList[weaponIdOnStart].GetComponent<Boomerang>().dmg, 1);
            }
            else if (wepChange.weaponList[weaponIdOnStart].GetComponent<Weapon>())
            {
                hud.StartWeaponChange(weaponIdOnStart, wepChange.weaponList[weaponIdOnStart].GetComponent<Weapon>().dmg, 0);
            }
        }
        else
        {
            hud.StartWeaponChange(0, wepChange.weaponList[0].GetComponent<Weapon>().dmg, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            //ReturnToMenu();
        }

        if (Time.time > respawnTimer && respawning)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        hud.timeLeft = 0f;
        respawning = false;
        Vector3 posToSpawnAt = new Vector3(plrRespawnPoints[h].position.x, plrRespawnPoints[h].position.y, -10f);
        Transform plr = Instantiate(plrPref, posToSpawnAt, Quaternion.identity) as Transform;
        hud.ResetPlr(plr.GetComponent<Player>());
        camFoll.target = plr;

        if (Player.Stats.hat != null)
        {
            hatSwitch.ChangeHat(Player.Stats.hat.sprt, Player.Stats.hat.dashLength, Player.Stats.hat.dashRegen, Player.Stats.hat.dashDmg, Player.Stats.hat.spd);
        }

        plr.GetComponent<Player>().Initialize(this);
        plr.GetComponent<PlayerMovement>().movJoystick = mvStick;
        plr.GetComponent<MouseFollow>().movJoystick = mvStick;
        plr.GetComponent<MouseFollow>().rotJoystick = rotStick;

        wepChange.WeaponChangeInven(wepChange.weaponObjectsInInventory[wepChange.currentWeaponId], wepChange.currentWeaponId, wepChange.weaponDamageInInventory[wepChange.currentWeaponId]);
    }

    public void KillPlayer(Transform plr, Transform deathParticles, float deathTimer = 0f)
    {
        lock (m_plrLock)
        {
            hud.ChangeSkillCooldown(0, 1);


            if (hud) hud.CloseRngBoxWepChange();

            h = Random.Range(0, plrRespawnPoints.Length);
            camFoll.target = null;

            Transform partClone = Instantiate(deathParticles, plr.position, plr.rotation) as Transform;

            Destroy(partClone.gameObject, deathTimer);
            Destroy(plr.gameObject, deathTimer);

            if (lives < 0)
            {
                ReturnToMenu();
            }

            camFoll.SmoothDampToPos(plrRespawnPoints[h].position, respawnTime);

            respawning = true;
            respawnTimer = respawnTime + Time.time;

            lives--;
            hud.LoseLife(lives);
        }
    }

    public void ReturnToMenu()
    {
        Player.Stats.name = null;
        Player.Stats.hat = null;

        hud.Pause(1f);

        if (timeForAds == 1)
        {
            timeForAds = 0;
            AdsManager.Instance.ShowRegularAd(OnDeathAdClosed);
        }
        else
        {
            timeForAds++;
            OnDeathAdClosed();
        }
    }

    private void OnDeathAdClosed(ShowResult so = 0)
    {
        StatManager.ConvertToCoins(Player.stoneResource + Player.woodResource + Player.foodResource, wavSpawner.wavesSurvived);
        SceneManager.LoadScene("MainMenu");
    }

    public void Shake(float amn, float length)
    {
        shakeAmn = amn;
        InvokeRepeating("DoShake", 0, 0.01f);
        Invoke("StopShake", length);
    }

    void DoShake()
    {
        if (shakeAmn > 0)
        {
            Vector3 camPos = mainCam.transform.position;

            float offsetX = Random.value * shakeAmn * 2 - shakeAmn;
            float offsetY = Random.value * shakeAmn * 2 - shakeAmn;

            camPos.x += offsetX;
            camPos.y += offsetY;

            mainCam.transform.position = camPos;
        }
    }

    void StopShake()
    {
        CancelInvoke("DoShake");
        mainCam.transform.localPosition = Vector3.zero;
    }

    private void OnEnable()
    {
        AudioMaster.Instance.Play("MatchTheme");
        turnedOff = false;
    }

    private void OnDisable()
    {
        if (AudioMaster.Instance)
        {
            AudioMaster.Instance.StopPlaying("MatchTheme");
        }
    }

    private void OnDestroy()
    {
        turnedOff = true;
    }

    private void OnApplicationQuit()
    {
        turnedOff = true;
    }

}
#pragma warning restore 0649