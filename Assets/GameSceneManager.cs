using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Extensions.Constants;
using Extensions;

public static class GameSceneManager
{
    public static int? StagePrepped { get; private set; } = null;
    static Navigation<int> sceneNavigation = new Navigation<int>();
    static AsyncOperation LoadingOperation;
    static IEnumerator LoadSceneAsyncCoroutine;
    static Scene ActiveScene => SceneManager.GetActiveScene();

    public static void LoadScene(int buildIndex, bool extendNavigation = false)
    {
        LoadSceneAsyncCoroutine = LoadSceneAsync(buildIndex);

        if (extendNavigation)
            sceneNavigation.Stretch(ActiveScene.buildIndex);

        LoadSceneAsyncCoroutine.Start();
    }

    static IEnumerator LoadSceneAsync(int buildIndex)
    {
        
        try
        {
            LoadingOperation = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Single);
            LoadingOperation.allowSceneActivation = false;
        }
        catch
        {
            LoadPrevious();
            yield break;
        }
        yield return (LoadingOperation.progress > 0.99f);
        LoadingOperation.allowSceneActivation = true;
        LoadSceneAsyncCoroutine.Stop();
    }

    public static void LoadPrevious(int distance = 1)
    {
        int previousStage = sceneNavigation.Condense(distance);
        LoadSceneAsyncCoroutine = LoadSceneAsync(previousStage);
        LoadSceneAsyncCoroutine.Start();
    }

    internal static void PrepareToLoad(int index)
    {
        StagePrepped = index;
    }

    internal static void Deploy()
    {
        LoadScene(StagePrepped ?? TITLE_SCREEN);
    }

    public static void ReloadScene()
    {
        LoadScene(StagePrepped.Value);
    }
}
