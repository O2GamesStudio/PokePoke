using UnityEngine;
using DG.Tweening;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] RectTransform titleTS;
    [SerializeField] RectTransform stageTS;
    [SerializeField] RectTransform leftBtnTS;
    [SerializeField] RectTransform rightBtnTS;
    [SerializeField] RectTransform startBtnTS;

    [SerializeField] float animDuration = 0.8f;
    [SerializeField] float overshoot = 1.2f;

    Vector2 titleOrigin, stageOrigin, leftOrigin, rightOrigin, startOrigin;

    void Start()
    {
        titleOrigin = titleTS.anchoredPosition;
        stageOrigin = stageTS.anchoredPosition;
        leftOrigin = leftBtnTS.anchoredPosition;
        rightOrigin = rightBtnTS.anchoredPosition;
        startOrigin = startBtnTS.anchoredPosition;

        titleTS.anchoredPosition = new Vector2(titleOrigin.x, Screen.height);
        stageTS.anchoredPosition = new Vector2(stageOrigin.x, -Screen.height);
        startBtnTS.anchoredPosition = new Vector2(startOrigin.x, -Screen.height);
        leftBtnTS.anchoredPosition = new Vector2(-Screen.width, leftOrigin.y);
        rightBtnTS.anchoredPosition = new Vector2(Screen.width, rightOrigin.y);

        titleTS.DOAnchorPos(titleOrigin, animDuration).SetEase(Ease.OutBack, overshoot);
        stageTS.DOAnchorPos(stageOrigin, animDuration).SetEase(Ease.OutBack, overshoot).SetDelay(0.05f);
        startBtnTS.DOAnchorPos(startOrigin, animDuration).SetEase(Ease.OutBack, overshoot).SetDelay(0.1f);
        leftBtnTS.DOAnchorPos(leftOrigin, animDuration).SetEase(Ease.OutBack, overshoot).SetDelay(0.15f);
        rightBtnTS.DOAnchorPos(rightOrigin, animDuration).SetEase(Ease.OutBack, overshoot).SetDelay(0.2f);
    }
}