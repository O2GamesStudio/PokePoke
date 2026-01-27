using UnityEngine;
using UnityEngine.UI;

public class StageUISet : MonoBehaviour
{
    public Image[] stageImages;
    private static readonly Color activeColor = new Color(0.35f, 0f, 0f);
    private static readonly Color inactiveColor = Color.white;

    public void UpdateStageVisual(int stageNumber)
    {
        if (stageImages == null || stageImages.Length == 0) return;

        int activeIndex = (stageNumber - 1) % 5;

        for (int i = 0; i < stageImages.Length; i++)
        {
            if (stageImages[i] != null)
            {
                stageImages[i].color = (i == activeIndex) ? activeColor : inactiveColor;
            }
        }
    }
}