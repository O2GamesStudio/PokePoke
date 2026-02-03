using UnityEngine;
using Unity.Services.Core;
using Unity.Services.LevelPlay;
using System;
using System.Collections;

public class UnityAdsManager : MonoBehaviour
{
    private static UnityAdsManager instance;
    public static UnityAdsManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("UnityAdsManager");
                instance = go.AddComponent<UnityAdsManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    [Header("LevelPlay Settings")]
    [SerializeField] private string appKey = "YOUR_APP_KEY";

    [Header("Ad Unit IDs")]
    [SerializeField] private string androidRewardedAdUnitId = "3qr3vgi9qnx52n2u";
    [SerializeField] private string iOSRewardedAdUnitId = "3qr3vgi9qnx52n2u";
    [SerializeField] private string androidBannerAdUnitId = "9ulhleug5p8oljo8";
    [SerializeField] private string iOSBannerAdUnitId = "9ulhleug5p8oljo8";

    private string rewardedAdUnitId;
    private string bannerAdUnitId;

    private LevelPlayRewardedAd rewardedAd;
    private LevelPlayBannerAd bannerAd;

    private bool isInitialized = false;
    private bool isAdLoaded = false;
    private bool isLoadingAd = false;
    private bool isBannerLoaded = false;

    private Action onRewardedAdCompleted;
    private Action onRewardedAdFailed;
    private bool rewardEarned = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID
            rewardedAdUnitId = androidRewardedAdUnitId;
            bannerAdUnitId = androidBannerAdUnitId;
#elif UNITY_IOS
            rewardedAdUnitId = iOSRewardedAdUnitId;
            bannerAdUnitId = iOSBannerAdUnitId;
#else
            rewardedAdUnitId = androidRewardedAdUnitId;
            bannerAdUnitId = androidBannerAdUnitId;
#endif
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(appKey) || appKey == "YOUR_APP_KEY") return;
        StartCoroutine(InitializeLevelPlay());
    }

    private IEnumerator InitializeLevelPlay()
    {
        var initTask = UnityServices.InitializeAsync();

        while (!initTask.IsCompleted)
        {
            yield return null;
        }

        if (initTask.IsFaulted) yield break;

        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;
        LevelPlay.Init(appKey);
    }

    private void OnInitSuccess(LevelPlayConfiguration config)
    {
        isInitialized = true;
        SetupRewardedAd();
        SetupBannerAd();
    }

    private void OnInitFailed(LevelPlayInitError error)
    {
        isInitialized = false;
    }

    #region Rewarded Ad

    private void SetupRewardedAd()
    {
        rewardedAd = new LevelPlayRewardedAd(rewardedAdUnitId);

        rewardedAd.OnAdLoaded += OnRewardedAdLoaded;
        rewardedAd.OnAdLoadFailed += OnRewardedAdLoadFailed;
        rewardedAd.OnAdDisplayFailed += OnRewardedAdDisplayFailed;
        rewardedAd.OnAdClosed += OnRewardedAdClosedInternal;
        rewardedAd.OnAdRewarded += OnRewardedAdRewardedInternal;

        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        if (isLoadingAd || !isInitialized)
        {
            if (!isInitialized) StartCoroutine(LoadAdWithDelay(3f));
            return;
        }

        isLoadingAd = true;
        isAdLoaded = false;

        try
        {
            rewardedAd?.LoadAd();
        }
        catch (Exception)
        {
            isLoadingAd = false;
            StartCoroutine(LoadAdWithDelay(10f));
        }
    }

    private IEnumerator LoadAdWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadRewardedAd();
    }

    private void OnRewardedAdLoaded(LevelPlayAdInfo adInfo)
    {
        isLoadingAd = false;
        isAdLoaded = true;
    }

    private void OnRewardedAdLoadFailed(LevelPlayAdError error)
    {
        isLoadingAd = false;
        isAdLoaded = false;
        StartCoroutine(LoadAdWithDelay(10f));
    }

    private void OnRewardedAdDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        isAdLoaded = false;
        onRewardedAdFailed?.Invoke();
        onRewardedAdCompleted = null;
        onRewardedAdFailed = null;
        rewardEarned = false;
        StartCoroutine(LoadAdWithDelay(0.5f));
    }

    private void OnRewardedAdClosedInternal(LevelPlayAdInfo adInfo)
    {
        StartCoroutine(HandleAdClosed());
    }

    private IEnumerator HandleAdClosed()
    {
        yield return new WaitForSeconds(0.5f);

        if (!rewardEarned)
        {
            onRewardedAdFailed?.Invoke();
        }
        onRewardedAdCompleted = null;
        onRewardedAdFailed = null;
        rewardEarned = false;
        StartCoroutine(LoadAdWithDelay(0.5f));
    }

    private void OnRewardedAdRewardedInternal(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        rewardEarned = true;
        onRewardedAdCompleted?.Invoke();
        onRewardedAdCompleted = null;
        onRewardedAdFailed = null;
    }

    public void ShowRewardedAd(Action onCompleted, Action onFailed = null)
    {
        if (!isInitialized || rewardedAd == null || !isAdLoaded || !rewardedAd.IsAdReady())
        {
            onFailed?.Invoke();
            if (!isLoadingAd) LoadRewardedAd();
            return;
        }

        onRewardedAdCompleted = onCompleted;
        onRewardedAdFailed = onFailed;
        rewardEarned = false;

        try
        {
            rewardedAd.ShowAd();
            isAdLoaded = false;
        }
        catch (Exception)
        {
            isAdLoaded = false;
            onRewardedAdFailed?.Invoke();
            onRewardedAdCompleted = null;
            onRewardedAdFailed = null;
            rewardEarned = false;
            StartCoroutine(LoadAdWithDelay(0.5f));
        }
    }

    public bool IsRewardedAdReady()
    {
        return isAdLoaded && rewardedAd != null && rewardedAd.IsAdReady();
    }

    #endregion

    #region Banner Ad

    private void SetupBannerAd()
    {
        try
        {
            var bannerConfig = new LevelPlayBannerAd.Config.Builder()
                .SetSize(LevelPlayAdSize.BANNER)
                .SetPosition(LevelPlayBannerPosition.BottomCenter)
                .SetDisplayOnLoad(false)
                .Build();

            bannerAd = new LevelPlayBannerAd(bannerAdUnitId, bannerConfig);

            bannerAd.OnAdLoaded += OnBannerAdLoaded;
            bannerAd.OnAdLoadFailed += OnBannerAdLoadFailed;
        }
        catch (Exception) { }
    }

    public void LoadBannerAd()
    {
        if (!isInitialized)
        {
            StartCoroutine(LoadBannerAfterInit());
            return;
        }

        bannerAd?.LoadAd();
    }

    private IEnumerator LoadBannerAfterInit()
    {
        float waitTime = 0f;
        while (!isInitialized && waitTime < 10f)
        {
            yield return new WaitForSeconds(0.5f);
            waitTime += 0.5f;
        }

        if (isInitialized) LoadBannerAd();
    }

    private void OnBannerAdLoaded(LevelPlayAdInfo adInfo)
    {
        isBannerLoaded = true;
        bannerAd?.ShowAd();
    }

    private void OnBannerAdLoadFailed(LevelPlayAdError error)
    {
        isBannerLoaded = false;
    }

    public void ShowBanner()
    {
        if (!isInitialized) return;

        if (isBannerLoaded)
            bannerAd?.ShowAd();
        else
            bannerAd?.LoadAd();
    }

    public void HideBanner()
    {
        bannerAd?.HideAd();
    }

    public void DestroyBanner()
    {
        bannerAd?.DestroyAd();
        isBannerLoaded = false;
    }

    #endregion

    private void OnDestroy()
    {
        LevelPlay.OnInitSuccess -= OnInitSuccess;
        LevelPlay.OnInitFailed -= OnInitFailed;

        if (rewardedAd != null)
        {
            rewardedAd.OnAdLoaded -= OnRewardedAdLoaded;
            rewardedAd.OnAdLoadFailed -= OnRewardedAdLoadFailed;
            rewardedAd.OnAdDisplayFailed -= OnRewardedAdDisplayFailed;
            rewardedAd.OnAdClosed -= OnRewardedAdClosedInternal;
            rewardedAd.OnAdRewarded -= OnRewardedAdRewardedInternal;
        }

        if (bannerAd != null)
        {
            bannerAd.OnAdLoaded -= OnBannerAdLoaded;
            bannerAd.OnAdLoadFailed -= OnBannerAdLoadFailed;
            bannerAd.DestroyAd();
        }
    }
}