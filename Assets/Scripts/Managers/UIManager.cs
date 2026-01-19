using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] Button retryBtn, exitBtn, continueBtn;
    [SerializeField] Button nextBtn, exitWinBtn;
    [SerializeField] Button screenBtn;
    [SerializeField] Image bgImage;
    [SerializeField] Image targetImage;
    [SerializeField] TextMeshProUGUI targetText;
    [SerializeField] TextMeshProUGUI stageText;
    [SerializeField] float fillDuration = 0.3f;
    [SerializeField] Ease fillEase = Ease.OutQuad;

    [Header("Button Animation Settings")]
    [SerializeField] float buttonMoveDistance = 300f;
    [SerializeField] float buttonMoveDuration = 0.4f;
    [SerializeField] Ease buttonMoveEase = Ease.OutCubic;
    [SerializeField] float bounceUpAmount = 20f;
    [SerializeField] float bounceDownAmount = 10f;
    [SerializeField] float bounceDuration = 0.15f;

    [Header("Scale Animation Settings")]
    [SerializeField] float scaleUpValue = 1.2f;
    [SerializeField] float scaleUpDuration = 0.2f;
    [SerializeField] float scaleDownDuration = 0.15f;
    [SerializeField] float exitButtonDelay = 0.3f;
    [SerializeField] float exitButtonScaleDuration = 0.5f;

    public CircleMaskController circleMask;

    private Vector3 nextBtnHiddenPos, exitWinBtnHiddenPos;
    private Vector3 nextBtnTargetPos, exitWinBtnTargetPos;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        screenBtn.onClick.AddListener(ScreenOnClick);
        exitBtn.onClick.AddListener(ExitOnClick);
        retryBtn.onClick.AddListener(RetryOnClick);
        continueBtn.onClick.AddListener(ContinueOnClick);
        exitWinBtn.onClick.AddListener(ExitOnClick);
        nextBtn.onClick.AddListener(NextOnClick);

        nextBtnHiddenPos = nextBtn.transform.localPosition;
        exitWinBtnHiddenPos = exitWinBtn.transform.localPosition;

        nextBtnTargetPos = nextBtnHiddenPos + new Vector3(0, buttonMoveDistance, 0);
        exitWinBtnTargetPos = exitWinBtnHiddenPos + new Vector3(0, buttonMoveDistance, 0);

        retryBtn.transform.localScale = Vector3.zero;
        exitBtn.transform.localScale = Vector3.zero;
        continueBtn.transform.localScale = Vector3.zero;
    }

    public void UpdateStageText(int stageNumber)
    {
        if (stageText != null)
        {
            stageText.text = "Stage " + stageNumber;
        }
    }

    public void ShowWinUI()
    {
        Sequence nextSeq = DOTween.Sequence();
        Sequence exitSeq = DOTween.Sequence();

        nextSeq.Append(nextBtn.transform.DOLocalMove(nextBtnTargetPos, buttonMoveDuration).SetEase(buttonMoveEase))
                .AppendCallback(() => StartButtonBounce(nextBtn, nextBtnTargetPos));

        exitSeq.Append(exitWinBtn.transform.DOLocalMove(exitWinBtnTargetPos, buttonMoveDuration).SetEase(buttonMoveEase))
               .AppendCallback(() => StartButtonBounce(exitWinBtn, exitWinBtnTargetPos));
    }

    public void ShowLoseUI()
    {
        continueBtn.transform.localScale = Vector3.zero;
        retryBtn.transform.localScale = Vector3.zero;
        exitBtn.transform.localScale = Vector3.zero;

        StartMainButtonScaleAnimation(continueBtn);
        StartMainButtonScaleAnimation(retryBtn);

        float mainButtonTotalDuration = scaleUpDuration + scaleDownDuration;
        DOVirtual.DelayedCall(mainButtonTotalDuration + exitButtonDelay, () =>
        {
            StartExitButtonScaleAnimation(exitBtn);
        });
    }

    void StartMainButtonScaleAnimation(Button button)
    {
        button.transform.localScale = Vector3.zero;

        Sequence scaleSeq = DOTween.Sequence();
        scaleSeq.Append(button.transform.DOScale(scaleUpValue, scaleUpDuration).SetEase(Ease.OutBack));
        scaleSeq.Append(button.transform.DOScale(1f, scaleDownDuration).SetEase(Ease.InOutQuad));
    }

    void StartExitButtonScaleAnimation(Button button)
    {
        button.transform.localScale = Vector3.zero;
        button.transform.DOScale(1f, exitButtonScaleDuration).SetEase(Ease.OutQuad);
    }

    public void UpdateBgImage(Sprite newBgSprite)
    {
        if (bgImage != null && newBgSprite != null)
        {
            bgImage.sprite = newBgSprite;
        }
    }

    void StartButtonBounce(Button button, Vector3 targetPos)
    {
        Sequence bounceSeq = DOTween.Sequence();

        Vector3 upPos = targetPos + new Vector3(0, bounceUpAmount, 0);
        bounceSeq.Append(button.transform.DOLocalMove(upPos, bounceDuration).SetEase(Ease.OutQuad));
        bounceSeq.Append(button.transform.DOLocalMove(targetPos, bounceDuration).SetEase(Ease.OutQuad));
    }

    public void TargetUIUpdate(int targetVal, int nowVal)
    {
        float targetFillAmount = (float)nowVal / targetVal;

        targetImage.DOKill();
        targetImage.DOFillAmount(targetFillAmount, fillDuration)
            .SetEase(fillEase);

        targetText.text = nowVal + "/" + targetVal;
    }

    void ContinueOnClick()
    {
        DOTween.KillAll();

        circleMask.Hide();

        retryBtn.transform.localScale = Vector3.zero;
        exitBtn.transform.localScale = Vector3.zero;
        continueBtn.transform.localScale = Vector3.zero;

        GameManager.Instance.RestartStage();
    }
    void NextOnClick()
    {

    }

    void ExitOnClick()
    {
        DOTween.KillAll();
        SceneLoader.LoadSingleScene(0);
    }

    void RetryOnClick()
    {
        DOTween.KillAll();

        int scene1Index = -1;
        int scene2Index = -1;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            int buildIndex = scene.buildIndex;

            if (scene1Index == -1)
                scene1Index = buildIndex;
            else if (scene2Index == -1)
                scene2Index = buildIndex;
        }

        SceneLoader.LoadGameScenes(scene1Index, scene2Index);
    }

    void ScreenOnClick()
    {
        GameManager.Instance.OnClick();
    }
}