using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;

    /// <summary>
    /// 씬 2개를 함께 로드 (첫 번째는 Single, 두 번째는 Additive)
    /// </summary>
    public static void LoadGameScenes(int scene1Index, int scene2Index)
    {
        CreateLoaderAndLoad(scene1Index, scene2Index);
    }

    /// <summary>
    /// 씬 이름으로 2개를 함께 로드
    /// </summary>
    public static void LoadGameScenes(string scene1Name, string scene2Name)
    {
        CreateLoaderAndLoad(scene1Name, scene2Name);
    }

    /// <summary>
    /// 단일 씬 로드
    /// </summary>
    public static void LoadSingleScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// 단일 씬 로드 (이름)
    /// </summary>
    public static void LoadSingleScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // 인덱스로 로드
    private static void CreateLoaderAndLoad(int scene1Index, int scene2Index)
    {
        GameObject loader = new GameObject("SceneLoader");
        DontDestroyOnLoad(loader);
        loader.AddComponent<SceneLoader>().StartLoadingByIndex(scene1Index, scene2Index);
    }

    // 이름으로 로드
    private static void CreateLoaderAndLoad(string scene1Name, string scene2Name)
    {
        GameObject loader = new GameObject("SceneLoader");
        DontDestroyOnLoad(loader);
        loader.AddComponent<SceneLoader>().StartLoadingByName(scene1Name, scene2Name);
    }

    // 인덱스로 로딩 시작
    private void StartLoadingByIndex(int scene1Index, int scene2Index)
    {
        StartCoroutine(LoadScenesCoroutine(scene1Index, scene2Index));
    }

    // 이름으로 로딩 시작
    private void StartLoadingByName(string scene1Name, string scene2Name)
    {
        StartCoroutine(LoadScenesCoroutine(scene1Name, scene2Name));
    }

    // 인덱스 기반 코루틴
    private IEnumerator LoadScenesCoroutine(int scene1Index, int scene2Index)
    {
        Debug.Log($"씬 {scene1Index} 로드 시작");
        yield return SceneManager.LoadSceneAsync(scene1Index, LoadSceneMode.Single);
        Debug.Log($"씬 {scene1Index} 로드 완료");

        Debug.Log($"씬 {scene2Index} 로드 시작");
        yield return SceneManager.LoadSceneAsync(scene2Index, LoadSceneMode.Additive);
        Debug.Log($"씬 {scene2Index} 로드 완료");

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(scene1Index));

        Debug.Log($"현재 로드된 씬 개수: {SceneManager.sceneCount}");

        Destroy(gameObject);
    }

    // 이름 기반 코루틴
    private IEnumerator LoadScenesCoroutine(string scene1Name, string scene2Name)
    {
        Debug.Log($"씬 {scene1Name} 로드 시작");
        yield return SceneManager.LoadSceneAsync(scene1Name, LoadSceneMode.Single);
        Debug.Log($"씬 {scene1Name} 로드 완료");

        Debug.Log($"씬 {scene2Name} 로드 시작");
        yield return SceneManager.LoadSceneAsync(scene2Name, LoadSceneMode.Additive);
        Debug.Log($"씬 {scene2Name} 로드 완료");

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene1Name));

        Debug.Log($"현재 로드된 씬 개수: {SceneManager.sceneCount}");

        Destroy(gameObject);
    }
}