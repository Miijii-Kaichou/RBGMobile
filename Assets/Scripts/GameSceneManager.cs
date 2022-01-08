using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Extensions;
using Photon.Pun;

/// <summary>
/// Define Scene from the Build Settings
/// exactly in order.
/// </summary>
public enum DefinedScenes
{
    Gateway,
    Username,
    MainSelection,
    SoloPlay,
    Game,
    Inbox,
    PlayModeSelection,
    GameNetworked
}

public static partial class GameSceneManager
{
    public static int? StagePrepped { get; private set; } = null;
    static Navigation<int> sceneNavigation = new Navigation<int>();
    static AsyncOperation LoadingOperation;
    static IEnumerator LoadSceneAsyncCoroutine;
    static Scene ActiveScene => SceneManager.GetActiveScene();
    static bool LoadAsNetwork = false;

    public static void LoadScene(int buildIndex, bool extendNavigation = false)
    {
        LoadSceneAsyncCoroutine = LoadSceneAsync(buildIndex);

        if (extendNavigation)
            sceneNavigation.Stretch(ActiveScene.buildIndex);

        if (LoadAsNetwork)
        {
            PhotonNetwork.LoadLevel(buildIndex);
            return;
        }

        LoadSceneAsyncCoroutine.Start();
    }

    public static void LoadScene(DefinedScenes scene, bool extendNavigation = false)
    {
        LoadScene((int)scene, extendNavigation);
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
        LoadAsNetwork = false;
    }

    internal static void PrepareToLoad(int index)
    {
        StagePrepped = index;
    }

    internal static void PrepareToLoad(DefinedScenes scene)
    {
        PrepareToLoad((int)scene);
    }

    internal static void Deploy()
    {
        LoadScene(StagePrepped ?? 0);
    }

    public static void ReloadScene()
    {
        LoadScene(StagePrepped.Value);
    }

    internal static void DeployAsNetworked()
    {
        LoadAsNetwork = true;   
        LoadScene(StagePrepped ?? 0);
    }
}