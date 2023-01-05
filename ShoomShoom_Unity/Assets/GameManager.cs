using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Screen.SetResolution(1920, 1080, true);
    }
}
