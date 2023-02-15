using UniRx;
using UnityEngine;

public enum PlatformType
{
    PC,
    Mobile
}

public class GameManager : Singleton<GameManager>
{
    // References
    [SerializeField] DebugManager _debugManager;

    public ReactProps<GameState> CurGameState = new ReactProps<GameState>();

    // Global vals
    public PlatformType CurPlatformType { private set; get; }

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
        Screen.SetResolution(1920, 1080, true);

        CurGameState.State.Subscribe(state =>
        {
            switch (state)
            {
                case GameState.BeforeInit:
                    PrepareScene();
                    break;
                case GameState.Inited:
                    SceneReadyHandler();
                    break;
                case GameState.Gaming:
                    break;
                case GameState.Paused:
                    break;
                default:
                    break;
            }
        }).AddTo(this);

        SetGameState(GameState.BeforeInit);

        // Detect current platform
        if (_debugManager != null && _debugManager.SimulateMobile) CurPlatformType = PlatformType.Mobile;
        else if (Application.isMobilePlatform) CurPlatformType = PlatformType.Mobile;
        else CurPlatformType = PlatformType.PC;
    }

    void PrepareScene()
    {
        // display the loading screen
        LoadingScreenController.Instance.Show();
        InputManager_Fightscene.Instance.EnableInput.SetState(false);
    }

    void SceneReadyHandler()
    {
        LoadingScreenController.Instance.FadeOut();
        Observable.Timer(System.TimeSpan.FromSeconds(.1f))
            .Subscribe(_ => InputManager_Fightscene.Instance.EnableInput.SetState(true))
            .AddTo(this);
    }

    public void SetGameState(GameState state)
    {
        CurGameState.SetState(state);
    }
}
