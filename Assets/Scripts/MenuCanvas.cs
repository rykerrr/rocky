using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

#pragma warning disable 0649
public class MenuCanvas : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject getRandomStats;
    [SerializeField] private GameObject getMoreRandomStats;
    [SerializeField] private GameObject notEnoughMoneyPrompt;
    [SerializeField] private GameObject characterInfoPanel;
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private Image nameInputFieldImage;
    // public InputField nameField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Image buyWeaponPanel;
    [SerializeField] private Image sylviaImg;
    [SerializeField] private Image yagekvapImg;
    [SerializeField] private Image Shop;
    [SerializeField] private Image wepImageWhenBuying;
    [SerializeField] private Image curWepImg;
    [SerializeField] private Image characterImage;
    [SerializeField] private Text notEnoughMoneyText;
    [SerializeField] private Text randomStatsText;
    [SerializeField] private Text coinText;
    [SerializeField] private Text shopOpenCloseText;
    [SerializeField] private Text wepDamage;
    [SerializeField] private Text resGain;
    [SerializeField] private Text wepName;
    [SerializeField] private Text extraDesc;
    [SerializeField] private Text coinCostForWeapon;
    [SerializeField] private Text curWepName;
    [SerializeField] private Text curWepDmg;
    [SerializeField] private Text curWepResGain;
    [SerializeField] private Text characterName;
    [SerializeField] private Text characterDescription;

    private static bool isGameStarted = false;
    private int wepID;

    [Header("Ad rewards")]
    [SerializeField] private int notEnCoinsAdReward;

    [System.Serializable]
    private class WeaponsList
    {
        public string weaponName;
        public int priceOfWeapon;
        public Transform wepTr;
    }

    [SerializeField] private WeaponsList[] weapons;
    private Color coinColor;

    private int coinGain;
    private bool isGainingCoins;

    private void Awake()
    {
        StatManager.currentCoins = PlayerPrefs.GetInt("Coins");
        StatManager.currentWeapon = PlayerPrefs.GetInt("WeaponID");

        if (StatManager.currentCoins == 0)
        {
            StatManager.currentCoins = 500000;
            StatManager.SaveChanges();
        }

        coinColor = coinText.color;
        ChangeCurWeaponImg();
    }

    private void Update()
    {
        if (!isGameStarted)
        {
            StoryPanelActivation(true);
            isGameStarted = true;
        }

        if (isGainingCoins)
        {
            if (coinGain < 0)
            {
                coinText.color = Color.red;
            }
            else
            {
                coinText.color = Color.green;
            }
            coinText.text = " " + StatManager.currentCoins + " +" + coinGain;
        }
        else
        {
            coinText.color = coinColor;
            coinText.text = " " + StatManager.currentCoins;
        }

    }

    public void OpenCloseShop()
    {
        if (!Shop.gameObject.activeSelf)
        {
            shopOpenCloseText.text = "Close shop";
        }
        else
        {
            shopOpenCloseText.text = "Open shop";
        }

        Shop.gameObject.SetActive(!Shop.gameObject.activeSelf);
    }

    public void BuyWeapon(int weaponId)
    {
        OpenWeaponShopPanel(weaponId);
    }

    public void OpenNameInput()
    {
        //nameInputFieldImage.gameObject.SetActive(!nameInputFieldImage.gameObject.activeSelf);
        sylviaImg.gameObject.SetActive(!sylviaImg.gameObject.activeSelf);
        yagekvapImg.gameObject.SetActive(!yagekvapImg.gameObject.activeSelf);
    }

    public void OpenWeaponShopPanel(int weaponId)
    {
        if (wepID != weaponId)
        {
            buyWeaponPanel.gameObject.SetActive(true);

            #region Weapon comparison for colors on what's better
            if (weapons[weaponId].wepTr.GetComponent<BowHandler>())
            {
                wepDamage.text = "Damage: " + weapons[weaponId].wepTr.GetComponent<BowHandler>().dmg.ToString();
                wepName.text = "Name: " + weapons[weaponId].weaponName;
                coinCostForWeapon.text = "Cost: " + weapons[weaponId].priceOfWeapon.ToString();
                resGain.text = "N/A";

                if (weapons[StatManager.currentWeapon].wepTr.GetComponent<BowHandler>())
                {
                    if (weapons[weaponId].wepTr.GetComponent<BowHandler>().dmg > weapons[StatManager.currentWeapon].wepTr.GetComponent<BowHandler>().dmg) wepDamage.color = Color.green;
                    else if (weapons[weaponId].wepTr.GetComponent<BowHandler>().dmg < weapons[StatManager.currentWeapon].wepTr.GetComponent<BowHandler>().dmg) wepDamage.color = Color.red;
                    else
                    {
                        wepDamage.color = Color.white;
                    }
                }
                else if (weapons[StatManager.currentWeapon].wepTr.GetComponent<Boomerang>())
                {
                    if (weapons[weaponId].wepTr.GetComponent<BowHandler>().dmg > weapons[StatManager.currentWeapon].wepTr.GetComponent<Boomerang>().dmg) wepDamage.color = Color.green;
                    else if (weapons[weaponId].wepTr.GetComponent<BowHandler>().dmg < weapons[StatManager.currentWeapon].wepTr.GetComponent<Boomerang>().dmg) wepDamage.color = Color.red;
                    else
                    {
                        wepDamage.color = Color.white;
                    }
                }

            }
            else if (weapons[weaponId].wepTr.GetComponent<Boomerang>())
            {
                wepDamage.text = "Damage: " + weapons[weaponId].wepTr.GetComponent<Boomerang>().dmg.ToString();
                wepName.text = "Name: " + weapons[weaponId].weaponName;
                coinCostForWeapon.text = "Cost: " + weapons[weaponId].priceOfWeapon.ToString();
                resGain.text = "Resource per hit: N/A";

                if (weapons[StatManager.currentWeapon].wepTr.GetComponent<BowHandler>())
                {
                    if (weapons[weaponId].wepTr.GetComponent<Boomerang>().dmg > weapons[StatManager.currentWeapon].wepTr.GetComponent<BowHandler>().dmg) wepDamage.color = Color.green;
                    else if (weapons[weaponId].wepTr.GetComponent<Boomerang>().dmg < weapons[StatManager.currentWeapon].wepTr.GetComponent<BowHandler>().dmg) wepDamage.color = Color.red;
                    else
                    {
                        wepDamage.color = Color.white;
                    }
                }
                else if (weapons[StatManager.currentWeapon].wepTr.GetComponent<Boomerang>())
                {
                    if (weapons[weaponId].wepTr.GetComponent<Boomerang>().dmg > weapons[StatManager.currentWeapon].wepTr.GetComponent<Boomerang>().dmg) wepDamage.color = Color.green;
                    else if (weapons[weaponId].wepTr.GetComponent<Boomerang>().dmg < weapons[StatManager.currentWeapon].wepTr.GetComponent<Boomerang>().dmg) wepDamage.color = Color.red;
                    else
                    {
                        wepDamage.color = Color.white;
                    }
                }
                else
                {
                    wepDamage.color = Color.green;
                }
            }
            else if (weapons[weaponId].wepTr.GetComponent<Weapon>())
            {
                wepDamage.text = "Damage: " + weapons[weaponId].wepTr.GetComponent<Weapon>().dmg.ToString();
                wepName.text = "Name: " + weapons[weaponId].weaponName;
                coinCostForWeapon.text = "Cost: " + weapons[weaponId].priceOfWeapon.ToString();
                resGain.text = "Resources per hit: " + weapons[weaponId].wepTr.GetComponent<ResourceGatheringScript>().resourceGain;

                if (weapons[StatManager.currentWeapon].wepTr.GetComponent<Weapon>())
                {
                    if (weapons[weaponId].wepTr.GetComponent<ResourceGatheringScript>().resourceGain > weapons[StatManager.currentWeapon].wepTr.GetComponent<ResourceGatheringScript>().resourceGain) resGain.color = Color.green;
                    else if (weapons[weaponId].wepTr.GetComponent<ResourceGatheringScript>().resourceGain < weapons[StatManager.currentWeapon].wepTr.GetComponent<ResourceGatheringScript>().resourceGain) resGain.color = Color.red;
                    else
                    {
                        resGain.color = Color.white;
                    }

                    if (weapons[weaponId].wepTr.GetComponent<Weapon>().dmg > weapons[StatManager.currentWeapon].wepTr.GetComponent<Weapon>().dmg) wepDamage.color = Color.green;
                    else if (weapons[weaponId].wepTr.GetComponent<Weapon>().dmg < weapons[StatManager.currentWeapon].wepTr.GetComponent<Weapon>().dmg) wepDamage.color = Color.red;
                    else
                    {
                        wepDamage.color = Color.white;
                    }
                }
                else
                {
                    resGain.color = Color.white;
                    wepDamage.color = Color.white;
                }
            }
            #endregion

            #region Skill/Extra description
            if (weapons[weaponId].wepTr.GetComponent<Ability>())
            {
                extraDesc.text = weapons[weaponId].wepTr.GetComponent<Ability>().SkillDesc;
            }
            else
            {
                extraDesc.text = "Extra: N/A";
            }

            if (weapons[weaponId].wepTr.GetComponent<SpriteRenderer>())
            {
                wepImageWhenBuying.sprite = weapons[weaponId].wepTr.GetComponent<SpriteRenderer>().sprite;
            }
            else
            {
                wepImageWhenBuying.sprite = weapons[weaponId].wepTr.GetComponentInChildren<SpriteRenderer>().sprite;
            }
            #endregion

            wepID = weaponId;
        }
        else
        {
            CloseTransaction();
            wepID = 0;
        }
    }

    public void BuyWeaponTransaction()
    {
        if (StatManager.currentCoins >= weapons[wepID].priceOfWeapon)
        {
            StartCoroutine(GainCoins(-weapons[wepID].priceOfWeapon));
            StatManager.currentCoins -= weapons[wepID].priceOfWeapon;
            StatManager.currentWeapon = wepID;
            StatManager.SaveChanges();
            notEnoughMoneyPrompt.SetActive(false);
        }
        else
        {
            OpenMoneyPrompt("You need " + (weapons[wepID].priceOfWeapon - StatManager.currentCoins).ToString() + " more coins! Watch an ad to get " + notEnCoinsAdReward + " coins and buy the weapon?", BuyWeaponTransaction);
        }

        ChangeCurWeaponImg();
        CloseTransaction();
    }

    private void ChangeCurWeaponImg()
    {
        if (StatManager.currentWeapon != 0)
        {
            if (weapons[StatManager.currentWeapon].wepTr.GetComponent<BowHandler>() || weapons[StatManager.currentWeapon].wepTr.GetComponent<Boomerang>())
            {
                curWepResGain.text = "Resource per hit: N/A";

                curWepImg.sprite = weapons[StatManager.currentWeapon].wepTr.GetComponent<SpriteRenderer>().sprite;

                if (weapons[StatManager.currentWeapon].wepTr.GetComponent<BowHandler>())
                {
                    curWepDmg.text = "Damage: " + weapons[StatManager.currentWeapon].wepTr.GetComponent<BowHandler>().dmg.ToString();
                }
                else
                {
                    curWepDmg.text = "Damage: " + weapons[StatManager.currentWeapon].wepTr.GetComponent<Boomerang>().dmg.ToString();
                }

            }
            else
            {
                curWepImg.sprite = weapons[StatManager.currentWeapon].wepTr.GetComponentInChildren<SpriteRenderer>().sprite;
                curWepDmg.text = "Damage: " + weapons[StatManager.currentWeapon].wepTr.GetComponentInChildren<ResourceGatheringScript>().resourceGain.ToString();
                curWepResGain.text = "Res gain: " + weapons[StatManager.currentWeapon].wepTr.GetComponentInChildren<Weapon>().dmg.ToString();
            }

            curWepName.text = "Name: " + weapons[StatManager.currentWeapon].weaponName;
        }
        else
        {
            curWepImg.sprite = null;
            curWepName.text = "Weapon not found, why not buy one at the shop!";
            curWepResGain.text = "Damage: N/A";
            curWepDmg.text = "Resource per hit: N/A";
        }


    }

    private void OpenMoneyPrompt(string msg, Action callback = null)
    {
        notEnoughMoneyPrompt.SetActive(true);
        notEnoughMoneyText.text = msg;
        callback?.Invoke();
    }

    public void CloseTransaction()
    {
        buyWeaponPanel.gameObject.SetActive(false);
    }

    public void CloseNotEnoughMoney()
    {
        notEnoughMoneyPrompt.SetActive(false);
    }

    public void StartMatchHat(Hat _hat) // retard it won't work if you don't start it from the main menu scene
    {
        Player.Stats.hat = _hat;
        Player.Stats.name = _hat.characterName;
        confirmButton.onClick.RemoveAllListeners();
        SceneManager.LoadScene("MatchScene");
    }

    public void Quit()
    {
        StatManager.SaveChanges();
        Application.Quit();
    }

    public void StartMatch()
    {
        // Player.Stats.name = nameField.text;
        GameMaster.normalStart = true;
        SceneManager.LoadScene("MatchScene");
    }

    public void OpenCloseGetRandomStat()
    {
        getRandomStats.SetActive(!getRandomStats.activeSelf);
    }

    public void OpenCloseGetMoreRandomStats()
    {
        getMoreRandomStats.SetActive(!getMoreRandomStats.activeSelf);
    }

    public void NotEnoughMoneyAdCall()
    {
        AdPrompt(NotEnoughMoneyAdFinish);
    }

    public void GetRandomStatsAdCall()
    {
        AdPrompt(RandomStatAdFinish);
    }

    private void AdPrompt(Action<ShowResult> resultCallback)
    {
        AdsManager.Instance.ShowRewardedAd(resultCallback);
    }

    private void FreeCoins(ShowResult so)
    {
        switch (so)
        {
            case ShowResult.Finished:
                StartCoroutine(GainCoins(notEnCoinsAdReward));
                StatManager.currentCoins += notEnCoinsAdReward;
                break;
            case ShowResult.Failed:
                OpenMoneyPrompt("Ad failed, coins not rewarded. Wait a bit or go online then try again.", delegate { });
                break;
            case ShowResult.Skipped:
                OpenMoneyPrompt("Ad skipped, coins not rewarded. Try again?", delegate { });
                break;
        }
    }

    private void NotEnoughMoneyAdFinish(ShowResult so)
    {
        switch (so)
        {
            case ShowResult.Failed:
                randomStatsText.text = "Ad failed, wait a little bit and try again or check your internet connection!";
                break;
            case ShowResult.Skipped:
                randomStatsText.text = "Ad skipped, nothing rewarded.";
                break;
            case ShowResult.Finished:
                StartCoroutine(GainCoins(notEnCoinsAdReward));
                StatManager.currentCoins += notEnCoinsAdReward;
                BuyWeaponTransaction();
                break;
        }
    }

    private void RandomStatAdFinish(ShowResult so)
    {
        getRandomStats.SetActive(false);
        getMoreRandomStats.SetActive(true);

        switch (so)
        {
            case ShowResult.Failed:
                randomStatsText.text = "Ad failed, wait a little bit and try again or check your internet connection!";
                break;
            case ShowResult.Skipped:
                randomStatsText.text = "Ad skipped, nothing rewarded.";
                break;
            case ShowResult.Finished:
                RandomStat();
                break;
        }
    }

    public void RandomStat()
    {
        Player.Stats.dexterityStat = 0;
        Player.Stats.strengthStat = 0;
        Player.Stats.vitalityStat = 0;

        int whichStat = UnityEngine.Random.Range(0, 3);
        if (whichStat == 0 || whichStat == 1)
        {
            whichStat = UnityEngine.Random.Range(0, 3);
        }

        int ranAmn = UnityEngine.Random.Range(1, 9);

        for (int i = 0; i < 5; i++)
        {
            if (ranAmn > 5)
            {
                if (ranAmn > 6)
                {

                    if (ranAmn > 7)
                    {

                        if (ranAmn > 8)
                        {
                            break;
                        }

                        ranAmn = UnityEngine.Random.Range(1, 9);
                        i += 2;
                        continue;
                    }

                    ranAmn = UnityEngine.Random.Range(1, 8);
                    i += 3;
                    continue;
                }

                ranAmn = UnityEngine.Random.Range(1, 7);
                i += 4;
                continue;
            }
            else
            {
                break;
            }
        }

        switch (whichStat)
        {
            case 0:
                Player.Stats.strengthStat += ranAmn;
                randomStatsText.text = "Congrats! You got " + ranAmn + " bonus strength!";
                break;
            case 1:
                Player.Stats.dexterityStat += ranAmn;
                randomStatsText.text = "Congrats! You got " + ranAmn + " bonus dexterity!";
                break;
            case 2:
                Player.Stats.vitalityStat += ranAmn;
                randomStatsText.text = "Congrats! You got " + ranAmn + " bonus vitality!";
                break;
            default:
                return;
        }

        randomStatsText.text += "Watch another video to try and get a better bonus?";
    }

    public void GetFreeCoins()
    {
        AdPrompt(FreeCoins);
    }

    public void ShowCharacterInfo(Hat hat)
    {
        characterInfoPanel.SetActive(true);

        characterName.text = "Name: " + hat.characterName;
        characterDescription.text = "Description: \n" + hat.characterDesc;
        characterImage.sprite = hat.sprt;
        confirmButton.onClick.AddListener(() => StartMatchHat(hat));
    }

    public void CloseCharacterInfo()
    {
        confirmButton.onClick.RemoveAllListeners();
        characterInfoPanel.SetActive(false);
    }

    public void PlayButtonSound()
    {
        AudioMaster.Instance.Play("ButtonClick" + UnityEngine.Random.Range(0, AudioMaster.Instance.amountOfButtonSounds));
    }

    private IEnumerator GainCoins(int coins, float wait = 1f)
    {
        isGainingCoins = true;
        coinGain = coins;
        yield return new WaitForSeconds(wait);
        isGainingCoins = false;
        coinGain = 0;
    }

    public void StoryPanelActivation(bool open)
    {
        if (open)
        {
            storyPanel.SetActive(true);
        }
        else
        {
            storyPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        AudioMaster.Instance.Play("MainMenuTheme");
    }

    private void OnDisable()
    {
        if (AudioMaster.Instance)
        {
            AudioMaster.Instance.StopPlaying("MainMenuTheme");
        }
    }
}
#pragma warning restore 0649