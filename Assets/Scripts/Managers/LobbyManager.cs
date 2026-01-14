using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] Button startBtn, infiniteModeBtn;

    void Awake()
    {
        startBtn.onClick.AddListener(StartOnClick);
        infiniteModeBtn.onClick.AddListener(InfiniteModeOnClick);
    }

    void StartOnClick()
    {
        // 간단하게 호출!
        SceneLoader.LoadGameScenes(1, 2);
    }

    void InfiniteModeOnClick()
    {
        // 나중에 무한 모드 구현
    }
}