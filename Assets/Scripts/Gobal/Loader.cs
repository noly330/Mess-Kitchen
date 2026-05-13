
using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        GameScene,
        LoadingScene,
        LobbyScene,
        CharacterSelectScene
    }
    private static Scene _targetScene;

    public static void LoadScene(Scene scene)
    {
        Loader._targetScene = scene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
        
    }
    public static void LoadNetwork(Scene targetScene)
    {
        //所有已连接的客户端一起加载场景
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(_targetScene.ToString());
    }
}
