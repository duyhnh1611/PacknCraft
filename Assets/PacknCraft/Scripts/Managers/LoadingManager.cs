using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Animation anim;
    [SerializeField] private string startClipName = "Start";
    [SerializeField] private string endClipName = "End";

    private AnimationClip startClip;
    private AnimationClip endClip;

    private int startDurationMs;
    private int endDurationMs;

    private void Awake()
    {
        startClip = anim.GetClip(startClipName);
        endClip = anim.GetClip(endClipName);

        if (startClip == null || endClip == null)
        {
            Debug.LogError("LoadingManager: Missing animation clips!");
            return;
        }

        startDurationMs = (int)(startClip.length * 1000f);
        endDurationMs = (int)(endClip.length * 1000f) + 500;
    }

    public async UniTask PlayStartAnimation()
    {
        anim.gameObject.SetActive(true);

        anim.Play(startClip.name);
        await UniTask.Delay(startDurationMs, DelayType.UnscaledDeltaTime);
    }

    public async UniTask PlayEndAnimation()
    {
        anim.Play(endClip.name);
        await UniTask.Delay(endDurationMs, DelayType.UnscaledDeltaTime);

        anim.gameObject.SetActive(false);
    }
}