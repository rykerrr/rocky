using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#pragma warning disable 0649
public class PlrHUD : MonoBehaviour
{
    public Action AttackEvent = delegate { };
    public event Action DashEvent = delegate { };
    public static PlrHUD Instance { get; private set; }
    public static Hat hat;
    public Transform weaponChangeBox;

    public float timeLeft;

    [SerializeField] private WeaponChange wepChange;
    [SerializeField] private HatSwitch hatSwitch;
    [SerializeField] private WaveSpawner wavSpawner;
    [SerializeField] private Player plr;
    [SerializeField] private GameObject[] textGuiders;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private Transform levelUpPart;
    [SerializeField] private Transform waveStartButton;
    [SerializeField] private Transform statManagerUI;
    [SerializeField] private Transform resetStatsConfirmWindow;
    [SerializeField] private RectTransform xpBar;
    [SerializeField] private Image skillCooldownImg;
    [SerializeField] private Text skillCooldownText;
    [SerializeField] private Text xpText;
    [SerializeField] private Text lev;
    [SerializeField] private Text wood;
    [SerializeField] private Text stone;
    [SerializeField] private Text food;
    [SerializeField] private Slider health;
    [SerializeField] private Text coinsYouEarn;
    [SerializeField] private Text wepImgDmg;
    [SerializeField] private Text wepImgResPerHit;
    [SerializeField] private Text slot;
    [SerializeField] private Text curWave;
    [SerializeField] private Text enLeft;
    [SerializeField] private Text difMultiplier;
    [SerializeField] private Text state;
    [SerializeField] private Text skillName;
    [SerializeField] private Text strStatText;
    [SerializeField] private Text dexStatText;
    [SerializeField] private Text stamStatText;
    [SerializeField] private Text freeStatPointsText;
    [SerializeField] private Text statResetsText;
    [SerializeField] private Text skillDesc;
    [SerializeField] private Image wepImg;
    [SerializeField] private Image skillImg;
    [SerializeField] private Image skillInfoImg;
    [SerializeField] private Image statsPanelOpennerImage;
    [SerializeField] private Image[] liveImgs;
    [SerializeField] private Image[] statUpgradeImgs;

    int _rng, _id, _dmg;

    [SerializeField] private GraphicRaycaster canvasGr; // canvas to be used in IsTouchOverUI
    PointerEventData eventPed = new PointerEventData(null);
    List<RaycastResult> UIRaycasts = new List<RaycastResult>();
    private Transform curWepSel;
    private Color skillNormCol;
    private static Camera cam;

    private void Start()
    {
        skillNormCol = skillCooldownImg.color;

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
            plr = FindObjectOfType<Player>();
        }
        if (!wepChange)
        {
            wepChange = WeaponChange.Instance;
        }
        if (!hatSwitch && FindObjectOfType<HatSwitch>())
        {
            hatSwitch = FindObjectOfType<HatSwitch>();
        }
        if (!wavSpawner)
        {
            wavSpawner = hatSwitch.gameObject.GetComponent<WaveSpawner>();
        }

        if (hat != null)
        {
            if (hatSwitch)
            {
                hatSwitch.ChangeHat(hat.sprt, hat.dashLength, hat.dashRegen, hat.dashDmg, hat.spd);
            }
        }

        skillInfoImg.gameObject.SetActive(false);
        skillName.text = "";
        skillDesc.text = "";

        cam = Camera.main;
    }

    private void Update()
    {
        wood.text = "Wood: " + Player.woodResource.ToString();
        stone.text = "Stone: " + Player.stoneResource.ToString();
        food.text = "Food: " + Player.foodResource.ToString();
        if (plr)
        {
            health.maxValue = plr.plrStats.RetMaxHP();
            health.value = plr.plrStats.Health;
        }
        else
        {
            health.maxValue = 100;
            health.value = 0;
        }
        lev.text = "Level: " + plr.plrStats.RetLev().ToString();
        xpText.text = "EXP: " + plr.plrStats.Exp + " / " + plr.plrStats.ExpToLevUp;
        enLeft.text = "Enemies left: " + wavSpawner.numOfEn.ToString();
        curWave.text = "Wave: " + wavSpawner.wavesSurvived.ToString();
        difMultiplier.text = "Difficulty Multiplier: " + ((wavSpawner.difficultyMultiplier - 1f) * 100).ToString() + "%";
        state.text = "State: " + wavSpawner.curState.ToString();
        dexStatText.text = "Dexterity: " + Player.Stats.dexterityStat;
        strStatText.text = "Strength: " + Player.Stats.strengthStat;
        stamStatText.text = "Vitality: " + Player.Stats.vitalityStat;
        freeStatPointsText.text = "Points left: " + Player.Stats.statPointsLeft;
        statResetsText.text = "Stat resets left: " + Player.Stats.statResetsLeft;


        if (Player.Stats.statPointsLeft > 0)
        {
            EnableOrDisableStatIncs(true);
            statsPanelOpennerImage.color = Color.green;
            statsPanelOpennerImage.color = new Color(statsPanelOpennerImage.color.r, statsPanelOpennerImage.color.g, statsPanelOpennerImage.color.b, 0.65f);
        }
        else
        {
            EnableOrDisableStatIncs(false);
            statsPanelOpennerImage.color = Color.white;
            statsPanelOpennerImage.color = new Color(statsPanelOpennerImage.color.r, statsPanelOpennerImage.color.g, statsPanelOpennerImage.color.b, 0.2f);
        }
    }

    private void EnableOrDisableStatIncs(bool yn)
    {
        if (yn == true)
        {
            for (int i = 0; i < statUpgradeImgs.Length; i++)
            {
                statUpgradeImgs[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < statUpgradeImgs.Length; i++)
            {
                statUpgradeImgs[i].gameObject.SetActive(false);
            }
        }
    }

    public static bool MouseOverUI(int pointerId = -1)
    {
        if (EventSystem.current.IsPointerOverGameObject(pointerId))
        {
            return true;
        }
        else return false;
    }

    public bool TouchOverUI(Touch touch)
    {
        eventPed.position = touch.position;
        canvasGr.Raycast(eventPed, UIRaycasts);

        if (UIRaycasts.Count > 0)
        {
            UIRaycasts.Clear();
            return true;
        }
        else
        {
            return false;
        }
        // raycast = Physics2D.OverlapCircle(cam.ScreenToWorldPoint(touch.position), 0.4f, whatIsUi);
    }

    public void ExpGain(int curXP, int levXP, bool levelUp)
    {
        if (levelUp == true)
        {
            if (levelUpPart)
            {
                Transform levPartClone = Instantiate(levelUpPart, new Vector3(plr.transform.position.x, plr.transform.position.y, -1), Quaternion.identity) as Transform;
                Destroy(levPartClone.gameObject, 2f);
            }
        }
        float value = (float)curXP / levXP;
        xpBar.localScale = new Vector3(value, xpBar.localScale.y, xpBar.localScale.z);
    }

    public void IncStatPoint(int statPoint)
    {
        Player.Stats.IncreaseStat(statPoint);

        if (plr != null)
        {
            if (statPoint == 0)
            {
                plr.plrStats.HealHealth(2);
            }
            else if (statPoint == 2)
            {
                plr.plrStats.HealHealth(5);
            }
        }
    }

    public void ResetStats()
    {
        Player.Stats.ResetStats();

        OpenCloseStatsResetConfirm();
    }

    public void OpenCloseStatManagerUI()
    {
        statManagerUI.gameObject.SetActive(!statManagerUI.gameObject.activeSelf);
    }

    public void OpenBox(int rng, int dmg, int resPerHit, int id, Sprite sprt)
    {
        if (!wepChange)
        {
            if (hatSwitch)
            {
                wepChange = hatSwitch.gameObject.GetComponent<WeaponChange>();
            }
            else
            {
                wepChange = FindObjectOfType<WeaponChange>();
            }
        }

        weaponChangeBox.gameObject.SetActive(true);
        wepImg.sprite = sprt;
        wepImgDmg.text = "Damage per hit: " + dmg.ToString();
        wepImgResPerHit.text = "Resources per hit: " + resPerHit.ToString();

        #region Changing color of test based on damage
        if (wepChange.weaponObjectsInInventory[0] != null)
        {
            if ((dmg > wepChange.weaponDamageInInventory[0]))
            {
                wepImgDmg.color = Color.green;
            }
            else if (dmg < wepChange.weaponDamageInInventory[0])
            {
                wepImgDmg.color = Color.red;
            }
            else
            {
                wepImgDmg.color = Color.white;
            }

            if (resPerHit > wepChange.weaponObjectsInInventory[0].GetComponent<ResourceGatheringScript>().resourceGain) wepImgResPerHit.color = Color.green;
            else if (resPerHit < wepChange.weaponObjectsInInventory[0].GetComponent<ResourceGatheringScript>().resourceGain) wepImgResPerHit.color = Color.red;
            else
            {
                wepImgResPerHit.color = Color.white;
            }
        }
        else
        {
            wepImgDmg.color = Color.green;
            wepImgResPerHit.color = Color.green;
        }
        #endregion

        slot.text = "Take weapon on slot " + id + " ?";
        _rng = rng;
        _id = id;
        _dmg = dmg;
    }

    public void OpenBox(int rng, int dmg, string resPerHit, int id, Sprite sprt)
    {
        if (!wepChange)
        {
            if (hatSwitch)
            {
                wepChange = hatSwitch.gameObject.GetComponent<WeaponChange>();
            }
            else
            {
                wepChange = FindObjectOfType<WeaponChange>();
            }
        }

        weaponChangeBox.gameObject.SetActive(true);
        wepImgResPerHit.color = Color.white;
        wepImg.sprite = sprt;
        wepImgDmg.text = dmg.ToString();
        wepImgResPerHit.text = resPerHit.ToString();

        #region Changing color of test based on damage
        if (wepChange.weaponObjectsInInventory[1] != null)
        {
            if (dmg > wepChange.weaponDamageInInventory[1])
            {
                wepImgDmg.color = Color.green;
            }
            else if (dmg < wepChange.weaponDamageInInventory[1])
            {
                wepImgDmg.color = Color.red;
            }
            else
            {
                wepImgDmg.color = Color.white;
            }
        }
        else
        {
            wepImgDmg.color = Color.green;
        }
        #endregion

        slot.text = "Take weapon on slot " + id + " ?";
        _dmg = dmg;
        _rng = rng;
        _id = id;
    }

    public void StartWeaponChange(int rng, int dmg, int id) // rng is weapon number, dmg is random damage amount for the new weapon, id is which weapon slot
    {
        _rng = rng;
        _dmg = dmg;
        _id = id;
        RngWeaponChange();
    }

    public void RngWeaponChange()
    {
        if (!wepChange) wepChange = FindObjectOfType<WeaponChange>();

        if (wepChange.weaponList[_rng].GetComponent<Ability>())
        {
            Ability ab = wepChange.weaponList[_rng].GetComponent<Ability>();

            skillImg.gameObject.SetActive(true);
            skillImg.sprite = ab.SkillImg;
            skillName.text = ab.SkillName;
            skillDesc.text = ab.SkillDesc;
        }
        else
        {
            skillImg.gameObject.SetActive(false);
            skillName.text = "";
            skillDesc.text = "";
        }

        ChangeSkillCooldown(0, 1);
        wepChange.WeaponChangeInven(wepChange.weaponList[_rng], _id, _dmg);
        wepChange.curWeaponNumInList = _rng;
        weaponChangeBox.gameObject.SetActive(false);

        if (wavSpawner.curBox)
        {
            wavSpawner.curBox.gameObject.SetActive(false);
        }

    }

    public void ChangeSkillCooldown(float _timeLeft, float cdTime)
    {
        float _value = _timeLeft / cdTime;
        timeLeft = _value;

        if (_value == 1)
        {
            skillCooldownImg.color = Color.gray;
        }

        if (_value > 0.05f)
        {
            skillCooldownImg.color = Color32.Lerp(skillCooldownImg.color, skillNormCol, 0.02f);
            skillCooldownText.text = ((int)_timeLeft).ToString();
        }
        else
        {
            skillCooldownImg.color = skillNormCol;
            skillCooldownText.text = "";
        }
    }

    public void LoseLife(int lifeNum)
    {
        if (lifeNum >= 0)
        {
            Animator anim = liveImgs[lifeNum].GetComponent<Animator>();
            anim.SetBool("playAnimation", true);
        }
    }

    public void HatChange(Hat props)
    {
        hatSwitch.ChangeHat(props.sprt, props.dashLength, props.dashRegen, props.dashDmg, props.spd);
    }

    public void StartWaves()
    {
        foreach (GameObject textObj in textGuiders)
        {
            textObj.GetComponent<Animator>().SetBool("fadeOut", true);
        }

        waveStartButton.gameObject.SetActive(false);
        WeaponChange.Instance.gameObject.GetComponent<WaveSpawner>().enabled = true;
    }

    public void ResetPlr(Player _plr)
    {
        plr = _plr;
    }

    public void CloseRngBoxWepChange()
    {
        weaponChangeBox.gameObject.SetActive(false);
    }

    public void MouseOverSkill()
    {
        if (skillImg.sprite != null)
        {
            skillInfoImg.gameObject.SetActive(true);
        }
    }

    public void OpenCloseStatsResetConfirm()
    {
        if (!resetStatsConfirmWindow.gameObject.activeSelf)
        {
            if (Player.Stats.statResetsLeft > 0)
            {
                resetStatsConfirmWindow.gameObject.SetActive(true);
            }
        }
        else
        {
            resetStatsConfirmWindow.gameObject.SetActive(false);
        }
    }

    public void StrikeButton()
    {
        if (plr)
        {
            AttackEvent?.Invoke();
        }
    }

    public void Dash()
    {
        if (plr)
        {
            DashEvent?.Invoke();
        }
    }

    public void MouseOutSkill()
    {
        if (skillImg.sprite != null)
        {
            skillInfoImg.gameObject.SetActive(false);
        }
    }

    public void Pause(float gameSpeed)
    {
        Time.timeScale = gameSpeed;
        pauseButton.SetActive(!pauseButton.activeSelf);
        pauseMenuPanel.SetActive(!pauseMenuPanel.activeSelf);
        coinsYouEarn.text = "You earn: " + StatManager.ConvertToCoins(Player.stoneResource + Player.woodResource + Player.foodResource, wavSpawner.wavesSurvived, false) + " coins!";
    }

    public void PlayButtonSound()
    {
        AudioMaster.Instance.Play("ButtonClick" + UnityEngine.Random.Range(0, AudioMaster.Instance.amountOfButtonSounds));
    }

    private void OnDisable()
    {
        DashEvent = delegate { };
        AttackEvent = delegate { };
    }
}
#pragma warning restore 0649