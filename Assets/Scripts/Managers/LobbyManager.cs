// LobbyManager.cs
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public enum GameMode { Chapter, Infinite }

    public static ChapterData SelectedChapter { get; private set; }
    public static int LastPlayedChapterIndex { get; private set; } = 0;
    public static GameMode SelectedGameMode { get; private set; } = GameMode.Chapter;

    [SerializeField] Image chapterImage, bgImage;
    [SerializeField] Button startBtn, infiniteModeBtn;
    [SerializeField] Button preBtn, nextBtn;
    [SerializeField] float rotationDuration = 0.3f;
    [SerializeField] TextMeshProUGUI highestStageText;
    int chapterNum = 0;
    [SerializeField] ChapterData[] chapterDatas;
    bool isAnimating = false;

    void Awake()
    {
        InitializeChapterProgress();

        chapterNum = LastPlayedChapterIndex;

        startBtn.onClick.AddListener(StartOnClick);
        infiniteModeBtn.onClick.AddListener(InfiniteModeOnClick);
        preBtn.onClick.AddListener(PreBtnOnClick);
        nextBtn.onClick.AddListener(NextBtnOnClick);

        UpdateChapterDisplay();
        UpdateHighestStageText();
        UpdateNavigationButtons();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(0);
        }
    }

    void InitializeChapterProgress()
    {
        if (chapterDatas == null) return;

        for (int i = 0; i < chapterDatas.Length; i++)
        {
            string key = $"Chapter_{i}_HighestStage";
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 0);
            }
        }
        PlayerPrefs.Save();
    }

    void PreBtnOnClick()
    {
        if (isAnimating || chapterDatas == null || chapterDatas.Length == 0) return;

        chapterNum--;
        if (chapterNum < 0) chapterNum = chapterDatas.Length - 1;

        AnimateChapterChange();
    }

    void NextBtnOnClick()
    {
        if (isAnimating || chapterDatas == null || chapterDatas.Length == 0) return;

        chapterNum++;
        if (chapterNum >= chapterDatas.Length) chapterNum = 0;

        AnimateChapterChange();
    }

    void AnimateChapterChange()
    {
        isAnimating = true;

        chapterImage.transform.DORotate(new Vector3(0, 90, 0), rotationDuration * 0.5f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                UpdateChapterDisplay();
                UpdateHighestStageText();
                UpdateNavigationButtons();

                chapterImage.transform.DORotate(Vector3.zero, rotationDuration * 0.5f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() => isAnimating = false);
            });
    }

    void UpdateChapterDisplay()
    {
        if (chapterDatas == null || chapterDatas.Length == 0) return;

        ChapterData currentChapter = chapterDatas[chapterNum];
        if (currentChapter == null) return;

        if (chapterImage != null && currentChapter.ChapterImage != null)
        {
            chapterImage.sprite = currentChapter.ChapterImage;
        }

        if (bgImage != null && currentChapter.ChapterBgImage != null)
        {
            bgImage.sprite = currentChapter.ChapterBgImage;
        }
    }

    void UpdateHighestStageText()
    {
        if (highestStageText == null || chapterDatas == null || chapterDatas.Length == 0) return;

        int savedStage = PlayerPrefs.GetInt($"Chapter_{chapterNum}_HighestStage", 0);
        highestStageText.text = savedStage > 0 ? $"Stage {savedStage}" : "Stage 1";
    }

    void UpdateNavigationButtons()
    {
        if (chapterDatas == null || chapterDatas.Length <= 1)
        {
            if (preBtn != null) preBtn.interactable = false;
            if (nextBtn != null) nextBtn.interactable = false;
            return;
        }

        if (preBtn != null) preBtn.interactable = chapterNum > 0;

        if (nextBtn != null)
        {
            bool canGoNext = chapterNum < chapterDatas.Length - 1;
            if (canGoNext)
            {
                int currentChapterMaxStage = chapterDatas[chapterNum].TotalStages;
                int clearedStage = PlayerPrefs.GetInt($"Chapter_{chapterNum}_HighestStage", 0);
                canGoNext = clearedStage >= currentChapterMaxStage;
            }
            nextBtn.interactable = canGoNext;
        }
    }

    void StartOnClick()
    {
        if (chapterDatas == null || chapterDatas.Length == 0) return;

        int maxStage = chapterDatas[chapterNum].TotalStages;
        int clearedStage = PlayerPrefs.GetInt($"Chapter_{chapterNum}_HighestStage", 0);

        if (chapterNum > 0)
        {
            int prevChapterMaxStage = chapterDatas[chapterNum - 1].TotalStages;
            int prevChapterClearedStage = PlayerPrefs.GetInt($"Chapter_{chapterNum - 1}_HighestStage", 0);

            if (prevChapterClearedStage < prevChapterMaxStage)
            {
                return;
            }
        }

        SelectedChapter = chapterDatas[chapterNum];
        LastPlayedChapterIndex = chapterNum;
        SelectedGameMode = GameMode.Chapter;
        SceneLoader.LoadGameScenes(1, 2);
    }

    void InfiniteModeOnClick()
    {
        SelectedGameMode = GameMode.Infinite;
        SceneLoader.LoadGameScenes(1, 2);
    }

    void OnDestroy()
    {
        if (chapterImage != null)
        {
            chapterImage.transform.DOKill();
        }
    }
}