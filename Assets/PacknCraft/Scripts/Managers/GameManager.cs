using Cysharp.Threading.Tasks;
using PacknCraft;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private LevelManager levelManager;
    [SerializeField] private MissionManager missionManager;
    [SerializeField] private SceneLoader sceneLoader;

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
        await levelManager.PreloadLevelAsync(levelManager.CurrentLevel);

        var levelInfo = levelManager.GetCurrentLevelInfo();

        missionManager.Init(levelInfo.targets);

        await sceneLoader.LoadSceneAsync(SceneId.Game);
    }

    private async void HandleMissionCompleted()
    {
        await NextLevel();
    }

    public async UniTask NextLevel()
    {
        var levelInfo = await levelManager.LoadNextLevelAsync();

        // For now, if there is no next level, loop to the first level
        // TODO: Add a proper flow for reaching the last level
        levelInfo ??= await levelManager.LoadLevelAsync(1);

        missionManager.Init(levelInfo.targets);

        await sceneLoader.LoadSceneAsync(SceneId.Game);
    }
}