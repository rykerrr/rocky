using System;
using UnityEngine;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class AdsManager : MonoBehaviour // literally the "manager", plays ads and then runs the callbacks given to it
{
    #region Singleton
    private static AdsManager instance;

    public static AdsManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<AdsManager>();
                if(instance == null)
                {
                    instance = new GameObject("Ad Manager Gameobject", typeof(AdsManager)).GetComponent<AdsManager>();

                    if(instance == null)
                    {
                        throw new Exception("AdsManager can't be spawned in...");
                    }

                }

            }

            return instance;
        }
    }
    #endregion

    [Header("Game Ads Config")]
    [SerializeField] private string gameID = default;
    [SerializeField] private bool testMode = true;
    [SerializeField] private string rewardedAdPlacementID = default;
    [SerializeField] private string normalAdPlacementID = default;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
        Advertisement.Initialize(gameID, testMode);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ShowRegularAd(Action<ShowResult> callBack)
    {
#if UNITY_ADS
        if(Advertisement.IsReady(normalAdPlacementID))
        {
            ShowOptions so = new ShowOptions();
            so.resultCallback = callBack; // the result callback is just an action that gets called at the end
            Advertisement.Show(normalAdPlacementID, so);
        }
#else
#endif
    }

    public void ShowRewardedAd(Action<ShowResult> callBack)
    {
#if UNITY_ADS
        if (Advertisement.IsReady(rewardedAdPlacementID))
        {
            ShowOptions so = new ShowOptions();
            so.resultCallback = callBack; // the result callback is just an action that gets called at the end
            Advertisement.Show(rewardedAdPlacementID, so);
        }
    }
#else
#endif
}
