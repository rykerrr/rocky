using UnityEngine;
using System;

#pragma warning disable 0649
public class Player : MonoBehaviour
{
    [System.Serializable]
    public class Stats
    {
        public static event Action<int> OnStatsChanged = delegate { };

        public static string name;
        public static Hat hat;
        [SerializeField] private PlrHUD hud;
        [SerializeField] private Transform plr;

        [SerializeField] private int startXPToLevUp = 30;
        [SerializeField] private int hpGainPerLev = 1;
        [SerializeField] private int hp;
        [SerializeField] private int maxHP;

        [SerializeField] private static int baseMaxHP = 90;
        [SerializeField] private static int xp;
        [SerializeField] private static int xpToLevelUp;
        [SerializeField] private static int level;

        public static int statResetsLeft = 1;
        public static int statPointsLeft;
        public static int dexterityStat;
        public static int strengthStat;
        public static int vitalityStat;

        public int Health
        {
            get { return hp; }
            set { hp = value; }
        }

        public int ExpToLevUp
        {
            get { return xpToLevelUp; }
            set { xpToLevelUp = value; }
        }

        public int Exp
        {
            get { return xp; }
            set { xp = value; }
        }

        public int RetMaxHP()
        {
            return maxHP;
        }

        public void HealHealth(int val)
        {
            CheckIfMaxHealthCorrect();

            if(val > 0)
            {
                if(val > maxHP || hp + val >= maxHP)
                {
                    hp = maxHP;
                    return;
                }

                hp += val;
            }


        }

        public void GainExp(int val)
        {
            if (val + Exp < ExpToLevUp)
            {
                Exp += val;
                hud.ExpGain(Exp, ExpToLevUp, false);
            }
            else
            {
                while (val + Exp >= ExpToLevUp)
                {
                    val -= (ExpToLevUp - Exp);
                    LevelUp();
                }
                if (val >= Exp)
                {
                    Exp = val;
                    hud.ExpGain(val, ExpToLevUp, false);
                }
            }
        }

        public int RetLev()
        {
            return level;
        }

        private void LevelUp()
        {
            ExpToLevUp += startXPToLevUp;
            level++;
            Exp = 0;
            hud.ExpGain(0, ExpToLevUp, true);
            statPointsLeft++;
            OnStatsChanged(99999);
        }

        private void CheckIfMaxHealthCorrect()
        {
            maxHP = baseMaxHP + (level * hpGainPerLev) + (vitalityStat * 15) + (strengthStat * 5);
        }

        public static void IncreaseStat(int statNum)
        {
            if (statPointsLeft > 0)
            {
                if (statNum == 0)
                {
                    strengthStat++;
                    OnStatsChanged(5);
                }
                else if (statNum == 1)
                {
                    dexterityStat++;
                }
                else if (statNum == 2)
                {
                    vitalityStat++;
                    OnStatsChanged(15);
                }
                else
                {
                    return;
                }

                statPointsLeft--;
                OnStatsChanged(0);
            }
        }

        public static void ResetStats()
        {
            if (statResetsLeft > 0)
            {
                statPointsLeft += dexterityStat + strengthStat + vitalityStat;

                dexterityStat = 0;
                strengthStat = 0;
                vitalityStat = 0;

                statResetsLeft--;
                OnStatsChanged(0);
            }
        }

        public void Init(PlrHUD _hud, Transform _plr)
        {
            Stats.OnStatsChanged += HealHealth;
            OnStatsChanged(99999);
            plr = _plr;
            hud = _hud;
            PlrHUD.hat = hat;
        }
    }

    [SerializeField] private Transform deathParticles;
    [SerializeField] private Transform currentWeapon;

    public static int woodResource;
    public static int stoneResource;
    public static int foodResource;

    [SerializeField] public Stats plrStats = new Stats();
    private GameMaster gm;

    private float timer;

    private void Start()
    {
        plrStats.Init(FindObjectOfType<PlrHUD>(), transform);
    }

    public void Initialize(GameMaster _gm)
    {
        gm = _gm;
    }

    public void TakeDamage(int _dmg)
    {
        if (plrStats.Health >    0)
        {
            if (plrStats.Health - _dmg > 0)
            {
                plrStats.Health -= _dmg;
                AudioMaster.Instance.Play("PlayerHurt");
            }
            else
            {
                GameMaster.Instance.KillPlayer(transform, deathParticles);
                //AudioMaster.Instance.Play("PlayerDeath");
            }
        }
    }

    public int ReturnHealth()
    {
        return plrStats.Health;
    }

    public int ReturnMaxHealth()
    {
        return plrStats.RetMaxHP();
    }

    public void SlowInTime(float slowAmn, float slowLength = 1f)
    {
        GetComponent<PlayerMovement>().Slow(slowAmn, slowLength);
    }

    public void ChangeSpd(float speedAmn)
    {
        GetComponent<PlayerMovement>().Slow(speedAmn);
    }

    private void OnEnable()
    {
        Stats.OnStatsChanged += plrStats.HealHealth;
    }

    private void OnDisable()
    {
        Stats.OnStatsChanged -= plrStats.HealHealth;
    }
}
#pragma warning restore 0649