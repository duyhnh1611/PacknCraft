using PacknCraft;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public AsyncOperation LoadSceneOperation(SceneId sceneId)
    {
        return SceneManager.LoadSceneAsync(GetSceneName(sceneId));
    }

    public string GetSceneName(SceneId id)
    {
        return id switch
        {
            SceneId.Game => "GameScene",
            _ => throw new System.Exception($"Unknown scene {id}")
        };
    }
}