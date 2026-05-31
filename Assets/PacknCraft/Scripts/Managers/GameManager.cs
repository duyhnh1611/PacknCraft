using Cysharp.Threading.Tasks;
using PacknCraft;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private LevelManager levelManager;
    [SerializeField] private MissionManager missionManager;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private LoadingManager loadingManager;

    private bool isLoading;

    public LevelManager LevelManager => levelManager;
    public MissionManager MissionManager => missionManager;
    public SceneLoader SceneLoader => sceneLoader;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        missionManager.OnAllMissionsCompleted += HandleMissionCompleted;
    }

    private void OnDisable()
    {
        missionManager.OnAllMissionsCompleted -= HandleMissionCompleted;
    }

    private async void Start()
    {
        await LoadCurrentLevelFlow();
    }

    private async UniTask LoadCurrentLevelFlow()
    {
        // start loading scene but do not activate yet
        var op = sceneLoader.LoadSceneOperation(SceneId.Game);
        op.allowSceneActivation = false;

        // wait until scene is ready (90%)
        UniTask sceneLoadTask = UniTask.WaitUntil(() => op.progress >= 0.9f);

        // ensure preload task is valid
        UniTask preloadTask = LoadCurrentLevelAsync();

        // wait for animation, preload, and scene loading
        await UniTask.WhenAll(sceneLoadTask, preloadTask);

        // activate scene
        op.allowSceneActivation = true;
        await UniTask.WaitUntil(() => op.isDone);

        // play end transition animation
        await loadingManager.PlayEndAnimation();
    }

    private async void HandleMissionCompleted()
    {
        await NextLevel();
    }

    private async UniTask LoadCurrentLevelAsync()
    {
        var levelInfo = await levelManager.LoadCurrentLevelAsync();

        missionManager.Init(levelInfo.targets);
    }

    public async UniTask NextLevel()
    {
        await LoadSceneAsync(SceneId.Game, LoadNextLevelAsync());
    }

    private async UniTask LoadNextLevelAsync()
    {
        var levelInfo = await levelManager.LoadNextLevelAsync();
        // fallback to first level if no next level
        levelInfo ??= await levelManager.LoadLevelAsync(1);

        missionManager.Init(levelInfo.targets);
    }

    public async UniTask LoadSceneAsync(SceneId sceneId, UniTask preloadTask = default)
    {
        if (isLoading) return;
        isLoading = true;

        // start transition animation
        UniTask startAnimTask = loadingManager.PlayStartAnimation();

        // start loading scene but do not activate yet
        var op = sceneLoader.LoadSceneOperation(sceneId);
        op.allowSceneActivation = false;

        // wait until scene is ready (90%)
        UniTask sceneLoadTask = UniTask.WaitUntil(() => op.progress >= 0.9f);

        // ensure preload task is valid
        UniTask safePreloadTask = preloadTask.Status == UniTaskStatus.Pending
            ? preloadTask
            : UniTask.CompletedTask;

        // wait for animation, preload, and scene loading
        await UniTask.WhenAll(startAnimTask, sceneLoadTask, safePreloadTask);

        // activate scene
        op.allowSceneActivation = true;
        await UniTask.WaitUntil(() => op.isDone);

        // play end transition animation
        await loadingManager.PlayEndAnimation();

        isLoading = false;
    }
}