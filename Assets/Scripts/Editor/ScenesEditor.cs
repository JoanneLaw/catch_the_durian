using UnityEditor;
using UnityEditor.SceneManagement;

public class ScenesEditor : Editor
{
    [MenuItem("Scenes/Loading Scene")]
    public static void OpenLoadingScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/LoadingScene.unity");
    }

    [MenuItem("Scenes/Main Scene")]
    public static void OpenMainScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
    }
}
