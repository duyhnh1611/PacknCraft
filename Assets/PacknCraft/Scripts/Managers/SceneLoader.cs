using Cysharp.Threading.Tasks;
using PacknCraft;
using UnityEngine.SceneManagement;

public class SceneLoader : UnityEngine.MonoBehaviour
{
    public async UniTask LoadSceneAsync(SceneId sceneId)
    {
        await SceneManager.LoadSceneAsync(GetSceneName(sceneId)).ToUniTask();
    }

    private string GetSceneName(SceneId id)
    {
        return id switch
        {
            SceneId.Game => "GameScene",
            _ => throw new System.Exception($"Unknown scene {id}")
        };
    }
}