using UnityEngine;
using UnityEngine.Pool;

public class PoolManager_Fightscene : Singleton<PoolManager_Fightscene>
{
    // Gun smoke pool
    [SerializeField] private GameObject _gunEffectPrefab;
    public ObjectPool<GameObject> GunEffectPool { get; set; }
    // Pistol bullet pool
    [SerializeField] private GameObject _pistolBulletPrefab;
    public ObjectPool<GameObject> PistolBulletPool { get; set; }


    private void Start()
    {
        CreateGunEffectPool();
        CreatePistolBulletPool();
    }

    private void CreateGunEffectPool()
    {
        GunEffectPool = new ObjectPool<GameObject>(
            () => Instantiate(_gunEffectPrefab, GlobalEffectsContainer.Instance.transform), // Create function
            obj => obj.gameObject.SetActive(true), // OnGet function, when init
            obj => obj.gameObject.SetActive(false), // OnRelease function, when give it back to pool
            obj => Destroy(obj.gameObject), // OnDestory function, when give back to the pool but the pool is filled
            false, // Collection check
            10, // pool normal size, Default 10
            20 // pool Max size, Default 10000
        );
    }
    private void CreatePistolBulletPool()
    {
        PistolBulletPool = new ObjectPool<GameObject>(
            () => Instantiate(_pistolBulletPrefab, GlobalEffectsContainer.Instance.transform), // Create function
            obj => obj.gameObject.SetActive(true), // OnGet function, when init
            obj => obj.gameObject.SetActive(false), // OnRelease function, when give it back to pool
            obj => Destroy(obj.gameObject), // OnDestory function, when give back to the pool but the pool is filled
            false, // Collection check
            20, // pool normal size, Default 10
            30 // pool Max size, Default 10000
        );
    }
}
