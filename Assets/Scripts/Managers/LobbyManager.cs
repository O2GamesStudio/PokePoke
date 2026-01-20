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

    void StartOnClick() => SceneLoader.LoadGameScenes(1, 2);

    void InfiniteModeOnClick() { }
}