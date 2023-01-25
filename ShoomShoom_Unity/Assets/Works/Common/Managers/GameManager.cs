using UniRx;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public ReactProps<GameState> CurGameState = new ReactProps<GameState>();

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
    }

    void PrepareScene()
    {
        // display the loading screen
        LoadingScreenController.Instance.Show();
    }

    void SceneReadyHandler()
    {
        LoadingScreenController.Instance.FadeOut();
    }

    public void SetGameState(GameState state)
    {
        CurGameState.SetState(state);
    }
}
