using UnityEngine;
using DG.Tweening;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] RectTransform targetTs;
    [SerializeField] RectTransform stageTs;

    [SerializeField] float animDuration = 0.6f;
    [SerializeField] float stageDelay = 0.1f;

    Vector2 targetOrigin, stageOrigin;

    void Start()
    {
        targetOrigin = targetTs.anchoredPosition;
        stageOrigin = stageTs.anchoredPosition;

        targetTs.anchoredPosition = new Vector2(targetOrigin.x, Screen.height);
        stageTs.anchoredPosition = new Vector2(stageOrigin.x, Screen.height);

        targetTs.DOAnchorPos(targetOrigin, animDuration).SetEase(Ease.OutCubic);
        stageTs.DOAnchorPos(stageOrigin, animDuration).SetEase(Ease.OutCubic).SetDelay(stageDelay);
    }
}