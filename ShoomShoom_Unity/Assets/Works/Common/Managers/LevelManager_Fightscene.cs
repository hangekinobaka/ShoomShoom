using UniRx;
using UnityEngine;

public class LevelManager_Fightscene : Singleton<LevelManager_Fightscene>
{
    [SerializeField] Transform _levelPlayerPos;
    [SerializeField] Transform _player;
    Vector3 _initPlayerPos;

    // public vars
    public Transform PlayerTrans => _player;

    protected override void Awake()
    {
        base.Awake();
        InitPlayer();
    }

    private void InitPlayer()
    {
        _initPlayerPos = _player.position;
        if (_player.position != _levelPlayerPos.position)
        {
            _player.position = _levelPlayerPos.position;
            _levelPlayerPos.gameObject.SetActive(false);
            Observable.NextFrame()
            .Do(_ =>
            {
                _player.position = _initPlayerPos;
                GameManager.Instance.SetGameState(GameState.Inited);
            })
            .Subscribe()
            .AddTo(this);
        }
    }
}